using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Segment
    {
        private readonly BigInteger[] _value;
        private BigInteger? _vertexesMask;

        public IReadOnlyList<BigInteger> Value => _value;
        public BigInteger VertexesMask
        {
            get
            {
                if (!_vertexesMask.HasValue)
                {
                    _vertexesMask = 0;

                    for (int i = 0; i < Value.Count; i++)
                    {
                        if (Value[i] != 0)
                        {
                            _vertexesMask |= new BigInteger(1) << i;
                        }
                    }
                }

                return _vertexesMask.Value;
            }
        }

        public Segment(BigInteger[] value)
        {
            _value = value;
            _vertexesMask = null;
        }

        public BigInteger this[int index] => Value[index];

        public void AddEdge(int from, BigInteger toMask)
        {
            _value[from] |= toMask;
            _vertexesMask = null;
        }
    }
}
