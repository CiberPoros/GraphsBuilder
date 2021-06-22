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

        #region IsConnected
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
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

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
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

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
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

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
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>()
            };

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.IsConnected(graph);

            Assert.IsFalse(result);
        }
        #endregion

        #region GetAllBridges
        [Test]
        public void GetAllBridges_OneVertexGraph_ReturnsEmptyArray()
        {
            var graph = new BigInteger[] { 0 };

            var result = GraphAlgorithms.GetAllBridges(graph);

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetAllBridges_TwoVertexesWithEdge_ReturnsOneBridge()
        {
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>()
            };

            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.GetAllBridges(graph);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void GetAllBridges_ChainWith4Wertexes_Returns3Bridges()
        {
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            adjacencyVectorGraph[1].Add(2);
            adjacencyVectorGraph[2].Add(1);

            adjacencyVectorGraph[2].Add(3);
            adjacencyVectorGraph[3].Add(2);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.GetAllBridges(graph);

            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void GetAllBridges_CycleWith4Wertexes_ReturnsEmptyArray()
        {
            var adjacencyVectorGraph = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

            adjacencyVectorGraph[0].Add(1);
            adjacencyVectorGraph[1].Add(0);

            adjacencyVectorGraph[1].Add(2);
            adjacencyVectorGraph[2].Add(1);

            adjacencyVectorGraph[2].Add(3);
            adjacencyVectorGraph[3].Add(2);

            adjacencyVectorGraph[3].Add(0);
            adjacencyVectorGraph[0].Add(3);

            var graph = Utils.AdjacencyVectorToMaskVector(adjacencyVectorGraph);

            var result = GraphAlgorithms.GetAllBridges(graph);

            Assert.IsEmpty(result);
        } 
        #endregion
    }
}
