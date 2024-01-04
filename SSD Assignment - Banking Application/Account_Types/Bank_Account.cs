using SSD_Assignment___Banking_Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD_Assignment___Banking_Application.Account_Types
{
    public abstract class Bank_Account
    {

        private string _accountNo;
        private string _name;
        private string _balance;
        private string _address_line_1;
        private string _address_line_2;
        private string _address_line_3;
        private string _town;



        public Bank_Account()
        {
        }

        public Bank_Account(string name, string address_line_1, string address_line_2, string address_line_3, string town, string balance)
        {

            AccountNo = Guid.NewGuid().ToString();
            Name = name;
            AddressLine1 = address_line_1;
            AddressLine2 = address_line_2;
            AddressLine3 = address_line_3;
            Town = town;
            Balance = balance;

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

            double.TryParse(Balance, out double _balance);
            _balance += amountIn;

            Balance = _balance.ToString() ;
        }

        public void Withdraw(double amountToWithdraw)
        {
            double.TryParse(Balance, out double _balance);
            if (amountToWithdraw <= 0 )
            {
                throw new ArgumentException("Amount to Withdraw must be more than 0 ");
            }
            else if (amountToWithdraw > _balance)
            {
                throw new ArgumentException("Amount to Withdraw must be less that your balance");
            }

            _balance -= amountToWithdraw;

            Balance = _balance.ToString() ;
        }

        public abstract double GetAvailableFunds();

        public string GetBalance()
        {
            return Balance;
        }

        protected void UpdateBalance(string newBalance)
        {
            _balance = newBalance;
        }

        public override string ToString()
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
