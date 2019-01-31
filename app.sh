#!/usr/bin/env bash
# Basic app that makes calls to Vault's API

# Depends on:
# VAULT_TOKEN for authentication
# VAULT_ADDR for Vault server address
# prerequisite packages - curl, jq

# Function to make a vault call. Takes three arguments - 
# HTTP method (mandatory), Vault path (mandatory) and Request body 
call_vault() {
    HTTP_METHOD=$1
    VAULT_API_PATH=$2
    [ "$3" == "" ] && REQ_DATA="none" || REQ_DATA=$3
    curl -s \
        --header "X-Vault-Token: $VAULT_TOKEN" \
        --request "$HTTP_METHOD" \
        --data "$REQ_DATA" \
        "$VAULT_ADDR/v1/$VAULT_API_PATH"
}

# Infinte loop to display menu, perform action, display menu
while true; do

    clear
    echo "Main menu:"
    echo "=========="
    echo "1) Add/Update KV secret"
    echo "2) Read a KV secret"
    echo "3) List all KV secrets"
    echo "4) Delete KV secret"
    echo "5) Quit"
    echo ""
    read -p "Enter action: " USR_CHOICE

    # Perform selected action
    case $USR_CHOICE in

        1) # add/update a kv secret
            clear
            read -p "provide secret name: " KV_NAME
            read -p "Provide key: " KV_KEY
            read -p "Provide value: " KV_VALUE
            echo ""
            V_REQ_BODY="{\"data\": {\"$KV_KEY\": \"$KV_VALUE\"}}"
            call_vault "POST" "secret/data/$KV_NAME" "$V_REQ_BODY" | jq .
            echo ""
            read -p "Press enter to continue"
            ;;

        2) # read a secret
            clear
            read -p "provide secret name: " KV_NAME
            echo ""
            call_vault "GET" "secret/data/$KV_NAME" | jq .
            echo ""
            read -p "Press enter to continue"
            ;;

        3) # list all secrets
            clear
            call_vault "LIST" "secret/metadata" | jq .data.keys[]
            echo ""
            read -p "Press enter to continue"
            ;;

        4) # delete secret
            clear
            read -p "KV secret name to delete: " KV_NAME
            call_vault "DELETE" "secret/metadata/$KV_NAME" | jq .
            echo ""
            read -p "Press enter to continue"
            ;;

        5) # quit
            exit 0
            ;;

    esac

done


