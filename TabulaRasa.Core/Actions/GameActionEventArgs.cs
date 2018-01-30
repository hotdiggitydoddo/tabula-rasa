using System;

namespace TabulaRasa.Core.Actions
{
    public class GameActionEventArgs : EventArgs
    {
        public GameAction Action { get; set; }
    }
}