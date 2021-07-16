using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using tetryds.Tools;

namespace tetryds.Tests
{
    [TestFixture]
    public class StateMachineConfigurationsTests
    {
        [Test]
        public void SkipStep()
        {
            List<string> stateChangeResults = new List<string>();
            List<string> updateResults = new List<string>();
            List<string> transitionResults = new List<string>();

            StateMachine<string, string, Action> stateMachine = new StateMachine<string, string, Action>("0", () => updateResults.Add("0"))
                .AddState("1", () => updateResults.Add("1"))
                .AddState("2", () => updateResults.Add("2"))
                .AddState("3", () => updateResults.Add("3"))
                .AddTransition("next", "0", "1", () => transitionResults.Add("next"))
                .AddTransition("next", "1", "2", () => transitionResults.Add("next"))
                .AddTransition("next", "2", "3", () => transitionResults.Add("next"));

            stateMachine.StateChanged += state => stateChangeResults.Add(state);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("next");
            stateMachine.Behavior();
            stateMachine.RaiseEvent("next");
            stateMachine.Behavior();
            stateMachine.RaiseEvent("next");
            stateMachine.Behavior();

            List<string> expectedStateChanges = new List<string> { "1", "2", "3" };
            List<string> expectedUpdates = new List<string> { "0", "1", "2", "3" };
            List<string> expectedTransitions = new List<string> { "next", "next", "next" };

            Assert.AreEqual("3", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedStateChanges, stateChangeResults);
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void MultipleTransitions()
        {
            List<string> stateChangeResults = new List<string>();
            List<string> transitionResults = new List<string>();

            StateMachine<string, string, object> stateMachine = new StateMachine<string, string, object>("0")
                .AddState("1")
                .AddTransition("a", "0", "1", () => transitionResults.Add("a"))
                .AddTransition("b", "0", "1", () => transitionResults.Add("b"))
                .AddTransition("c", "0", "1", () => transitionResults.Add("c"))
                .AddTransition("d", "0", "1", () => transitionResults.Add("d"))

                .AddTransition("e", "1", "0", () => transitionResults.Add("e"))
                .AddTransition("f", "1", "0", () => transitionResults.Add("f"))
                .AddTransition("g", "1", "0", () => transitionResults.Add("g"))
                .AddTransition("h", "1", "0", () => transitionResults.Add("h"));

            stateMachine.StateChanged += state => stateChangeResults.Add(state);

            stateMachine.RaiseEvent("a");
            stateMachine.RaiseEvent("e");
            stateMachine.RaiseEvent("b");
            stateMachine.RaiseEvent("f");
            stateMachine.RaiseEvent("c");
            stateMachine.RaiseEvent("g");
            stateMachine.RaiseEvent("d");
            stateMachine.RaiseEvent("h");

            List<string> expectedStateChanges = new List<string> { "1", "0", "1", "0", "1", "0", "1", "0" };
            List<string> expectedTransitions = new List<string> { "a", "e", "b", "f", "c", "g", "d", "h" };

            Assert.AreEqual("0", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedStateChanges, stateChangeResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }
    }
}
