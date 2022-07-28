using System;

namespace Anya_2d
{
    public class GridGraph
    {

        private bool[,] tiles;    // matriz representando os pontos do grid
        public int sizeX;         // tamanho do grid em X
        public int sizeY;         // tamanho do grid em Y

        private static float SQRT_TWO = (float)Math.Sqrt(2);
        private static double SQRT_TWO_DOUBLE = Math.Sqrt(2);
        private static float SQRT_TWO_MINUS_ONE = (float)(Math.Sqrt(2) - 1);
        public static double epsilon = 0.0000001;

        public double smallest_step;        // menor distância entre dois pontos contínuos adjacentes 
        public double smallest_step_div2;   // menor distância entre dois pontos contínuos adjacentes dividido por 2

        public GridGraph(int width, int height)
        {
            sizeX = width;
            sizeY = height;

            tiles = new bool[sizeY, sizeX];

            smallest_step = Math.Min(1 / width, 1 / height);
            smallest_step_div2 = smallest_step / 2;
        }

        public int Get_width()
        {
            return sizeX;
        }

        public int Get_height()
        {
            return sizeY;
        }

        public int Get_num_cells()
        {
            return sizeX * sizeY;
        }

        /// <summary>
        /// Seta o ponto com o valor booleano passado, sem testar se ele é válido.
        /// </summary>
        public void SetBlocked(int x, int y, bool value)
        {
            tiles[y, x] = value;
        }

        /// <summary>
        /// Caso seja um ponto válido, seta ele com o valor booleano passado.
        /// </summary>
        public void TrySetBlocked(int x, int y, bool value)
        {
            if (IsValidPoint(x, y))
            {
                tiles[y, x] = value;
            }
        }

        /// <summary>
        /// Retorna o valor booleano do ponto passado na matriz, ou verdadeiro caso seja um ponto inválido. 
        /// </summary>
        public bool IsBlocked(int x, int y)
        {
            if (x >= sizeX || y >= sizeY)
            {
                return true;
            }

            if (x < 0 || y < 0)
            {
                return true;
            }

            return !tiles[y, x];
        }

        /// <summary>
        /// Retorna o valor booleano do ponto passado, sem testar se é um ponto válido.
        /// </summary>
        public bool IsBlockedRaw(int x, int y)
        {
            return tiles[y, x];
        }

        /// <summary>
        /// Retorna verdadeiro caso seja um ponto válido, ou seja, dentro da matriz.
        /// </summary>
        public bool IsValidPoint(int x, int y)
        {
            return (x < sizeX && y < sizeY && x >= 0 && y >= 0);
        }


        //   Exemplo de matriz 3X3 com índices correspondentes
        //     Y/X  0    1    2  
        //     0    .    .    .    
        //     1    .    .    .  
        //     2    .    .    .   
        //          
        //    Em relação ao ponto (1,1):
        //    Célula: {(1,1),(2,1),(1,2),(2,2)}
        //    Célula NW: {(0,0),...}           noroeste
        //    Célula NE: {(1,0),...}           nordeste
        //    Célula SW: {(0,1),...}           sudoeste         
        //    Célula SE: {(1,1),...}           sudeste


        /// <summary>
        /// Retorna verdadeiro caso a célula não seja um obstáculo, ou seja,
        /// caso nenhum dos 4 pontos discretos que a formam sejam bloqueados.
        /// </summary>
        public bool Get_cell_is_traversable(int cx, int cy)
        {
            return !IsBlocked(cx, cy) ||
                    !IsBlocked(cx + 1, cy) ||
                    !IsBlocked(cx, cy + 1) ||
                    !IsBlocked(cx + 1, cy + 1);
        }

        /// <summary>
        /// Retorna verdadeiro caso o ponto seja visível de algum outro ponto discreto no grid,
        /// ou seja, se ele não é adjacente a 4 células bloqueadas.
        /// </summary>
        public bool Get_point_is_visible(int x, int y)
        {
            bool cellNW = Get_cell_is_traversable(x - 1, y - 1);
            bool cellNE = Get_cell_is_traversable(x, y - 1);
            bool cellSW = Get_cell_is_traversable(x - 1, y);
            bool cellSE = Get_cell_is_traversable(x, y);

            return cellNW || cellNE || cellSW || cellSE;
        }

        /// <summary>
        /// Retorna verdadeiro caso o ponto seja uma double corner, ou seja, se ele é adjacente a 2 
        /// diagonalmente adjacentes células bloqueadas.
        /// </summary>
        public bool Get_point_is_double_corner(int x, int y)
        {
            bool cellNW = Get_cell_is_traversable(x - 1, y - 1);
            bool cellNE = Get_cell_is_traversable(x, y - 1);
            bool cellSW = Get_cell_is_traversable(x - 1, y);
            bool cellSE = Get_cell_is_traversable(x, y);

            return ((!cellNW & !cellSE) & cellSW & cellNE) || ((!cellSW & !cellNE) & cellNW & cellSE);
        }

        /// <summary>
        /// Retorna verdadeiro caso o ponto seja uma corner, ou seja, se ele é adjacente a apenas
        /// uma célula bloqueada, ou a 2 diagonalmente adjacentes células bloqueadas.
        /// </summary>
        public bool Get_point_is_corner(int x, int y)
        {
            bool cellNW = Get_cell_is_traversable(x - 1, y - 1);
            bool cellNE = Get_cell_is_traversable(x, y - 1);
            bool cellSW = Get_cell_is_traversable(x - 1, y);
            bool cellSE = Get_cell_is_traversable(x, y);

            return ((!cellNW || !cellSE) & cellSW & cellNE) || ((!cellNE | !cellSW) & cellNW & cellSE);
        }

        /// <summary>
        /// Escaneia as células do grid, começando em (x,y) e indo na direção positiva.
        /// Retorna as coordenadas x do primeiro ponto de um obstáculo atingido.
        /// Se nenhum obstáculo foi atingido, retorna o índice do último ponto da linha.
        /// </summary>
        public int Scan_cells_right(int x, int y)
        {
            for (int i = x; i < sizeX; i++)
            {
                if (!Get_cell_is_traversable(i, y))
                {
                    return i;
                }
            }
            return sizeX - 1;
        }

        /// <summary>
        /// Escaneia as células do grid, começando em (x,y) e indo na direção negativa.
        /// Retorna as coordenadas x do primeiro ponto de um obstáculo atingido.
        /// Se nenhum obstáculo foi atingido, retorna o índice do primeiro ponto da linha.
        /// OBS: LEMBRAR DE MUDAR NAS CHAMADAS DESSA FUNÇÃO QUE NÃO É MAIS PRA SOMAR 1, JA A SOMA DE 1 EMBUTIDA NA FUNÇÃO
        /// </summary>
        public int Scan_cells_left(int x, int y)
        {
            for (int i = x; i >= 0; i--)
            {
                if (!Get_cell_is_traversable(i, y))
                {
                    return i + 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Escaneia as células do grid, começando em (x,y) e indo na direção positiva.
        /// Retorna as coordenadas x da primeira corner atingida, ou do último ponto antes de uma parede de obstáculo atingida.
        /// </summary>
        public int Scan_right(double x, int y)
        {
            int discrete_x = (int)(x + smallest_step);

            for (int i = discrete_x; i < sizeX; i++)
            {
                if (Get_cell_is_traversable(i, y) || !Get_cell_is_traversable(i, y + 1))
                {
                    return i;
                }
            }
            return discrete_x;
        }

        /// <summary>
        /// Escaneia as células do grid, começando em (x,y) e indo na direção negativa.
        /// Retorna as coordenadas x da primeira corner atingida, ou do último ponto antes de uma parede de obstáculo atingida.
        /// </summary>
        public int Scan_left(double x, int y)
        {
            int discrete_x = (int)(x + smallest_step);

            for (int i = discrete_x; i >= 0; i--)
            {
                if (Get_cell_is_traversable(i, y) || !Get_cell_is_traversable(i, y + 1))
                {
                    return i + 1;
                }
            }
            return discrete_x;
        }


        /// <summary>
        /// Retorna a distância Euclideana entre dois pontos no espaço, no formato float.
        /// </summary>
        public float Distance(int x1, int y1, int x2, int y2)
        {
            int distX = x2 - x1;
            int distY = y2 - y1;

            if (distY == 0)
            {
                return Math.Abs(distX);
            }
            if (distX == 0)
            {
                return Math.Abs(distY);
            }
            if (distX == distY || distX == -distY)
            {
                return SQRT_TWO * Math.Abs(distX);       //sqrt[2*(dist)^2] = sqrt(2)*dist
            }

            int squareDistance = distX * distX + distY * distY;

            return (float)Math.Sqrt(squareDistance);     //sqrt[(distX)^2 + (distY)^2]
        }

        /// <summary>
        /// Retorna a distância Euclideana entre dois pontos no espaço, no formato double.
        /// </summary>
        public double Distance_double(int x1, int y1, int x2, int y2)
        {
            int distX = x2 - x1;
            int distY = y2 - y1;

            if (distX == 0)
            {
                return Math.Abs(distY);
            }
            if (distY == 0)
            {
                return Math.Abs(distX);
            }
            if (distX == distY || distX == -distY)
            {
                return SQRT_TWO_DOUBLE * Math.Abs(distX);    //sqrt[2*(dist)^2] = sqrt(2)*dist
            }

            int squareDistance = distX * distX + distY * distY;

            return Math.Sqrt(squareDistance);                //sqrt[(distX)^2 + (distY)^2]
        }

        /// <summary>
        /// Retorna a distância Octile entre dois pontos na matriz.
        /// The octile distance is used to estimate the distance between two cells heuristically for grid-based maps.
        /// </summary>
        public float OctileDistance(int x1, int y1, int x2, int y2)
        {
            int distX = Math.Abs(x1 - x2);
            int distY = Math.Abs(y1 - y2);

            int min = distX;
            int max = distY;
            if (distY < distX)
            {
                min = distY;
                max = distX;
            }
            return min * SQRT_TWO_MINUS_ONE + max;
        }
    }
}
