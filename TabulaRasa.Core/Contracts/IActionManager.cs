using System;
using TabulaRasa.Core.Actions;

namespace TabulaRasa.Core.Contracts
{
    public interface IActionManager
    {
        Action<GameAction> ActionDispatched { get; set; }
        void Add(TimedGameAction action);
        void Update();
    }
}