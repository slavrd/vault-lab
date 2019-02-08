# Vault Lab

A Vagrant project that builds two VMs. One with Vault installed and running in dev mode. Another to use with Vault clients.

The repository includes also a python Vault client for basic interaction with Vault's KV secrets engine.

## Prerequisites

* Install VirtualBox - [instructions](https://www.virtualbox.org/wiki/Downloads)
* Install Vagrant - [instructions](https://www.vagrantup.com/downloads.html)

## Running the project

* Start the Vagrant VMs - `vagrant up`
* Login to the Vault server VM - `vagrant ssh vault01`

At this point Vault is running in dev mode.

The Vault server address is set in `VAULT_ADDR` an the root token in `VAULT_TOKEN` env variables.

The Vault API can be used from the command line with curl. For example:

* To create a KV secret:

```Bash
curl \
    --header "X-Vault-Token: $VAULT_TOKEN" \
    --request "POST"
    --data '{"data": {"my-secret-key": "my-secret-value"}}'
    "$VAULT_ADDR/v1/secret/data/my-secret"
```

* To read a KV secret:

```Bash
curl \
    --header "X-Vault-Token: $VAULT_TOKEN" \
    "$VAULT_ADDR/v1/secret/data/my-secret"
```

The project also includes a bash script that simulates a basic application that uses the Vault KV engine. It is in `/vagrant/app.sh`

## Vagrant client machine

The repository includes a python client for the Vault KV secrets engine in the `python-client` folder. The vagrant project will create a second VM called client01 with the latest version of the client installed on it.

The client reads its Vault access configuration from the environment variables - `VAULT_TOKEN`,`VAULT_ADDR`,`VAULT_KV_PATH`. They are preset for the vagrant user.

* Login to the client VM - `vagrant ssh client01`

### Using the Python client

* Change to the client directory - `cd vault-python-client`
* Start the client = `pipenv run python3 main.py`