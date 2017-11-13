﻿using NSubstitute;
using NUnit.Framework;
using Pathfindax.Nodes;
using Pathfindax.PathfindEngine;
using Pathfindax.Test.Setup;

namespace Pathfindax.Test.Tests
{
    [TestFixture]
    public class PathRequestTests
    {
        [Test]
        public void Integration_StatusFlowToSolved()
        {
            var pathfinder = MultithreadedPathfinderSetup.Substitute(1);
	        var request = PathRequest.Create(pathfinder ,Substitute.For<IDefinitionNode>(), Substitute.For<IDefinitionNode>());

            Assert.AreEqual(PathRequestStatus.Created, request.Status);
            request.StartSolvePath(pathfinder);
            Assert.AreEqual(PathRequestStatus.Solving, request.Status);
            pathfinder.Start();
            request.WaitHandle.WaitOne(1000);
            Assert.AreEqual(PathRequestStatus.Solved, request.Status);
        }

        [Test]
        public void Integration_AddCallbackAfterPathIsSolved_CallbackIsCalled()
        {
            var pathfinder = MultithreadedPathfinderSetup.Substitute(1);
            pathfinder.Start();
			var request = PathRequest.Create(pathfinder, Substitute.For<IDefinitionNode>(), Substitute.For<IDefinitionNode>());
            request.WaitHandle.WaitOne(1000);
            bool done = false;
            request.AddCallback(x =>
            {
                done = true;
            });
            Assert.AreEqual(true, done);
        }
    }
}
