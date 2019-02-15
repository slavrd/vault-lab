using System;
using VaultDotnetClient.Interfaces;

namespace VaultDotnetClient
{
    public class GetUserInput : VaultDotnetClient.Interfaces.IUserInput
    {
        string IUserInput.GetUserInput()
        {
            return Console.ReadLine().Trim();
        }
    }
}