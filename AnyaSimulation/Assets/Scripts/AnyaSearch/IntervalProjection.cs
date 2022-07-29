using System;
using System.Diagnostics;

namespace Anya_2d
{
    public class IntervalProjection
    {
        public double left;          // sempre serão ou um inteiro, em caso de intervalos fechados [a, b],
        public double right;         // ou um double, em caso de intervalos abertos (a, b) = [a - smallest_step, b - smallest_step]

        // the furthest point left (resp. right) which is
        // visible from the left (resp. right) endpoint
        // of the projected interval.

        /// <summary>
        /// Ponto mais a esquerda visivel pelo interval
        /// </summary>
        public double max_left;

        /// <summary>
        /// Ponto mais a direita visivel pelo interval
        /// </summary>
        public double max_right;
        public int row;

        /// <summary>
        /// <see langword="true"/> se é possível mover os endpoints do intervalo às rows adjacentes (up down) sem obstáculos
        /// </summary>
        public bool valid;

        /// <summary>
        /// <see langword="true"/> se o endpoint esquerdo da projeção é menor que o direito
        /// </summary>
        public bool observable;

        // these variables only used for conical projection
        // some terminology:
        // we analyse the cells of this row in order to determine if 
        // the successors inside a conical projection are sterile or not. 
        public int sterile_check_row;
        public int check_vis_row;

        // used when generating type iii non-observable conical successors
        public int type_iii_check_row;

        /// <summary>
        /// <see langword="true"/> se o nodo flat não pode mais ser projetado
        /// </summary>
        public bool deadend;

        /// <summary>
        /// <see langword="true"/> se o nodo flat não toca nenhum obstáculo
        /// </summary>
        public bool intermediate;

        /// <summary>
        /// Cria uma IntervalProjection
        /// </summary>
        public IntervalProjection()
        {
            valid = false;
        }

        /// <summary>
        /// Projeta o intervalo do nodo na grid
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        public void Project(Node node, GridGraph grid)
        {
            Project(node.interval.GetLeft(), node.interval.GetRight(),
                    node.interval.GetRow(),
                    (int)node.root.x, (int)node.root.y, grid);
        }

        /// <summary>
        /// Projeta o intervalo de um nodo utilizando de seus valores
        /// </summary>
        /// <param name="ileft"></param>
        /// <param name="iright"></param>
        /// <param name="irow"></param>
        /// <param name="rootx"></param>
        /// <param name="rooty"></param>
        /// <param name="grid"></param>
        public void Project(double ileft, double iright, int irow, int rootx, int rooty, GridGraph grid)
        {
            observable = false; //por padrão seta essas bools para false
            valid = false;

            if (rooty == irow)  //se coluna de root e intervalo é a mesma
            {
                Project_flat(ileft, iright, rootx, rooty, grid);    //gera projeção flat
            }
            else
            {
                Project_cone(ileft, iright, irow, rootx, rooty, grid); //gera projeção conica
            }
        }

        /// <summary>
        /// Gera uma projeção conica de um nodo
        /// </summary>
        /// <param name="ileft"></param>
        /// <param name="iright"></param>
        /// <param name="irow"></param>
        /// <param name="rootx"></param>
        /// <param name="rooty"></param>
        /// <param name="grid"></param>
        public void Project_cone(double ileft, double iright, int irow,
                int rootx, int rooty, GridGraph grid)
        {
            if (rooty < irow) // caso coluna da root seja menor que a do intervalo projeta pra baixo
            {
                check_vis_row = irow;       //row testada para visib é a propria do intervalo
                sterile_check_row = row = irow + 1; //sterile_check_row é a row abaixo do intervalo
                type_iii_check_row = irow - 1;
            }
            else //caso contrário, projeta pra cima
            {
                Debug.Assert(rooty > irow);
                sterile_check_row = irow - 2;   //sterile_check_row é duas rows acima do intervalo
                row = check_vis_row = irow - 1; //row testada para visib é acima do intervalo
                type_iii_check_row = irow;
            }

            //verifica se a projecao é válida (ver def. de valid)
            valid = grid.Get_cell_is_traversable((int)(ileft + grid.smallest_step), check_vis_row) &&
                    grid.Get_cell_is_traversable((int)(iright - grid.smallest_step), check_vis_row);

            if (!valid) { return; }

            // interpolate the endpoints of the new interval onto the next row.

            double rise = Math.Abs(irow - rooty); //distancia entre coluna da root e do intervalo (eixo y)
            double lrun = rootx - ileft;    //distancia entre root e esquerda do intervalo (eixo x)
            double rrun = iright - rootx;  //distancia entre root e direita do intervalo (eixo x)

            // corta o intervalo se houver obstaculo na visibilidade da root
            max_left = grid.Scan_cells_left((int)ileft, check_vis_row); //ponto que encontra obstáculo a esq
            left = Math.Max(ileft - lrun / rise, max_left); //max entre proj triangular ou obtáculo

            //mesmo para a direita
            max_right = grid.Scan_cells_right((int)iright, check_vis_row);
            right = Math.Min(iright + rrun / rise, max_right);

            //ver def. observable
            observable = (left < right);

            // sanity checking; sometimes an interval cannot be projected 
            // all the way to the next row without first hitting an obstacle.
            // in these cases we need to reposition the endpoints appropriately
            if (left >= max_right)
            {
                left = grid.Get_cell_is_traversable(
                                (int)(ileft - grid.smallest_step), check_vis_row) ? right : max_left;
            }
            if (right <= max_left)
            {
                right = grid.Get_cell_is_traversable(
                            (int)iright, check_vis_row) ?
                                left : max_right;
            }
        }

        /// <summary>
        /// Faz a projeção flat do intervalo, alterando suas variáveis e características.
        /// (left, right, deadend, intermediate, valid)
        /// </summary>
        public void Project_flat(double ileft, double iright, int rootx, int rooty, GridGraph grid)
        {
            if (rootx <= ileft)      // se a raiz está pra esquerda do endpoint da esquerda do intervalo
            {
                left = iright;       // o endpoint da esquerda da projeção flat vai ser a direita do intervalo
                right = grid.Scan_right(left, rooty);    //projeção se estende até o obstáculo a direita
                deadend = !(grid.Get_cell_is_traversable((int)right, rooty)
                          && grid.Get_cell_is_traversable((int)right, rooty - 1));
            }
            else                     // se a raiz está pra direita do endpoint da esquerda do intervalo
            {
                right = ileft;       // o endpoint da direita da projeção flat vai ser a esquerda do intervalo
                left = grid.Scan_left(right, rooty);      //projeção se estende até o obstáculo a esquerda
                deadend = !(grid.Get_cell_is_traversable((int)(left - grid.smallest_step), rooty)
                          && grid.Get_cell_is_traversable((int)(left - grid.smallest_step), rooty - 1));
            }

            intermediate = grid.Get_cell_is_traversable((int)left, rooty) && grid.Get_cell_is_traversable((int)left, rooty - 1);

            row = rooty;
            valid = (left != right);
        }

        /// <summary>
        /// Projeção de um nodo flat para uma coluna adjacente
        /// </summary>
        /// <param name="node"></param>
        /// <param name="grid"></param>
        public void Project_f2c(Node node, GridGraph grid)
        {
            Debug.Assert(node.interval.GetRow() == node.root.y);
            Project_f2c(node.interval.GetLeft(), node.interval.GetRight(),
                    node.interval.GetRow(),
                    (int)node.root.x, (int)node.root.y, grid);
        }

        /// <summary>
        /// Projeção de um nodo flat para uma coluna adjacente
        /// </summary>
        /// <param name="ileft"></param>
        /// <param name="iright"></param>
        /// <param name="irow"></param>
        /// <param name="rootx"></param>
        /// <param name="rooty"></param>
        /// <param name="grid"></param>
        private void Project_f2c(double ileft, double iright, int irow,
                int rootx, int rooty, GridGraph grid)
        {
            // look to the right for successors
            // recall that each point (x, y) corresponds to the
            // top-left corner of a tile at location (x, y)
            if (rootx <= ileft) //se a root  está mais a esquerda que a esquerda do intervalo
            {
                // can we make a valid turn? valid means 
                // (i) the path bends around a corner; 
                // (ii) we do not step through any obstacles or through 
                // double-corner points.
                bool can_step =                                               //verifica se podemos dobrar no ponto mais a direita
                        grid.Get_cell_is_traversable((int)iright, irow) &&
                        grid.Get_cell_is_traversable((int)iright, irow - 1);
                if (!can_step) { valid = false; observable = false; return; }


                if (!grid.Get_cell_is_traversable((int)iright - 1, irow)) //se temos um obstáculo acima
                {                                                           //vamos pra baixo
                    sterile_check_row = row = irow + 1;     //sterile_check_row abaixo do intervalo
                    check_vis_row = irow;                   //vis row na mesma coluna do intervalo
                }
                else
                {                                           // caso conrário vamos pra cima
                    row = check_vis_row = irow - 1;     //vis row acima do intervalo
                    sterile_check_row = irow - 2;           //sterile_check_row 2 acima do intervalo
                }

                left = max_left = iright;           //nova esquerda é a antiga direita
                right = max_right = grid.Scan_cells_right((int)left, check_vis_row);    //nova direita é
            }
            else
            { // look to the left for successors
                Debug.Assert(rootx >= iright);
                bool can_step =
                        grid.Get_cell_is_traversable((int)ileft - 1, irow) &&
                        grid.Get_cell_is_traversable((int)ileft - 1, irow - 1);
                if (!can_step) { valid = false; observable = false; return; }

                // if the tiles below are free, we must be going up
                // else we round the corner and go down		
                if (!grid.Get_cell_is_traversable((int)ileft, irow))
                {   // going down
                    check_vis_row = irow;
                    sterile_check_row = row = irow + 1;
                }
                else
                {   // going up
                    row = check_vis_row = irow - 1;
                    sterile_check_row = irow - 2;
                }

                right = max_right = ileft;
                left = max_left = grid.Scan_cells_left((int)right - 1, check_vis_row);
            }
            valid = true;
            observable = false;
        }

        public bool GetValid() { return valid; }
    }
}
