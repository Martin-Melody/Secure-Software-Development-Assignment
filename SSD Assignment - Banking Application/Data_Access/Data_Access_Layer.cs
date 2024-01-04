using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SSD_Assignment___Banking_Application;
using SSD_Assignment___Banking_Application.Account_Types;
using SSD_Assignment___Banking_Application.Encryption;
using SSD_Assignment___Banking_Application.Interfaces;
using SSD_Assignment___Banking_Application.Services;

namespace SSD_Assignment___Banking_Application.Data_Access
{
    public class Data_Access_Layer
    {
        public static string databaseName = "Banking Database.sqlite";
        private EventLogService _eventLogService;

        private readonly CryptoManager _cryptoManager; 

        private readonly IEncryptionService encryptionService;
        private static Data_Access_Layer instance;

        // Adjusted constructor
        public Data_Access_Layer(IEncryptionService encryptionService, EventLogService eventLogService, CryptoManager cryptoManager)
        {
            this.encryptionService = encryptionService;
            this._eventLogService = eventLogService;
            this._cryptoManager = cryptoManager; 
            EnsureDatabaseInitialized();
        }

        public static void Initialize(IEncryptionService encryptionService, EventLogService eventLogService, CryptoManager cryptoManager)
        {
            if (instance == null)
            {
                instance = new Data_Access_Layer(encryptionService, eventLogService, cryptoManager);
            }
        }

        public static Data_Access_Layer getInstance()
        {
            return instance;
        }

        private SqliteConnection getDatabaseConnection()
        {

            string databaseConnectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = databaseName,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            // if statement here 

            _eventLogService.WriteToEventLog("Successfully connected to database", EventLogEntryType.Information, 1003);

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
                        balance TEXT NOT NULL,
                        accountType INTEGER NOT NULL,
                        overdraftAmount TEXT,
                        interestRate TEXT,
                        creationDate TEXT NOT NULL,
                        hash Text NOT NULL
                    ) WITHOUT ROWID
                ";

                try
                {
                    command.ExecuteNonQuery();
                    _eventLogService.WriteToEventLog("Database Successfully Initialized", EventLogEntryType.Information, 1002);

                }
                catch
                {
                    _eventLogService.WriteToEventLog("Error creating database", EventLogEntryType.Error, 1015);
                }

            }
        }

        public string AddBankAccount(Bank_Account ba)
        {
            // Perform type-specific logic
            Current_Account currentAccount = ba as Current_Account;
            Savings_Account savingsAccount = ba as Savings_Account;

            int accountType = ba switch
            {
                Current_Account _ => 1,
                Savings_Account _ => 2,
                _ => 0,
            };

            // Encrypt the account number
            string encryptedAccountNumber = encryptionService.Encrypt(ba.AccountNo);

            // Store the encrypted account number
            ba.AccountNo = encryptedAccountNumber;

            // Generate a Has for the Account info 
            string accountHash = encryptionService.CalculateRowHash(ba);

            // Use a parameterized query to prevent SQL injection
            using (var connection = getDatabaseConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                                    @"INSERT INTO Bank_Accounts 
                                      (accountNo, name, address_line_1, address_line_2, address_line_3, town, balance, accountType, overdraftAmount, interestRate, creationDate, hash)
                                      VALUES 
                                      (@accountNo, @name, @address_line_1, @address_line_2, @address_line_3, @town, @balance, @accountType, @overdraftAmount, @interestRate, @creationDate, @hash)";

                command.Parameters.AddWithValue("@accountNo", !string.IsNullOrEmpty(encryptedAccountNumber) ? encryptedAccountNumber : DBNull.Value);
                command.Parameters.AddWithValue("@name", !string.IsNullOrEmpty(ba.Name) ? ba.Name : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_1", !string.IsNullOrEmpty(ba.AddressLine1) ? ba.AddressLine1 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_2", !string.IsNullOrEmpty(ba.AddressLine2) ? ba.AddressLine2 : DBNull.Value);
                command.Parameters.AddWithValue("@address_line_3", !string.IsNullOrEmpty(ba.AddressLine3) ? ba.AddressLine3 : DBNull.Value);
                command.Parameters.AddWithValue("@town", !string.IsNullOrEmpty(ba.Town) ? ba.Town : DBNull.Value);
                command.Parameters.AddWithValue("@balance", ba.Balance);
                command.Parameters.AddWithValue("@accountType", accountType);
                command.Parameters.AddWithValue("@overdraftAmount", ba is Current_Account ca && !string.IsNullOrEmpty(ca.OverdraftAmount) ? ca.OverdraftAmount : DBNull.Value);
                command.Parameters.AddWithValue("@interestRate", ba is Savings_Account sa && !string.IsNullOrEmpty(sa.InterestRate.ToString()) ? sa.InterestRate : DBNull.Value);
                command.Parameters.AddWithValue("@creationDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@hash", accountHash);

                try
                {
                    command.ExecuteNonQuery();
                    _eventLogService.WriteToEventLog("Account information added to database", EventLogEntryType.Information, 1004);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing SQL command: " + ex.Message);
                    _eventLogService.WriteToEventLog("Error adding account information to database", EventLogEntryType.Error, 1005);
                }
            }

            // Return the encrypted account number as confirmation
            return encryptedAccountNumber;
        }


        public Bank_Account findBankAccountByAccNo(string accNo)
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
                            Bank_Account account = accountType == Account_Type.Current_Account ? new Current_Account() : new Savings_Account();

                            // Set properties with encrypted values
                            SetAccountPropertiesFromReader(account, dr);

                            // Perform hash integrity check with encrypted values
                            string storedHash = dr.GetString(11);
                            string calculatedHash = encryptionService.CalculateRowHash(account);

                            if (calculatedHash != storedHash)
                            {
                                _eventLogService.WriteToEventLog("Data integrity check failed: Hash mismatch", EventLogEntryType.Error, 1006);
                                throw new Exception("Data integrity check failed: Hash mismatch.");
                            }

                            // Decrypt properties as needed after hash check
                            account.Name = encryptionService.Decrypt(account.Name);
                            account.AddressLine1 = encryptionService.Decrypt(account.AddressLine1);
                            account.AddressLine2 = encryptionService.Decrypt(account.AddressLine2);
                            account.AddressLine3 = encryptionService.Decrypt(account.AddressLine3);
                            account.Town = encryptionService.Decrypt(account.Town);
                            account.Balance = encryptionService.Decrypt(account.Balance);


                            // Set type-specific properties (decrypt if necessary)
                            if (account is Current_Account ca)
                            {
                                ca.OverdraftAmount = dr.IsDBNull(8) ? null : encryptionService.Decrypt(dr.GetString(8));
                            }
                            else if (account is Savings_Account sa)
                            {
                                sa.InterestRate = dr.IsDBNull(9) ? null : encryptionService.Decrypt(dr.GetString(9));
                            }

                            return account;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _eventLogService.WriteToEventLog($"Error verifying account integrity: {ex.Message}", EventLogEntryType.Error, 1007);
                Console.WriteLine($"Error verifying account integrity: {ex.Message}");
                return null;
            }
            _eventLogService.WriteToEventLog("Error establishing connection with the database.", EventLogEntryType.Error, 1008);
            Console.WriteLine("Error establishing connection with the database.");
            return null;
        }


        private void SetAccountPropertiesFromReader(Bank_Account account, SqliteDataReader dr)
        {
            account.AccountNo = dr.GetString(0);
            account.Name = dr.GetString(1);
            account.AddressLine1 = dr.GetString(2);
            account.AddressLine2 = dr.GetString(3);
            account.AddressLine3 = dr.GetString(4);
            account.Town = dr.GetString(5);
            account.Balance = dr.GetString(6);

            if (account is Current_Account ca)
            {
                ca.OverdraftAmount = dr.GetString(8);
            }
            else if (account is Savings_Account sa)
            {
                sa.InterestRate = dr.GetString(9);
            }
        }



        public bool closeBankAccount(string accNo)
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
                        return false; 
                    }

                    _eventLogService.WriteToEventLog("Account Successfully Delete", EventLogEntryType.Information, 1009);
                    return true; 
                }
            }
            catch (Exception ex)
            {
                _eventLogService.WriteToEventLog("Error closing account", EventLogEntryType.Error, 1010);
                Console.WriteLine($"Error closing account: {ex.Message}");
                return false; 
            }
        }

        public bool Lodge(string accNo, double amountToLodge)
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
                    
                    
                    try 
                    { 
                        command.ExecuteNonQuery();
                        _eventLogService.WriteToEventLog("Lodgment to account successful", EventLogEntryType.Information, 1011);
                    } 
                    catch 
                    {
                        _eventLogService.WriteToEventLog("Error adding lodgment to account", EventLogEntryType.Error, 1012);
                        Console.WriteLine("There was an error adding the lodgment to your account");
                    }

                }

                return true;
            }
        }


        public bool Withdraw(string accNo, double amountToWithdraw)
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

                    try
                    {
                        command.ExecuteNonQuery();
                        _eventLogService.WriteToEventLog("Withdrawal from account successful", EventLogEntryType.Information, 1013);
                    }
                    catch
                    {
                        _eventLogService.WriteToEventLog("Error withdrawing from account", EventLogEntryType.Error, 1014);
                        Console.WriteLine("There was an error withdrawing from your account");
                    }
                }

                return true;
            }
        }


        private void EnsureDatabaseInitialized()
        {
            bool shouldInitialize = false;

            if (!File.Exists(databaseName))
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
