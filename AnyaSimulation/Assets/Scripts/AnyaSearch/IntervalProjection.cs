using System;
using System.Diagnostics;

namespace Anya_2d
{
    public class IntervalProjection
    {
        // obs: left e right sempre serão ou um inteiro, em caso de intervalos fechados [a, b],
        // ou um double, em caso de intervalos abertos (a, b) = [a - smallest_step, b - smallest_step]

        /// <summary>
        /// Endpoint esquerdo da projeção.
        /// </summary>
        public double left;          

        /// <summary>
        /// Endpoint direito da projeção.
        /// </summary>
        public double right;

        /// <summary>
        /// Ponto mais a esquerda visível do endpoint esquerdo da projeção.
        /// </summary>
        public double max_left;

        /// <summary>
        /// Ponto mais a direita visível do endpoint direito da projeção.
        /// </summary>
        public double max_right;

        /// <summary>
        /// Linha da projeção (y).
        /// </summary>
        public int row;

        /// <summary>
        /// No caso de projeção cônica, a projeção vai ser válida quando é possível mover os endpoints do intervalo às linhas adjacentes 
        /// (de cima ou de baixo, dependendo da direção da projeção) sem interceptar nenhum obstáculo.
        /// No caso de projeção flat, a projeção vai ser válida quando realmente gerou uma projeção (endpoint esquerdo é diferente do endpoint direito).
        /// </summary>
        public bool valid;

        /// <summary>
        /// Se o endpoint esquerdo da projeção é estritamente menor que o direito.
        /// </summary>
        public bool observable;

        ////////////////// TERMINOLOGIA PARA PROJEÇÃO FLAT //////////////////
        
        /// <summary>
        /// Se o endpoint da projeção flat do intervalo é adjacente a uma parede de obstáculos.
        /// Se a projeção é pra direita do intervalo, o endpoint testado é o da direita.
        /// Se a projeção é pra esquerda do intervalo, o endpoint testado é o da esquerda.
        /// </summary>
        public bool deadend;

        /// <summary>
        /// Se o endpoint esquerdo da projeção flat do intervalo não é adjacente a uma parede de obstáculos.
        /// </summary>
        public bool intermediate;

        /////////////////////////////////////////////////////////////////////


        ////////////////// TERMINOLOGIA PARA PROJEÇÃO CÔNICA //////////////////
        /// <summary>
        /// Linha para teste se os sucessores são estéreis: não geram outros sucessores.
        /// </summary>
        public int sterile_check_row;

        /// <summary>
        /// Linha para teste de projeção (para identificar os obstáculos no meio de uma projeção).
        /// </summary>
        public int check_vis_row;

        /// <summary>
        /// Linha usada para gerar os sucessores não observáveis tipo 3
        /// </summary>
        public int type_iii_check_row;
        /////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Cria uma IntervalProjection
        /// </summary>
        public IntervalProjection()
        {
            valid = false;
        }

        /// <summary>
        /// Faz as projeções do nodo na grid (processo inicial para gerar os sucessores).
        /// </summary>
        public void Project(Node node, GridGraph grid)
        {
            Project(node.interval.GetLeft(), node.interval.GetRight(), node.interval.GetRow(), (int)node.root.x, (int)node.root.y, grid);
        }

        /// <summary>
        /// Projeta o intervalo de um nodo utilizando de seus valores
        /// </summary>
        public void Project(double ileft, double iright, int irow, int rootx, int rooty, GridGraph grid)
        {
            observable = false;  // por padrão seta essas bools para false
            valid = false;

            if (rooty == irow)   // se a linha da raiz e intervalo é a mesma
            {
                Project_flat(ileft, iright, rootx, rooty, grid);       // gera projeção flat
            }
            else
            {
                Project_cone(ileft, iright, irow, rootx, rooty, grid); // senão, gera projeção cônica
            }
        }

        /// <summary>
        /// Gera a projeção flat do nodo, alterando suas variáveis e características.
        /// </summary>
        public void Project_flat(double ileft, double iright, int rootx, int rooty, GridGraph grid)
        {
            if (rootx <= ileft)      // se a raiz está pra esquerda ou é igual ao endpoint esquerdo do intervalo
            {
                left = iright;                            // o endpoint esquerdo da projeção flat vai ser o endpoint direito do intervalo
                right = grid.Scan_right(left, rooty);     // o endpoint direito da projeção flat é a primeira corner ou obstaculo atingido indo para direita
                deadend = !(grid.Get_cell_is_traversable((int)right, rooty) && grid.Get_cell_is_traversable((int)right, rooty - 1));
            }
            else                     // se a raiz está pra direita do endpoint esquerdo do intervalo
            {
                right = ileft;                            // o endpoint direito da projeção flat vai ser o endpoint esquerdo do intervalo
                left = grid.Scan_left(right, rooty);      // o endpoint esquerdo da projeção flat é a primeira corner ou obstaculo atingido indo para esquerda 
                deadend = !(grid.Get_cell_is_traversable((int)(left - grid.smallest_step), rooty) && grid.Get_cell_is_traversable((int)(left - grid.smallest_step), rooty - 1));
            }

            intermediate = grid.Get_cell_is_traversable((int)left, rooty) && grid.Get_cell_is_traversable((int)left, rooty - 1);

            // como é projeção flat, a linha da projeção vai ser igual a linha da raiz
            row = rooty;
            // se o endpoint da esquerda da projeção for diferente do da direita, então a projeção é válida, senão não foi gerada projeção
            valid = (left != right);
        }

        /// <summary>
        /// Gera a projeção cônica do nodo na linha adjacente, alterando suas variáveis e características.
        /// </summary>
        public void Project_cone(double ileft, double iright, int irow, int rootx, int rooty, GridGraph grid)
        {
            if (rooty < irow)                           // se a linha da raiz é menor que a do intervalo, projeta pra baixo
            {
                row = irow + 1;                         // a linha da projeção vai ser a de baixo do intervalo
                check_vis_row = irow;                   // para testar a visibilidade para baixo deve-se considerar a própria linha do intervalo
                sterile_check_row = irow + 1;           // sterile_check_row vai ser a de baixo do intervalo
                type_iii_check_row = irow - 1;          // type_iii_check_row vai ser a linha acima do intervalo
            }
            else                                        // se a linha da raiz é maior ou igual que a do intervalo, projeta pra cima
            {
                row = irow - 1;                         // a linha da projeção vai ser a de cima do intervalo 
                check_vis_row = irow - 1;               // para testar a visibilidade para cima deve-se considerar a linha de cima do intervalo
                sterile_check_row = irow - 2;           // sterile_check_row vai ser duas linhas acima da do intervalo
                type_iii_check_row = irow;              // type_iii_check_row vai ser a própria linha do intervalo
            }

            // verifica se os endpoints podem ser projetados
            valid = grid.Get_cell_is_traversable((int)(ileft + grid.smallest_step), check_vis_row) && grid.Get_cell_is_traversable((int)(iright - grid.smallest_step), check_vis_row);
            if (!valid) { return; }

            double rise = Math.Abs(irow - rooty);     // distância entre linha do intervalo e linha da raiz
            double lrun = rootx - ileft;              // distância entre coluna da raiz e endpoint esquerdo do intervalo 
            double rrun = iright - rootx;             // distância entre coluna da raiz e endpoint direito do intervalo 

            max_left = grid.Scan_cells_left((int)ileft, check_vis_row);    // último ponto antes de um obstáculo indo para esquerda
            left = Math.Max((ileft - lrun)/rise, max_left);                // o endpoint esquerdo da projeção vai ser ..............

            max_right = grid.Scan_cells_right((int)iright, check_vis_row); // último ponto antes de um obstáculo indo para direita
            right = Math.Min((iright + rrun)/rise, max_right);             // o endpoint direito da projeção vai ser ...............

            observable = (left < right);

            if (left >= max_right)      // se o endpoint esquerdo gerado estiver na direita do ponto mais a direita possível para a projeção
            {
                // o endpoint esquerdo da projeção vai ser ou o ponto direito da projeção, caso a célula da esquerda do intervalo esteja bloqueada,
                // ou o ponto mais a esquerda possível para a projeção
                left = grid.Get_cell_is_traversable((int)(ileft - grid.smallest_step), check_vis_row) ? right : max_left;
            }
            if (right <= max_left)       // se o endpoint direito gerado estiver na esquerda do ponto mais a esquerda possível para a projeção
            {
                // o endpoint direito da projeção vai ser ou o ponto esquerdo da projeção, caso a célula da direita do intervalo esteja bloqueada,
                // ou o ponto mais a direita possível para a projeção
                right = grid.Get_cell_is_traversable((int)iright, check_vis_row) ? left : max_right;
            }
        }

        /// <summary>
        /// Projeção de um nodo flat para uma coluna adjacente.
        /// </summary>
        public void Project_f2c(Node node, GridGraph grid)
        {
            Debug.Assert(node.interval.GetRow() == node.root.y);
            Project_f2c(node.interval.GetLeft(), node.interval.GetRight(), node.interval.GetRow(), (int)node.root.x, (int)node.root.y, grid);
        }

        /// <summary>
        /// Projeção de um nodo flat para uma linha adjacente.
        /// </summary>
        private void Project_f2c(double ileft, double iright, int irow, int rootx, int rooty, GridGraph grid)
        {
            if (rootx <= ileft) // se a raiz está pra esquerda ou é igual ao endpoint esquerdo do intervalo
            {
                // testa se o endpoint direito não é adjacente a uma parede de obstáculos (deadend)
                bool can_step =  grid.Get_cell_is_traversable((int)iright, irow) && grid.Get_cell_is_traversable((int)iright, irow - 1);
                // se for, retorna
                if (!can_step) { valid = false; observable = false; return; }


                if (!grid.Get_cell_is_traversable((int)iright - 1, irow)) // se o endpoint direito é uma corner
                {
                    row = irow + 1;                    // a linha da projeção vai ser a de baixo do intervalo 
                    check_vis_row = irow;              // para testar a visibilidade para baixo deve-se considerar a própria linha do intervalo
                    sterile_check_row = irow + 1;      // sterile_check_row vai ser a linha abaixo da do intervalo
                }
                else                                                     // se o endpoint direito está livre
                {                                           
                    row = irow - 1;                     // a linha da projeção vai ser a de cima do intervalo
                    check_vis_row = irow - 1;           // para testar a visibilidade para cima deve-se considerar a linha acima do intervalo
                    sterile_check_row = irow - 2;       // sterile_check_row vai ser duas linhas acima da do intervalo
                }

                // o endpoint esquerdo da nova projeção vai ser o endpoint direito do intervalo
                left = max_left = iright;
                // o endpoint direito da nova projeção vai ser o primeiro ponto antes de um obstáculo encontrado indo pra direita
                right = max_right = grid.Scan_cells_right((int)left, check_vis_row);    
            }
            else      // se a raiz está pra esquerda ou é igual ao endpoint esquerdo do intervalo
            { 
                // testa se o endpoint esquerdo não é adjacente a uma parede de obstáculos (deadend)
                bool can_step = grid.Get_cell_is_traversable((int)ileft - 1, irow) && grid.Get_cell_is_traversable((int)ileft - 1, irow - 1);
                // se for, retorna
                if (!can_step) { valid = false; observable = false; return; }

                if (!grid.Get_cell_is_traversable((int)ileft, irow))  // se o endpoint esquerdo é uma corner
                {
                    row = irow + 1;                    // a linha da projeção vai ser a de baixo do intervalo 
                    check_vis_row = irow;              // para testar a visibilidade para baixo deve-se considerar a própria linha do intervalo
                    sterile_check_row = irow + 1;      // sterile_check_row vai ser a linha abaixo da do intervalo
                }
                else                                                  // se o endpoint esquerdo está livre
                {
                    row = irow - 1;                     // a linha da projeção vai ser a de cima do intervalo
                    check_vis_row = irow - 1;           // para testar a visibilidade para cima deve-se considerar a linha acima do intervalo
                    sterile_check_row = irow - 2;       // sterile_check_row vai ser duas linhas acima da do intervalo
                }

                // o endpoint direito da nova projeção vai ser o endpoint esquerdo do intervalo
                right = max_right = ileft;
                // o endpoint esquerdo da nova projeção vai ser o primeiro ponto antes de um obstáculo encontrado indo pra esquerda
                left = max_left = grid.Scan_cells_left((int)right - 1, check_vis_row);
            }

            valid = true;
            observable = false;
        }

        /// <summary>
        /// Getter de valid
        /// </summary>
        public bool GetValid() { return valid; }
    }
}
