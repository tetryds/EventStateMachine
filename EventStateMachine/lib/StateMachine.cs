﻿using System;
using System.Collections.Generic;

namespace tetryds.Tools
{
    public class StateMachine<TState, TEvent, TBehav>
    {
        Dictionary<TState, TBehav> stateMap = new Dictionary<TState, TBehav>();

        Dictionary<TEvent, TransitionMap> eventMap = new Dictionary<TEvent, TransitionMap>();

        TState current;

        public TState Current
        {
            get => current;
            private set
            {
                current = value;
                StateChanged?.Invoke(current);
            }
        }

        public TBehav Behavior => stateMap[current];

        public event Action<TState> StateChanged;

        public StateMachine(TState initial)
        {
            AddState(initial);
            Current = initial;
        }

        public StateMachine(TState initial, TBehav behavior)
        {
            AddState(initial, behavior);
            Current = initial;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key)
        {
            AddState(key, default);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key, TBehav behavior)
        {
            stateMap.Add(key, behavior);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to)
        {
            AddTransition(key, from, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(from))
                throw new Exception("Attempting to add transition FROM unknown state. Add state before creating transitions.");

            if (!stateMap.ContainsKey(to))
                throw new Exception("Attempting to add transition TO unknown state. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap());

            TransitionMap tMap = eventMap[key];
            tMap.AddTransition(from, to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(to))
                throw new Exception($"Attempting to add transition TO unknown state '{to}'. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap());

            eventMap[key].SetGlobalTransition(to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to)
        {
            AddGlobalTransition(key, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> SetState(TState key)
        {
            if (!stateMap.ContainsKey(key))
                throw new Exception($"Cannot set unknown state '{key}'. Make sure it has been added.");

            Current = key;
            return this;
        }

        public void RaiseEvent(TEvent eventKey)
        {
            if (eventMap.TryGetValue(eventKey, out TransitionMap map)
                && map.TryGetTransition(Current, out Transition transition))
            {
                Current = transition.Target;
                transition.Trigger();
            }
        }

        //Internal Classes
        private class Transition
        {
            public TState Target { get; }
            private Action trigger;

            public Transition(TState target, Action trigger)
            {
                Target = target;
                this.trigger = trigger;
            }

            public void Trigger() => trigger?.Invoke();
        }

        private class TransitionMap
        {
            Dictionary<TState, Transition> transitionMap = new Dictionary<TState, Transition>();
            Transition global;

            public void SetGlobalTransition(TState to, Action trigger)
            {
                global = new Transition(to, trigger);
            }

            public void AddTransition(TState from, TState to, Action trigger)
            {
                Transition transition = new Transition(to, trigger);
                if (transitionMap.ContainsKey(from))
                    throw new Exception($"Transition from '{from}' to '{to}' already exists!");
                transitionMap.Add(from, transition);
            }

            public bool TryGetTransition(TState from, out Transition transition)
            {
                if (transitionMap.TryGetValue(from, out transition))
                {
                    return true;
                }
                if (global != null)
                {
                    transition = global;
                    return true;
                }
                return false;
            }
        }
    }
}