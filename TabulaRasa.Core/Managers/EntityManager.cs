using System.Collections.Generic;
using System.Linq;
using TabulaRasa.Services.Contracts;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Managers
{
    public class EntityManager : IEntityManager
    {
        private readonly Dictionary<int, Entity> _entities;
        private int _nextId;
        private List<int> _usedIds;

        public EntityManager()
        {
            _entities = new Dictionary<int, Entity>();
            _usedIds = new List<int>();
        }

        public Entity CreateEntity(string name = null)
        {
            _nextId = 1;
            while (_usedIds.Contains(_nextId))
                _nextId++;
            var ent = new Entity(_nextId, name ?? $"entity #{_nextId}");
            _entities.Add(ent.Id, ent);
            _usedIds.Add(_nextId);
            return ent;
        }

        public void DestroyEntity(int id)
        {
            _entities.Remove(id);
            _usedIds.Remove(id);
        }

        public Entity GetById(int id)
        {
            if (!_entities.TryGetValue(id, out var entity))
                //TODO: logging no entity found
                return null;
            return entity;
        }

        public List<Entity> GetEntitiesWithTrait(string trait)
        {
            return _entities.Values.Where(x => !string.IsNullOrWhiteSpace(x.Traits[trait])).ToList();
        }
    }
}