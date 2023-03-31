using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Text;

namespace flashplus.Services
{
    public class EntryService
    {
        private readonly IEntryDataAccess EntryDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;
        
        public EntryModel entryModel;
        public string errorMessage { get; set; }

        public EntryService(IEntryDataAccess entryDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            EntryDataAccess = entryDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;
            entryModel = new EntryModel();
        }

        

        public async Task HandleLogin()
        {
            if (!(String.IsNullOrEmpty(entryModel.Username) || String.IsNullOrEmpty(entryModel.Password)))
            { //checks neither fields are empty
                try
                {
                    entryModel.PasswordHash = CreatePasswordHash(entryModel.Password); //creates password hash
                    UserModel userModel = await EntryDataAccess.GetUserDetailsAsync(entryModel); //checks entrydetails are valid

                    if (userModel.ID != 0)
                    {
                        await localStorage.SetItemAsync("SessionID", userModel.ID); //stores the users ID in local storage
                        await localStorage.SetItemAsync("Username", userModel.Username); //stores the users username in local storage
                        NavigationManager.NavigateTo("/dashboard"); //redirects user to the user dashboard
                    }
                    else
                    {
                        errorMessage = "Invalid Credentials";
                    }
                }

                finally
                {
                    entryModel = new EntryModel();
                }
            }

            else
            {
                errorMessage = "Fields cannot be left blank";
                entryModel = new EntryModel();
            }
        }

        public async Task HandleRegister()
        {

            errorMessage = await RegisterValidation(); //checks register is valid

            if (String.IsNullOrEmpty(errorMessage))
            {
                entryModel.PasswordHash = CreatePasswordHash(entryModel.Password); //creates password hash
                bool complete = await EntryDataAccess.AddUserDetailsAsync(entryModel); //adds entrydetails to DB

                if (complete == true)
                {
                    NavigationManager.NavigateTo("login"); //redirects user to the login page
                }
                else
                {
                    errorMessage = "There has been an error processing your request, please try again later";
                }
            }
            else
            {
                entryModel = new EntryModel();
            }
        }

        public async Task HandleLogout()
        {
            await localStorage.ClearAsync(); //removes all values in local storage
            NavigationManager.NavigateTo("/"); //redirects to home page
        }

        private string CreatePasswordHash(string password)
        {
            string hashPassword = null;

            foreach (char character in password)
            {
                char hashValue = Convert.ToChar(Convert.ToInt32(character*17) % 257); //converts a character into its ascii value multiplies it by 17 then MODS it by 257
                hashPassword += hashValue; //each character is added to the string
            }

            hashPassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(hashPassword));

            return hashPassword;
        }

        private async Task<string> RegisterValidation()
        {

            if (string.IsNullOrEmpty(entryModel.Username) || string.IsNullOrEmpty(entryModel.Password) || string.IsNullOrEmpty(entryModel.ConfirmPassword))
            {
                return "Fields cannot be left blank";
            }

            bool usernameTaken = await EntryDataAccess.CheckUsernameTakenAsync(entryModel.Username);

            if (usernameTaken == true)
            {
                return "Username is already taken";
            }

            if (entryModel.Username.Length <= 4)
            {
                return "Username must be at least 4 characters";
            }

            if (entryModel.Username.Length > 20)
            {
                return "Username must be less than 20 characters";
            }

            if (entryModel.Password.Length <= 8)
            {
                return "Password must be at least 8 characters";
            }

            if (!(entryModel.Password.Contains('!') || entryModel.Password.Contains('?') || entryModel.Password.Contains('@') || entryModel.Password.Contains('&')))
            {
                return "Password must contain at least one: ! ? @ &";
            }

            if (entryModel.Password != entryModel.ConfirmPassword)
            {
                return "Passwords don't match";
            }
            else
            {
                return null;
            }
        }
    }
}
