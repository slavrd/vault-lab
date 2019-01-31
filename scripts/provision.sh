#!/usr/bin/env bash
# Install needed software and basic configs

VAULT=0.11.4

# install
PKG="wget unzip curl jq"
which ${PKG} &>/dev/null || {
  export DEBIAN_FRONTEND=noninteractive
  apt-get update
  apt-get install -y ${PKG}
}

# check vault binary
which vault &>/dev/null || {
  pushd /usr/local/bin
  [ -f vault_${VAULT}_linux_amd64.zip ] || {
    sudo wget -q https://releases.hashicorp.com/vault/${VAULT}/vault_${VAULT}_linux_amd64.zip
  }
  sudo unzip vault_${VAULT}_linux_amd64.zip
  sudo chmod +x vault
  popd
}

# Set .bash_profile to load .bashrc
grep '\.bashrc' ~/.bash_profile &>/dev/null || \
echo -e "\nif [ -f ~/.bashrc ]; then\n\tsource ~/.bashrc\nfi" | \
sudo tee -a ~/.bash_profile

grep '\.bashrc' /vagrant/home/.bash_profile &>/dev/null || \
echo -e "\nif [ -f ~/.bashrc ]; then\n\tsource ~/.bashrc\nfi" | \
sudo tee -a /home/vagrant/.bash_profile

sudo chown vagrant /home/vagrant/.bash_profile