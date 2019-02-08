#!/usr/bin/env bash
# Prepares realease artifact for the python client

CLIENT_NAME="vault-python-client"

# install zip if not installed
which zip >> /dev/null || {
    sudo apt-get update
    sudo apt-get install -y zip
}

[ -d $CLIENT_NAME ] || {
    mkdir $CLIENT_NAME 
}

# copy needed files
pushd python-client >> /dev/null
cp -r main.py Pipfile modules/ ~-/$CLIENT_NAME/. || {
    echo "Copying files failed"
    exit 1
}
popd >> /dev/null

# create archive
zip -qr9 $CLIENT_NAME.zip $CLIENT_NAME || {
    echo "Creating archive failed"
    exit 1
} && {
    echo "Artefact creted - $(pwd)/$CLIENT_NAME.zip"
}
 