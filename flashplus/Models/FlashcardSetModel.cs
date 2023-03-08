using flashplus.Services;

namespace flashplus.Models
{
    public class FlashcardSetModel
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string CreatedUsername { get; set; }
        public string[] Flashcard { get; set; }
        public List<string[]> Flashcards { get; set; }
        public List<string[]> FlashcardSets { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int TotalCards { get; set; }
        public int SetID { get; set; }
        public DateTime DateCreated { get; set; }

        //Model used to store and show data for flashcard sets

    }
}
