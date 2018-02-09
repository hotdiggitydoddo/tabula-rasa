using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TabulaRasa.Services;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Test
{
    class Game : IGame
    {
        private readonly IEntityService _entityService;
        private readonly IComponentService _componentService;
        private readonly ITemplateService _templateService;
        private readonly ITimedActionService _timedActionService;
        private readonly ILocaleService _localeService;

        private Dictionary<int, Entity> _entities;
        private Dictionary<int, Room> _rooms;
        private Dictionary<int, Region> _regions;
        private Dictionary<int, Zone> _zones;
        private Dictionary<int, Portal> _portals;
        private Dictionary<int, PortalEntry> _portalEntries;

        private bool _isUpdating;
        private Timer _timer;
        private long _lastTime;

        public bool IsRunning { get; private set; }
        public long TimeRunning { get; private set; }
        public long CurrentTime { get; private set; }

        public Game(IEntityService entityService, 
            IComponentService componentService, 
            ITemplateService templateService, 
            ITimedActionService timedActionService,
            ILocaleService localeService)
        {
            _entityService = entityService;
            _componentService = componentService;
            _templateService = templateService;
            _timedActionService = timedActionService;
            _localeService = localeService;

            _timedActionService.ActionDispatched = HandleAction;

            Init().GetAwaiter().GetResult();

        }

        public void Start()
        {
            if (IsRunning)
                return;
            TimeRunning = 0;
            _lastTime = DateTime.UtcNow.Ticks;
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1000 / 60));
            IsRunning = true;
        }

        public void Quit()
        {
            IsRunning = false;
        }

        private async Task Init()
        {
            _entities = await _entityService.LoadAllEntities();
            _rooms = await _localeService.GetLocales<Room>();
            _regions = await _localeService.GetLocales<Region>();
            _zones = await _localeService.GetLocales<Zone>();
            _portals = await _localeService.GetLocales<Portal>();
            _portalEntries = await _localeService.GetPortalEntries();
        }

        public void DoStuff()
        {
            //var woodsy = _entityService.CreateEntityFromTemplate("owl", "Woodsy").Result;
            //var unnamed = _entityService.CreateEntityFromTemplate("owl").Result;

            //var owl = _templateService.LoadTemplateByName("owl").Result;
            //owl.Traits["maxHp"] = "315";
            //_templateService.SaveEntityTemplate(owl).GetAwaiter().GetResult();
            //var a = _templateService.LoadTemplateByName("owl").Result;

            //var snake = CreateASnake();
            //_entityService.SaveEntity(snake).GetAwaiter().GetResult();

            //var createZone = _localeService.CreateLocale<Zone>("Kalimdor").GetAwaiter().GetResult();
            //var region = _localeService.CreateLocale<Region>("The Crossroads").GetAwaiter().GetResult();

            //var zones = _localeService.GetLocales<Zone>().GetAwaiter().GetResult();
            //zones[1].Regions.Add(2);
           // _localeService.UpdateLocale(zones[1]).GetAwaiter().GetResult();
            
            
            var all = _entityService.LoadAllEntities().Result;

            //var owl = new Template { Name = "owl" };
            //owl.Traits.Add("currHp", "20");
            //owl.Traits.Add("maxHp", "20");
            //owl.ComponentList.Add("health");

            //var res = CreateTemplate(owl);
        }

        public void HandleAction(GameAction action)
        {
            //chat
            //announce
            //do
            if (action.Type.StartsWith("do-"))
                RouteActionTo(action);
            else if (action.Type == "attemptenterportal")
                EnterPortal(action);
            else if (action.Type == "addcomponent")
                AddComponentToEntity(action);
            else if (action.Type == "removecomponent")
                RemoveComponentFromEntity(action);
            else if (action.Type == "spawnentity")
                SpawnEntity(action);
            else if (action.Type == "messagecomponent")
                MessageComponent(action);
            else if (action.Type == "messageentity")
                MessageEntity(action);
            else if (action.Type == "stopgame")
                Stop();

        }

        public void AddActionAbsolute(DateTime time, GameAction action)
        {
            _timedActionService.Add(new TimedGameAction(time.Ticks, action));
        }

        public void AddActionRelative(TimeSpan time, GameAction action)
        {
            var dt = DateTime.UtcNow.AddTicks(time.Ticks);
            _timedActionService.Add(new TimedGameAction(dt.Ticks, action));
        }

        private void Stop()
        {
            IsRunning = false;
        }

        //spawning chars/mobs/items?  should take into account how many to make, and perhaps mins and maxs on attributes?

        private void SpawnEntity(GameAction action)
        {
            var entity = _entityService.CreateEntityFromTemplate(action.Value, action.AdditionalData).Result;
            var room = _rooms[action.ReceiverId];
            var region = _regions[room.RegionId];

            //physically place it into the realm
            entity.Traits["room"] = room.Id.ToString();
            room.Entities.Add(entity.Id);

            //tell the room and region about the new entity
            var spawnedEntity = new GameAction("spawnedentity", entity.Id);
            room.HandleAction(spawnedEntity);
            region.HandleAction(spawnedEntity);
        }

        private void EnterPortal(GameAction action)
        {
            var mob = _entities[action.SenderId];
            var portal = _portals[action.ReceiverId];

            if (!int.TryParse(mob.Traits["room"], out int oldRoomId))
                return;

            var oldRoom = _rooms[oldRoomId];

            if (!oldRoom.Portals.Contains(portal.Id))
                // log - mob cannot enter a portab when it's not in the room they're in
                return;

            // get the destination room
            var entry = _portalEntries.Values.Where(x => x.StartRoomId == oldRoomId).Single(x => x.DirectionName == action.Value);
            var newRoom = _rooms[entry.EndRoomId];
            var changeRegion = oldRoom.RegionId != newRoom.RegionId;
            var oldRegion = _zones[oldRoom.RegionId];
            var newRegion = _zones[newRoom.RegionId];

            // ask permission of everyone to leave current room
            if (changeRegion)
            {
                var canLeaveRegion = new GameAction("canleaveregion", mob.Id, oldRegion.Id);
                var canEnterRegion = new GameAction("canenterregion", mob.Id, newRegion.Id);
                if (!oldRegion.HandleAction(canLeaveRegion))
                    return;
                if (!newRegion.HandleAction(canEnterRegion))
                    return;
                if (!mob.HandleAction(canLeaveRegion))
                    return;
                if (!mob.HandleAction(canEnterRegion))
                    return;
            }

            var canLeaveRoom = new GameAction("canleaveroom", mob.Id);
            var canEnterRoom = new GameAction("canenterroom", mob.Id);
            var canEnterPortal = new GameAction("canenterportal", mob.Id);

            if (!oldRoom.HandleAction(canLeaveRoom))
                return;
            if (!newRoom.HandleAction(canEnterRoom))
                return;
            if (!mob.HandleAction(canLeaveRoom))
                return;
            if (!portal.HandleAction(canEnterPortal))
                return;

            //tell the room/region that the player is leaving
            if (changeRegion)
            {
                var leaveRegion = new GameAction("leaveregion", mob.Id);
                oldRegion.HandleAction(leaveRegion);
                mob.HandleAction(leaveRegion);
            }

            var leaveRoom = new GameAction("leaveroom", mob.Id);
            ActionRoomMobs(leaveRoom, oldRoom.Id);
            ActionRoomItems(leaveRoom, oldRoom.Id);

            //tell the portal the mob has entered
            var enterPortal = new GameAction("enterportal", mob.Id, portal.Id);
            portal.HandleAction(enterPortal);
            mob.HandleAction(enterPortal);

            //now move the mob
            oldRoom.Entities.Remove(mob.Id);
            mob.Traits["room"] = newRoom.Id.ToString();
            newRoom.Entities.Add(mob.Id);

            //tell everyone in the region/room that the player has entered
            if (changeRegion)
            {
                var enterRegion = new GameAction("enterregion", mob.Id, newRegion.Id);
                newRegion.HandleAction(enterRegion);
                mob.HandleAction(enterRegion);
            }

            var enterRoom = new GameAction("enterroom", mob.Id, portal.Id);
            newRoom.HandleAction(enterRoom);
            ActionRoomMobs(enterRoom, newRoom.Id);
            ActionRoomItems(enterRoom, newRoom.Id);
        }

        private void ActionRoomItems(GameAction action, int roomId)
        {
            var roomEntities = _rooms[roomId].Entities;
            var items = _entities.Where(x => roomEntities.Contains(x.Key) && x.Value.Traits["item"] != null).ToList();
            items.ForEach(x => 
            {
                x.Value.HandleAction(action);
            });
        }

        private void ActionRoomMobs(GameAction action, int roomId)
        {
            var roomEntities = _rooms[roomId].Entities;
            var mobs = _entities.Where(x => roomEntities.Contains(x.Key) && x.Value.Traits["race"] != null).ToList();
            mobs.ForEach(x =>
            {
                x.Value.HandleAction(action);
            });
        }

        Template CreateTemplate(Template template)
        {
            return _templateService.SaveEntityTemplate(template).Result;
        }
        Entity CreateASnake()
        {
            var snake = _entityService.CreateEntity("snake").Result;
            var healthComp = _componentService.CreateComponent("health", "maxHp=150, currHp=95");
            snake.AddComponent(healthComp);

            return snake;
        }

        private void OnTimerElapsed(object state)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            CurrentTime = DateTime.UtcNow.Ticks;

            var elapsed = (CurrentTime - _lastTime);

            _timedActionService.Update();

            TimeRunning += elapsed;
            _lastTime = CurrentTime;

            _isUpdating = false;
        }

        private void AddComponentToEntity(GameAction action)
        {
            var comp = _componentService.CreateComponent(action.Value, action.AdditionalData);
            var entity = _entities[action.SenderId];
            entity.AddComponent(comp);
            
            //save?
        }

        private void RemoveComponentFromEntity(GameAction action)
        {
            var entity = _entities[action.SenderId];
            entity.RemoveComponent(action.Value);

            //save?
        }

        private void MessageComponent(GameAction action)
        {
            //get component by name of receiverId and call do action
            var entity = _entities[action.ReceiverId];
            if (entity == null) return;
            entity.GetComponent(action.Value)?.HandleAction(action);
        }

        private void MessageEntity(GameAction action)
        {
            var entity = _entities[action.ReceiverId];
            if (entity == null) return;
            entity.HandleAction(action);
        }

        private void RouteActionTo(GameAction action)
        {
            switch (action.AdditionalData)
            {
                case "entity":
                    _entities[action.ReceiverId]?.HandleAction(action);
                    break;
                case "room":
                //    _locations.GetRoomById(action.ReceiverId)?.HandleAction(action);
                    break;
                case "region":
               //     _locations.GetRegionById(action.ReceiverId)?.HandleAction(action);
                    break;
                case "portal":
                //    _locations.GetPortalById(action.ReceiverId)?.HandleAction(action);
                    break;
            }
        }


       // private Portal GetCurrentPortal()
       
    }
}
