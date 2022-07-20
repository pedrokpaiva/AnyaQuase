using System.Collections;
using System.Numerics;

namespace Anya_2d
{
    public class BaseVertex
    {
        public long id;
        public Vector2 pos;
        public int hash;

        ///<summary>
        ///Lista de arestas saindo do vértice
        ///</summary>
        private ArrayList outgoings = new ArrayList();

        ///<summary>
        ///Lista de arestas chegando no vértice
        ///</summary>
        private ArrayList incomings = new ArrayList();

        ///<summary>
        ///Lista de todas as arestas tocando o vértice
        ///</summary>
        private ArrayList touchings = new ArrayList();

        public BaseVertex(long id, Vector2 pos)
        {
            this.id = id;
            this.pos = pos;
            hash = base.GetHashCode();
        }

        ///<summary>
        ///Retorna as arestas saindo do vértice
        ///</summary>
        public ArrayList GetOutgoings()
        {
            return outgoings;
        }


        ///<summary>
        ///Retorna as arestas dos vértices vizinhos
        ///</summary>
        public ArrayList GetOutgoingNeighbors()
        {
            ArrayList on = new ArrayList();

            foreach (BaseEdge e in outgoings)
            {
                on.Add(e.end);
            }
            return on;
        }

        ///<summary>
        ///Retorna as arestas chegando no vértice
        ///</summary>
        public ArrayList GetIncomings()
        {
            return incomings;
        }

        ///<summary>
        ///Verifica se um vértice é igual a outro
        ///</summary>
        public bool Equals(BaseVertex o)
        {
            return id == o.id;
        }

        ///<summary>
        ///Retorna a aresta saindo em direção ao vértice alvo, se houver
        ///</summary>
        public BaseEdge GetOutgoingTo(BaseVertex target)
        {
            if (target == null)
            {
                return null;
            }

            foreach (BaseEdge e in outgoings)
            {
                if (e.end.Equals(target))
                {
                    return e;
                }
            }
            return null;
        }

        ///<summary>
        ///Retorna a aresta saindo do vértice start, se houver
        ///</summary>
        public BaseEdge GetIncomingFrom(BaseVertex start)
        {
            foreach (BaseEdge e in incomings)
            {
                if (e.start.Equals(start))
                {
                    return e;
                }
            }
            return null;
        }

        ///<summary>
        ///Adiciona uma aresta saindo do vértice
        ///</summary>
        public void AddOutgoing(BaseEdge e)
        {
            outgoings.Add(e);
            touchings.Add(e);
        }

        ///<summary>
        ///Adiciona uma aresta chegando no vértice
        ///</summary>
        public void AddIncoming(BaseEdge e)
        {
            incomings.Add(e);
            touchings.Add(e);
        }

        ///<summary>
        ///Remove uma aresta chegando no vértice
        ///</summary>
        public void RemoveIncoming(BaseEdge e)
        {
            incomings.Remove(e);
            touchings.Remove(e);
        }

        ///<summary>
        ///Remove uma aresta saindo do vértice
        ///</summary>
        public void RemoveOutgoing(BaseEdge e)
        {
            outgoings.Remove(e);
            touchings.Remove(e);
        }

        ///<summary>
        ///Compara o vértice com outro
        ///</summary>
        public int CompareTo(BaseVertex o)
        {
            return id.CompareTo(o.id);
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public override string ToString()
        {
            return "BaseVertex " + id + "; " + pos.ToString();

        }
    }
}
