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

    }

}