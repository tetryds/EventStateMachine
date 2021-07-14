using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using tetryds.Tools;

namespace tetryds.Tests
{
    [TestFixture]
    public class StateMachineExceptionTests
    {
        [Test]
        public void ExceptionSetInvalidState()
        {
            StateMachine<string, string> stateMachine = new StateMachine<string, string>("0");

            Assert.Throws<Exception>(() => stateMachine.SetState("1"));
        }

        [Test]
        public void ExceptionAddInvalidLocalTransition()
        {
            StateMachine<string, string> stateMachine = new StateMachine<string, string>("0");

            Assert.Throws<Exception>(() => stateMachine.AddTransition("0->1", "0", "1"));
            Assert.Throws<Exception>(() => stateMachine.AddTransition("0->1", "1", "2"));
        }

        [Test]
        public void ExceptionAddInvalidGlobalTransition()
        {
            StateMachine<string, string> stateMachine = new StateMachine<string, string>("0");

            Assert.Throws<Exception>(() => stateMachine.AddGlobalTransition("x->1", "1"));
        }

        [Test]
        public void ExceptionAddInvalidDuplicateTransitionFrom()
        {
            StateMachine<string, string> stateMachine = new StateMachine<string, string>("0")
                .AddState("1")
                .AddState("2");

            stateMachine.AddTransition("0->x", "0", "1");
            Assert.Throws<Exception>(() => stateMachine.AddTransition("0->x", "0", "2"));
        }

        [Test]
        public void ExceptionAddInvalidDuplicateTransitionTo()
        {
            StateMachine<string, string> stateMachine = new StateMachine<string, string>("0")
                .AddState("1");

            stateMachine.AddTransition("0->1", "0", "1");
            Assert.Throws<Exception>(() => stateMachine.AddTransition("0->1", "0", "1"));
        }
    }
}
