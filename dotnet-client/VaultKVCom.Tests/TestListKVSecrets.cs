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
    public class TestListKVSecrets
    {
        private List<string> expSecretsList;

        private StringContent vaultRespBody;

        public TestListKVSecrets()
        {
            expSecretsList = new List<string>{ "my-secret1","my-secret2" };
            vaultRespBody = new StringContent(System.IO.File.ReadAllText("sampleListKVSecretsResp.json",System.Text.Encoding.UTF8));
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
                   Content = vaultRespBody
                })
                .Verifiable();

            return moqHandler;

        }
    
        [Fact]
        ///<summary>
        /// Test ListKVSecret() with a successfull response form Vault API
        ///</summary>

        public async void TestSuccess_ListKVSecrets()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.OK);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            var callResult = await vcom.ListKVSecrets();

            // Verify returned result
            Assert.NotNull(callResult);
            Assert.Equal(expSecretsList,callResult);

            // Verify HttpClient.SendAsync() call
            Uri expReqUrl = new Uri("http://test.com/v1/vault_path/metadata");
            moqHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>( req =>
                    req.Method == new HttpMethod("LIST")
                    && req.RequestUri == expReqUrl
                    && req.Headers.Contains("X-Vault-Token")
                    && new List<string>(req.Headers.GetValues("X-Vault-Token")).Contains("vault_token")
                ),
                ItExpr.IsAny<CancellationToken>()
            );

        }
    
        [Fact]
        ///<summary>
        /// Test ListKVSecret() with a failure response form Vault API
        ///</summary>

        public async void TestFailure_ListKVSecrets()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.BadRequest);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            var callResult = await vcom.ListKVSecrets();

            // Verify returned result
            Assert.Null(callResult);

        }

    }
}