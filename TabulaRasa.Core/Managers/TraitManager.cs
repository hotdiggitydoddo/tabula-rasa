using System.Collections.Generic;

namespace TabulaRasa.Core.Managers
{
    public class TraitManager
    {
        private readonly Dictionary<int, Dictionary<string, string>> _entityTraits;

        public TraitManager()
        {
            _entityTraits = new Dictionary<int, Dictionary<string, string>>();
        }

        public void OnEntityCreated(int entityId)
        {
            _entityTraits.Add(entityId, new Dictionary<string, string>());
        }
        
        public void OnEntityDestroyed(int entityId)
        {
            _entityTraits.Remove(entityId);
        }

        public bool HasTrait(int entityId, string trait)
        {
            return _entityTraits[entityId].ContainsKey(trait);
        }
        public string GetTrait(int entityId, string trait)
        {
            return _entityTraits[entityId]?[trait];
        }

        public string AddOrUpdateTrait(int entityId, string trait, string val)
        {
            var traits = _entityTraits[entityId];
            if (!traits.ContainsKey(trait))
            {
                traits.Add(trait, val);
                return null;
            }
            var old = traits[trait];
            traits[trait] = val;
            return old;
        }
    }
}