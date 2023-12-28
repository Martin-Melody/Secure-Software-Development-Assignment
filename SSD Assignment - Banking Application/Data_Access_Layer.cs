using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;
using SSD_Assignment___Banking_Application.Interfaces;

namespace Banking_Application
{
    public class Data_Access_Layer
    {
        public static string databaseName = "Banking Database.sqlite";

        private CryptoManager cryptoManager = new CryptoManager();
        private CngKeyManager cngKeyManager = new CngKeyManager();

        private readonly IEncryptionService encryptionService;
        private static Data_Access_Layer instance;


        public Data_Access_Layer(IEncryptionService encryptionService) 
        {
            this.encryptionService = encryptionService;
            cngKeyManager.SetupCngProvider();
            EnsureDatabaseInitialized();
        }

        public static void Initialize(IEncryptionService encryptionService)
        {
            if (instance == null)
            {
                instance = new Data_Access_Layer(encryptionService);
            }
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
                        interestRate REAL,
                        creationDate TEXT NOT NULL
                    ) WITHOUT ROWID
                ";

                command.ExecuteNonQuery();

            }
        }

        public void loadBankAccounts()
        {

            // Replace this with check if database and tables exists method to remove one if else statement

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

                    while (dr.Read())
                    {

                        int accountType = dr.GetInt16(7);

                        if (accountType == Account_Type.Current_Account)
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
                            sa.InterestRate = dr.GetString(9);
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

            // Use a parameterized query to prevent SQL injection
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO Bank_Accounts 
              (accountNo, name, address_line_1, address_line_2, address_line_3, town, balance, accountType, overdraftAmount, interestRate, creationDate)
              VALUES 
              (@accountNo, @name, @address_line_1, @address_line_2, @address_line_3, @town, @balance, @accountType, @overdraftAmount, @interestRate, @creationDate)";

                command.Parameters.AddWithValue("@accountNo", !string.IsNullOrEmpty(encryptedAccountNumber) ? encryptedAccountNumber : DBNull.Value);
                command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(ba.Name) ? ba.Name : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_1", !string.IsNullOrEmpty(ba.AddressLine1) ? ba.AddressLine1 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_2", !string.IsNullOrEmpty(ba.AddressLine2) ? ba.AddressLine2 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_3", !string.IsNullOrEmpty(ba.AddressLine3) ? ba.AddressLine3 : DBNull.Value);
                command.Parameters.AddWithValue("@town", !string.IsNullOrEmpty(ba.Town) ? ba.Town : DBNull.Value);
                command.Parameters.AddWithValue("@balance", ba.Balance); 
                command.Parameters.AddWithValue("@accountType", currentAccount != null ? 1 : 2);
                command.Parameters.AddWithValue("@overdraftAmount", currentAccount != null && !string.IsNullOrEmpty(currentAccount.OverdraftAmount) ? (object)currentAccount.OverdraftAmount : DBNull.Value);
                command.Parameters.AddWithValue("@interestRate", savingsAccount != null && !string.IsNullOrEmpty(savingsAccount.InterestRate.ToString()) ? (object)savingsAccount.InterestRate : DBNull.Value);
                command.Parameters.AddWithValue("@creationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing SQL command: " + ex.Message);
                }
            }

            // Return the encrypted account number as confirmation
            return encryptedAccountNumber;
        }


        public Bank_Account findBankAccountByAccNo(String accNo)
        {

            try
            {
                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM Bank_Accounts WHERE accountNo = @accountNo";
                    command.Parameters.AddWithValue("@accountNo", accNo);

                    using (SqliteDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int accountType = dr.GetInt16(7);
                            if (accountType == Account_Type.Current_Account)
                            {
                                Current_Account ca = new Current_Account();
                                ca.AccountNo = dr.GetString(0);
                                ca.Name = encryptionService.Decrypt(dr.GetString(1));
                                ca.AddressLine1 = encryptionService.Decrypt(dr.GetString(2));
                                ca.AddressLine2 = encryptionService.Decrypt(dr.GetString(3));
                                ca.AddressLine3 = encryptionService.Decrypt(dr.GetString(4));
                                ca.Town = encryptionService.Decrypt(dr.GetString(5));
                                ca.Balance = encryptionService.Decrypt(dr.GetString(6));
                                ca.OverdraftAmount = encryptionService.Decrypt(dr.GetString(8));

                                return ca;
                            }
                            else
                            {
                                Savings_Account sa = new Savings_Account();
                                sa.AccountNo = dr.GetString(0);
                                sa.Name = encryptionService.Decrypt(dr.GetString(1));
                                sa.AddressLine1 = encryptionService.Decrypt(dr.GetString(2));
                                sa.AddressLine2 = encryptionService.Decrypt(dr.GetString(3));
                                sa.AddressLine3 = encryptionService.Decrypt(dr.GetString(4));
                                sa.Town = encryptionService.Decrypt(dr.GetString(5));
                                sa.Balance = encryptionService.Decrypt(dr.GetString(6));
                                sa.InterestRate = encryptionService.Decrypt(dr.GetString(8));
                                return sa;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error finding account with account number of : {accNo}\n {ex.Message}");
            }

            return null;
        }

        public bool closeBankAccount(String accNo)
        {
            Bank_Account toRemove = findBankAccountByAccNo(accNo);

            if (toRemove == null)
            {
                Console.WriteLine("Account not found.");
                return false; // Account not found
            }

            try
            {
                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Bank_Accounts WHERE accountNo = @accountNo";
                    command.Parameters.AddWithValue("@accountNo", toRemove.AccountNo);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No account was deleted.");
                        return false; // No rows affected, account not deleted
                    }

                    return true; // Account successfully deleted
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing account: {ex.Message}");
                return false; // Error occurred
            }
        }

        public bool lodge(String accNo, double amountToLodge)
        {
            var toLodgeTo = findBankAccountByAccNo(accNo);

            if (toLodgeTo == null)
            {
                return false;
            }
            else
            {
                toLodgeTo.Lodge(amountToLodge);

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";
                    command.Parameters.AddWithValue("@balance", toLodgeTo.Balance);
                    command.Parameters.AddWithValue("@accountNo", toLodgeTo.AccountNo);
                    command.ExecuteNonQuery();
                }

                return true;
            }
        }


        public bool withdraw(String accNo, double amountToWithdraw)
        {
            var toWithdrawFrom = findBankAccountByAccNo(accNo);

            if (toWithdrawFrom == null)
            {
                return false;
            }
            else
            {
                bool result = toWithdrawFrom.Withdraw(amountToWithdraw);

                if (!result)
                {
                    return false;
                }

                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Bank_Accounts SET balance = @balance WHERE accountNo = @accountNo";
                    command.Parameters.AddWithValue("@balance", toWithdrawFrom.Balance);
                    command.Parameters.AddWithValue("@accountNo", toWithdrawFrom.AccountNo);
                    command.ExecuteNonQuery();
                }

                return true;
            }
        }


        private void EnsureDatabaseInitialized()
        {
            bool shouldInitialize = false;

            if (!File.Exists(Data_Access_Layer.databaseName))
            {
                shouldInitialize = true;
            }
            else
            {
                using (var connection = getDatabaseConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Bank_Accounts';";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            shouldInitialize = true;
                        }
                    }
                }
            }

            if (shouldInitialize)
            {
                initialiseDatabase();
            }
        }



    }
}
