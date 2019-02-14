using System;
using Xunit;
using VaultKVCom;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VaultKVCom.Tests
{
    public class TestDeleteKVSecret
    {

        ///<summary>
        /// Creates a Moq.Mock of HttpRequestMessage to use with HttpClient
        /// The Mock Returns HttpResponseMessage with StatusCode set to provided code.
        ///</summary>
        private Mock<HttpMessageHandler> NewMoqHttpHandler(HttpStatusCode code)
        {
            Mock<HttpMessageHandler> moqHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

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
                   StatusCode = code
                })
                .Verifiable();

            return moqHandler;
        }
    
        [Fact]
        public async void TestSuccess_DeleteKVSecret()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.OK);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            bool callResult = await vcom.DeleteKVSecret("test-secret");      

            // Verify returned result
            Assert.IsType<bool>(callResult);
            Assert.True(callResult);

            // Verify the call to the API
            Uri expectedUri = new Uri("http://test.com/v1/vault_path/metadata/test-secret");
            moqHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>( req =>
                    req.Method == HttpMethod.Delete 
                    && req.RequestUri == expectedUri 
                    && req.Headers.Contains("X-Vault-Token")
                    && new List<string>(req.Headers.GetValues("X-Vault-Token")).Contains("vault_token")
                    ),
                ItExpr.IsAny<CancellationToken>()
            );

        }
    
        [Fact]
        public async void TestFailure_DeleteKVSecret()
        {
            var moqHandler = NewMoqHttpHandler(HttpStatusCode.BadRequest);

            // Make the method call
            var httpClient = new HttpClient(moqHandler.Object);
            VaultCom vcom = new VaultCom("http://test.com","vault_token","vault_path", httpClient);
            bool callResult = await vcom.DeleteKVSecret("test-secret");      

            // Verify returned result
            Assert.IsType<bool>(callResult);
            Assert.False(callResult);
        }

    }
}