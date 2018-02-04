using System.Collections.Generic;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Contracts
{
    public interface IEntityManager
    {
        Entity CreateEntity(string name = null);
        Entity GetById(int id);
        void DestroyEntity(int id);
        List<Entity> GetEntitiesWithTrait(string trait);
    }
}