using System;
using Priority_Queue;
using TabulaRasa.Services.Contracts;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Managers
{
    public class ActionManager : IActionManager
    {
        private readonly SimplePriorityQueue<TimedGameAction> _timedActions;
        
        public Action<GameAction> ActionDispatched { get; set; }

        public ActionManager()
        {
            _timedActions = new SimplePriorityQueue<TimedGameAction>();
        }

        public void Add(TimedGameAction action)
        {
            _timedActions.Enqueue(action, action.DispatchTime);
        }

        public void Update()
        {
            if (_timedActions.Count == 0 || _timedActions.First.DispatchTime > DateTime.UtcNow.Ticks)
                return;

            while (_timedActions.Count > 0)
            {
                while (_timedActions.First.DispatchTime <= DateTime.UtcNow.Ticks)
                {
                    var action = _timedActions.Dequeue();

                    if (!action.IsValid)
                        break;
                    
                    action.Unhook();
                    ActionDispatched(action);
                    break;
                }
            }
        }
    }
}