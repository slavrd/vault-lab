#!/usr/bin/env bash
# Installs the latest release of the provided vault client from slavrd/vault-lab repo.
# Optional: 1st argument - user for which to install. Defaults to vagrant.
# Required: client name as 1st argument
# Rquired: packages - wget, curl, unzip, jq

CLIENT_NAME="$1"
[ "$2" == "" ] && V_USR="vagrant" || V_USR="$2"

# downiload latest realease form github.com
DOWNLOAD_URL=$(curl -s "https://api.github.com/repos/slavrd/vault-lab/releases/latest" | \
                jq -r ".assets[] | select( .name == \"$CLIENT_NAME.zip\" ).browser_download_url")

wget -q -P /tmp $DOWNLOAD_URL || {
    echo "Download of $CLIENT_NAME failed"
    exit 1
}

# remove current client install if present
[ -d /home/$V_USR/$CLIENT_NAME/ ] && sudo rm -rf /home/$V_USR/$CLIENT_NAME/

# unzip files to vagrant user home
sudo su -c "unzip -q /tmp/$CLIENT_NAME.zip -d ~" $V_USR

# if the client is the vault-python-client install needed python packages for provided user
if [ "$CLIENT_NAME" == "vault-python-client" ]; then
    sudo su -l -c 'pushd ~/vault-python-client; pipenv install' $V_USR >>/dev/null 2>&1
fi

# cleanup
rm -f /tmp/$CLIENT_NAME
