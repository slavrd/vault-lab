using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VaultKVCom.Contracts
{
    // Payload of vault response to read kv secret
    [JsonObject]
    internal class RespReadSecret
    {   

        // Response metadata structure
        [JsonObject]
        internal class MetaData
        {
            [JsonProperty("created_time")]
            KeyValuePair<string, string> createdTime;

            [JsonProperty("deleteion_time")]
            KeyValuePair<string, string> deleteionTime;

            [JsonProperty("destroyed")]
            KeyValuePair<string,bool> destroyed;

            [JsonProperty("version")]
            KeyValuePair<string,int> version;

            internal MetaData()
            {
                createdTime = new KeyValuePair<string, string>();
                deleteionTime = new KeyValuePair<string, string>();
                destroyed = new KeyValuePair<string, bool>();
                version = new KeyValuePair<string, int>(); 
            }
        }

        [JsonProperty("metadata")]
        internal MetaData metaData {get; set;}

        [JsonProperty("data")]
        internal Dictionary<string, Dictionary<string,string>> data {get; set;}

        internal RespReadSecret()
        {
            this.data = new  Dictionary<string, Dictionary<string,string>>();
            this.metaData = new MetaData();
        }       
    }
}