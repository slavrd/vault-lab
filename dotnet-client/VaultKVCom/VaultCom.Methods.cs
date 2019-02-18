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
    // Defines the methods for the VaultCom class
    public partial class VaultCom
    {
        
        ///<summary>
        /// Create or update Vault KV secret with seceretData.
        /// Returns bool to indicate success.
        ///</summary>
        public virtual async Task<bool> AddKVSecret(string secret, Dictionary<string, string> secretData)
        {
            // Check for Null argument
            if(secret == null)
            {
                Console.Error.WriteLine("Provided secret is null");
                return false;
            }
            // prepare request body
            ReqAddSecret secData = new ReqAddSecret(secretData);
            StringContent content = new StringContent(JsonConvert.SerializeObject(secData,jsonConvertSettings),Encoding.UTF8,"application/json");

            // execute request
            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                result = await webClient.PostAsync(Url.Combine(KVBaseAPI,"data",secret),content);
            }
            catch(HttpRequestException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
            

            // Handle response
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.Error.WriteLine($"{result.StatusCode}: {result.ReasonPhrase}");
                return false;
            }
        }
    
        ///<summary>
        /// Returns the keyvalue pair in the provided secret data as dictionary.
        /// Reterns null on network error
        ///</summary>
        public virtual async Task<Dictionary<string, string>> GetKVSecret(string secret)
        {
            // Handle empty argument
            if(String.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("The secret string is empty or null");
            }

            // call Vault API
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {   
                HttpRequestMessage req = new HttpRequestMessage();
                response = await webClient.GetAsync(Url.Combine(KVBaseAPI,"data",secret));
            }
            catch(HttpRequestException e)
            {
                Console.Error.WriteLine(e.Message);
                return null;
            }
            
            // Handle Response
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }
            else
            {
                var respString = await response.Content.ReadAsStringAsync();
                var secretData = JsonConvert.DeserializeObject<RespReadSecret>(respString);
                return secretData.data["data"];
            }
            
        }

        ///<summary>
        /// Deletes the provided secret from Vault
        /// Returns bool to indicate success
        ///</summary>
        public virtual async Task<bool> DeleteKVSecret(string secret)
        {
            // Handle empty argument
            if(String.IsNullOrEmpty(secret))
            {
                throw new ArgumentNullException("The secret string is empty or null");
            }

            // call Vault API
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await webClient.DeleteAsync(Url.Combine(KVBaseAPI,"metadata",secret));
            }
            catch(HttpRequestException e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }

            // Handle response
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                return false;
            }
            else
            {
                return true;
            }

        }

        ///<summary>
        /// Lists all secrets on the current Vault path - this.VaultKVPath
        /// Returns null on failure
        ///</summary>
        public virtual async Task<List<string>> ListKVSecrets()
        {

            // Call the vault API
            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage req = new HttpRequestMessage
            {
                Method = new HttpMethod("LIST"),
                RequestUri = new Uri(Url.Combine(KVBaseAPI,"metadata"))
            };
            try
            {
                response = await webClient.SendAsync(req);
            }
            catch(HttpRequestException e)
            {
                Console.Error.WriteLine(e.Message);
                return null;
            }

            // Handle response
            if(!response.IsSuccessStatusCode)
            {
                Console.Error.WriteLine($"{(int)response.StatusCode}: {response.ReasonPhrase}");
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<RespListSecrets>(await response.Content.ReadAsStringAsync()).data["keys"];
            }

        }

    }

}
