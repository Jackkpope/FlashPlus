using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;

namespace flashplus.Services
{
    public class FlashcardSetViewService
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetModel flashcardSetModel;
        public int currentCardNo;
        private string SetID;
        private string ID;
        public bool owner;

        public FlashcardSetViewService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;

            flashcardSetModel = new FlashcardSetModel();
        }

        public async Task AccessInitiliazeView()
        {
            await InitiliazeView();
        }

        public async Task GetDeleteFlashcardSet()
        {
            await DeleteFlashcardSet();
        }

        private async Task InitiliazeView()
        {
            SetID = await localStorage.GetItemAsync<string>("SetID");
            ID = await localStorage.GetItemAsync<string>("ID");
            string Username = await localStorage.GetItemAsync<string>("Username");

            flashcardSetModel = await FlashcardSetDataAccess.GetFlashcardSetDetailsAsync(SetID);
            currentCardNo = 1;
            flashcardSetModel.Flashcard = flashcardSetModel.Flashcards[0];

            if(Username == flashcardSetModel.CreatedUsername)
            {
                owner = true;
            }
            else
            {
                owner = false;
            }
        }

        private async Task DeleteFlashcardSet()
        {
            await FlashcardSetDataAccess.RemoveFlashcardSetAsync(ID, SetID);
            NavigationManager.NavigateTo("/flashcardset/library");
        }

        public void GetNextCard()
        {
            if (currentCardNo == flashcardSetModel.TotalCards)
            {
                currentCardNo = 1;
            }
            else
            {
                currentCardNo++;
            }

            flashcardSetModel.Flashcard = flashcardSetModel.Flashcards[currentCardNo - 1];
        }

        public void GetPreviousCard()
        {
            if (currentCardNo == 1)
            {
                currentCardNo = flashcardSetModel.TotalCards;
            }
            else
            {
                currentCardNo--;
            }

            flashcardSetModel.Flashcard = flashcardSetModel.Flashcards[currentCardNo - 1];

        }
    }
}
