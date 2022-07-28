using System;
using System.Diagnostics;

namespace Anya_2d
{
    /// <summary>
    /// Heuristica do Anya+
    /// </summary>
    public class AnyaHeuristic : IHeuristic<Node>
    {
        private EuclideanDistanceHeuristic h;

        public AnyaHeuristic()
        {
            h = new EuclideanDistanceHeuristic();
        }

        public double GetValue(Node n)
        {
            return 0;
        }

        public double GetValue(Node n, Node t)
        {
            Debug.Assert((t.root.y == t.interval.GetRow()) &&
                    (t.root.x == t.interval.GetLeft()) &&
                    (t.root.x == t.interval.GetRight())); ///Garante que o nodo target é apenas um ponto

            double rootx = n.root.x;
            double rooty = n.root.y;
            int irow = n.interval.GetRow();
            double ileft = n.interval.GetLeft();
            double iright = n.interval.GetRight();

            double targetx = t.root.x;
            double targety = t.root.y;



            //É necessário garantir que interval esteja entre root e target
            if ((rooty < irow && targety < irow)) //caso target E root estiverem ambos acima do intervalo
            {
                targety += 2 * (irow - targety); //posição de target é espelhada para outro lado do intervalo

            }
            else if (rooty > irow && targety > irow) //caso target E root estiverem ambos abaixo do intervalo
            {
                targety -= 2 * (targety - irow); //posição de target é espelhada para outro lado do intervalo
            }

            //projeta os extremos do intervalo para a coluna do target como se fosse um triângulo
            //setup
            double rise_root_to_irow = Math.Abs(rooty - irow); //distancia root -> interval (eixo y)
            double rise_irow_to_target = Math.Abs(irow - targety); //distancia target -> interval (eixo y)
            double lrun = rootx - ileft; //distancia root -> esquerda do interval (eixo x)
            double rrun = iright - rootx;//distancia direita do interval -> root (eixo x)
                                         //exec
            double left_proj = ileft - lrun * (rise_irow_to_target / rise_root_to_irow);//esquerda projetada
            double right_proj = iright + rrun * (rise_irow_to_target / rise_root_to_irow);//direita projetada

            if ((t.root.x + GridGraph.epsilon) < left_proj)// caso rootx menor que proj_left
            {
                return // a distancia vai passar pelo extremo esquerdo do intervalo (pois gostaria de ser
                       // ainda mais a esquerda)
                        h.H(rootx, rooty, ileft, irow) + h.H(ileft, irow, targetx, targety);
            }
            if (t.root.x > (right_proj + GridGraph.epsilon))// caso rootx maior que proj_right
            {
                return // a distancia vai passar pelo extremo direito do intervalo (pois gostaria de ser
                       // ainda mais a direita)
                        h.H(rootx, rooty, iright, irow) + h.H(iright, irow, targetx, targety);
            }

            // a distancia vai passar por algum ponto dentro do intervalo
            return h.H(rootx, rooty, targetx, targety);
        }
    }
}