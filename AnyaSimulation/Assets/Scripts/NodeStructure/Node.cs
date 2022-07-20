using System.Collections.Generic;
using UnityEngine;

namespace Anya_2d
{
    public class Node
    {
        public Node parentNode;
        public Interval interval;
        public Vector2 root;

        private double f = 0;
        private double g;

        public Node(Node parent, Interval interval, int rootx, int rooty)
        {
            parentNode = parent;
            this.interval = interval;
            root = new Vector2(rootx, rooty);

            if (parent == null)
            {
                g = 0;
            }
            else
            {
                g = parent.g + Vector2.Distance(parent.root, root);
            }
        }

        public Node(Node parent, Interval interval, Vector2 root)
        {
            parentNode = parent;
            this.interval = interval;
            this.root = root;

            if (parent == null)
            {
                g = 0;
            }
            else
            {
                g = parent.g + Vector2.Distance(parent.root, root);
            }
        }

        public double GetF()
        {
            return f;
        }

        public void SetF(double f)
        {
            this.f = f;
        }

        public double GetG()
        {
            return g;
        }

        public void SetG(double g)
        {
            this.g = g;
        }

        ///<summary>
        ///Verifica se um nodo é igual a outro
        ///</summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !typeof(Node).IsInstanceOfType(obj))
            {
                return false;
            }

            Node n = (Node)obj;
            if (!n.interval.Equals(interval))
            {
                return false;
            }

            if (n.root.x != root.x || n.root.y != root.y)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = interval != null ? interval.GetHashCode() : 0;
            result = 31 * result + (root != null ? root.GetHashCode() : 0);
            return result;
        }

        public Node GetParentNode()
        {
            return parentNode;
        }

        public void SetParentNode(Node parentNode)
        {
            this.parentNode = parentNode;
        }

        public Interval GetInterval()
        {
            return interval;
        }

        public void SetInterval(Interval interval)
        {
            this.interval = interval;
        }

        public Vector2 GetRoot()
        {
            return root;
        }

        public void SetRoot(Vector2 root)
        {
            this.root = root;
        }

        public static void AddNodeToList(List<Node> nodeList, Node node)
        {
            if (NotExists(nodeList, node))
            {
                nodeList.Add(node);
            }
        }

        public static void AddNodeListToList(List<Node> dest, List<Node> source)
        {
            foreach (Node n in source)
            {
                AddNodeToList(dest, n);
            }
        }

        private static bool NotExists(List<Node> nodeList, Node node)
        {

            foreach (Node n in nodeList)
            {
                if (n.GetParentNode() == node.GetParentNode() &&
                        n.GetInterval().GetRight() == node.GetInterval().GetRight() &&
                        n.GetInterval().GetLeft() == node.GetInterval().GetLeft() &&
                        n.GetRoot() == node.GetRoot())
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return "root: " + root.ToString() + " " + interval.ToString();
        }
    }
}
