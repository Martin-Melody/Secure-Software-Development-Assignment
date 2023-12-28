using Banking_Application;
using SSD_Assignment___Banking_Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application.Services
{
    public class BankAccountService
    {
        private readonly ValidationService _validationService;
        private readonly Data_Access_Layer _dataAccessLayer;
        private readonly IEncryptionService _encryptionService;

        public BankAccountService(ValidationService validationService, Data_Access_Layer dataAccessLayer, IEncryptionService encryptionService)
        {
            _validationService = validationService;
            _dataAccessLayer = dataAccessLayer;
            _encryptionService = encryptionService;
        }

        public void AddBankAccount()
        {
            Console.WriteLine("***Add Bank Account***");

            string accountType = GetValidAccountType();
            string name = _validationService.GetValidInput("Enter Name:");
            string encryptedName = _encryptionService.Encrypt(name);

            string addressLine1 = _validationService.GetValidInput("Enter Address Line 1:");
            string encryptedaddressLine1 = _encryptionService.Encrypt(addressLine1);

            string addressLine2 = _validationService.GetValidInput("Enter Address Line 2:");
            string encryptedaddressLine2 = _encryptionService.Encrypt(addressLine2);

            string addressLine3 = _validationService.GetValidInput("Enter Address Line 3:");
            string encryptedaddressLine3 = _encryptionService.Encrypt(addressLine3);

            string town = _validationService.GetValidInput("Enter Town:");
            string encryptedtown = _encryptionService.Encrypt(town);

            double balance = _validationService.GetValidDouble("Enter Opening Balance:");
            string encryptedBalance = _encryptionService.Encrypt(balance.ToString());

            Bank_Account bankAccount;
            if (accountType == "1") // Current Account
            {
                double overdraftAmount = _validationService.GetValidDouble("Enter Overdraft Amount:");
                string encryptedOverdraftAmount = _encryptionService.Encrypt(overdraftAmount.ToString());
                
                bankAccount = new Current_Account(encryptedName, encryptedaddressLine1, encryptedaddressLine2, encryptedaddressLine3, encryptedtown, encryptedBalance, encryptedOverdraftAmount);
            }
            else // Savings Account
            {
                double interestRate = _validationService.GetValidDouble("Enter Interest Rate:");
                string encryptedInterestRate = _encryptionService.Encrypt(interestRate.ToString());
                
                bankAccount = new Savings_Account(encryptedName, encryptedaddressLine1, encryptedaddressLine2, encryptedaddressLine3, encryptedtown, encryptedBalance, encryptedInterestRate);
            }

            string accNo = _dataAccessLayer.AddBankAccount(bankAccount);
            Console.WriteLine("New Account Number Is: " + accNo);
        }

        private string GetValidAccountType()
        {
            string accountType;
            do
            {
                Console.WriteLine("\n***Account Types***:");
                Console.WriteLine("1. Current Account.");
                Console.WriteLine("2. Savings Account.");
                Console.Write("Choose Option: ");
                accountType = Console.ReadLine();
            }
            while (!_validationService.IsValidAccountType(accountType));

            return accountType;
        }



        public void CloseBankAccount()
        {
            Console.WriteLine("Enter Account Number:");
            string accNo = Console.ReadLine();

            var bankAccount = _dataAccessLayer.findBankAccountByAccNo(accNo);

            if (bankAccount == null)
            {
                Console.WriteLine("Account Does Not Exist");
                return;
            }

            Console.WriteLine(bankAccount.ToString());
            if (ConfirmAction("Proceed With Deletion (Y/N)?"))
            {
                _dataAccessLayer.closeBankAccount(accNo);
                Console.WriteLine("Account closed successfully.");
            }
        }

        private bool ConfirmAction(string prompt)
        {
            string answer;
            do
            {
                Console.WriteLine(prompt);
                answer = Console.ReadLine().ToLower();
            }
            while (answer != "y" && answer != "n");

            return answer == "y";
        }

        public void ViewAccountInformation()
        {
            Console.WriteLine("Enter Account Number:");
            string accNo = Console.ReadLine();

            var bankAccount = _dataAccessLayer.findBankAccountByAccNo(accNo);

            if (bankAccount == null)
            {
                Console.WriteLine("Account Does Not Exist");
            }
            else
            {
                Console.WriteLine("Account Details:");
                Console.WriteLine(bankAccount.ToString());
            }
        }

        public void MakeLodgement()
        {
            Console.WriteLine("Enter Account Number:");
            string accNo = Console.ReadLine();

            var bankAccount = _dataAccessLayer.findBankAccountByAccNo(accNo);

            if (bankAccount == null)
            {
                Console.WriteLine("Account Does Not Exist");
                return;
            }

            Console.WriteLine("Enter Amount to Lodge:");
            if (!double.TryParse(Console.ReadLine(), out double amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount entered.");
                return;
            }

            bool lodgementSuccessful = _dataAccessLayer.lodge(accNo, amount);
            if (lodgementSuccessful)
            {
                Console.WriteLine($"Amount {amount} lodged successfully to account {accNo}.");
            }
            else
            {
                Console.WriteLine("Lodgment failed.");
            }
        }

        public void MakeWithdrawal()
        {
            Console.WriteLine("Enter Account Number:");
            string accNo = Console.ReadLine();

            var bankAccount = _dataAccessLayer.findBankAccountByAccNo(accNo);

            if (bankAccount == null)
            {
                Console.WriteLine("Account Does Not Exist");
                return;
            }

            Console.WriteLine("Enter Amount to Withdraw:");
            string input = Console.ReadLine();

            if (!_validationService.IsValueDouble(input, out double amount))
            {
                Console.WriteLine("You must enter a valid number for the amount.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Withdrawal amount must be greater than zero.");
                return;
            }

            if (bankAccount.GetAvailableFunds() < amount)
            {
                Console.WriteLine("Insufficient funds available for withdrawal.");
                return;
            }

            bool withdrawalSuccessful = _dataAccessLayer.withdraw(accNo, amount);
            if (withdrawalSuccessful)
            {
                Console.WriteLine($"Amount {amount} withdrawn successfully from account {accNo}.");
            }
            else
            {
                Console.WriteLine("Withdrawal failed.");
            }
        }

    }
}
