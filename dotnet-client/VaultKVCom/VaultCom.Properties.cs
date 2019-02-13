using System;
using System.Text;
using Flurl;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VaultKVCom.Contracts;

namespace VaultKVCom
{
    // Defines the properties and constructors for VaultKVCom
    public partial class VaultCom
    {
        public Url VaultAddr { get; set; }

        public string VaultToken { get; set; }

        public string VaultKVPath { get; set;}

        public Url KVBaseAPI 
        {
            get {
                return Url.Combine(VaultAddr,"v1",VaultKVPath);
            }
        }

        private HttpClient webClient;

        private JsonSerializerSettings jsonConvertSettings;

        public VaultCom(string vaddr, string vtoken, string kvpath, HttpClient webClient = null)
        {
            this.VaultAddr = new Url(vaddr);
            this.VaultToken = vtoken;
            this.VaultKVPath = kvpath;

            // Check if webClient was provided. If not create a new instance
            if (webClient == null)
            {
                this.webClient = new HttpClient();
            }
            else
            {
                this.webClient = webClient;
            }

            // Add authorization vault Authorization header
            this.webClient.DefaultRequestHeaders.Add("X-Vault-Token",this.VaultToken);
            
            // Basic json converter settings to use when needed
            jsonConvertSettings = new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };
        }
        
    }
}
