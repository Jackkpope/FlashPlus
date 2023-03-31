using flashplus.Models;
using System.Data.OleDb;

namespace flashplus.Data
{
    public interface IFlashcardSetDataAccess
    {
        Task<FlashcardSetModel> GetFlashcardSetDetailsAsync(string SetID);
        Task<FlashcardSetModel> GetAllFlashcardSetsByUserAsync(string ID);
        Task<FlashcardSetModel> GetAllFlashcardSetsByTitleAsync(string Title);
        Task<FlashcardSetModel> GetAllFlashcardSetsBySubjectAsync(string Subject);
        Task<bool> SetFlashcardSetDetailsAsync(string ID, FlashcardSetModel flashcardSetModel);
        Task<bool> RemoveFlashcardSetAsync(string ID, string SetID);
    }

    public class FlashcardSetDataAccess : IFlashcardSetDataAccess
    {

        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Jackp\Documents\flashplus.accdb;Persist Security Info=False;";

        public async Task<FlashcardSetModel> GetFlashcardSetDetailsAsync(string SetID)
        {
            FlashcardSetModel flashcardSetModel = new FlashcardSetModel();

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT ID, SetName, Subject, TotalCards FROM FlashcardSets WHERE SetID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    flashcardSetModel.UserID = (int)reader["ID"];
                    flashcardSetModel.Title = (string)reader["SetName"];
                    flashcardSetModel.Subject = (string)reader["Subject"];
                    flashcardSetModel.TotalCards = (int)reader["TotalCards"];
                }

                queryString = @"SELECT Username FROM UserDetails WHERE ID=?;";
                command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("ID", flashcardSetModel.UserID);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    flashcardSetModel.CreatedUsername = (string)reader["Username"];
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

                    string[] FlashcardSet = { flashcardSetModel.Title, flashcardSetModel.Subject, Convert.ToString(flashcardSetModel.SetID) };

                    flashcardSetModel.FlashcardSets.Add(FlashcardSet);
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

                    rowsAffected = command.ExecuteNonQuery();
                }

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

        public async Task<FlashcardSetModel> GetAllFlashcardSetsByTitleAsync(string Title)
        {
            FlashcardSetModel flashcardSetModel = new FlashcardSetModel();

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT Subject, SetID FROM FlashcardSets WHERE SetName=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetName", Title);
                OleDbDataReader reader = command.ExecuteReader();

                flashcardSetModel.FlashcardSets = new List<string[]> { };
                Title = Title[0].ToString().ToUpper() + Title.Substring(1).ToLower();

                while (reader.Read())
                {
                    flashcardSetModel.Title = Title;
                    flashcardSetModel.Subject = (string)reader["Subject"];
                    flashcardSetModel.SetID = (int)reader["SetID"];

                    string[] FlashcardSet = { flashcardSetModel.Title, flashcardSetModel.Subject, Convert.ToString(flashcardSetModel.SetID) };

                    flashcardSetModel.FlashcardSets.Add(FlashcardSet);
                }

                return flashcardSetModel;
            }
        }

        public async Task<FlashcardSetModel> GetAllFlashcardSetsBySubjectAsync(string Subject)
        {
            FlashcardSetModel flashcardSetModel = new FlashcardSetModel();

            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"SELECT ID, SetName, SetID FROM FlashcardSets WHERE Subject=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("Subject", Subject);
                OleDbDataReader reader = command.ExecuteReader();

                flashcardSetModel.FlashcardSets = new List<string[]> { };
                Subject = Subject[0].ToString().ToUpper() + Subject.Substring(1).ToLower();

                while (reader.Read())
                {

                    flashcardSetModel.Title = (string)reader["SetName"];
                    flashcardSetModel.Subject = Subject;
                    flashcardSetModel.SetID = (int)reader["SetID"];


                    string[] FlashcardSet = { flashcardSetModel.Title, flashcardSetModel.Subject, Convert.ToString(flashcardSetModel.SetID)};

                    flashcardSetModel.FlashcardSets.Add(FlashcardSet);
                }

                return flashcardSetModel;
            }
        }

        public async Task<bool> RemoveFlashcardSetAsync(string ID, string SetID)
        {
            await using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                string queryString = @"DELETE FROM Flashcards WHERE SetID=?;";
                OleDbCommand command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    return false;
                }


                queryString = @"DELETE FROM FlashcardSets WHERE SetID=?;";
                command = new OleDbCommand(queryString, connection);
                command.Parameters.AddWithValue("SetID", SetID);

                rowsAffected = command.ExecuteNonQuery();

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
