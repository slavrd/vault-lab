using System;
using System.ComponentModel;
using VaultKVCom;
using System.Collections.Generic;

namespace VaultDotnetClient
{
    public class ConsoleUI
    {
        internal enum menuItems: ushort
        {
            AddUpdateSecret=1,
            ReadSecret,
            DeleteSecret,
            ListAllSecrets,
            Exit
        }

        // Defines the environment variables to use when initializing the vaultCommunicator
        private static readonly Dictionary<string,string> vaultEnvConfig = new Dictionary<string,string>(){
            {"vault-address","VAULT_ADDR"},
            {"vault-token","VAULT_TOKEN"},
            {"vault-kv-path","VAULT_KV_PATH"}
        };

        private VaultCom vaultCommunicator;

        public ConsoleUI()
        {
            vaultCommunicator = InitVaultCom();
            if(vaultCommunicator == null)
            {               
                throw new ApplicationException("Error initializing vault communicator");
            } 
        }

        ///<summary>
        /// Initialize a VaultCom object based on 
        /// environment variables
        ///</summary>
        private VaultCom InitVaultCom()
        {
            bool areVarsOK = true;
            
            // Get Verify that environment variables exit
            foreach(var item in vaultEnvConfig)
            {
                if(String.IsNullOrEmpty(Environment.GetEnvironmentVariable(item.Value)))
                {
                    Console.Error.WriteLine($"The variable {item.Value} is not set!");
                    areVarsOK = false;
                }
            }

            // Initialize and retrun a VaultCom object if the variables are set
            if(areVarsOK)
            {
                return new VaultCom(
                    Environment.GetEnvironmentVariable(vaultEnvConfig["vault-address"]),
                    Environment.GetEnvironmentVariable(vaultEnvConfig["vault-token"]),
                    Environment.GetEnvironmentVariable(vaultEnvConfig["vault-kv-path"])
                );
            }
            else
            {
                return null;
            }

        }

        ///<summary>
        /// Displays menu based on the class menuItems and
        /// returns the user choice
        ///</summary>
        internal menuItems DisplayMenu()
        {
            ushort choice = 0;
            
            // Redisplay main menu until the user makes a valid choice
            while(!Enum.IsDefined(typeof(menuItems),choice))
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("=========\n");
                
                // Display all available choices based on this.menuItems
                foreach(var item in Enum.GetValues(typeof(menuItems)))
                {
                    Console.WriteLine("{0}) {1}", (ushort)item,item);
                }
                Console.WriteLine();
                Console.Write("Enter your choice: ");
                
                // Get user input, validate and assign to choice if valid
                ushort.TryParse(Console.ReadLine(),out choice);
            }
            return (menuItems)Enum.ToObject(typeof(menuItems),choice);
        }
    
        ///<summary>
        /// Executes appropriate action for provided menuItem choice
        ///</summary>
        internal void ExecuteAction(menuItems choice)
        {
            switch(choice)
            {
                case menuItems.AddUpdateSecret:
                    AddSecret();
                    break;
                case menuItems.ReadSecret:
                    ReadSecret();
                    break;
                case menuItems.DeleteSecret:
                    DeleteSecret();
                    break;
                case menuItems.ListAllSecrets:
                    ListSecrets();
                    break;
                case menuItems.Exit:
                    Exit();
                    break;
                default:
                    throw new ApplicationException(String.Format("The menu action {0} is not implemented",choice));
            };
        }
    
        ///<summary>
        /// Adds or updates secret based on user input
        ///</summary>
        internal void AddSecret()
        {
            throw new NotImplementedException();
        }
    
        ///<summary>
        /// Read a user provided secret
        ///</summary>
        internal void ReadSecret()
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Delete a user provided secret
        ///</summary>
        internal void DeleteSecret()
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Lest all secrets available
        ///</summary>
        internal void ListSecrets()
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Exits the program
        ///</summary>
        internal void Exit()
        {
            Environment.Exit(0);
        }

    }
}