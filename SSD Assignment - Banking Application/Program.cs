using Banking_Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.FileIO;
using SSD_Assignment___Banking_Application;
using SSD_Assignment___Banking_Application.Interfaces;
using SSD_Assignment___Banking_Application.Services;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Banking_Application
{
    public class Program : ValidationService
    {
        static void Main(string[] args)
        {
            // Setup Dependency Injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<CryptoManager>()
                .AddSingleton<ValidationService>()
                .AddSingleton<Data_Access_Layer>() 
                .AddSingleton<BankAccountService>()
                .AddSingleton<IEncryptionService ,EncryptionService>()
                .BuildServiceProvider();

            var bankAccountService = serviceProvider.GetService<BankAccountService>();
            var validationService = serviceProvider.GetService<ValidationService>();

            bool running = true;
            while (running && bankAccountService != null && validationService != null)
            {
                Console.WriteLine("\n***Banking Application Menu***");
                Console.WriteLine("1. Add Bank Account");
                Console.WriteLine("2. Close Bank Account");
                Console.WriteLine("3. View Account Information");
                Console.WriteLine("4. Make Lodgment");
                Console.WriteLine("5. Make Withdrawal");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");
                string option = Console.ReadLine();

                if (!validationService.IsValidMenuOption(option))
                {
                    Console.WriteLine("Invalid option chosen. Please try again.");
                    continue;
                }

                switch (option)
                {
                    case "1":
                        // Add Bank Account
                        bankAccountService.AddBankAccount();
                        break;
                    case "2":
                        // Close Bank Account
                        bankAccountService.CloseBankAccount();
                        break;
                    case "3":
                        // View Account information
                        bankAccountService.ViewAccountInformation();
                        break;
                    case "4":
                        // Make a Lodgment
                        bankAccountService.MakeLodgement();
                        break;
                    case "5":
                        // Make a Withdrawal 
                        bankAccountService.MakeWithdrawal();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option chosen. Please try again.");
                        break;
                }
            }
        }
    }
}

