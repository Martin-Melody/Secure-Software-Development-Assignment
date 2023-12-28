using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public class Savings_Account: Bank_Account
    {

        public String interestRate;

       public Savings_Account(): base()
        {

        }

        public Savings_Account(string[] variables)
        {

        }
        
        public Savings_Account(String name, String address_line_1, String address_line_2, String address_line_3, String town, String balance, String interestRate) : base(name, address_line_1, address_line_2, address_line_3, town, balance)
        {
            this.interestRate = interestRate;
        }
        public override double getAvailableFunds()
        {
            double.TryParse(base.balance, out double result);
            return result;
        }

        public override bool withdraw(double amountToWithdraw)
        {
            double avFunds = getAvailableFunds();
            double.TryParse(this.balance, out double _balance);

            if (avFunds >= amountToWithdraw)
            {
                _balance -= amountToWithdraw;
                return true;
            }

            else
                return false;
        }

        public override String ToString()
        {

            return base.ToString() + 
                "Account Type: Savings Account\n" +
                "Interest Rate: " + interestRate + "\n";

        }


    }
}
