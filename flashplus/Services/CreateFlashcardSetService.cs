using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;

namespace flashplus.Services
{
    public class CreateFlashcardSetService
    {
        IFlashcardSetDataAccess FlashcardSetDataAccess;
        ILocalStorageService localStorage;
        NavigationManager NavigationManager;

        public string errorMessage {get; set;}
        public FlashcardSetModel flashcardSetModel;

        public CreateFlashcardSetService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;

            flashcardSetModel = new FlashcardSetModel();
            flashcardSetModel.TotalCards = 0;
            flashcardSetModel.CardID = 1;
            flashcardSetModel.Flashcards = new List<string[]>();
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
            if(flashcardSetModel.CardID == flashcardSetModel.TotalCards+1)
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
            if(String.IsNullOrEmpty(flashcardSetModel.Title) || String.IsNullOrEmpty(flashcardSetModel.Subject))
            {
                errorMessage = "Title or Subject cannot be left blank";
            }
            if(flashcardSetModel.Flashcards.Count < 3)
            {
                errorMessage = "Must contain at least 3 cards in a set";
            }

            else
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
    }
}
