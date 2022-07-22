using System.Collections.Generic;
using UnityEngine;

namespace Anya_2d
{
    /// <summary>
    /// Nodo formado pela área entre uma raíz e um intervalo
    /// </summary>
    public class Node
    {
        public Node parentNode;
        public Interval interval;
        public Vector2 root;

        private double f = 0;

        /// <summary>
        /// Distancia do nodo ao ponto de origem
        /// </summary>
        private double g;

        /// <summary>
        /// Cria um nodo a partir de um nodo pai, um intervalo e duas coordenadas da raiz
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="interval"></param>
        /// <param name="rootx"></param>
        /// <param name="rooty"></param>
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

        /// <summary>
        /// Cria um nodo a partir de um nodo pai, um intervalo e uma raiz
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="interval"></param>
        /// <param name="root"></param>
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

        /// <summary>
        /// Getter de f
        /// </summary>
        /// <returns>f</returns>
        public double GetF()
        {
            return f;
        }

        /// <summary>
        /// Setter de f
        /// </summary>
        /// <param name="f"></param>
        public void SetF(double f)
        {
            this.f = f;
        }

        /// <summary>
        /// Getter de g
        /// </summary>
        /// <returns>g</returns>
        public double GetG()
        {
            return g;
        }

        /// <summary>
        /// Setter de g
        /// </summary>
        /// <param name="g"></param>
        public void SetG(double g)
        {
            this.g = g;
        }

        ///<summary>
        ///Verifica se dois nodos são iguais. Desconsidera o nodo pai
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

        /// <summary>
        /// Getter de parentNode
        /// </summary>
        /// <returns>nodo pai</returns>
        public Node GetParentNode()
        {
            return parentNode;
        }

        /// <summary>
        /// Setter de parentNode
        /// </summary>
        /// <param name="parentNode"></param>
        public void SetParentNode(Node parentNode)
        {
            this.parentNode = parentNode;
        }

        /// <summary>
        /// Getter de interval
        /// </summary>
        /// <returns>intervalo do nodo</returns>
        public Interval GetInterval()
        {
            return interval;
        }

        /// <summary>
        /// Setter de interval
        /// </summary>
        /// <param name="interval"></param>
        public void SetInterval(Interval interval)
        {
            this.interval = interval;
        }

        /// <summary>
        /// Getter de root
        /// </summary>
        /// <returns>raiz do nodo</returns>
        public Vector2 GetRoot()
        {
            return root;
        }

        /// <summary>
        /// Setter de root
        /// </summary>
        /// <param name="root"></param>
        public void SetRoot(Vector2 root)
        {
            this.root = root;
        }

        /// <summary>
        /// Adiciona um nodo a uma lista de nodos
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="node"></param>
        public static void AddNodeToList(List<Node> nodeList, Node node)
        {
            if (NotExists(nodeList, node))
            {
                nodeList.Add(node);
            }
        }

        /// <summary>
        /// Adiciona uma lista de nodos a outra lista de nodos
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="source"></param>
        public static void AddNodeListToList(List<Node> dest, List<Node> source)
        {
            foreach (Node n in source)
            {
                AddNodeToList(dest, n);
            }
        }

        /// <summary>
        /// Verifica se um nodo não está numa lista de nodos
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gera um código para uma tabela Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int result = interval != null ? interval.GetHashCode() : 0;
            result = 31 * result + (root != null ? root.GetHashCode() : 0);
            return result;
        }

        /// <summary>
        /// Converte um nodo para uma string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "root: " + root.ToString() + " " + interval.ToString();
        }
    }
}
