using System;
using System.Diagnostics;
using System.Text;
using TabulaRasa.Services;
using TabulaRasa.Services.Objects;

namespace TabulaRasa.Test.Admin.Menus
{
    class MainMenu : AdminMenu
    {
        public MainMenu(IGame game) : base(game)
        {

        }
        public override void HandleInput(string input)
        {
            if (input == "q")
            {
                _game.HandleAction(new GameAction("stopgame", 0));
                //_game.AddActionRelative(TimeSpan.FromSeconds(5), new GameAction("stopgame", 0));
                //_game.AddActionAbsolute(DateTime.UtcNow.AddSeconds(5), new GameAction("stopgame", 0));
            }
            else if (input == "t")
            {
                PrintTimeRunning();
            }
        }

        private void PrintTimeRunning()
        {
            var time = new TimeSpan(_game.TimeRunning);
            var sb = new StringBuilder();

            if (time.Seconds == 1)
                sb.Insert(0, $"{time.Seconds} second.");
            else
                sb.Insert(0, $"{time.Seconds} seconds.");

            if (time.Minutes == 1)
                sb.Insert(0, $"{time.Minutes} minute, ");
            else if (time.Hours > 0 || time.Minutes > 1)
                sb.Insert(0, $"{time.Minutes} minutes, ");

            if (time.Hours == 1)
                sb.Insert(0, $"{time.Hours} hour, ");
            else if (time.Days > 1 || time.Hours > 1)
                sb.Insert(0, $"{time.Hours} hours, ");

            if (time.Days == 1)
                sb.Insert(0, $"{time.Days} day, ");
            else if (time.Days > 1)
                sb.Insert(0, $"{time.Days} days, ");

            Console.WriteLine($"Time Running: {sb.ToString()}");
        }

        public override void WritePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("[Main Menu]");
            Console.Write("Your command? => ");
        }
    }
}
