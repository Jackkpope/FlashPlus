namespace flashplus.Models
{
    public class FlashcardSetModel
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string CreatedUsername { get; set; }
        public List<string[]> Flashcards { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int TotalCards { get; set; }
        public DateTime DateCreated { get; set; }

    }
    public class FlashcardSetCircularQueue : FlashcardSetModel
    {
        private List<string[]> flashcardCircularQueue = new List<string[]>();
        private int front;
        private int back;
        private string[] currentFlashcard;
        private string[] newFlashcard;


        public FlashcardSetCircularQueue(List<string[]> Flashcards, int TotalCards)
        {
            front = 0;
            back = TotalCards - 1;

            foreach (var i in Flashcards)
            {
                flashcardCircularQueue.Add(i);
            }

            newFlashcard = flashcardCircularQueue[front];
            Question = newFlashcard[0];
            Answer = newFlashcard[1];
        }

        public void GetNextFlashcard()
        {
            currentFlashcard = flashcardCircularQueue[front];
            flashcardCircularQueue.RemoveAt(front);
            flashcardCircularQueue.Add(currentFlashcard);

            newFlashcard = flashcardCircularQueue[front];
            Question = newFlashcard[0];
            Answer = newFlashcard[1];
        }

        public void GetPreviousFlashcard()
        {
            newFlashcard = flashcardCircularQueue[back];
            Question = newFlashcard[0];
            Answer = newFlashcard[1];

            currentFlashcard = flashcardCircularQueue[back];
            flashcardCircularQueue.RemoveAt(back);
            flashcardCircularQueue.Insert(0, currentFlashcard);

        }
    }

    public class FlashcardSetStack : FlashcardSetModel
    {
        public FlashcardSetStack()
        {

        }
    }
}
