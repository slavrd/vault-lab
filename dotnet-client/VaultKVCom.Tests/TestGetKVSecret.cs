using System;
using System.Collections.Generic;
using Xunit;
using VaultKVCom;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace VaultKVCom.Tests
{
    public class TestGetKVSecret
    {
        private Dictionary<string, string> expectedKVSecret;

        private string expResp;

        public TestGetKVSecret()
        {
            expectedKVSecret = new Dictionary<string,string>
            {
                { "key1","value1" },
                {"key2","value2"}
            };

            expResp = System.IO.File.ReadAllText("sampleGetKVSecretResp.json",System.Text.Encoding.UTF8);          
        }
    

        ///<summary>
        /// Creates a Moq.Mock of HttpRequestMessage to use with HttpClient
        /// The Mock Returns HttpResponseMessage with StatusCode set to provided code.
        ///</summary>
        private Mock<HttpMessageHandler> NewMoqHttpHandler(HttpStatusCode code) {
            // Define a Mock to use with the httpclient
            var moqHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            moqHandler
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                   StatusCode = code,
                   Content = new StringContent(expResp,System.Text.Encoding.UTF8)
                })
                .Verifiable();

            return moqHandler;
        }

        [Fact]
        ///<summary>
        /// Test GetKVSecret() with a successfull response form Vault API
        ///</summary>

        public async void TestSuccess_GetKVSecret()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.OK);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            var callResult = await vcom.GetKVSecret("test-secret");

            // Verify returned result
            Assert.NotNull(callResult);
            Assert.Equal(expectedKVSecret,callResult);

        }

        [Fact]
        ///<summary>
        /// Test GetKVSecret() with a failure response form Vault API
        ///</summary>

        public async void TestFailure_GetKVSecret()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.BadRequest);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            var callResult = await vcom.GetKVSecret("test-secret");

            // Verify returned result
            Assert.Null(callResult);

        }
    }
}