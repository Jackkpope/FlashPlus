using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using Microsoft.AspNetCore.Components;

namespace flashplus.Services
{
    public class FlashcardSetSearchService  : FlashcardSetLibraryService
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public SearchModel searchModel;

        public string searchText;
        public bool search = false;

        public FlashcardSetSearchService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
            : base(flashcardSetDataAccess, localStorage, navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;

            searchModel = new SearchModel();
        }

        public async Task SubmitSearch()
        {
            await Search();
        }

        private async Task Search()
        {
            flashcardSetModel = new FlashcardSetModel();
            flashcardSetModel = await FlashcardSetDataAccess.GetAllFlashcardSetsByTitleAsync(searchModel.Search);

            currentPageNo = 1;
            totalPageNo = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(flashcardSetModel.FlashcardSets.Count) / 6)); //Divides total flashcard sets by 6 then rounds up (e.g. 7 sets = 2 pages)
            Console.WriteLine(totalPageNo);
            search = true;
            GetDisplayedSets();
        }
    }
}
