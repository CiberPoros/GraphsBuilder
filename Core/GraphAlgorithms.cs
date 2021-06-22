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

            void Dfs(BigInteger vertexMask)
            {
                used |= vertexMask;

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((used & toMask) == 0 && (graph[_numbersByMasks[toMask]] & toMask) != 0)
                    {
                        Dfs(toMask);
                    }
                }
            }
        }

        public static (BigInteger fromMask, BigInteger toMask)[] GetAllBridges(BigInteger[] graph)
        {
            BigInteger vertexMaskLimit = new BigInteger(1) << graph.Length;
            BigInteger used = 0;
            int[] timeOfIn = new int[graph.Length];
            int[] dynamicTime = new int[graph.Length];
            int currentTime = 0;

            List<(BigInteger fromMask, BigInteger toMask)> result = new List<(BigInteger fromMask, BigInteger toMask)>();

            Dfs(1, -1);

            return result.ToArray();

            void Dfs(BigInteger vertexMask, BigInteger prevVertexMask)
            {
                used |= vertexMask;
                timeOfIn[_numbersByMasks[vertexMask]] = dynamicTime[_numbersByMasks[vertexMask]] = currentTime++;

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if (toMask == prevVertexMask)
                    {
                        continue;
                    }

                    if ((used & toMask) != 0)
                    {
                        dynamicTime[_numbersByMasks[vertexMask]] = Math.Min(dynamicTime[_numbersByMasks[vertexMask]], timeOfIn[_numbersByMasks[toMask]]);
                    }
                    else
                    {
                        Dfs(toMask, vertexMask);

                        dynamicTime[_numbersByMasks[vertexMask]] = Math.Min(dynamicTime[_numbersByMasks[vertexMask]], dynamicTime[_numbersByMasks[toMask]]);

                        if (dynamicTime[_numbersByMasks[toMask]] > timeOfIn[_numbersByMasks[vertexMask]])
                        {
                            result.Add((vertexMask, toMask));
                        }
                    }
                }
            }
        }
    }
}
