import requests
import json
from typing import Dict, Any

class VaultComKV:
    # Communicator for Vault's KV secrets engine

    def __init__(self, vaddr: str, vtoken: str, vpath: str) -> None:
        """Construct a Vault comminication object using provided
        vault address, vault authentication token and
        KV secrets engine path
        """

        self.vault_address = vaddr
        self.vault_token = vtoken
        self.vkvengine_path = vpath
        self.vkvengine_base_url = "/".join([self.vault_address,"v1",self.vkvengine_path])

    def call_vault_api(self, request: Dict[str, Any]) -> requests.Response:
        """Make a request to Vault's API based on the data in req.
        req dictionary must contain method,url,headers and data keys.
        """
        if not "data" in request:
            request.update({"data": ""})
        try:
            req = requests.request(request["method"],request["url"],headers=request["headers"],data=request["data"])
        except requests.HTTPError as err:
            print(err)
            return None
        except requests.ConnectionError as err:
            print(err)
            return None

        return req

    def add_secret(self, secret: str, kv: Dict[str, str]) -> requests.Response:
        """Add or update a secret in Vault
        """

        url = "/".join([self.vkvengine_base_url, "data", secret])
        headers = { "X-Vault-Token": self.vault_token }
        data = json.dumps({"data" : kv})

        vaultreq = {
            "method": "POST",
            "url": url,
            "headers": headers,
            "data": data
        }

        return self.call_vault_api(vaultreq)

    def delete_secret(self, secret: str) -> requests.Response:
        """Completely deletes a KV secret from Vault
        """

        url = "/".join([self.vkvengine_base_url, "metadata", secret])
        headers = { "X-Vault-Token": self.vault_token }

        vaultreq = {
            "method": "DELETE",
            "url": url,
            "headers": headers
        }

        return self.call_vault_api(vaultreq)

    def read_secret(self, secret: str) -> requests.Response:
        """Retrieve the data of a Vault secret
        """

        url = "/".join([self.vkvengine_base_url, "data", secret])
        headers = { "X-Vault-Token": self.vault_token }

        vaultreq = {
            "method": "GET",
            "url": url,
            "headers": headers
        }

        return self.call_vault_api(vaultreq)

    def list_secrets(self) -> requests.Response:
        """List all kv secrets available at the object's
        kv engine path.
        """

        url = "/".join([self.vkvengine_base_url, "metadata"])
        headers = { "X-Vault-Token": self.vault_token }

        vaultreq = {
            "method": "LIST",
            "url": url,
            "headers": headers
        }

        return self.call_vault_api(vaultreq)
        