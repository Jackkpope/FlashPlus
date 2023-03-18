using Microsoft.AspNetCore.Mvc.RazorPages;

namespace flashplus.Services
{
    public class FlashcardSetLibraryService
    {
        public FlashcardSetLibraryService()
        {

        }

        public void NextPage()
        {
            NextElement();
        }

        public void PreviousPage()
        {
            PreviousElement();
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
    }
}
