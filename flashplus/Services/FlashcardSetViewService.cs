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
        private CircularQueueService flashcardSetCircularQueue;

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
            string SessionID = await localStorage.GetItemAsync<string>("SessionID");
            string SetID = await localStorage.GetItemAsync<string>("SetID");

            flashcardSetModel = await FlashcardSetDataAccess.GetFlashcardSetDetailsAsync(SessionID, SetID);
            flashcardSetCircularQueue = new CircularQueueService(flashcardSetModel.Flashcards, flashcardSetModel.TotalCards);

            flashcardSetCircularQueue.GetCurrentElement();
            flashcardSetModel.Flashcard = flashcardSetCircularQueue.currentElement;
            currentCardNo = 1;
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

            flashcardSetCircularQueue.GetNextElement();
            flashcardSetModel.Flashcard = flashcardSetCircularQueue.currentElement;
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

            flashcardSetCircularQueue.GetPreviousElement();
            flashcardSetModel.Flashcard = flashcardSetCircularQueue.currentElement;
        }
    }
}
