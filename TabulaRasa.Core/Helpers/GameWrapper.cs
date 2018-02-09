using System;
using System.Linq;
using System.Threading;
using TabulaRasa.Services.Contracts;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Helpers
{
    public class GameWrapper
    {
        private readonly IEntityManager _entities;
        private readonly IComponentManager _components;
        private readonly IActionManager _actions;
        private readonly ILocationManager _locations;

        //Timing
        private bool _isRunning;
        private bool _isUpdating;
        private Timer _timer;
        private long _lastTime;
        public bool IsRunning => _isRunning;
        public long TimeRunning { get; private set; }
        public long CurrentTime { get; private set; }

        public static GameWrapper Instance;

        public GameWrapper(IEntityManager entities, IComponentManager components, IActionManager actions, ILocationManager locations)
        {
            _entities = entities;
            _components = components;
            _actions = actions;
            _locations = locations;
            Instance = this;
            _actions.ActionDispatched = HandleAction;

            TimeRunning = 0;
            _lastTime = DateTime.UtcNow.Ticks;
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1000 / 60));
            _isRunning = true;
        }

        private void OnTimerElapsed(object state)
        {
            if (_isUpdating) return;
            _isUpdating = true;

            CurrentTime = DateTime.UtcNow.Ticks;

            var elapsed = (CurrentTime - _lastTime);

            Tick(elapsed);

            TimeRunning += elapsed;
            _lastTime = CurrentTime;

            _isUpdating = false;
        }

        private void Tick(long elapsed)
        {
            _actions.Update();
        }

        public Entity CreateEntity(string name = null)
        {
            return _entities.CreateEntity(name);
        }

        public void HandleAction(GameAction action)
        {
            //chat
            //announce
            //do
            if (action.Type.StartsWith("do-"))
                RouteActionTo(action);
            else if (action.Type == "addcomponent")
                AddComponentToEntity(action);
            else if (action.Type == "removecomponent")
                RemoveComponentFromEntity(action);
            else if (action.Type == "messagecomponent")
                MessageComponent(action);
            else if (action.Type == "messageentity")
                MessageEntity(action);
           
        }

        private void SpawnEntity(int entityId, int roomId)
        {
            var entity = _entities.GetById(entityId);
            var room = _locations.GetRoomById(roomId);

            entity.Traits["room"] = room.Id.ToString();
            _locations.AddEntityToRoom(entity, roomId);

            var action = new GameAction("spawnentity", entityId);
            room.HandleAction(action);
           // room.Region.HandleAction(action);
        }

        private void AddComponentToEntity(GameAction action)
        {
            var comp = _components.CreateComponent(action.Value, action.AdditionalData);
            var entity = _entities.GetById(action.SenderId);
            entity.AddComponent(comp);
        }

        private void RemoveComponentFromEntity(GameAction action)
        {
            var entity = _entities.GetById(action.SenderId);
            entity.RemoveComponent(action.Value);
        }

        private void MessageComponent(GameAction action)
        {
            //get component by name of receiverId and call do action
            var entity = _entities.GetById(action.ReceiverId);
            if (entity == null) return;
            entity.GetComponent(action.Value)?.HandleAction(action);
        }

        private void MessageEntity(GameAction action)
        {
            var entity = _entities.GetById(action.ReceiverId);
            if (entity == null) return;
            entity.HandleAction(action);
        }

        private void RouteActionTo(GameAction action)
        {
            switch (action.AdditionalData)
            {
                case "entity":
                    _entities.GetById(action.ReceiverId)?.HandleAction(action);
                    break;
                case "room":
                    _locations.GetRoomById(action.ReceiverId)?.HandleAction(action);
                    break;
                case "region":
                    _locations.GetRegionById(action.ReceiverId)?.HandleAction(action);
                    break;
                case "portal":
                    _locations.GetPortalById(action.ReceiverId)?.HandleAction(action);
                    break;
            }
        }

        private void ActionRealmPlayers(GameAction action)
        {
            _entities.GetEntitiesWithTrait("accountId").Select(p => p.HandleAction(action));
        }

        private void ActionRealmMobs(GameAction action)
        {
            _entities.GetEntitiesWithTrait("race").Select(p => p.HandleAction(action));
        }

        private void ActionRoomMobs(GameAction action, int roomId)
        {
            var entities = _locations.GetEntitiesInRoom(roomId, x => !string.IsNullOrEmpty(x.Traits["race"]));

            foreach (var e in entities)
                e.HandleAction(action);
        }
    }
}