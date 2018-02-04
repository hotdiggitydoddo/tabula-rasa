using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TabulaRasa.Services.Contracts;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Managers
{
    public class LocationManager : ILocationManager
    {
        private readonly Dictionary<int, Zone> _regions;
        private readonly Dictionary<int, Room> _rooms;
        private readonly Dictionary<int, Portal> _portals;

        private int _nextId;
        private List<int> _usedIds;

        public LocationManager()
        {
            _regions = new Dictionary<int, Zone>();
            _rooms = new Dictionary<int, Room>();
            _portals = new Dictionary<int, Portal>();

            _usedIds = new List<int>();
            _nextId = 1;
        }

        public List<Entity> GetEntitiesInRoom(int roomId, Expression<Func<Entity, bool>> predicate = null)
        {
            if (predicate == null)
                return new List<Entity>(_rooms[roomId].Entities);

            var query = _rooms[roomId].Entities.AsQueryable();
            query = query.Where(predicate);

            return query.ToList();
        }

        public List<Entity> GetEntitiesInRegion(int regionId, Expression<Func<Entity, bool>> predicate = null)
        {
            if (predicate == null)
                return new List<Entity>(_regions[regionId].Rooms.SelectMany(x => x.Entities));

            var query = _regions[regionId].Rooms.SelectMany(x => x.Entities).AsQueryable();
            query = query.Where(predicate);

            return query.ToList();
        }

        public void AddEntityToRoom(Entity e, int roomId)
        {
            _rooms[roomId].Entities.Add(e);
        }

        public void RemoveEntityFromRoom(Entity e, int roomId)
        {
            _rooms[roomId].Entities.Remove(e);
        }

        public Room GetRoomById(int roomId)
        {
            if (_rooms.TryGetValue(roomId, out var room))
                return room;
            return null;
        }

        public Zone GetRegionById(int regionId)
        {
            if (_regions.TryGetValue(regionId, out var region))
                return region;
            return null;
        }

        public Portal GetPortalById(int portalId)
        {
            if (_portals.TryGetValue(portalId, out var portal))
                return portal;
            return null;
        }

        public Room CreateRoom(string name, int? regionId = null, string description = null)
        {
            _nextId = 1;
            while (_usedIds.Contains(_nextId))
                _nextId++;

            var room = new Room(_nextId, name);

            _usedIds.Add(_nextId);

            if (regionId.HasValue)
            {
                var region = _regions[regionId.Value];
                room.Region = region;
                region.Rooms.Add(room);
            }
            room.Description = description;
            return room;
        }
    }
}
