using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using flashplus.Pages.Authorized.Pages;
using Microsoft.AspNetCore.Components;

namespace flashplus.Services
{
    public class FlashcardSetCreateService
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetModel flashcardSetModel;
        public string errorMessage;

        public FlashcardSetCreateService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;

            flashcardSetModel = new FlashcardSetModel();
            flashcardSetModel.TotalCards = 0;
            flashcardSetModel.CardID = 1;
            flashcardSetModel.Flashcards = new List<string[]>();
        }

        public FlashcardSetModel GetFlashcardSetModel()
        {
            return flashcardSetModel;
        }

        public void AddCard()
        {

            if (flashcardSetModel.TotalCards == 20)
            {
                errorMessage = "Max card count of 20 has been reached";
            }

            if (string.IsNullOrEmpty(flashcardSetModel.Question) || string.IsNullOrEmpty(flashcardSetModel.Answer))
            {
                errorMessage = "Fields cannot be left blank";
            }
            else
            {
                errorMessage = null;
                string[] Flashcard = { flashcardSetModel.Question, flashcardSetModel.Answer };
                flashcardSetModel.Flashcard = Flashcard;
                flashcardSetModel.Flashcards.Add(flashcardSetModel.Flashcard);
                flashcardSetModel.TotalCards++;
                flashcardSetModel.CardID++;

                flashcardSetModel.Question = null;
                flashcardSetModel.Answer = null;
            }
        }

        public void RemoveCard()
        {
            if (flashcardSetModel.CardID == flashcardSetModel.TotalCards + 1)
            {
                errorMessage = "Cannot remove card";
            }
            else
            {
                errorMessage = null;
                flashcardSetModel.Flashcards.RemoveAt(flashcardSetModel.CardID - 1);
                flashcardSetModel.TotalCards--;
                flashcardSetModel.CardID--;
                NextCard();
            }
        }

        public void SaveCard()
        {
            if (string.IsNullOrEmpty(flashcardSetModel.Question) || string.IsNullOrEmpty(flashcardSetModel.Answer))
            {
                errorMessage = "Fields cannot be left blank";
            }
            else
            {
                errorMessage = null;
                string[] Flashcard = { flashcardSetModel.Question, flashcardSetModel.Answer };
                flashcardSetModel.Flashcard = Flashcard;
                flashcardSetModel.Flashcards[flashcardSetModel.CardID - 1] = Flashcard;
                NextCard();
            }
        }

        public void NextCard()
        {
            if (flashcardSetModel.CardID == flashcardSetModel.TotalCards)
            {
                flashcardSetModel.CardID++;
                flashcardSetModel.Question = null;
                flashcardSetModel.Answer = null;
            }
            else
            {
                flashcardSetModel.CardID++;
                flashcardSetModel.Question = flashcardSetModel.Flashcards[flashcardSetModel.CardID - 1][0];
                flashcardSetModel.Answer = flashcardSetModel.Flashcards[flashcardSetModel.CardID - 1][1];
            }
        }

        public void PreviousCard()
        {
            flashcardSetModel.CardID--;
            flashcardSetModel.Question = flashcardSetModel.Flashcards[flashcardSetModel.CardID - 1][0];
            flashcardSetModel.Answer = flashcardSetModel.Flashcards[flashcardSetModel.CardID - 1][1];
        }

        public async Task Submit()
        {
            ValidateSubmit();

            if (String.IsNullOrEmpty(errorMessage))
            {
                string SessionID = await localStorage.GetItemAsync<string>("SessionID");
                bool complete = await FlashcardSetDataAccess.SetFlashcardSetDetailsAsync(SessionID, flashcardSetModel);

                if (complete)
                {
                    NavigationManager.NavigateTo("/dashboard");
                }
                else
                {
                    errorMessage = "There has been an error processing your request, please try again later";
                }
            }
        }

        private void ValidateSubmit()
        {
            if (String.IsNullOrEmpty(flashcardSetModel.Title) || String.IsNullOrEmpty(flashcardSetModel.Subject))
            {
                errorMessage = "Title or Subject cannot be left blank";
            }
            if (flashcardSetModel.Flashcards.Count < 3)
            {
                errorMessage = "Must contain at least 3 cards in a set";
            }
            else
            {
                errorMessage = null;
            }
        }
    }
}
