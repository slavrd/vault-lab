#!/usr/bin/env bash
# Prepares realease artifact for the dotnet client

CLIENT_NAME="vault-dotnet-client"

# install zip if not installed
which zip >> /dev/null || {
    sudo apt-get update
    sudo apt-get install -y zip
}

# Create the build
dotnet publish -c Release -o "../../$CLIENT_NAME" ./dotnet-client/VaultDotnetClient/VaultDotnetClient.csproj 

# create archive
zip -qr9 $CLIENT_NAME.zip $CLIENT_NAME || {
    echo "Creating archive failed"
    exit 1
} && {
    echo "Artefact creted - $(pwd)/$CLIENT_NAME.zip"
}