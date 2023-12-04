using Banking_Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.FileIO;
using SSD_Assignment___Banking_Application;
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
                .AddSingleton<ValidationService>()
                .AddSingleton<Data_Access_Layer>() // Assuming this is your data access layer
                .AddSingleton<BankAccountService>()
                .BuildServiceProvider();

            var bankAccountService = serviceProvider.GetService<BankAccountService>();

            bool running = true;
            while (running && bankAccountService != null)
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

public class ValidationService
{
    public bool IsValidMenuOption(string input)
    {
        return input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6";
    }

    // ... other validation methods
}



//        public static void Main(string[] args)
//        {

//            string text = "Hello World";
//            CryptoManager cryptoManager = new CryptoManager();
//            CngKeyManager cngKeyManager = new CngKeyManager();

//            // Setup the CNG Provider and ensure key is created.
//            cngKeyManager.SetupCngProvider();

//            // Encrypt and Decrypt operations.
//            byte[] encryptedData = cryptoManager.EncryptText(text);
//            string decryptedText = cryptoManager.DecryptText(encryptedData);

//            Console.WriteLine("Encrypted (Base64): " + Convert.ToBase64String(encryptedData));
//            Console.WriteLine("Decrypted Text: " + decryptedText);



//            Data_Access_Layer dal = Data_Access_Layer.getInstance();
//            dal.loadBankAccounts();
//            bool running = true;

//            do
//            {

//                Console.WriteLine("");
//                Console.WriteLine("***Banking Application Menu***");
//                Console.WriteLine("1. Add Bank Account");
//                Console.WriteLine("2. Close Bank Account");
//                Console.WriteLine("3. View Account Information");
//                Console.WriteLine("4. Make Lodgement");
//                Console.WriteLine("5. Make Withdrawal");
//                Console.WriteLine("6. Exit");
//                Console.WriteLine("CHOOSE OPTION:");
//                String option = Console.ReadLine();

//                switch(option)
//                {
//                    case "1":
//                        String accountType = "";
//                        int loopCount = 0;

//                        do
//                        {

//                           if(loopCount > 0)
//                                Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");

//                            Console.WriteLine("");
//                            Console.WriteLine("***Account Types***:");
//                            Console.WriteLine("1. Current Account.");
//                            Console.WriteLine("2. Savings Account.");
//                            Console.WriteLine("CHOOSE OPTION:");
//                            accountType = Console.ReadLine();

//                            loopCount++;

//                        } while (!(accountType.Equals("1") || accountType.Equals("2")));

//                        String name = "";
//                        loopCount = 0;

//                        do
//                        {

//                            if (loopCount > 0)
//                                Console.WriteLine("INVALID NAME ENTERED - PLEASE TRY AGAIN");

//                            Console.WriteLine("Enter Name: ");
//                            name = Console.ReadLine();

//                            loopCount++;

//                        } while (name.Equals(""));

//                        String addressLine1 = "";
//                        loopCount = 0;

//                        do
//                        {

//                            if (loopCount > 0)
//                                Console.WriteLine("INVALID ÀDDRESS LINE 1 ENTERED - PLEASE TRY AGAIN");

//                            Console.WriteLine("Enter Address Line 1: ");
//                            addressLine1 = Console.ReadLine();

//                            loopCount++;

//                        } while (addressLine1.Equals(""));

//                        Console.WriteLine("Enter Address Line 2: ");
//                        String addressLine2 = Console.ReadLine();

//                        Console.WriteLine("Enter Address Line 3: ");
//                        String addressLine3 = Console.ReadLine();

//                        String town = "";
//                        loopCount = 0;

//                        do
//                        {

//                            if (loopCount > 0)
//                                Console.WriteLine("INVALID TOWN ENTERED - PLEASE TRY AGAIN");

//                            Console.WriteLine("Enter Town: ");
//                            town = Console.ReadLine();

//                            loopCount++;

//                        } while (town.Equals(""));

//                        double balance = -1;
//                        loopCount = 0;

//                        do
//                        {

//                            if (loopCount > 0)
//                                Console.WriteLine("INVALID OPENING BALANCE ENTERED - PLEASE TRY AGAIN");

//                            Console.WriteLine("Enter Opening Balance: ");
//                            String balanceString = Console.ReadLine();

//                            try
//                            {
//                                balance = Convert.ToDouble(balanceString);
//                            }

//                            catch 
//                            {
//                                loopCount++;
//                            }

//                        } while (balance < 0);

//                        Bank_Account ba;

//                        if (Convert.ToInt32(accountType) == Account_Type.Current_Account)
//                        {
//                            double overdraftAmount = -1;
//                            loopCount = 0;

//                            do
//                            {

//                                if (loopCount > 0)
//                                    Console.WriteLine("INVALID OVERDRAFT AMOUNT ENTERED - PLEASE TRY AGAIN");

//                                Console.WriteLine("Enter Overdraft Amount: ");
//                                String overdraftAmountString = Console.ReadLine();

//                                try
//                                {
//                                    overdraftAmount = Convert.ToDouble(overdraftAmountString);
//                                }

//                                catch
//                                {
//                                    loopCount++;
//                                }

//                            } while (overdraftAmount < 0);

//                            ba = new Current_Account(name, addressLine1, addressLine2, addressLine3, town, balance, overdraftAmount);
//                        }

//                        else
//                        {

//                            double interestRate = -1;
//                            loopCount = 0;

//                            do
//                            {

//                                if (loopCount > 0)
//                                    Console.WriteLine("INVALID INTEREST RATE ENTERED - PLEASE TRY AGAIN");

//                                Console.WriteLine("Enter Interest Rate: ");
//                                String interestRateString = Console.ReadLine();

//                                try
//                                {
//                                    interestRate = Convert.ToDouble(interestRateString);
//                                }

//                                catch
//                                {
//                                    loopCount++;
//                                }

//                            } while (interestRate < 0);

//                            ba = new Savings_Account(name, addressLine1, addressLine2, addressLine3, town, balance, interestRate);
//                        }

//                        String accNo = dal.AddBankAccount(ba);

//                        Console.WriteLine("New Account Number Is: " + accNo);

//                        break;
//                    case "2":
//                        Console.WriteLine("Enter Account Number: ");
//                        accNo = Console.ReadLine();

//                        ba = dal.findBankAccountByAccNo(accNo);

//                        if (ba is null)
//                        {
//                            Console.WriteLine("Account Does Not Exist");
//                        }
//                        else
//                        {
//                            Console.WriteLine(ba.ToString());

//                            String ans = "";

//                            do
//                            {

//                                Console.WriteLine("Proceed With Delection (Y/N)?"); 
//                                ans = Console.ReadLine();

//                                switch (ans)
//                                {
//                                    case "Y":
//                                    case "y": dal.closeBankAccount(accNo);
//                                        break;
//                                    case "N":
//                                    case "n":
//                                        break;
//                                    default:
//                                        Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");
//                                        break;
//                                }
//                            } while (!(ans.Equals("Y") || ans.Equals("y") || ans.Equals("N") || ans.Equals("n")));
//                        }

//                        break;
//                    case "3":
//                        Console.WriteLine("Enter Account Number: ");
//                        accNo = Console.ReadLine();

//                        ba = dal.findBankAccountByAccNo(accNo);

//                        if(ba is null) 
//                        {
//                            Console.WriteLine("Account Does Not Exist");
//                        }
//                        else
//                        {
//                            Console.WriteLine(ba.ToString());
//                        }

//                        break;
//                    case "4": //Lodge
//                        Console.WriteLine("Enter Account Number: ");
//                        accNo = Console.ReadLine();

//                        ba = dal.findBankAccountByAccNo(accNo);

//                        if (ba is null)
//                        {
//                            Console.WriteLine("Account Does Not Exist");
//                        }
//                        else
//                        {
//                            double amountToLodge = -1;
//                            loopCount = 0;

//                            do
//                            {

//                                if (loopCount > 0)
//                                    Console.WriteLine("INVALID AMOUNT ENTERED - PLEASE TRY AGAIN");

//                                Console.WriteLine("Enter Amount To Lodge: ");
//                                String amountToLodgeString = Console.ReadLine();

//                                try
//                                {
//                                    amountToLodge = Convert.ToDouble(amountToLodgeString);
//                                }

//                                catch
//                                {
//                                    loopCount++;
//                                }

//                            } while (amountToLodge < 0);

//                            dal.lodge(accNo, amountToLodge);
//                        }
//                        break;
//                    case "5": //Withdraw
//                        Console.WriteLine("Enter Account Number: ");
//                        accNo = Console.ReadLine();

//                        ba = dal.findBankAccountByAccNo(accNo);

//                        if (ba is null)
//                        {
//                            Console.WriteLine("Account Does Not Exist");
//                        }
//                        else
//                        {
//                            double amountToWithdraw = -1;
//                            loopCount = 0;

//                            do
//                            {

//                                if (loopCount > 0)
//                                    Console.WriteLine("INVALID AMOUNT ENTERED - PLEASE TRY AGAIN");

//                                Console.WriteLine("Enter Amount To Withdraw (€" + ba.getAvailableFunds() + " Available): ");
//                                String amountToWithdrawString = Console.ReadLine();

//                                try
//                                {
//                                    amountToWithdraw = Convert.ToDouble(amountToWithdrawString);
//                                }

//                                catch
//                                {
//                                    loopCount++;
//                                }

//                            } while (amountToWithdraw < 0);

//                            bool withdrawalOK = dal.withdraw(accNo, amountToWithdraw);

//                            if(withdrawalOK == false)
//                            {

//                                Console.WriteLine("Insufficient Funds Available.");
//                            }
//                        }
//                        break;
//                    case "6":
//                        running = false;
//                        break;
//                    default:    
//                        Console.WriteLine("INVALID OPTION CHOSEN - PLEASE TRY AGAIN");
//                        break;
//                }


//            } while (running != false);

//        }

//    }
//}