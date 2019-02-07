import sys
sys.path.append('../')
import unittest
import requests
from main import UI
from unittest.mock import Mock, patch
import io

class TestUI(unittest.TestCase):
    """Tests for the UI behavior
    """
    # Initialize a UI object to test
    ui = UI("test_addr","test_token","test_path")
    
    # An expected menu map against which to test the ui object
    menu_map = {
        "Add/Update secret": ui.update_vault_secret,
        "Read secret": ui.read_vault_secret,
        "Delete secret": ui.delete_vault_secret,
        "List all secrets": ui.list_vault_secrets,
        "Quit": exit
    }

    def test_ui_init(self):
        """Test if the UI initializes correctly.
        The vault communicator construction and the Menu
        mappings are correct.
        """

        self.assertEqual(self.ui.vcom.vault_address,"test_addr")
        self.assertEqual(self.ui.vcom.vault_token,"test_token")
        self.assertEqual(self.ui.vcom.vkvengine_path,"test_path")

        for k,v in self.ui.menu_items.items():
            if v[0] in self.menu_map.keys():
                self.assertEqual(v[1],self.menu_map[v[0]],str.format("Missmatch in menu action binding for {0}: \"{1}\"",k,v[0]))
            else:
                self.assertTrue(False, str.format("Unexpected menu item {0}: \"{1}\"",k,v[0]))
    
    @patch('main.input')
    def test_display_main_menu_ok(self, mock_input):
        """Tests menu items can be reached.
        """

        for k in self.ui.menu_items.keys():
            mock_input.return_value = k
            self.assertEqual(self.ui.display_main_menu(),k)
    
    @patch('main.input',side_effect=["","string",0,1000,1])
    def test_display_main_menu_error(self, mock_input):
        """Test that only defined menu choices can be returned
        """
        
        self.assertEqual(self.ui.display_main_menu(),1)

        # checks if all the side_effect values were used
        self.assertTrue(mock_input.call_count == 5)

    @patch('modules.vault_com_kv.VaultComKV.add_secret')
    @patch('main.input')
    def test_update_vault_secret_vault_call(self,mock_input,mock_add_secret):
        """Test if update_vault_secret method calls correctly VComKV.add_secret
        """

        # define the user input
        mock_input.side_effect = [
            "my-secret", # name of the secret
            "non-num",# enter non numeric value for kv number in secret
            "-1", # enter negative value for kv number in secret
            "2", # the number of kv pairs in the secret
            "key1",
            "value1",
            "key2",
            "value2"
        ]

        self.ui.update_vault_secret()

        # test that the VComKV.add_secret() was called correctly
        mock_add_secret.assert_called_once_with("my-secret",
            {"key1":"value1","key2":"value2"})

    @patch('modules.vault_com_kv.VaultComKV.read_secret')
    @patch('main.input')
    def test_read_vault_secret_vault_call(self,mock_input,mock_read_secret):
        """Test if read_vault_secret method calls correctly VComKV.read_secret
        """
        # define the user input
        mock_input.side_effect = [
            "my-secret" # name of the secret
        ]
        mock_read_secret.return_value.ok = True
        mock_read_secret.return_value.content = """{
                                                  "data": {
                                                    "data": {
                                                      "key1": "value1",
                                                      "key2": "value2"
                                                    }
                                                  }
                                                }"""
        self.ui.read_vault_secret()
        # test that the VComKV.read_secret() was called correctly
        mock_read_secret.assert_called_once_with("my-secret")

    @patch('modules.vault_com_kv.VaultComKV.delete_secret')
    @patch('main.input')
    def test_delete_vault_secret_vault_call(self,mock_input,mock_delete_secret):
        """Test if delete_vault_secret method calls correctly VComKV.delete_secret
        """

        # define the user input
        mock_input.side_effect = [
            "my-secret" # name of the secret
        ]

        self.ui.delete_vault_secret()

        # test that the VComKV.read_secret() was called correctly
        mock_delete_secret.assert_called_once_with("my-secret")       

    @patch('modules.vault_com_kv.VaultComKV.list_secrets')
    def test_list_vault_secret_vault_call(self,mock_list_secrets):
        """Test if list_vault_secrets() calls correctly VComKV.list_secrets()
        """

        mock_list_secrets.return_value.ok = True
        mock_list_secrets.return_value.content = '{ "data": { "keys" : []}}'
        self.ui.list_vault_secrets()

        # test that the VComKV.list_secrets() was called correctly
        mock_list_secrets.assert_called_once_with()          


if __name__ == "__main__":
    unittest.main()    

