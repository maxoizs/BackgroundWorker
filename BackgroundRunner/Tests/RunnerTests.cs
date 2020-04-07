using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using BackgroundRunner.Runner;

namespace BackgroundRunner.Tests
{

    [TestFixture]
    public class RunnerTests
    {
        [Test]
        public void GiveCancelRun_ShouldStopExecuting()
        {
            var console = new Mock<IOutput>();
            var runner = new Worker(console.Object);
            runner.Add(Act1);
            runner.Add(Act2);
            runner.Run();
            runner.Cancel();

            Thread.Sleep(3000);
            Assert.That(runner.Executed, Has.Count.EqualTo(1));
        }

        [Test]
        public void GiveActionToRun_ShouldExecuteInSequence()
        {
            var console = new Mock<IOutput>();
            var runner = new Worker(console.Object);
            runner.Add(Act1);
            runner.Add(Act2);
            runner.Run();
            Thread.Sleep(3000);

            Assert.That(runner.Executed[0], Is.EqualTo("Act1"));
            Assert.That(runner.Executed[1], Is.EqualTo("Act2"));
        }

        [Test]
        public void GiveActionToRun_CanAddActionAnyTime()
        {
            var console = new Mock<ConsoleOutput>();
            var runner = new Worker(console.Object);
            var result = string.Empty;
            Action<string> act = (string x) =>
            {
                console.Object.Write(x);
            };

            runner.Add(Act1);
            runner.Add(Act2);
            runner.Run();
            Thread.Sleep(3000);
            runner.Add(Act1);
            runner.Add(Act2);
            runner.Run();

            Thread.Sleep(3000);
            Assert.That(runner.Executed, Has.Count.EqualTo(4));
        }

        public void Act1()
        {
            Console.WriteLine("Act 1 ");
            Thread.Sleep(1000);
        }
        public void Act2()
        {
            Console.WriteLine("Act 2 ");
        }
    }
}
