using System;
using System.ComponentModel;
using VaultKVCom;
using System.Collections.Generic;
using Flurl;
using VaultDotnetClient.Interfaces;

namespace VaultDotnetClient
{
    public class ConsoleUI
    {
        public enum MenuItems: ushort
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

        private IUserInput userInput;


        /// <summary>
        /// Constructor that initializes new VaultCom instance for vaultCommunicator
        /// </summary>
        public ConsoleUI()
        {
            VaultCom vaultCom = InitVaultCom();
            if(vaultCom == null)
            {               
                throw new ApplicationException("Error initializing vault communicator!");
            }
            this.userInput = new GetUserInput();

        }

        /// <summary>
        /// Constructor that uses the provided VaultCom and IUserInput objects.
        /// </summary>
        /// <param name="vaultComunicator"></param>
        public ConsoleUI(VaultCom vaultComunicator,IUserInput userInput)
        {
            this.userInput = userInput;
            this.vaultCommunicator = vaultComunicator;
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
        public MenuItems DisplayMenu()
        {
            ushort choice = 0;
            
            // Redisplay main menu until the user makes a valid choice
            while(!Enum.IsDefined(typeof(MenuItems),choice))
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("=========\n");
                
                // Display all available choices based on this.menuItems
                foreach(var item in Enum.GetValues(typeof(MenuItems)))
                {
                    Console.WriteLine("{0}) {1}", (ushort)item,item);
                }
                Console.WriteLine();
                Console.Write("Enter your choice: ");
                
                // Get user input, validate and assign to choice if valid
                ushort.TryParse(userInput.GetUserInput(),out choice);
            }
            return (MenuItems)Enum.ToObject(typeof(MenuItems),choice);
        }
    
        ///<summary>
        /// Executes appropriate action for provided menuItem choice
        ///</summary>
        public void ExecuteAction(MenuItems choice)
        {
            switch(choice)
            {
                case MenuItems.AddUpdateSecret:
                    AddSecret();
                    break;
                case MenuItems.ReadSecret:
                    ReadSecret();
                    break;
                case MenuItems.DeleteSecret:
                    DeleteSecret();
                    break;
                case MenuItems.ListAllSecrets:
                    ListSecrets();
                    break;
                case MenuItems.Exit:
                    Exit();
                    break;
                default:
                    throw new ApplicationException(String.Format("The menu action {0} is not implemented!",choice));
            };
        }
    
        ///<summary>
        /// Adds or updates secret based on user input
        ///</summary>
        public void AddSecret()
        {
            // Gather user input
            Console.WriteLine();

                // Get secret name
            string secretName = GetSecretName();

                // Get number of KV pairs in the secret
            uint kvNum = 0;
            while(kvNum <= 0)
            {
                Console.Write("Enter number of KV pairs: ");
                uint.TryParse(userInput.GetUserInput(),out kvNum);
            }
                
                // Get the KV pairs
            var secretData = new Dictionary<string,string>();

            for(uint i = 1; i <= kvNum; i++)
            {   

                Console.WriteLine();

                string key = String.Empty;
                string value = String.Empty;

                // Get key, ensuring it does not exist
                while(String.IsNullOrEmpty(key))
                {
                    Console.Write("Enter key{0}: ",i);
                    key = userInput.GetUserInput();
                    if(secretData.ContainsKey(key))
                    {
                        Console.Error.WriteLine("Key already exists!");
                        key = string.Empty;
                    }
                }
                Console.WriteLine();
                
                // Get Vaule
                while(String.IsNullOrEmpty(value))
                {
                    Console.Write("Enter value{0}: ",i);
                    value = userInput.GetUserInput();
                }
                
                secretData.Add(key,value);
            }

            // Call vault API and handle response
            Console.WriteLine();
            if(vaultCommunicator.AddKVSecret(secretName,secretData).Result)
            {
                Console.WriteLine("Secret written successfully!");
            }
            else
            {
                Console.WriteLine("Writing secret failed!");
            }

            Console.Write("Press any key to continue...");
            userInput.GetUserInput();

        }
    
        ///<summary>
        /// Read a user provided secret
        ///</summary>
        public void ReadSecret()
        {
            throw new NotImplementedException();
            // Get secret name

            // Make call to Vault API

            // Handle output
        }

        ///<summary>
        /// Delete a user provided secret
        ///</summary>
        public void DeleteSecret()
        {

            throw new NotImplementedException();
        }

        ///<summary>
        /// List all available secrets
        ///</summary>
        public void ListSecrets()
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Exits the program
        ///</summary>
        public void Exit()
        {
            Environment.Exit(0);
        }


        /// <summary>
        /// Get a secret name from the user
        /// </summary>
        private string GetSecretName()
        {
            string secretName = null;
            while(String.IsNullOrEmpty(secretName) || !Uri.IsWellFormedUriString(secretName,UriKind.Relative) || secretName.Contains('/'))             
            {
                Console.Write("Enter secret name: ");
                secretName = userInput.GetUserInput().Trim('/');
            }
            Console.WriteLine();
            return secretName;
        }

    }
}