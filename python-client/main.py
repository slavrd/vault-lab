from modules.vault_com_kv import VaultComKV
from typing import Dict
import json
import requests
import os
import sys


class UI:
    """Represents the UI logic
    """

    def __init__(self, vault_addr: str, vault_token: str, vault_kv_path: str):
        """Constructor for that sets the vault communicator
        and menu items
        """

        self.vcom = VaultComKV(vault_addr,vault_token,vault_kv_path)
        self.menu_items = {
            1: ["Add/Update secret", self.update_vault_secret],
            2: ["Read secret", self.read_vault_secret],
            3: ["Delete secret", self.delete_vault_secret],
            4: ["List all secrets", self.list_vault_secrets],
            5: ["Quit", exit]
        }
            
    def display_main_menu(self) -> int:
        """Display main menu and return
        the user's choice
        """

        # display main menu until user makes acceptable choice
        user_choice=0
        while(True):
            # clear screen
            if os.name == 'nt':
                _ = os.system('cls')
            else:
                _ = os.system('clear')
            # display menu
            print("Main menu","=========",sep='\n')
            for k in self.menu_items.keys():
                print(str.format("{0}) {1}",k,self.menu_items[k][0]))
            user_input=input("\nEnter choice: ")
            try:
                user_choice = int(user_input)
                if user_choice in self.menu_items.keys():
                    return user_choice
            except ValueError:
                pass

    def main_loop(self) -> None:
        """Runs the main procram loop
        display menu -> action -> display menu
        """

        while(True):
            usr_choice = self.display_main_menu()
            print()
            action = self.menu_items.get(usr_choice,"Invalid choice")[1]
            action()
            input("\nPress any key to continue...")

    def parse_vault_error(self,resp: requests.Response) -> None:
        """Parses and desplays the errors from a
        Vault error response in resp
        """
        try:
            err_list=list(json.loads(resp.content)["errors"])
            for msg in err_list:
                print(msg,file=sys.stderr)
        except ValueError:
            print("Vault server returned error but message could not be parsed", file=sys.stderr)

    def update_vault_secret(self) -> None:
        """Adds or updates a vault kv secret
        using the provided instance vcom object
        """
        # Get user input
        secret_name = input("Enter the secret name: ")

            # Get the number of kv pairs. Loop until input is int
        while(True):
            kv_num_str = input("Enter the number of kv pairs: ")
            try:
                kv_num = int(kv_num_str)
                if kv_num > 0:
                    break
            except ValueError:
                pass
            
            # Get the keys and values 
        kv_dict = dict()
        for i in range(0,kv_num):
            print("")
            k = input(str.format("Enter key {0}: ",i+1))
            v = input(str.format("Enter value {0}: ",i+1))
            kv_dict[k]=v
        print("")

        # call vault API
        resp = self.vcom.add_secret(secret_name, kv_dict)

        # handle response
        if resp == None:
            print("Error calling vault API.",file=sys.stderr)
        elif resp.ok:
            print("Secret added successfully!")
        else:
            self.parse_vault_error(resp)

    def read_vault_secret(self) -> None:
        """Gets a vault secret from the instance vcom object
        and displays it
        """
        resp = self.vcom.read_secret(input("Enter secret name: "))
        print()

        # handle response
        if resp == None:
            print("Error calling vault API.",file=sys.stderr)
            return None
        elif resp.ok:
            try:
                secret_content = dict(json.loads(resp.content)["data"]["data"])
            except ValueError:
                print("Error: Vault response could not be parsed",file=sys.stderr)
                return None
        else:
            self.parse_vault_error(resp)
            return None

        # display parsed response
        for k,v in secret_content.items():
            print(str.format("{0} : {1}",k,v))

    def delete_vault_secret(self) -> None:
        """Deletes a provided secret 
        from the instance vcom object
        """

        resp = self.vcom.delete_secret(input("Enter secret to delete: "))
        print("")

        # handle response
        if resp == None:
            print("Error calling vault API.",file=sys.stderr)
        elif resp.ok:
            print("Secret deleted successfully if present")
        else:
            self.parse_vault_error(resp)

    def list_vault_secrets(self) -> None:
        """List all secrets available 
        in the instance's vcom object
        """

        resp = self.vcom.list_secrets()

        #handle response
        if resp == None:
            print("Error calling vault API.",file=sys.stderr)
        elif resp.ok:
            try:
                data = json.loads(resp.content)["data"]["keys"]
                for item in data:
                    print(item)
            except ValueError:
                print("Error: Vault response could not be parsed",file=sys.stderr)
        else:
            self.parse_vault_error(resp)
    
if __name__ == "__main__":

    try:
        ui = UI(os.environ["VAULT_ADDR"],os.environ["VAULT_TOKEN"],os.environ["VAULT_KV_PATH"])
    except Exception:
        print("Error initializing vault communicator.",
            "Verify that 'VAULT_ADDR', 'VAULT_TOKEN' and 'VAULT_KV_PATH' environment variables are set.",
            sep='\n',file=sys.stderr)
        exit(1)

    ui.main_loop()
