using SSD_Assignment___Banking_Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public class Current_Account : Bank_Account
    {

        private String _overdraftAmount;


        public String OverdraftAmount
        {
            get { return _overdraftAmount; }
            set { _overdraftAmount = value; }
        }

        public Current_Account() : base()
        {

        }

        public Current_Account(String name, String address_line_1, String address_line_2, String address_line_3, String town, String balance, String overdraftAmount) : base(name, address_line_1, address_line_2, address_line_3, town, balance)
        {

            this.OverdraftAmount = OverdraftAmount;
        }

        public override bool Withdraw(double amountToWithdraw)
        {
            //string avFunds = GetBalance();

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

        public override double GetAvailableFunds()
        {

            double.TryParse(OverdraftAmount, out double overdraftAmount);
            double.TryParse(Balance, out double balance);


            return balance + overdraftAmount;
        }

        public override String ToString()
        {

            return base.ToString() +
                "Account Type: Current Account\n" +
                "Overdraft Amount: " + OverdraftAmount + "\n";

        }

    }
}
