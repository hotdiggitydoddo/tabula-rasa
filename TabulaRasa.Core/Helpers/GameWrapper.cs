using System;
using System.Collections.Generic;
using System.Linq;
using TabulaRasa.Core.Actions;
using TabulaRasa.Core.Contracts;
using TabulaRasa.Core.Locations;
using TabulaRasa.Core.Objects;

namespace TabulaRasa.Core.Helpers
{
    public class GameWrapper
    {
        private readonly IEntityManager _entities;
        private readonly IComponentManager _components;
        private readonly IActionManager _actions;

        private Dictionary<int, Room> _rooms;

        public static GameWrapper Instance;

        public GameWrapper(IEntityManager entities, IComponentManager components, IActionManager actions)
        {
            _entities = entities;
            _components = components;
            _actions = actions;
            _rooms = new Dictionary<int, Room>();
            Instance = this;
            _actions.ActionDispatched = HandleAction;
        }

        public Entity CreateEntity(string name)
        {
            return _entities.CreateEntity(name);
        }

        public void HandleAction(GameAction action)
        {
            if (action.Type == "addcomponent")
                AddComponentToEntity(action);
            else if (action.Type == "removecomponent")
                RemoveComponentFromEntity(action);
            else if (action.Type == "messagecomponent")
                MessageComponent(action);
            else if (action.Type == "messageentity")
                MessageEntity(action);
            else
                RouteActionToEntity(action);
        }
      
        private void AddComponentToEntity(GameAction action)
        {
            var comp = _components.CreateComponent(action.Args[0], action.Args.Skip(1).ToArray());
            var entity = _entities.GetById(action.SenderId);
            entity.AddComponent(comp);
        }

        private void RemoveComponentFromEntity(GameAction action)
        {
            var entity = _entities.GetById(action.SenderId);
            entity.RemoveComponent(action.Args[0]);
        }

        private void MessageComponent(GameAction action)
        {
            //get component by name of receiverId and call do action
            var entity = _entities.GetById(action.ReceiverId);
            if (entity == null) return;
            entity.GetComponent(action.Args[0])?.HandleAction(action);
        }

        private void MessageEntity(GameAction action)
        {
            var entity = _entities.GetById(action.ReceiverId);
            if (entity == null) return;
            entity.HandleAction(action);
        }

        private void RouteActionToEntity(GameAction action)
        {
            var entity = _entities.GetById(action.ReceiverId);
            if (entity == null) return;
            entity.HandleAction(action);
        }
    }
}