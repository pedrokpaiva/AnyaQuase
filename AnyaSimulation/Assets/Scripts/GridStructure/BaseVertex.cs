using System.Collections.Generic;
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
        private List<BaseEdge> outgoings = new List<BaseEdge>();

        ///<summary>
        ///Lista de arestas chegando no vértice
        ///</summary>
        private List<BaseEdge> incomings = new List<BaseEdge>();

        ///<summary>
        ///Lista de todas as arestas tocando o vértice
        ///</summary>
        private List<BaseEdge> touchings = new List<BaseEdge>();

        /// <summary>
        /// Cria um vértice a partir de um id e de um ponto 2D
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        public BaseVertex(long id, Vector2 pos)
        {
            this.id = id;
            this.pos = pos;
            hash = base.GetHashCode();
        }

        /// <summary>
        /// Getter de outgoings
        /// </summary>
        /// <returns>lista de arestas saindo do vértice</returns>
        public List<BaseEdge> GetOutgoings()
        {
            return outgoings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>lista de vértices vizinhos</returns>
        public List<BaseVertex> GetOutgoingNeighbors()
        {
            List<BaseVertex> on = new List<BaseVertex>();

            foreach (BaseEdge e in outgoings)
            {
                on.Add(e.end);
            }
            return on;
        }

        /// <summary>
        /// Getter de incomings
        /// </summary>
        /// <returns>lista de arestas chegando no vértice</returns>
        public List<BaseEdge> GetIncomings()
        {
            return incomings;
        }

        /// <summary>
        /// Verifica se dois vértices são iguais, baseado no id
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool Equals(BaseVertex o)
        {
            return id == o.id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>aresta que chega no vértice target</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns>aresta que sai do vértice start e chega no vértice atual</returns>
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

        /// <summary>
        /// Adiciona uma aresta que está saindo as listas outgoings e touchings
        /// </summary>
        /// <param name="e"></param>
        public void AddOutgoing(BaseEdge e)
        {
            outgoings.Add(e);
            touchings.Add(e);
        }

        /// <summary>
        /// Adiciona uma aresta que está chegando as listas incoming e touchings
        /// </summary>
        /// <param name="e"></param>
        public void AddIncoming(BaseEdge e)
        {
            incomings.Add(e);
            touchings.Add(e);
        }

        /// <summary>
        /// Remove uma aresta que está chegando das listas incomings e touchings
        /// </summary>
        /// <param name="e"></param>
        public void RemoveIncoming(BaseEdge e)
        {
            incomings.Remove(e);
            touchings.Remove(e);
        }

        /// <summary>
        /// Remove uma aresta que está saindo das listas outgoings e touchings
        /// </summary>
        /// <param name="e"></param>
        public void RemoveOutgoing(BaseEdge e)
        {
            outgoings.Remove(e);
            touchings.Remove(e);
        }

        /// <summary>
        /// Compara duas arestas
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public int CompareTo(BaseVertex o)
        {
            return id.CompareTo(o.id);
        }

        /// <summary>
        /// Gera um código para uma tabela Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return hash;
        }

        /// <summary>
        /// Converte o vértice para string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "BaseVertex " + id + "; " + pos.ToString();

        }
    }
}
