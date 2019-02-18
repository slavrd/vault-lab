using System;
using Xunit;
using VaultKVCom;
using Moq;

namespace VaultDotnetClient.Tests
{
    public class TestDisplayMenu
    {
        /// <summary>
        /// Verify only acceptable user choices are taken and
        /// the the matching ConsoleUI.MainMenu enum is returned.
        /// </summary>

        [Fact]
        public void VerifyUserChoice()
        {
            // Prepeare test user input
            Helpers.MockUserInput testInput = new Helpers.MockUserInput(
                new string[] {
                        //test input strings
                    "string",
                    "",
                        // test ints outside menuItems defined enums
                    "-1",
                    "1000",
                        // test that correct input is returned
                    "1", 
                }
            );

            // Mock VaultCom
            Mock<VaultCom> mockVaultCom = new Mock<VaultCom>(MockBehavior.Strict
                ,"http://localhost:8200"
                ,"test_token"
                ,"test_kv_path"
                ,null
            );

            // Create ConsoleUI instance for testing
            ConsoleUI testConsoleUI = new ConsoleUI(mockVaultCom.Object, testInput);

            var result = testConsoleUI.DisplayMenu();

            // Ensure that all test input was consumed
            Assert.True(testInput.CurrentInputIndex == testInput.UserInput.Length
                ,"Not all user input was consumed");

            // Ensure that the correct item was returned, based on the provided input
            Assert.IsType<ConsoleUI.MenuItems>(result);
            Assert.Equal(Enum.Parse(typeof(ConsoleUI.MenuItems),"1"), result);

        }
    }
}
