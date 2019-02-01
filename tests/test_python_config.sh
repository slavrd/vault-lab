#!/usr/bin/env bash
# Test the VM python configuratoin
ERR=0

# Check if ~/.local/bin is in the user's path

echo "$PATH" | grep ~/\.local/bin >>/dev/null || {
    echo "~/.local/bin is not in PATH"; ERR=1
}

# check if pipenv is installed and avaulable
which pipenv >>/dev/null || {
    echo "pipenv is not isntalled or not in PATH"; ERR=1
}

exit $ERR