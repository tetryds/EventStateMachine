using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using tetryds.Tools;

namespace tetryds.Tests
{
    [TestFixture]
    public class StateMachineTransitionsTests
    {
        StateMachine<string, string, Action> stateMachine;

        event Action<string> StateChanged;
        event Action<string> StateUpdated;
        event Action<string> TransitionTriggered;

        [SetUp]
        public void SetUp()
        {
            stateMachine = new StateMachine<string, string, Action>("0", () => StateUpdated?.Invoke("0"))
                .AddState("1", () => StateUpdated?.Invoke("1"))
                .AddState("2", () => StateUpdated?.Invoke("2"))
                .AddState("3", () => StateUpdated?.Invoke("3"))
                .AddTransition("0->1", "0", "1", () => TransitionTriggered?.Invoke("0->1"))
                .AddTransition("1->1", "1", "1", () => TransitionTriggered?.Invoke("1->1"))
                .AddTransition("1->2", "1", "2", () => TransitionTriggered?.Invoke("1->2"))
                .AddTransition("2->3", "2", "3", () => TransitionTriggered?.Invoke("2->3"))
                .AddGlobalTransition("x->3", "3", () => TransitionTriggered?.Invoke("x->3"));

            stateMachine.StateChanged += state => StateChanged?.Invoke(state);
        }

        [Test]
        public void DefaultStateExists()
        {
            Assert.AreEqual("0", stateMachine.Current, "Current state differs from expected state");
        }

        [Test]
        public void SetState()
        {
            List<string> stateChangeResults = new List<string>();
            StateChanged += state => stateChangeResults.Add(state);
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.SetState("2");
            stateMachine.Behavior();

            List<string> expectedStateChanges = new List<string> { "2" };
            List<string> expectedUpdates = new List<string> { "0", "2" };
            List<string> expectedTransitions = new List<string> { };

            Assert.AreEqual("2", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedStateChanges, stateChangeResults);
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateUpdates()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.Behavior();
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "0", "0" };
            List<string> expectedTransitions = new List<string> { };

            Assert.AreEqual("0", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }


        [Test]
        public void StateTransitions()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.RaiseEvent("0->1");

            List<string> expectedUpdates = new List<string> { };
            List<string> expectedTransitions = new List<string> { "0->1" };

            Assert.AreEqual("1", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateTransitionsAndUpdates()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->1");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "1" };
            List<string> expectedTransitions = new List<string> { "0->1" };

            Assert.AreEqual("1", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateGlobalTransition()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("x->3");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "3" };
            List<string> expectedTransitions = new List<string> { "x->3" };

            Assert.AreEqual("3", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateDoesntTransitionIfNoTarget()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->3");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "0" };
            List<string> expectedTransitions = new List<string> { };

            Assert.AreEqual("0", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateDoesntTransitionIfNoSource()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("2->3");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "0" };
            List<string> expectedTransitions = new List<string> { };

            Assert.AreEqual("0", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateTransitionsAndUpdatesMultipleTimes()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->1");
            stateMachine.Behavior();
            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->1");
            stateMachine.Behavior();
            stateMachine.RaiseEvent("1->2");
            stateMachine.RaiseEvent("2->3");
            stateMachine.RaiseEvent("1->2");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "1", "1", "1", "3" };
            List<string> expectedTransitions = new List<string> { "0->1", "1->2", "2->3" };

            Assert.AreEqual("3", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateTransitionToSelf()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->1");
            stateMachine.Behavior();
            stateMachine.RaiseEvent("1->1");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "1", "1" };
            List<string> expectedTransitions = new List<string> { "0->1", "1->1" };

            Assert.AreEqual("1", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }

        [Test]
        public void StateTransitionToSelfGlobal()
        {
            List<string> updateResults = new List<string>();
            StateUpdated += stateKey => updateResults.Add(stateKey);
            List<string> transitionResults = new List<string>();
            TransitionTriggered += transitionKey => transitionResults.Add(transitionKey);

            stateMachine.Behavior();
            stateMachine.RaiseEvent("0->1");
            stateMachine.RaiseEvent("1->2");
            stateMachine.RaiseEvent("2->3");
            stateMachine.Behavior();
            stateMachine.RaiseEvent("x->3");
            stateMachine.Behavior();

            List<string> expectedUpdates = new List<string> { "0", "3", "3" };
            List<string> expectedTransitions = new List<string> { "0->1", "1->2", "2->3", "x->3" };

            Assert.AreEqual("3", stateMachine.Current, "Current state differs from expected state");
            CollectionAssert.AreEqual(expectedUpdates, updateResults);
            CollectionAssert.AreEqual(expectedTransitions, transitionResults);
        }
    }
}
