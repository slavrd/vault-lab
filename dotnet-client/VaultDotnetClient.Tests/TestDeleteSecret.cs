using System;
using Xunit;
using VaultKVCom;
using Moq;
using System.Collections.Generic;

namespace VaultDotnetClient.Tests
{
    /// <summary>
    /// Tests for ConsoleUI.DeleteSecret()
    /// </summary>
    public class TestDeleteSecret
    {
        /// <summary>
        /// Verify the call to Vault API
        /// </summary>
        [Fact]
        public void TestDeleteSecretSuccess()
        {
            // Ssetup user input
            Helpers.MockUserInput testInput = new Helpers.MockUserInput(
                new string[] {"my-secret",""});

            // Set up mocked VaultCom object
            Mock<VaultCom> mockVaultCom = new Mock<VaultCom>(MockBehavior.Strict
                , "http://localhost:8200"
                , "test_token"
                , "test_kv_path"
                , null
            );
            mockVaultCom
                .Setup(mock => mock.DeleteKVSecret(
                   It.IsAny<string>()
                   ))
                .ReturnsAsync(true)
                .Verifiable();

            // Call Vault API
            ConsoleUI testConsoleUI = new ConsoleUI(mockVaultCom.Object, testInput);
            testConsoleUI.DeleteSecret();

            // Verify the call
            mockVaultCom.Verify(mock => mock.DeleteKVSecret("my-secret"));
        }

    }

}