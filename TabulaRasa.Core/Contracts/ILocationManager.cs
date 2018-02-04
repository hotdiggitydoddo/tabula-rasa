using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Contracts
{
    public interface ILocationManager
    {
        void AddEntityToRoom(Entity e, int roomId);
        List<Entity> GetEntitiesInRegion(int regionId, Expression<Func<Entity, bool>> predicate = null);
        List<Entity> GetEntitiesInRoom(int roomId, Expression<Func<Entity, bool>> predicate = null);
        void RemoveEntityFromRoom(Entity e, int roomId);
        Room GetRoomById(int id);
        Zone GetRegionById(int id);
        Portal GetPortalById(int id);
    }
}