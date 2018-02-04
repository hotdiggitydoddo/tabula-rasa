using System;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Contracts
{
    public interface IActionManager
    {
        Action<GameAction> ActionDispatched { get; set; }
        void Add(TimedGameAction action);
        void Update();
    }
}