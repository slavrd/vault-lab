﻿using System;
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
        /// Create or update Vault KV secret with secertData.
        /// Returns bool to indicate success.
        ///</summary>
        public bool AddKVSecret(string secret, Dictionary<string, string> secretData)
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
            content.Headers.Add("X-Vault-Token",VaultToken);

            // execute request
            HttpResponseMessage result = new HttpResponseMessage();
            try
            {
                result = webClient.PostAsync(Url.Combine(KVBaseAPI,"data",secret),content).Result;
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
    
    }

}