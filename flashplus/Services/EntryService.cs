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
            {
                try
                {
                    UserModel userModel = await EntryDataAccess.GetUserDetailsAsync(entryModel);

                    if (userModel.ID != 0)
                    {
                        await localStorage.SetItemAsync("SessionID", userModel.ID);
                        await localStorage.SetItemAsync("Username", userModel.Username);
                        NavigationManager.NavigateTo("/dashboard");
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

            errorMessage = await RegisterValidation();

            if (String.IsNullOrEmpty(errorMessage))
            {
                bool complete = await EntryDataAccess.AddUserDetailsAsync(entryModel);

                if (complete == true)
                {
                    NavigationManager.NavigateTo("login");
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
            await localStorage.ClearAsync();
            NavigationManager.NavigateTo("/");
        }

        private async Task<string> CreatePasswordHash()
        {
            string salt = "473827642";
            string saltedPassword = entryModel.Password + salt;
            byte[] saltedPasswordInBytes = Encoding.UTF8.GetBytes(saltedPassword);

            int hashValue = 0;

            foreach (var i in saltedPasswordInBytes)
            {
                hashValue = (hashValue + i) % 2736;
            }

            string hashPassword = Convert.ToString(hashValue);

            return hashPassword;
        }

        private async Task<string> RegisterValidation()
        {

            if (String.IsNullOrEmpty(entryModel.Username) || String.IsNullOrEmpty(entryModel.Password) || String.IsNullOrEmpty(entryModel.ConfirmPassword))
            {
                return "Fields cannot be left blank";
            }

            bool usernameTaken = await EntryDataAccess.CheckUsernameTakenAsync(entryModel.Username);

            if (usernameTaken == true)
            {
                return "Username is already taken";
            }

            if (entryModel.Username.Length < 4)
            {
                return "Username must be at least 4 characters";
            }

            if (entryModel.Password.Length < 8)
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
