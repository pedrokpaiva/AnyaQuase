using System;
using System.Diagnostics;

namespace Anya_2d
{
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
                    (t.root.x == t.interval.GetRight()));

            int irow = n.interval.GetRow();
            double ileft = n.interval.GetLeft();
            double iright = n.interval.GetRight();
            double targetx = t.root.x;
            double targety = t.root.y;
            double rootx = n.root.x;
            double rooty = n.root.y;

            // root and target must be on opposite sides of the interval
            // (or both on the same row as the interval). we mirror the
            // target through the interval if this is not the case
            if ((rooty < irow && targety < irow))
            {
                targety += 2 * (irow - targety);

            }
            else if (rooty > irow && targety > irow)
            {
                targety -= 2 * (targety - irow);
            }

            // project the interval endpoints onto the target row
            double rise_root_to_irow = Math.Abs(n.root.y - n.interval.GetRow());
            double rise_irow_to_target = Math.Abs(n.interval.GetRow() - t.root.y);
            double lrun = n.root.x - n.interval.GetLeft();
            double rrun = n.interval.GetRight() - n.root.x;
            double left_proj = n.interval.GetLeft() - rise_irow_to_target * (lrun / rise_root_to_irow);
            double right_proj = n.interval.GetRight() + rise_irow_to_target * (rrun / rise_root_to_irow);

            if ((t.root.x + GridGraph.epsilon) < left_proj)
            {
                return // pass through the left endpoint
                        h.H(rootx, rooty, ileft, irow) +
                        h.H(ileft, irow, targetx, targety);
            }
            if (t.root.x > (right_proj + GridGraph.epsilon))
            {
                return // pass through the right endpoint
                        h.H(rootx, rooty, iright, irow) +
                        h.H(iright, irow, targetx, targety);
            }

            //representative point is interior to the interval
            return h.H(rootx, rooty, targetx, targety);
        }
    }
}