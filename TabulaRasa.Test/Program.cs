using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger;
using TabulaRasa.Core.Actions;
using TabulaRasa.Core.Contracts;
using TabulaRasa.Core.Helpers;
using TabulaRasa.Core.Managers;
using TabulaRasa.Core.Objects;

namespace TabulaRasa.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IScriptManager scripts = new ScriptManager();
            IComponentManager components = new ComponentManager(scripts);
            IEntityManager entities = new EntityManager();
            IActionManager actions = new ActionManager();

            GameWrapper g = new GameWrapper(entities, components, actions);

            scripts.RegisterObjectType<Entity>();
            scripts.RegisterObjectType<Component>();
            scripts.RegisterObjectType<GameWrapper>();
            scripts.RegisterObjectType<TraitSet>();
            scripts.RegisterObjectType<Script>();
            scripts.RegisterObjectType<GameAction>();
            scripts.RegisterObjectType<TimedGameAction>();

            var e = g.CreateEntity("snake");
            g.HandleAction(new GameAction("addcomponent", e.Id, "health", "maxHP|150", "currHP|22"));
            g.HandleAction(new GameAction("addcomponent", e.Id, "invulnerability"));
            g.HandleAction(new GameAction("takedamage", 0, e.Id, "300"));
            // g.HandleAction(new GameAction("messageentity", 0, e.Id, "health", "takedamage", "300"));
            // g.HandleAction(new GameAction("messagecomponent", 0, e.Id, "health", "takedamage", "300"));
            // g.HandleAction(new GameAction("messagecomponent", 0, e.Id, "health", "heal", "325"));
            g.HandleAction(new GameAction("heal", 0, e.Id, "325"));
            g.HandleAction(new GameAction("removecomponent", e.Id, "health"));
        }

        
        /* public static void AssignComponent(Entity gameObj, string componentName, Dictionary<string, string> defaults)
        {
            var script = new Script();
            script.Globals["Component"] = typeof(Component);
            //script.Globals["Trait"] = typeof(AiGTrait);
            script.Globals["TraitSet"] = typeof(TraitSet);
            script.Globals["Entity"] = typeof(Entity);
            script.DoString(_components[componentName]);

            var args = new Dictionary<string, string>();
            foreach (var trait in defaults)
                args.Add(trait.Name, trait.Value);

            var cmp = (AiGComponent)script.Call(script.Globals["init"], entity, script, args).UserData.Object;
            entity.Components.Add(cmp);
        } */
    }
}
