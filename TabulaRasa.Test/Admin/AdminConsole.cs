using System;
using System.Collections.Generic;
using System.Text;
using TabulaRasa.Services;
using TabulaRasa.Test.Admin.Menus;

namespace TabulaRasa.Test.Admin
{
    class AdminConsole : IAdminConsole
    {
        private readonly IGame _game;
        private Stack<IAdminMenu> _menus;
        public AdminConsole(IGame game)
        {
            _game = game;
            _menus = new Stack<IAdminMenu>();
            _menus.Push(new MainMenu(_game));
            _menus.Peek().WritePrompt();
        }

        public void HandleInput(string input)
        {
            _menus.Peek().HandleInput(input);
            _menus.Peek().WritePrompt();
        }
    }
}
