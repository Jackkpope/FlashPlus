using Blazored.LocalStorage;
using flashplus.Data;
using flashplus.Models;
using flashplus.Pages.Authorized.Pages;
using Microsoft.AspNetCore.Components;
using System;
using System.Timers;

namespace flashplus.Services
{
    public class FlashcardSetFlashService : ComponentBase
    {
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetModel flashcardSetModel;
        public bool IsInitialized;

        public bool OutOfTime;
        public int currentTimeRemaining;
        public int currentStreak;
        public string errorMessage;
        public List<string> displayedAnswers;

        private string[] Questions;
        private string[] Answers;
        private int currentCard;
        private System.Timers.Timer timer;

        public FlashcardSetFlashService(IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            FlashcardSetDataAccess = flashcardSetDataAccess;
            this.localStorage = localStorage;
            NavigationManager = navigationManager;
        }

        public async Task InitializeFlash()
        {
            await Flash();
        }

        private async Task Flash()
        {
            flashcardSetModel = new FlashcardSetModel();
            string SetID = await localStorage.GetItemAsync<string>("SetID");
            flashcardSetModel = await FlashcardSetDataAccess.GetFlashcardSetDetailsAsync(SetID);

            Questions = new string[flashcardSetModel.TotalCards];
            Answers = new string[flashcardSetModel.TotalCards];
            errorMessage = null;

            SetValues();

        }

        private void SetValues()
        {
            flashcardSetModel.Flashcards = RandomizeListOfArrays(flashcardSetModel.Flashcards);

            for (int i = 0; i < flashcardSetModel.TotalCards; i++)
            {
                Questions[i] = flashcardSetModel.Flashcards[i][0];
                Answers[i] = flashcardSetModel.Flashcards[i][1];
            }

            currentCard = 0;

            flashcardSetModel.Question = Questions[currentCard];
            flashcardSetModel.Answer = Answers[currentCard];

            displayedAnswers = new List<string>(RetrieveRandomAnswers(Answers));
            displayedAnswers = RandomizeList(displayedAnswers, 4);

            ResetTimer();
        }

        private void NextCard()
        {
            currentCard++;

            if(currentCard != flashcardSetModel.TotalCards - 1)
            {
                flashcardSetModel.Question = Questions[currentCard];
                flashcardSetModel.Answer = Answers[currentCard];

                displayedAnswers = new List<string>(RetrieveRandomAnswers(Answers));
                displayedAnswers = RandomizeList(displayedAnswers, 4);

                ResetTimer();
            }
            else
            {
                SetValues();
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            OutOfTime = false;
            currentTimeRemaining = 20;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimer;
            timer.Enabled = true;
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            currentTimeRemaining--;

            if (currentTimeRemaining == 0)
            {
                timer.Enabled = false;
                timer.Dispose();
                OutOfTime = true;
                errorMessage = "Ran out of time";
            }
        }

        public void OnSubmit(int answer)
        {
            Submit(answer);
        }

        private void Submit(int answer)
        {
            timer.Enabled = false;
            timer.Dispose();

            if (displayedAnswers[answer]==flashcardSetModel.Answer)
            {
                errorMessage = null;
                currentStreak++;

                NextCard();
            }
            else
            {
                errorMessage = "Incorrect Answer";
            }
        }

        private List<string[]> RandomizeListOfArrays(List<string[]> list)
        {
            Random random = new Random();
            int count = flashcardSetModel.TotalCards;

            while (count > 1) // fisher-yates shuffle algorithm
            {
                count--;
                int k = random.Next(count);
                string[] value = list[k];
                list[k] = list[count];
                list[count] = value;
            }

            return list;
        }

        private List<string> RandomizeList(List<string> list, int count) // randomizes an array of values
        {
            Random random = new Random();

            while (count > 1) // fisher-yates shuffle algorithm
            {
                count--;
                Console.WriteLine(count);
                int k = random.Next(count+1);
                Console.WriteLine(k);
                string value = list[k];
                list[k] = list[count];
                list[count] = value;
            }

            return list;
        }

        private List<string> RetrieveRandomAnswers(string[] array)
        {
            Random random = new Random();
            List<string> randomAnswers = new List<string>();
            randomAnswers.Add(flashcardSetModel.Answer);

            while(randomAnswers.Count<4)
            {
                int k = random.Next(flashcardSetModel.TotalCards);
                string randomAnswer = array[k];

                if (!(randomAnswers.Contains(randomAnswer)))
                {
                    randomAnswers.Add(randomAnswer);
                }
            }

            return randomAnswers;
        }
    }
}
