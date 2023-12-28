using SSD_Assignment___Banking_Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking_Application
{
    public abstract class Bank_Account
    {

        private String _accountNo;
        private String _name;
        private String _balance;
        private String _address_line_1;
        private String _address_line_2;
        private String _address_line_3;
        private String _town;



        public Bank_Account()
        {
        }

        public Bank_Account(String name, String address_line_1, String address_line_2, String address_line_3, String town, String balance)
        {

            this.AccountNo = System.Guid.NewGuid().ToString();
            this.Name = name;
            this.AddressLine1 = address_line_1;
            this.AddressLine2 = address_line_2;
            this.AddressLine3 = address_line_3;
            this.Town = town;
            this.Balance = balance;

        }

        public string AccountNo
        {
            get { return _accountNo; }
            internal set { _accountNo = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Balance
        {
            get { return _balance; }
            internal set
            {
                _balance = value;
            }
        }

        public string AddressLine1
        {
            get { return _address_line_1; }
            set { _address_line_1 = value; }
        }

        public string AddressLine2
        {
            get { return _address_line_2; }
            set { _address_line_2 = value; }
        }

        public string AddressLine3
        {
            get { return _address_line_3; }
            set { _address_line_3 = value; }
        }

        public string Town
        {
            get { return _town; }
            set { _town = value; }
        }

        public void Lodge(double amountIn)
        {
            if (amountIn < 0)
            {
                throw new ArgumentException("Amount to lodge must be positive");
            }
            Balance += amountIn;
        }

        public abstract bool Withdraw(double amountToWithdraw);

        public abstract double GetAvailableFunds();

        public String GetBalance()
        {
            return Balance;
        }

        protected void UpdateBalance(string newBalance)
        {
            _balance = newBalance;
        }

        public override String ToString()
        {

            return "\nAccount No: " + AccountNo + "\n" +
            "Name: " + Name + "\n" +
            "Address Line 1: " + AddressLine1 + "\n" +
            "Address Line 2: " + AddressLine2 + "\n" +
            "Address Line 3: " + AddressLine3 + "\n" +
            "Town: " + Town + "\n" +
            "Balance: " + Balance + "\n";

        }

    }
}
