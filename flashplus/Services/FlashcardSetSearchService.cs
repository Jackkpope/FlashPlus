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

        public string errorMessage = null;
        public bool search = false;

        public FlashcardSetSearchService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
            : base(flashcardSetDataAccess, localStorage, navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;

            searchModel = new SearchModel();
            searchModel.Search = null;
        }

        public async Task SubmitSearch()
        {
            await Search();
        }

        private async Task Search()
        {

            errorMessage = ValidateSearch();

            if (errorMessage == null)
            {
                flashcardSetModel = new FlashcardSetModel();

                TypeOfSearch();
                SortSearch();

                if (flashcardSetModel.FlashcardSets.Count != 0)
                {
                    currentPageNo = 1;
                    totalPageNo = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(flashcardSetModel.FlashcardSets.Count) / 6)); //Divides total flashcard sets by 6 then rounds up (e.g. 7 sets = 2 pages)
                    Console.WriteLine(totalPageNo);
                    search = true;

                    GetDisplayedSets();
                }
                else
                {
                    ResetSearch();
                    errorMessage = "There are no matches for your search";
                }
            }
        }
        
        private string ValidateSearch()
        {
            if(string.IsNullOrWhiteSpace(searchModel.Search))
            {
                ResetSearch();
                return errorMessage = "Fields cannot be left blank";
            }

            if(string.IsNullOrWhiteSpace(searchModel.SearchType))
            {
                ResetSearch();
                return errorMessage = "Must select a search type";
            }
            else
            {
                return null;
            }
        }

        private void ResetSearch()
        {
            flashcardSetModel = new FlashcardSetModel();
            search = false;
        }

        private async void TypeOfSearch()
        {
            switch (searchModel.SearchType)
            {
                case ("Title"):
                    flashcardSetModel = await FlashcardSetDataAccess.GetAllFlashcardSetsByTitleAsync(searchModel.Search);
                    break;

                case ("Subject"):
                    flashcardSetModel = await FlashcardSetDataAccess.GetAllFlashcardSetsBySubjectAsync(searchModel.Search);
                    break;
            }
        }

        private void SortSearch()
        {
            switch(searchModel.SortType)
            {
                case ("AZ"):
                    BubbleSort(flashcardSetModel);
                    break;

                case ("ZA"):
                    BubbleSort(flashcardSetModel);
                    flashcardSetModel.FlashcardSets.Reverse();
                    break;

                case ("Newest"):
                    break;
            }
        }

        private void BubbleSort(FlashcardSetModel flashcardSetModel)
        {
            int total = flashcardSetModel.FlashcardSets.Count;
            bool swapped;

            do
            {
                swapped = false;
                for (int i = 0; i < total-1; i++)
                {
                    if ((int)flashcardSetModel.FlashcardSets[i][0][0] > (int)flashcardSetModel.FlashcardSets[i + 1][0][0])
                    {
                        string[] tempElement = flashcardSetModel.FlashcardSets[i];
                        flashcardSetModel.FlashcardSets[i] = flashcardSetModel.FlashcardSets[i + 1];
                        flashcardSetModel.FlashcardSets[i + 1] = tempElement;
                        swapped = true;
                    }
                }
                total--;
            } while (swapped);
        }
    }
}
