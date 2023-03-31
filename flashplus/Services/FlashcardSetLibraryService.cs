using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace flashplus.Services
{
    public class FlashcardSetLibraryService
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetModel flashcardSetModel;

        private string SessionID;
        public List<string[]> displayedSets;
        public int currentPageNo;
        public int totalPageNo;

        public FlashcardSetLibraryService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;
        }

        public async Task AccessInitiliazeLibrary()
        {
            await InitiliazeLibrary();
        }

        private async Task InitiliazeLibrary()
        {
            SessionID = await localStorage.GetItemAsync<string>("SessionID");
            flashcardSetModel = await FlashcardSetDataAccess.GetAllFlashcardSetsByUserAsync(SessionID);

            currentPageNo = 1;
            totalPageNo = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(flashcardSetModel.FlashcardSets.Count) / 6)); //Divides total flashcard sets by 6 then rounds up (e.g. 7 sets = 2 pages)
            GetDisplayedSets();
        }

        public void GetDisplayedSets()
        {
            DisplayedSets();
        }

        public async Task LoadFlashcardSet(int SetID)
        {
            await FlashcardSet(SetID);
        }

        public void NextPage()
        {
            if (currentPageNo == totalPageNo)
            {
                currentPageNo = 1;
            }
            else
            {
                currentPageNo++;
            }
            GetDisplayedSets();
        }

        public void PreviousPage()
        {

            if (currentPageNo == 1)
            {
                currentPageNo = totalPageNo;
            }
            else
            {
                currentPageNo--;
            }
            GetDisplayedSets();

        }

        private void DisplayedSets()
        {
            displayedSets = new List<string[]>();

            foreach (var flashcardset in flashcardSetModel.FlashcardSets.Skip((currentPageNo - 1) * 6).Take(6)) //skips the pageNo*6 and takes the next 6 values that follow
            {
                displayedSets.Add(flashcardset);
            }
        }

        private async Task FlashcardSet(int SetID)
        {
            await localStorage.SetItemAsync("SetID", SetID); //stores the SetID in the users local storage
            NavigationManager.NavigateTo("/flashcardset/view"); //redirects user to view the flashcard set
        }
    }
}
