#!/usr/bin/env bash
# Installs the latest release of the vault python client
# Optional: 1st argument - user for which to install. Defaults to vagrant.
# Rquired: packages - wget, curl, unzip, jq

[ "$1" == "" ] && V_USR="vagrant" || V_USR="$1"

# downiload latest realease form github.com
DOWNLOAD_URL=$(curl -s "https://api.github.com/repos/slavrd/vault-lab/releases/latest" | \
                jq -r '.assets[] | select( .name == "vault-python-client.zip" ).browser_download_url')

wget -q -P /tmp $DOWNLOAD_URL

# remove current client install if present
[ -d /home/$V_USR/vault-python-client/ ] && sudo rm -rf /home/$V_USR/vault-python-client/

# unzip files to vagrant user home
sudo su -c 'unzip -q /tmp/vault-python-client.zip -d ~' $V_USR

# install needed python packages for vagrant user
sudo su -l -c 'pushd ~/vault-python-client; pipenv install' $V_USR >>/dev/null 2>&1

# cleanup
rm -f /tmp/vault-python-client.zip
