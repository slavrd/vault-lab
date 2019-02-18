using System;
using Xunit;
using VaultKVCom;
using Moq;
using System.Collections.Generic;

namespace VaultDotnetClient.Tests
{
    /// <summary>
    /// Tests for ConsoleUI.AddSecret()
    /// </summary>
    public class TestAddSecret
    {

        /// <summary>
        /// Verify only acceptable user choices are taken,
        /// and the call to VaultCom.AddKVSecret()
        /// </summary>
        [Fact]
        public void TestAddSecretSuccess()
        {
            // Setup user input
            Helpers.MockUserInput testInput = new Helpers.MockUserInput(
                new string[] {
                    // input for secret name
                    "",
                    "/",
                    "my-secret",
                    // input for number of kv pairs
                    "",
                    "string",
                    "0",
                    "2",
                    // input for 2 kv pairs
                    "","key1",
                    "","value1",
                    "key1","key2", // confirm no duplicate keys allowed
                    "value2",
                    // input for continue after success/failure message
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
                .Setup(mock => mock.AddKVSecret(
                   It.IsAny<string>(),
                   It.IsAny<Dictionary<string, string>>()
                   ))
                .ReturnsAsync(true)
                .Verifiable();

            // Invoke ConsoleUI.AddSecret()
            ConsoleUI testConsoleUI = new ConsoleUI(mockVaultCom.Object, testInput);
            testConsoleUI.AddSecret();

            // Confirm all user input was consumed
            Assert.True(testInput.CurrentInputIndex == testInput.UserInput.Length
                , "Not all user input was consumed");

            // Verify the call to VaultCom.AddKVSecret()
            mockVaultCom.Verify(mock => mock.AddKVSecret("my-secret",expSecretData),
                Times.Once(),
                "The call to VaultCom.AddKVSecret() was not as expected."
            );

        }
    }
}
