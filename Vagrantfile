Vagrant.configure("2") do |config|
  config.vm.box = "slavrd/xenial64"
  
  # set VM specs
  config.vm.provider "virtualbox" do |v|
    v.memory = 1024
    v.cpus = 2
  end

  config.vm.provision :shell, :path => "scripts/provision.sh"
  config.vm.provision :shell, :path => "scripts/python_setup.sh"
  config.vm.provision :shell, :path => "scripts/vault_setup.sh", run: "always"
  
  config.vm.define "vault01" do |l1|
      l1.vm.hostname = "vault01"
      # l1.vm.network "private_network", ip: "192.168.2.10"
      l1.vm.network "forwarded_port", guest: 8200, host: 8200
  end

end
