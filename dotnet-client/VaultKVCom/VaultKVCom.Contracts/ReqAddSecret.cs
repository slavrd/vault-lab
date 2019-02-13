using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VaultKVCom.Contracts
{
    // Payload of vault request to create/update kv secrets
    [JsonObject]
    internal class ReqAddSecret
    {   
        [JsonProperty("options")]
        internal Dictionary<string, string> options {get; set;}

        [JsonProperty("data")]
        internal Dictionary<string, string> data {get; set;}

        internal ReqAddSecret(Dictionary<string, string> secretData, Dictionary<string, string> options = null)
        {
            this.data = secretData;
            this.options = options;
        }       
    }
}