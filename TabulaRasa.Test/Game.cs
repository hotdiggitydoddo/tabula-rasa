using System;
using System.Collections.Generic;
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

        private bool _isRunning;
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
            TimeRunning = 0;
            _lastTime = DateTime.UtcNow.Ticks;
            _timer = new Timer(OnTimerElapsed, null, TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(1000 / 60));

        }

        public void Quit()
        {
            IsRunning = false;
        }

        private async Task Init()
        {
            _entities = await _entityService.LoadAllEntities();
           // _localeService
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
            else if (action.Type == "addcomponent")
                AddComponentToEntity(action);
            else if (action.Type == "removecomponent")
                RemoveComponentFromEntity(action);
            else if (action.Type == "messagecomponent")
                MessageComponent(action);
            else if (action.Type == "messageentity")
                MessageEntity(action);

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

       
    }
}
