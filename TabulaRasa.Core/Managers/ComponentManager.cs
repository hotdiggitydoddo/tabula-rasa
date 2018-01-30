using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using TabulaRasa.Core.Actions;
using TabulaRasa.Core.Contracts;
using TabulaRasa.Core.Helpers;
using TabulaRasa.Core.Objects;

namespace TabulaRasa.Core.Managers
{
    public class ComponentManager : IComponentManager
    {
        private readonly IScriptManager _scripts;

        public ComponentManager(IScriptManager scripts)
        {
            _scripts = scripts;
        }
        public Component CreateComponent(string componentName, params string[] traits)
        {
            if (traits.Any(x => !x.Contains("|")))
                throw new ArgumentException($"Config for component {componentName} is bad - traits are not formatted correctly");
            
            var scriptSource = _scripts.GetComponentScript(componentName);
            var script = new Script();

            script.Globals["Component"] = typeof(Component);
            script.Globals["Entity"] = typeof(Entity);
            script.Globals["GameWrapper"] = typeof(GameWrapper);
            script.Globals["GameAction"] = typeof(GameAction);
            script.Globals["TimedGameAction"] = typeof(TimedGameAction);
            script.DoString(scriptSource);

            var args = new Dictionary<string, string>();
            foreach(var trait in traits)
            {
                var split = trait.Split('|');
                args.Add(split[0], split[1]);
            }
            
            var component = (Component)script.Call(script.Globals["init"], script, args).UserData.Object;
            return component;
        }
    }
}