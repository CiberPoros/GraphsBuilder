using System;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class GraphAlgorithms
    {
        private const int VertexLimit = 1000;
        private static readonly Dictionary<BigInteger, int> _numbersByMasks;

        static GraphAlgorithms()
        {
            _numbersByMasks = new Dictionary<BigInteger, int>();

            BigInteger mask = 1;

            for (int i = 0; i < VertexLimit; i++)
            {
                _numbersByMasks.Add(mask, i);

                mask <<= 1;
            }
        }

        public static bool IsConnected(BigInteger[] graph)
        {
            BigInteger vertexMaskLimit = new BigInteger(1) << graph.Length;
            BigInteger used = 0;

            Dfs(1);

            return used == vertexMaskLimit - 1;

            void Dfs(BigInteger prevVertexMask)
            {
                used |= prevVertexMask;

                for (BigInteger i = 1; i < vertexMaskLimit; i <<= 1)
                {
                    if ((used & i) == 0 && (graph[_numbersByMasks[i]] & i) != 0)
                    {
                        Dfs(i);
                    }
                }
            }
        }
    }
}
