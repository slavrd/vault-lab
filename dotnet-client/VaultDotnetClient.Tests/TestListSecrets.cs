using System;
using Xunit;
using VaultKVCom;
using Moq;
using System.Collections.Generic;

namespace VaultDotnetClient.Tests
{
    public class TestListSecrets
    {

        ///<summary>
        /// Verify the Vault API call of ListSecrets()
        ///</summary>
        [Fact]
        public void TestListSecretsSuccess()
        {

            // Set up a list for the mock to retrun
            List<string> secretsList = new List<string>
            {
                "my-secret-1","my-secret-2"
            };

            // Setup mock VaultCom object
            Mock<VaultCom> mockVaultCom = new Mock<VaultCom>(MockBehavior.Strict
                , "http://localhost:8200"
                , "test_token"
                , "test_kv_path"
                , null
            );

            mockVaultCom
                .Setup(mock => mock.ListKVSecrets())
                .ReturnsAsync(secretsList)
                .Verifiable();

            // Invoke ConsoleUI.ListSecrets()
            ConsoleUI testConsoleUI = new ConsoleUI(mockVaultCom.Object, new Helpers.MockUserInput(new string[]{""}));
            testConsoleUI.ListSecrets();

            // Verify the Vault API call
            mockVaultCom.Verify( mock => mock.ListKVSecrets());

        }

    }

}