using System;
using Xunit;
using VaultKVCom;
using Moq;
using System.Collections.Generic;

namespace VaultDotnetClient.Tests
{
    /// <summary>
    /// Tests for ConsoleUI.ReadSecret()
    /// </summary>
    public class TestReadSecret
    {
        
        /// <summary>
        /// Test the API call to VaultCom.GetKVSecret() 
        /// </summary>
        [Fact]
        public void TestReadSecretSuccess()
        {

        // Setup user input
            Helpers.MockUserInput testInput = new Helpers.MockUserInput(
                new string[] {
                    // input for secret name
                    "",
                    "/",
                    "my/secret",
                    "my-secret",
                    ""
                }
            );

        // define expected secret data dictionary based on input
            Dictionary<string, string> expSecretData = new Dictionary<string, string>
            {
                { "key1","value1" },
                { "key2","value2" }
            };

        // Set up mocked VaultCom object
            Mock<VaultCom> mockVaultCom = new Mock<VaultCom>(MockBehavior.Strict
                , "http://localhost:8200"
                , "test_token"
                , "test_kv_path"
                , null
            );
            mockVaultCom
                .Setup(mock => mock.GetKVSecret(
                   It.IsAny<string>()
                   ))
                .ReturnsAsync(expSecretData)
                .Verifiable();

        // Invoke ConsoleUI.ReadSecret()
            ConsoleUI testConsoleUI = new ConsoleUI(mockVaultCom.Object, testInput);
            testConsoleUI.ReadSecret();

            // Confirm all user input was consumed
            Assert.True(testInput.CurrentInputIndex == testInput.UserInput.Length
                , "Not all user input was consumed");

            // Verify the call to VaultCom.GetKVSecret()
            mockVaultCom.Verify(mock => mock.GetKVSecret("my-secret"),
                Times.Once(),
                "The call to VaultCom.AddKVSecret() was not as expected."
            );

        }

    }

}