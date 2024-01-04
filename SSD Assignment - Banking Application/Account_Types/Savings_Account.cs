using SSD_Assignment___Banking_Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application.Account_Types
{
    public class Savings_Account : Bank_Account
    {

        private string _interestRate;

        public string InterestRate
        {
            get { return _interestRate; }
            set { _interestRate = value; }
        }

        public Savings_Account(string name, string address_line_1, string address_line_2, string address_line_3, string town, string balance, string interestRate) : base(name, address_line_1, address_line_2, address_line_3, town, balance)
        {

            InterestRate = interestRate;

        }

        public Savings_Account() : base()
        {

        }

        public override double GetAvailableFunds()
        {

            double.TryParse(Balance, out double balance);
            return balance;
        }

       

        public override string ToString()
        {

            return base.ToString() +
                "Account Type: Savings Account\n" +
                "Interest Rate: " + InterestRate + "%\n";

        }


    }
}
