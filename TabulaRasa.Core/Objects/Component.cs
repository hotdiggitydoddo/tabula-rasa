using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using TabulaRasa.Core.Actions;

namespace TabulaRasa.Core.Objects
{
    public class Component
    {
        private readonly Script _script;
        public string[] EventSubscriptions;
        public int Priority { get; private set; }
        public string Name { get; private set; }
        public Entity Owner { get; private set; }
        public Component(string name, Script script, int priority = 1000, params string[] eventSubscriptions)
        {
            Name = name;
            Priority = priority;
            _script = script;
            EventSubscriptions = eventSubscriptions.Skip(1).ToArray();
        }

        public void OnAdded(Entity entity)
        {
            Owner = entity;
            _script.Call(_script.Globals["onAdded"]);
        }

        public void OnRemoved()
        {
            _script.Call(_script.Globals["onRemoved"]);
        }

        public bool HandleAction(GameAction action)
        {
            return _script.Call(_script.Globals["handleAction"], action).Boolean;
        }

        public GameAction HandleAct(GameAction action)
        {
             return (GameAction)_script.Call(_script.Globals["handleAction"], action).UserData.Object;
        }

    }
}