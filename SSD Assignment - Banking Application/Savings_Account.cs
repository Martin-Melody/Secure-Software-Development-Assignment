using SSD_Assignment___Banking_Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public class Savings_Account : Bank_Account
    {

        private double _interestRate;

        public double InterestRate
        {
            get { return _interestRate; }
            set { _interestRate = value; }
        }

        public Savings_Account(String name, String address_line_1, String address_line_2, String address_line_3, String town, string balance, string interestRate) : base(name, address_line_1, address_line_2, address_line_3, town, balance)
        {

            if (double.TryParse(balance, out double result))
            {
                this.InterestRate = result;
            }
            else
            {
                throw new ArgumentException("Cannot convert interest rate to double");
            }

        }

        public Savings_Account() : base()
        {

        }
       
        public override double GetAvailableFunds()
        {

            double.TryParse(Balance, out double balance);
            return balance;
        }

        public override bool Withdraw(double amountToWithdraw)
        {
            //double avFunds = GetBalance();

            //if (avFunds >= amountToWithdraw)
            //{
            //    UpdateBalance(Balance - amountToWithdraw); // Use the protected method
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

        public override String ToString()
        {

            return base.ToString() +
                "Account Type: Savings Account\n" +
                "Interest Rate: " + InterestRate + "\n";

        }


    }
}
