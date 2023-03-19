using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace flashplus.Services
{
    public class FlashcardSetLibraryService
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetLibraryService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;
        }

        public void NextPage()
        {
            NextElement();
        }

        public void PreviousPage()
        {
            PreviousElement();
        }

        private void CurrentElement()
        {

        }

        private void NextElement()
        {
            CurrentNo = (CurrentNo % Total) + 1;
            GetDisplayedSets(CurrentNo);
        }

        private void PreviousElement()
        {
            CurrentNo = ((CurrentNo - 2 + Total) % Total) + 1;
            GetDisplayedSets(CurrentNo);
        }

        public void GetDisplayedSets(int pageNo)
        {
            displayedSets = new List<string[]>();
            foreach (string[] flashcardSet in flashcardSetModel.FlashcardSets.Skip((pageNo - 1) * 6).Take(6))
            {
                displayedSets.Add(flashcardSet);
                Console.WriteLine(flashcardSet[0]);
            }
        }

        public async Task LoadFlashcardSet(int SetID)
        {
            await localStorage.SetItemAsync("SetID", SetID);
            NavigationManager.NavigateTo("/flashcardset/view");
        }
    }
}
