using System.Collections.Generic;
using System.Linq;
using Priority_Queue;
using TabulaRasa.Core.Actions;

namespace TabulaRasa.Core.Objects
{
    public class Entity
    {
        private readonly SimplePriorityQueue<Component> _components;
        public int Id { get; }
        public TraitSet Traits { get; }
        public string Name { get; set; }

        public Entity(int id, string name)
        {
            Id = id;
            _components = new SimplePriorityQueue<Component>();
            Traits = new TraitSet(this);
            Traits["name"] = name;
        }
   
        public bool HandleAction(GameAction action)
        {
            action.Value = "200";
            action.Args[0] = "22";
            foreach (var component in _components)
            {
                var a = component.HandleAct(action);

                if (!component.EventSubscriptions.Contains(action.Type)) continue;
                if (!component.HandleAction(action))
                    return false;
            }
            return true;
        }

        public void AddComponent(Component comp)
        {
            _components.Enqueue(comp, comp.Priority);
            comp.OnAdded(this);
        }

        public void RemoveComponent(string name)
        {
            var comp = _components.SingleOrDefault(x => x.Name.ToLower() == name.ToLower());
            if (comp == null) return;

            comp.OnRemoved();
            _components.Remove(comp);
        }

        public Component GetComponent(string name)
        {
            var comp = _components.SingleOrDefault(x => x.Name.ToLower() == name.ToLower());
            return comp ?? null;
        }
    }
}