#!/usr/bin/env bash
# Install/Setup python prerequisites

[ "$1" == "" ] && V_USR="vagrant" || V_USR="$1"

# install pip
sudo apt-get update >> /dev/null
sudo apt-get install -y python3-virtualenv python3-pip >> /dev/null

# install pipenv for specified user
sudo su -c "pip3 install --user pipenv" $V_USR >> /dev/null

# add ./local/bin to the specified user path
grep '~/.local/bin' /home/$V_USR/.bash_profile || { 
    echo 'export PATH=~/.local/bin:$PATH' | tee -a  /home/$V_USR/.bash_profile
}
