using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VaultKVCom.Contracts
{
    // Payload of vault response to list secrets request
    [JsonObject()]
    internal class RespListSecrets
    {

        [JsonProperty("data")]
        internal Dictionary<string,List<string>> data;

        internal RespListSecrets()
        {
            data = new Dictionary<string,List<string>>();
        }
    
    }
}