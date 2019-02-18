using System;
using VaultKVCom;
using System.Collections.Generic;

namespace VaultDotnetClient
{
    class Program
    {
        static void Main(string[] args)
        {

            ConsoleUI ui = new ConsoleUI();
            while(true)
            {
                ui.ExecuteAction(ui.DisplayMenu());
            };

        }
    }
}
