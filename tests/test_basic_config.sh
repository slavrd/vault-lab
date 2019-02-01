#!/usr/bin/env bash
# Tests if the machine is configured
ERR=0

# Test env variables
[ "$VAULT_ADDR" == "" ] && { 
    echo "VAULT_ADDR is not set" >&2; ERR=1 
}
[ "$VAULT_TOKEN" == "" ] && { 
    echo "VAULT_TOKEN is not set" >&2; ERR=1 
}

# Test vault is installed running
which vault >> /dev/null || { 
    echo "Vault is not installed" >&2; ERR=1 
}
sudo pgrep vault >> /dev/null || { 
    echo "Vault proccess is not running" >&2; ERR=1 
}

exit $ERR
