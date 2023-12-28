using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application.Services
{
    public class ValidationService
    {

        // Maybe move these to bank_account
        private const double MinOverdraftAmount = 0.0;
        private const double MaxOverdraftAmount = 10000.0;

        private const double MinInterestRate = 0.0;
        private const double MaxInterestRate = 10.0;

        public string GetValidInput(string prompt)
        {

            string input;
            do
            {
                Console.Write(prompt + " ");
                input = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public double GetValidDouble(string prompt)
        {
            double value;
            string input;
            do
            {
                Console.Write(prompt + " ");
                input = Console.ReadLine();
            }
            while (!double.TryParse(input, out value));

            return value;
        }
        public bool IsValidAccountType(string input)
        {
            // Example validation logic for account type
            return input == "1" || input == "2";
        }

        public bool IsValidWithdrawalAmount(double amount)
        {
            return amount > 0;
        }

        public bool IsValueDouble(string input, out double value)
        {
            return double.TryParse(input, out value);
        }


        public bool IsValidMenuOption(string input)
        {
            // Assuming your menu options are numbers from 1 to 6
            return int.TryParse(input, out int option) && option >= 1 && option <= 6;
        }

        public bool IsValidName(string name)
        {
            // Check if the name is not null or empty
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            // Check if the name contains only letters and spaces
            if (!name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                return false;
            }

            // Check for maximum length (50 characters)
            if (name.Length > 50)
            {
                return false;
            }

            return true;
        }

        public bool IsValidAddress(string address)
        {
            // Check if the address is not null or empty
            if (string.IsNullOrWhiteSpace(address))
            {
                return false;
            }

            // Check if the address contains only allowed characters
            // Allowing letters, digits, spaces, and some special characters
            if (!address.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '.'))
            {
                return false;
            }

            // Check for maximum length (100 characters)
            if (address.Length > 100)
            {
                return false;
            }

            return true;
        }

        public bool IsValidOverdraftAmount(double overdraftAmount)
        {
            // Check if the overdraft amount is within the acceptable range
            return overdraftAmount >= MinOverdraftAmount && overdraftAmount <= MaxOverdraftAmount;
        }

        public bool IsValidInterestRate(double interestRate)
        {

            // Check if the interest rate is within the acceptable range
            return interestRate >= MinInterestRate && interestRate <= MaxInterestRate;
        }


    }

}