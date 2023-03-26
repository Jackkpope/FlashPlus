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

        private async Task InitiliazeView()
        {
            string SetID = await localStorage.GetItemAsync<string>("SetID");

            flashcardSetModel = await FlashcardSetDataAccess.GetFlashcardSetDetailsAsync(SetID);
            currentCardNo = 1;
            flashcardSetModel.Flashcard = flashcardSetModel.Flashcards[0];
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
