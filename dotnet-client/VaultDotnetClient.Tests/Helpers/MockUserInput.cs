using System;
using System.Collections.Generic;
using System.Text;
using VaultDotnetClient.Interfaces;

namespace VaultDotnetClient.Tests.Helpers
{
    /// <summary>
    /// User input simulator
    /// </summary>
    internal class MockUserInput : Interfaces.IUserInput
    {
        internal uint CurrentInputIndex { get; set; }

        internal string[] UserInput { get; set; }

        internal MockUserInput(string[] userInput)
        {
            this.UserInput = userInput;
            CurrentInputIndex = 0;
        }

        /// <summary>
        /// Generates user input from predefined UserInput and increases InputIndex by 1.
        /// </summary>
        /// <returns>The value from UserInput at InputIndex</returns>
        string IUserInput.GetUserInput()
        {
            CurrentInputIndex++;
            return UserInput[CurrentInputIndex - 1];
        }
    }
}
