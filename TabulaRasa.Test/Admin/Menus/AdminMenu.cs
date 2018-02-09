using System;
using System.Collections.Generic;
using System.Text;
using TabulaRasa.Services;

namespace TabulaRasa.Test.Admin.Menus
{
    abstract class AdminMenu : IAdminMenu
    {
        protected readonly IGame _game;
        public AdminMenu(IGame game)
        {
            _game = game;
        }

        public abstract void HandleInput(string input);

        public abstract void WritePrompt();
    }
}
