using flashplus.Models;
using System.Data.OleDb;

namespace flashplus.Data
{
    public interface IFlashcardSetDataAccess
    {
        Task<FlashcardSetModel> GetFlashcardSetDetailsAsync(string ID, string SetID);
        Task<FlashcardSetModel> GetAllFlashcardSetsByUserAsync(string ID);
        Task<bool> SetFlashcardSetDetailsAsync(string ID, FlashcardSetModel flashcardSetModel);
    }

    public class FlashcardSetDataAccess : IFlashcardSetDataAccess
    {
        public FlashcardSetDataAccess()
        {

        }

        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Jackp\Documents\flashplus.accdb;Persist Security Info=False;";

        public async Task<FlashcardSetModel> GetFlashcardSetDetailsAsync(string ID, string SetID)
        {
            FlashcardSetModel flashcardSetModel = new FlashcardSetModel();

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT Username FROM UserDetails WHERE ID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("ID", ID);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    flashcardSetModel.CreatedUsername = (string)reader["Username"];
                }

                queryString = @"SELECT SetName, Subject, TotalCards FROM FlashcardSets WHERE SetID=?;";
                command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    flashcardSetModel.Title = (string)reader["SetName"];
                    flashcardSetModel.Subject = (string)reader["Subject"];
                    flashcardSetModel.TotalCards = (int)reader["TotalCards"];
                }

                queryString = @"SELECT Question, Answer FROM Flashcards WHERE SetID=?;";
                command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                reader = command.ExecuteReader();

                flashcardSetModel.Flashcards = new List<string[]> { };

                while (reader.Read())
                {
                    flashcardSetModel.Question = (string)reader["Question"];
                    flashcardSetModel.Answer = (string)reader["Answer"];

                    string[] CardDetails = { flashcardSetModel.Question, flashcardSetModel.Answer };

                    flashcardSetModel.Flashcards.Add(CardDetails);
                }

                return flashcardSetModel;
            }
        }

        public async Task<FlashcardSetModel> GetAllFlashcardSetsByUserAsync(string ID)
        {
            FlashcardSetModel flashcardSetModel = new FlashcardSetModel();

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT SetName, Subject, SetID FROM FlashcardSets WHERE ID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("ID", ID);
                OleDbDataReader reader = command.ExecuteReader();

                flashcardSetModel.FlashcardSets = new List<string[]> { };

                while (reader.Read())
                {
                    flashcardSetModel.Title = (string)reader["SetName"];
                    flashcardSetModel.Subject = (string)reader["Subject"];
                    flashcardSetModel.SetID = (int)reader["SetID"];

                    string[] FlashcardSets = { flashcardSetModel.Title, flashcardSetModel.Subject, Convert.ToString(flashcardSetModel.SetID) };

                    flashcardSetModel.FlashcardSets.Add(FlashcardSets);
                }

                return flashcardSetModel;
            }
        }

        public async Task<bool> SetFlashcardSetDetailsAsync(string ID, FlashcardSetModel flashcardSetModel)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"INSERT INTO FlashcardSets (SetName, Subject, TotalCards, ID) VALUES (@SetName,@Subject,@TotalCards,@ID);";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("@SetName", flashcardSetModel.Title);
                command.Parameters.AddWithValue("@Subject", flashcardSetModel.Subject);
                command.Parameters.AddWithValue("@TotalCards", flashcardSetModel.TotalCards);
                command.Parameters.AddWithValue("@ID", ID);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return false;
                }

                queryString = @"SELECT @@IDENTITY AS SetID;";
                command = new OleDbCommand(queryString, connection);
                OleDbDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    flashcardSetModel.SetID = (int)reader["SetID"];
                }

                queryString = @"INSERT INTO Flashcards (CardID, SetID, Question, Answer) VALUES (@CardID,@SetID,@Question,@Answer);";

                flashcardSetModel.CardID = 1;

                foreach(var card in flashcardSetModel.Flashcards)
                {
                    command = new OleDbCommand(queryString, connection);
                    command.Parameters.AddWithValue("@CardID", flashcardSetModel.CardID);
                    command.Parameters.AddWithValue("@SetID", flashcardSetModel.SetID);
                    command.Parameters.AddWithValue("@Question", card[0]);
                    command.Parameters.AddWithValue("@Answer", card[1]);
                    flashcardSetModel.CardID++;

                    Console.WriteLine(flashcardSetModel.CardID.ToString()+" "+ flashcardSetModel.SetID.ToString() + " " + card[0]+" "+ card[1]);

                    rowsAffected = command.ExecuteNonQuery();
                }

                if (rowsAffected == 0)
                {
                    return false;
                }

                return true;

            }
        }
    }
}
