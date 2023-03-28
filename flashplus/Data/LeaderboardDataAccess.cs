using flashplus.Models;
using System.Data.OleDb;

namespace flashplus.Data
{
    public interface ILeaderboardDataAccess
    {
        Task<int> GetHighscoreAsync(int ID, int SetID);
        Task<bool> SetRecentScore(int ID, int SetID, int recentScore);
        Task<bool> SetHighScore(int ID, int SetID, int highScore);
        Task<bool> CheckSetLeaderboard(int ID, int SetID);
        Task<bool> SetNewEntry(int ID, int SetID, int recentScore);
    }

    public class LeaderboardDataAccess : ILeaderboardDataAccess
    {
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Jackp\Documents\flashplus.accdb;Persist Security Info=False;";

        public async Task<int> GetHighscoreAsync(int ID, int SetID)
        {
            int Highscore = 0;

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT HighScore FROM Leaderboard WHERE SetID=? AND ID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                command.Parameters.AddWithValue("ID", ID);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Highscore = (int)reader["HighScore"];
                }
                return Highscore;
            }
        }

        public async Task<bool> SetRecentScore(int ID, int SetID, int recentScore)
        {

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"UPDATE Leaderboard RecentScore=? WHERE SetID=? AND ID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                command.Parameters.AddWithValue("ID", ID);
                command.Parameters.AddWithValue("RecentScore", recentScore);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<bool> SetHighScore(int ID, int SetID, int highScore)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"UPDATE Leaderboard HighScore=? WHERE SetID=? AND ID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                command.Parameters.AddWithValue("ID", ID);
                command.Parameters.AddWithValue("HighScore", highScore);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<bool> CheckSetLeaderboard(int ID, int SetID)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT COUNT(*) FROM Leaderboard WHERE SetID=? AND ID=?";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                command.Parameters.AddWithValue("ID", ID);
                OleDbDataReader reader = command.ExecuteReader();

                reader.Read();
                int count = (int)reader[0];

                Console.WriteLine("the count is: "+count);

                if (count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<bool> SetNewEntry(int ID, int SetID, int recentScore)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"INSERT INTO Leaderboard (SetID, ID, HighScore, RecentScore) VALUES (@SetID, @ID, @RecentScore, @HighScore)";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("@SetID", SetID);
                command.Parameters.AddWithValue("@ID", ID);
                command.Parameters.AddWithValue("@RecentScore", recentScore);
                command.Parameters.AddWithValue("@HighScore", recentScore);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
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
