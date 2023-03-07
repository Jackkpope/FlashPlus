using flashplus.Models;
using System.Data.OleDb;

namespace flashplus.Data
{
    public interface IFlashcardSetDataAccess
    {
        Task<FlashcardSetModel> GetFlashcardSetDetailsAsync(string ID, string SetID);
        Task<bool> AddFlashcardSetDetailsAsync(FlashcardSetModel flashcardSetModel);
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

                    Console.WriteLine(flashcardSetModel.Question);
                    Console.WriteLine(flashcardSetModel.Answer);

                    flashcardSetModel.Flashcards.Add(CardDetails);
                }

                return flashcardSetModel;
            }
        }

        public async Task<bool> AddFlashcardSetDetailsAsync(FlashcardSetModel flashcardSetModel)
        {
            return true;
        }
    }
}
