using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public static class Utils
    {
        public static BigInteger[] AdjacencyVectorToMaskVector(List<List<int>> graph)
        {
            var result = new BigInteger[graph.Count];

            for (int i = 0; i < graph.Count; i++)
            {
                for (int j = 0; j < graph[i].Count; j++)
                {
                    var toMask = new BigInteger(1) << graph[i][j];

                    result[i] |= toMask;
                }
            }

            return result;
        }
    }
}
