using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Core;
using NUnit.Framework;

namespace Tests
{
    class GraphAlgorithmsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsConnected_OneVertexGraph_ReturnsTrue()
        {
            var graph = new BigInteger[] { 0 };

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsConnected_SimpleCycle_ReturnsTrue()
        {
            var adjacencyVectorGraph = Enumerable.Repeat(new List<int>(), 5).ToList();
            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            adjacencyVectorGraph[1].Add(2);
            adjacencyVectorGraph[2].Add(1);

            adjacencyVectorGraph[2].Add(3);
            adjacencyVectorGraph[3].Add(2);

            adjacencyVectorGraph[3].Add(4);
            adjacencyVectorGraph[4].Add(3);

            adjacencyVectorGraph[4].Add(0);
            adjacencyVectorGraph[0].Add(4);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsConnected_SimpleChain_ReturnsTrue()
        {
            var adjacencyVectorGraph = Enumerable.Repeat(new List<int>(), 5).ToList();
            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            adjacencyVectorGraph[1].Add(2);
            adjacencyVectorGraph[2].Add(1);

            adjacencyVectorGraph[2].Add(3);
            adjacencyVectorGraph[3].Add(2);

            adjacencyVectorGraph[3].Add(4);
            adjacencyVectorGraph[4].Add(3);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsConnected_HaveOneSeparatedVertex_ReturnsFalse()
        {
            var adjacencyVectorGraph = Enumerable.Repeat(new List<int>(), 5).ToList();
            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            adjacencyVectorGraph[1].Add(2);
            adjacencyVectorGraph[2].Add(1);

            adjacencyVectorGraph[2].Add(3);
            adjacencyVectorGraph[3].Add(2);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsConnected_TwoVertexesWithoutEdge_ReturnsFalse()
        {
            var adjacencyVectorGraph = Enumerable.Repeat(new List<int>(), 2).ToList();

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsFalse(result);
        }
    }
}
