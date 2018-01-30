using System.Collections.Generic;
using TabulaRasa.Core.Objects;

namespace TabulaRasa.Core.Contracts
{
    public interface IEntityManager
    {
        Entity CreateEntity(string name = null);
        Entity GetById(int id);
        void DestroyEntity(int id);
        List<Entity> GetEntitiesWithTrait(string trait);
    }
}