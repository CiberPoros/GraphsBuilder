using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class PieceOfGraph
    {
        private readonly List<int> _vertexes;
        private BigInteger? _vertexesMask;
        
        public IReadOnlyList<int> Vertexes => _vertexes;
        public BigInteger VertexesMask 
        { 
            get
            {
                if (!_vertexesMask.HasValue)
                {
                    _vertexesMask = 0;

                    foreach (var vertex in Vertexes)
                    {
                        _vertexesMask |= new BigInteger(1) << vertex;
                    }
                }

                return _vertexesMask.Value;
            }
        }

        public PieceOfGraph(List<int> vertexes)
        {
            _vertexes = vertexes;
            _vertexesMask = null;
        }

        public int this[int index] => Vertexes[index];

        public void AddVertex(int vertex)
        {
            _vertexes.Add(vertex);
            _vertexesMask = null;
        }
    }

    public class PieceOfGraphBuilder
    {
        private readonly List<int> _vertexes = new List<int>();

        public void AddVertex(int vertex) => _vertexes.Add(vertex);

        public PieceOfGraph Build() => new(_vertexes);
    }
}
