using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Core
{
    public static class GraphAlgorithms
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

        public static bool TryGetPiecesByGammaAlgorithm(BigInteger[] graph, out List<PieceOfGraph> pieces)
        {
            if (!TryGetRandomCycle(graph, out var cycle))
            {
                pieces = null;
                return false;
            }

            //todo: remove this example
            cycle = new int[] { 0, 1, 2, 3, 4, 5 };

            Segment gPlane;
            (gPlane, pieces) = CalcGPlaneAndStartedPieces(graph, cycle);

            var segments = GetStartedSegments(graph, gPlane);

            while (segments.Any())
            {
                if (!TryGetMinSegment(segments, pieces, gPlane, out var minSegment, out var connectedPiece))
                {
                    return false;
                }

                var chain = SearchRandomPathInSegmentConnectedWithPiece(minSegment, connectedPiece);

                gPlane = AddChainToGPlane(gPlane, chain);
                (var firstPiece, var secondPiece) = SeparatePieceByChainOnTwoPieces(connectedPiece, chain);

                pieces.Remove(connectedPiece);
                pieces.Add(firstPiece);
                pieces.Add(secondPiece);

                var newSegments = RecalculateSegment(minSegment, gPlane);
                segments.Remove(minSegment);
                segments.AddRange(newSegments);

                var a = 3;
            }

            foreach (var piece in pieces)
                Debug.WriteLine(string.Join(" ", piece.Vertexes));

            return true;
        }

        private static List<Segment> RecalculateSegment(Segment segment, Segment gPlane)
        {
            var result = new List<Segment>();

            var vertexCount = segment.Value.Count;

            var globalUsed = new BigInteger[vertexCount];
            var vertexMaskLimit = new BigInteger(1) << vertexCount;

            for (BigInteger fromMask = 1; fromMask < vertexMaskLimit; fromMask <<= 1)
            {
                var from = _numbersByMasks[fromMask];

                if (gPlane[from] == 0)
                {
                    continue;
                }

                if (segment[from] == 0)
                {
                    continue;
                }

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((globalUsed[from] & toMask) != 0)
                    {
                        continue;
                    }

                    if ((segment[from] & toMask) == 0)
                    {
                        continue;
                    }

                    if ((gPlane[from] & toMask) != 0)
                    {
                        continue;
                    }

                    if (gPlane[_numbersByMasks[toMask]] != 0)
                    {
                        var currentSegment = new BigInteger[vertexCount];

                        currentSegment[from] |= toMask;
                        currentSegment[_numbersByMasks[toMask]] |= fromMask;

                        globalUsed[from] |= toMask;
                        globalUsed[_numbersByMasks[toMask]] |= fromMask;

                        result.Add(new Segment(currentSegment));

                        continue;
                    }

                    var localUsed = new BigInteger[vertexCount];
                    Dfs(toMask, localUsed, 0);

                    result.Add(new Segment(localUsed));
                }
            }

            return result;

            void Dfs(BigInteger vertexMask, BigInteger[] localUsed, BigInteger used)
            {
                used |= vertexMask;

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((segment[_numbersByMasks[vertexMask]] & toMask) == 0)
                    {
                        continue;
                    }

                    localUsed[_numbersByMasks[vertexMask]] |= toMask;
                    localUsed[_numbersByMasks[toMask]] |= vertexMask;

                    globalUsed[_numbersByMasks[vertexMask]] |= toMask;
                    globalUsed[_numbersByMasks[toMask]] |= vertexMask;

                    if ((used & toMask) != 0)
                    {
                        continue;
                    }

                    if (gPlane.Value[_numbersByMasks[toMask]] != 0)
                    {
                        continue;
                    }

                    Dfs(toMask, localUsed, used);
                }
            }
        }

        private static (PieceOfGraph first, PieceOfGraph second) SeparatePieceByChainOnTwoPieces(PieceOfGraph piece, List<int> chain)
        {
            var first = new List<int>();
            var second = new List<int>();

            var startIndex = 0;
            for (; piece[startIndex] != chain[0]; startIndex++) { }
            for (int i = startIndex; piece[i] != chain[^1]; i = (i + 1) % piece.Vertexes.Count)
            {
                first.Add(piece[i]);
            }
            for (int i = chain.Count - 1; i > 0; i--)
            {
                first.Add(chain[i]);
            }

            startIndex = 0;
            for (; piece[startIndex] != chain[^1]; startIndex++) { }
            for (int i = startIndex; piece[i] != chain[0]; i = (i + 1) % piece.Vertexes.Count)
            {
                second.Add(piece[i]);
            }
            for (int i = 0; i < chain.Count - 1; i++)
            {
                second.Add(chain[i]);
            }

            return (new PieceOfGraph(first), new PieceOfGraph(second));
        }

        private static Segment AddChainToGPlane(Segment gPlane, List<int> chain)
        {
            for (int i = 0; i < chain.Count - 1; i++)
            {
                var maskFrom = new BigInteger(1) << chain[i];
                var maskTo = new BigInteger(1) << chain[i + 1];

                gPlane.AddEdge(chain[i], maskTo);
                gPlane.AddEdge(chain[i + 1], maskFrom);
            }

            return gPlane;
        }

        private static List<int> SearchRandomPathInSegmentConnectedWithPiece(Segment segment, PieceOfGraph piece)
        {
            var startVertexMask = Utils.GetMinMaskValueFromSetMask(segment.VertexesMask & piece.VertexesMask);
            var result = new List<int>();

            BigInteger endVertexMask = -1;
            BigInteger vertexMaskLimit = new BigInteger(1) << segment.Value.Count;

            var prevs = new int[segment.Value.Count];
            BigInteger used = 0;

            Dfs(startVertexMask);

            for (int vertex = _numbersByMasks[endVertexMask]; vertex != _numbersByMasks[startVertexMask]; vertex = prevs[vertex])
            {
                result.Add(vertex);
            }
            result.Add(_numbersByMasks[startVertexMask]);

            return result;

            void Dfs(BigInteger vertexMask)
            {
                used |= vertexMask;

                if (endVertexMask != -1)
                {
                    return;
                }

                for (BigInteger toMask = 1; toMask <= vertexMaskLimit; toMask <<= 1)
                {
                    if ((used & toMask) != 0)
                    {
                        continue;
                    }

                    if ((segment[_numbersByMasks[vertexMask]] & toMask) == 0)
                    {
                        continue;
                    }

                    prevs[_numbersByMasks[toMask]] = _numbersByMasks[vertexMask];

                    if ((toMask & piece.VertexesMask) != 0)
                    {
                        endVertexMask = toMask;
                        return;
                    }

                    Dfs(toMask);
                }
            }
        }

        private static bool TryGetMinSegment(List<Segment> segments, List<PieceOfGraph> pieces, Segment gPlane, out Segment resultSegment, out PieceOfGraph connectedPiece)
        {
            resultSegment = null;
            connectedPiece = null;
            var totalCount = Int32.MaxValue;

            foreach (var segment in segments)
            {
                var currentCount = 0;
                var currentVertexesMask = segment.VertexesMask & gPlane.VertexesMask;
                PieceOfGraph currentConnectedPiece = null;

                foreach (var piece in pieces)
                {
                    if ((currentVertexesMask & piece.VertexesMask) == currentVertexesMask)
                    {
                        currentCount++;
                        currentConnectedPiece = piece;
                    }
                }

                if (currentCount < totalCount)
                {
                    totalCount = currentCount;
                    resultSegment = segment;
                    connectedPiece = currentConnectedPiece;
                }
            }

            return totalCount > 0;
        }

        private static (Segment gPlane, List<PieceOfGraph> pieces) CalcGPlaneAndStartedPieces(BigInteger[] graph, int[] cycle)
        {
            BigInteger[] gPlane = new BigInteger[graph.Length];
            var pieces = new List<PieceOfGraph>();

            for (int i = 0; i < 2; i++)
            {
                var pieceBuilder = new PieceOfGraphBuilder();

                foreach (var vertex in cycle)
                {
                    pieceBuilder.AddVertex(vertex);
                }

                pieces.Add(pieceBuilder.Build());
            }

            for (int i = 0; i < cycle.Length; i++)
            {
                var fromMask = new BigInteger(1) << cycle[i];
                var toMask = new BigInteger(1) << cycle[(i + 1) % cycle.Length];

                gPlane[cycle[i]] |= toMask;
                gPlane[cycle[(i + 1) % cycle.Length]] |= fromMask;
            }

            return (new Segment(gPlane), pieces);
        }

        private static List<Segment> GetStartedSegments(BigInteger[] graph, Segment gPlane)
        {
            var result = new List<Segment>();

            var globalUsed = new BigInteger[graph.Length];
            var vertexMaskLimit = new BigInteger(1) << graph.Length;

            for (BigInteger fromMask = 1; fromMask < vertexMaskLimit; fromMask <<= 1)
            {
                var from = _numbersByMasks[fromMask];

                if (gPlane.Value[from] == 0)
                {
                    continue;
                }

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((globalUsed[from] & toMask) != 0)
                    {
                        continue;
                    }

                    if ((graph[from] & toMask) == 0)
                    {
                        continue;
                    }

                    if ((gPlane.Value[from] & toMask) != 0)
                    {
                        continue;
                    }

                    if (gPlane.Value[_numbersByMasks[toMask]] != 0)
                    {
                        var segment = new BigInteger[graph.Length];

                        segment[from] |= toMask;
                        segment[_numbersByMasks[toMask]] |= fromMask;

                        globalUsed[from] |= toMask;
                        globalUsed[_numbersByMasks[toMask]] |= fromMask;

                        result.Add(new Segment(segment));

                        continue;
                    }

                    var localUsed = new BigInteger[graph.Length];
                    Dfs(toMask, localUsed, 0);

                    result.Add(new Segment(localUsed));
                }
            }

            return result;

            void Dfs(BigInteger vertexMask, BigInteger[] localUsed, BigInteger used)
            {
                used |= vertexMask;

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((graph[_numbersByMasks[vertexMask]] & toMask) == 0)
                    {
                        continue;
                    }

                    localUsed[_numbersByMasks[vertexMask]] |= toMask;
                    localUsed[_numbersByMasks[toMask]] |= vertexMask;

                    globalUsed[_numbersByMasks[vertexMask]] |= toMask;
                    globalUsed[_numbersByMasks[toMask]] |= vertexMask;

                    if ((used & toMask) != 0)
                    {
                        continue;
                    }

                    if (gPlane.Value[_numbersByMasks[toMask]] != 0)
                    {
                        continue;
                    }

                    Dfs(toMask, localUsed, used);
                }
            }
        }

        public static bool TryGetRandomCycle(BigInteger[] graph, out int[] cycle)
        {
            BigInteger vertexMaskLimit = new BigInteger(1) << graph.Length;
            int[] colors = new int[graph.Length];
            int[] prevs = Enumerable.Repeat(-1, graph.Length).ToArray();

            int start = -1;
            int end = -1;

            if (!Dfs(1))
            {
                cycle = null;
                return false;
            }

            var result = new List<int>();
            result.Add(start);
            for (int vertex = end; vertex != start; vertex = prevs[vertex])
            {
                result.Add(vertex);
            }
            result.Add(start);
            result.Reverse();

            cycle = result.ToArray();
            return true;

            bool Dfs(BigInteger vertexMask)
            {
                colors[_numbersByMasks[vertexMask]] = 1;

                for (BigInteger toMask = 1; toMask < vertexMaskLimit; toMask <<= 1)
                {
                    if ((graph[_numbersByMasks[vertexMask]] & toMask) == 0)
                    {
                        continue;
                    }

                    if (colors[_numbersByMasks[toMask]] == 0)
                    {
                        prevs[_numbersByMasks[toMask]] = _numbersByMasks[vertexMask];

                        if (Dfs(toMask))
                        {
                            return true;
                        }
                    }
                    else if (colors[_numbersByMasks[toMask]] == 1 && _numbersByMasks[toMask] != prevs[_numbersByMasks[vertexMask]])
                    {
                        start = _numbersByMasks[toMask];
                        end = _numbersByMasks[vertexMask];

                        return true;
                    }
                }

                colors[_numbersByMasks[vertexMask]] = 2;
                return false;
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
                    if ((used & toMask) == 0 && (graph[_numbersByMasks[vertexMask]] & toMask) != 0)
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
                    if ((graph[_numbersByMasks[vertexMask]] & toMask) == 0)
                    {
                        continue;
                    }

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
