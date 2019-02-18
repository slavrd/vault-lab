Vagrant.configure("2") do |config|

  # Vault server ip var
  vault_server_ip = "192.168.2.10"
  
  config.vm.box = "slavrd/xenial64"
  config.vm.provision :shell, :path => "scripts/provision.sh"

  # Define Vault server
  config.vm.define "vault01" do |l1|

    # set VM specs
    l1.vm.provider "virtualbox" do |v|
      v.memory = 1024
      v.cpus = 2
    end

    l1.vm.hostname = "vault01"
    l1.vm.network "private_network", ip: vault_server_ip
    l1.vm.network "forwarded_port", guest: 8200, host: 8200

    l1.vm.provision :shell, :path => "scripts/vault_setup.sh", run: "always"

  end
  
  # Define client VM
  config.vm.define "client01" do |c1|

    c1.vm.hostname = "client01"
    c1.vm.network "private_network", ip: "192.168.2.20"

    c1.vm.provision :shell, :path => "scripts/python_setup.sh"
    c1.vm.provision :shell, :path => "scripts/dotnet_setup.sh"
    c1.vm.provision :shell, :path => "scripts/install_python_client.sh"   
    c1.vm.provision "shell" do |p1|
      p1.path = "scripts/config_vault_client.sh"
      p1.args = [vault_server_ip]
    end
  
  end

end
