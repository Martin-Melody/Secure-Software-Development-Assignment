using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;

namespace Banking_Application
{
    public class Data_Access_Layer
    {
        private List<Bank_Account> accounts;
        public static string databaseName = "Banking Database.sqlite";
        private static Data_Access_Layer instance = new Data_Access_Layer();

        private CryptoManager cryptoManager = new CryptoManager();
        private CngKeyManager cngKeyManager = new CngKeyManager();

        public Data_Access_Layer() // Singleton Design Pattern
        {
            // Initialize your crypto manager and cng manager here
            cngKeyManager.SetupCngProvider();
            accounts = new List<Bank_Account>();
        }

        public static Data_Access_Layer getInstance()
        {
            return instance;
        }

        private SqliteConnection getDatabaseConnection()
        {

            String databaseConnectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = Data_Access_Layer.databaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            return new SqliteConnection(databaseConnectionString);

        }

        private void initialiseDatabase()
        {
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    CREATE TABLE IF NOT EXISTS Bank_Accounts(    
                        accountNo TEXT PRIMARY KEY,
                        name TEXT NOT NULL,
                        address_line_1 TEXT,
                        address_line_2 TEXT,
                        address_line_3 TEXT,
                        town TEXT NOT NULL,
                        balance REAL NOT NULL,
                        accountType INTEGER NOT NULL,
                        overdraftAmount REAL,
                        interestRate REAL
                    ) WITHOUT ROWID
                ";

                command.ExecuteNonQuery();
                
            }
        }

        public void loadBankAccounts()
        {
            if (!File.Exists(Data_Access_Layer.databaseName))
                initialiseDatabase();
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Bank_Accounts";
                    SqliteDataReader dr = command.ExecuteReader();
                    
                    while(dr.Read())
                    {

                        int accountType = dr.GetInt16(7);

                        if(accountType == Account_Type.Current_Account)
                        {
                            Current_Account ca = new Current_Account();
                            ca.AccountNo = dr.GetString(0);
                            ca.Name = dr.GetString(1);
                            ca.AddressLine1 = dr.GetString(2);
                            ca.AddressLine2 = dr.GetString(3);
                            ca.AddressLine3 = dr.GetString(4);
                            ca.Town = dr.GetString(5);
                            ca.Balance = dr.GetString(6);
                            ca.OverdraftAmount = dr.GetString(8);
                            accounts.Add(ca);
                        }
                        else
                        {
                            Savings_Account sa = new Savings_Account();
                            sa.AccountNo = dr.GetString(0);
                            sa.Name = dr.GetString(1);
                            sa.AddressLine1 = dr.GetString(2);
                            sa.AddressLine2 = dr.GetString(3);
                            sa.AddressLine3 = dr.GetString(4);
                            sa.Town = dr.GetString(5);
                            sa.Balance = dr.GetString(6);
                            sa.InterestRate = dr.GetDouble(9);
                            accounts.Add(sa);
                        }


                    }

                }

            }
        }

        public string AddBankAccount(Bank_Account ba)
        {
            // Perform type-specific logic
            Current_Account currentAccount = ba as Current_Account;
            Savings_Account savingsAccount = ba as Savings_Account;

            // Encrypt the account number
            byte[] encryptedData = cryptoManager.EncryptText(ba.AccountNo);
            string encryptedAccountNumber = Convert.ToBase64String(encryptedData);

            // Store the encrypted account number
            ba.AccountNo = encryptedAccountNumber;
            accounts.Add(ba);

            // Use a parameterized query to prevent SQL injection
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO Bank_Accounts 
              (accountNo, name, address_line_1, address_line_2, address_line_3, town, balance, accountType, overdraftAmount, interestRate)
              VALUES 
              (@accountNo, @name, @address_line_1, @address_line_2, @address_line_3, @town, @balance, @accountType, @overdraftAmount, @interestRate)";

                command.Parameters.AddWithValue("@accountNo", !string.IsNullOrEmpty(encryptedAccountNumber) ? encryptedAccountNumber : DBNull.Value);
                command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(ba.Name) ? ba.Name : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_1", !string.IsNullOrEmpty(ba.AddressLine1) ? ba.AddressLine1 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_2", !string.IsNullOrEmpty(ba.AddressLine2) ? ba.AddressLine2 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_3", !string.IsNullOrEmpty(ba.AddressLine3) ? ba.AddressLine3 : DBNull.Value);
                command.Parameters.AddWithValue("@town", !string.IsNullOrEmpty(ba.Town) ? ba.Town : DBNull.Value);
                command.Parameters.AddWithValue("@balance", ba.Balance); // Assuming balance is a non-nullable double
                command.Parameters.AddWithValue("@accountType", currentAccount != null ? 1 : 2);
                command.Parameters.AddWithValue("@overdraftAmount", currentAccount != null && !string.IsNullOrEmpty(currentAccount.OverdraftAmount) ? (object)currentAccount.OverdraftAmount : DBNull.Value);
                command.Parameters.AddWithValue("@interestRate", savingsAccount != null && !string.IsNullOrEmpty(savingsAccount.InterestRate.ToString()) ? (object)savingsAccount.InterestRate : DBNull.Value);


                Console.WriteLine("================================================================");
                Console.WriteLine("Account No : ", ba.AccountNo);
                Console.WriteLine("Name : ", ba.Name);
                Console.WriteLine("Address line 1 : ", ba.AddressLine1);
                Console.WriteLine("Address line 2 : ", ba.AddressLine2);
                Console.WriteLine("Address line 3 : ", ba.AddressLine3);
                Console.WriteLine("Town : ", ba.Town);
                Console.WriteLine("Balance", ba.Balance);
                Console.WriteLine("AccountType : ", currentAccount != null? 1: 2);
                Console.WriteLine("Over Draft Amount : ", currentAccount != null ? (object)currentAccount.OverdraftAmount : DBNull.Value);
                Console.WriteLine("Interest Rate : ", savingsAccount != null ? (object)savingsAccount.InterestRate : DBNull.Value);
                Console.WriteLine("================================================================");


                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing SQL command: " + ex.Message);
                    // Optionally, log the full exception details
                    // This can help identify which parameter is causing the issue
                }
            }

            // Return the encrypted account number as confirmation
            return encryptedAccountNumber;
        }


        public Bank_Account findBankAccountByAccNo(String accNo) 
        { 
        
            foreach(Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    return ba;
                }

            }

            return null; 
        }

        public bool closeBankAccount(String accNo) 
        {

            Bank_Account toRemove = null;
            
            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    toRemove = ba;
                    break;
                }

            }

            if (toRemove == null)
                return false;
            else
            {
                accounts.Remove(toRemove);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Bank_Accounts WHERE accountNo = '" + toRemove.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                return true;
            }

        }

        public bool lodge(String accNo, double amountToLodge)
        {

            Bank_Account toLodgeTo = null;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    ba.Lodge(amountToLodge);
                    toLodgeTo = ba;
                    break;
                }

            }

            if (toLodgeTo == null)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = " + toLodgeTo.Balance + " WHERE accountNo = '" + toLodgeTo.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                return true;
            }

        }

        public bool withdraw(String accNo, double amountToWithdraw)
        {

            Bank_Account toWithdrawFrom = null;
            bool result = false;

            foreach (Bank_Account ba in accounts)
            {

                if (ba.AccountNo.Equals(accNo))
                {
                    result = ba.Withdraw(amountToWithdraw);
                    toWithdrawFrom = ba;
                    break;
                }

            }

            if (toWithdrawFrom == null || result == false)
                return false;
            else
            {

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = " + toWithdrawFrom.Balance + " WHERE accountNo = '" + toWithdrawFrom.AccountNo + "'";
                    command.ExecuteNonQuery();

                }

                return true;
            }

        }

    }
}
