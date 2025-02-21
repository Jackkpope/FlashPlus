﻿using flashplus.Models;
using Microsoft.AspNetCore.Identity;
using System.Data.OleDb;

namespace flashplus.Data
{
    public interface IEntryDataAccess
    {
        Task<UserModel> GetUserDetailsAsync(EntryModel loginModel);
        Task<bool> AddUserDetailsAsync(EntryModel registerModel);
        Task<bool> CheckUsernameTakenAsync(string Username);

    }

    public class EntryDataAccess : IEntryDataAccess
    {

        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Jackp\Documents\flashplus.accdb;Persist Security Info=False;";

        public async Task<UserModel> GetUserDetailsAsync(EntryModel loginModel)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT ID,Username FROM UserDetails WHERE Username=? AND PasswordHash=?;";

                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("Username", loginModel.Username);
                command.Parameters.AddWithValue("PasswordHash", loginModel.PasswordHash);
                OleDbDataReader reader = command.ExecuteReader();

                UserModel userDetails = new UserModel();

                while (reader.Read())
                {
                    userDetails.ID = (int)reader["ID"];
                    userDetails.Username = (string)reader["Username"];
                }

                return userDetails;
            }

        }

        public async Task<bool> AddUserDetailsAsync(EntryModel registerModel)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"INSERT INTO UserDetails (Username,PasswordHash) VALUES (@Username,@PasswordHash);";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("@Username", registerModel.Username);
                command.Parameters.AddWithValue("@PasswordHash", registerModel.PasswordHash);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
        }

        public async Task<bool> CheckUsernameTakenAsync(string Username)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT Username FROM UserDetails WHERE Username='" + Username + "';";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                OleDbDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    return false;
                }

                else
                {
                    return true;
                }
            }
        }
    }
}
