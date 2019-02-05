import sys
sys.path.append('../')
import unittest
import requests
from modules.vault_com_kv import VaultComKV
from unittest.mock import Mock, patch

class TestCallValultAPI(unittest.TestCase):
    """Tests for the call_vault_api method
    of VaultComKV class
    """

    # Define a VaultComKV object to be used in the test
    vcom = VaultComKV("fake_address","fake_token","fake_engine_path")

    # A sample vault request data to be used with the call_vault_api request
    vaultreq = {
            "method": "LIST",
            "url": "/".join([vcom.vkvengine_base_url, "metadata"]),
            "headers": { "X-Vault-Token": vcom.vault_token }
        }
   
    # patch the requests.request method with a Mock that returns a response
    @patch('modules.vault_com_kv.requests.request')
    def test_call_vault_api_no_data(self,mock_request):
        """Verify the method executes the request with correct parameters
        when "data" key is missing
        """

        mock_request.return_value.ok=True
        req = self.vcom.call_vault_api(self.vaultreq)
        
        # Check the requests.request method is called correctly
        mock_request.assert_called_once_with(self.vaultreq["method"], self.vaultreq["url"], headers=self.vaultreq["headers"],data="")

        # Check the response
        self.assertIsNotNone(req)
        self.assertTrue(req.ok)

    # patch the requests.request method with a Mock that returns a response
    @patch('modules.vault_com_kv.requests.request')
    def test_call_vault_api_full_req(self,mock_request):
        """Verify the method executes the request with correct parameters
        when "data" key is present
        """

        # Vault request with "data"
        vaultreq_full = {
            "method": "POST",
            "url": "/".join([self.vcom.vkvengine_base_url, "data", "secret"]),
            "headers": { "X-Vault-Token": self.vcom.vault_token },
            "data": '{"data" : {"key": "value"}}'
        }

        full_req = self.vcom.call_vault_api(vaultreq_full)
        
        mock_request.assert_called_once_with(vaultreq_full["method"], vaultreq_full["url"], headers=vaultreq_full["headers"],data=vaultreq_full["data"])
        self.assertIsNotNone(full_req)

    # patch the requests.request method with a Mock that throws exceptions"
    @patch('modules.vault_com_kv.requests.request',side_effect=[requests.ConnectionError,requests.HTTPError])
    def test_call_vault_api_exception(self,mock_request):
        """Verify that the method returns None on exceptions
        """

        req1 = self.vcom.call_vault_api(self.vaultreq)
        req2 = self.vcom.call_vault_api(self.vaultreq)
        self.assertIsNone(req1)
        self.assertIsNone(req2)

# patch call_vault_api() with a mock
@patch('modules.vault_com_kv.VaultComKV.call_vault_api')
class TestCRUDMethods(unittest.TestCase):
    """Test test the CRUD methods
    of VaultComKV class
    """

    # Define a VaultComKV object to be used in the test
    vcom = VaultComKV("fake_address","fake_token","fake_engine_path")

    def test_add_secret(self,mock_call_vault):
        """Test add_secret() calls call_vault_api() correctly and returns the response
        """

        mock_call_vault.return_value.ok=True
        resp = self.vcom.add_secret("secret_name",{ "key1": "value1", "key2": "value2"})

        mock_call_vault.assert_called_once_with({
            "method": "POST",
            "url": "/".join([self.vcom.vkvengine_base_url, "data", "secret_name"]),
            "headers": {"X-Vault-Token": self.vcom.vault_token},
            "data": '{"data": {"key1": "value1", "key2": "value2"}}'
        })
        self.assertTrue(resp.ok)

    def test_delete_secret(self,mock_call_vault):
        """Test delete_secret() calls call_vault_api() correctly and returns the response
        """

        mock_call_vault.return_value.ok=True
        resp = self.vcom.delete_secret("secret_name")

        mock_call_vault.assert_called_once_with({
            "method": "DELETE",
            "url": "/".join([self.vcom.vkvengine_base_url, "metadata", "secret_name"]),
            "headers": {"X-Vault-Token": self.vcom.vault_token}
        })
        self.assertTrue(resp.ok)

    def test_read_secret(self,mock_call_vault):
        """Test read_secret() calls call_vault_api() correctly and returns the response
        """

        mock_call_vault.return_value.ok=True
        resp = self.vcom.read_secret("secret_name")

        mock_call_vault.assert_called_once_with({
            "method": "GET",
            "url": "/".join([self.vcom.vkvengine_base_url, "data", "secret_name"]),
            "headers": {"X-Vault-Token": self.vcom.vault_token}
        })
        self.assertTrue(resp.ok)

    def test_list_secrets(self,mock_call_vault):
        """Test list_secrets() calls call_vault_api() correctly and returns the response
        """

        mock_call_vault.return_value.ok=True
        resp = self.vcom.list_secrets()

        mock_call_vault.assert_called_once_with({
            "method": "LIST",
            "url": "/".join([self.vcom.vkvengine_base_url, "metadata"]),
            "headers": {"X-Vault-Token": self.vcom.vault_token}
        })
        self.assertTrue(resp.ok)

if __name__ == "__main__":
    unittest.main()
