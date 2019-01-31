# Vault Lab

A Vagrant project that builds a VM with Vault installed and running in dev mode.

## Prerequisites

* Install VirtualBox - [instructions](https://www.virtualbox.org/wiki/Downloads)
* Install Vagrant - [instructions](https://www.vagrantup.com/downloads.html)

## Running the project

* Start the Vagrant VM - `vagrant up`
* Login to the VM - `vagrant ssh`

At this point vault is running in dev mode.

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