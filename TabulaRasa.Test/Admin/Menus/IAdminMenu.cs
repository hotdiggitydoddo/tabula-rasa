using System;
using System.Collections.Generic;
using System.Text;

namespace TabulaRasa.Test.Admin.Menus
{
    interface IAdminMenu
    {
        void HandleInput(string input);
        void WritePrompt();
    }
}
