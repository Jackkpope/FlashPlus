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
        private readonly ILeaderboardDataAccess LeaderboardDataAccess;
        private readonly IFlashcardSetDataAccess FlashcardSetDataAccess;
        private readonly ILocalStorageService localStorage;
        private readonly NavigationManager NavigationManager;

        public FlashcardSetModel flashcardSetModel;
        public LeaderboardModel leaderboardModel;

        public bool IsInitialized;

        public int currentStreak;
        public string errorMessage;
        public List<string> displayedAnswers;

        private string[] Questions;
        private string[] Answers;
        private int currentCard;

        private int ID;
        private int SetID;

        public FlashcardSetFlashService(ILeaderboardDataAccess leaderboardDataAccess, IFlashcardSetDataAccess flashcardSetDataAccess, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            LeaderboardDataAccess = leaderboardDataAccess;
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
            ID = await localStorage.GetItemAsync<int>("ID");
            SetID = await localStorage.GetItemAsync<int>("SetID");
            flashcardSetModel = await FlashcardSetDataAccess.GetFlashcardSetDetailsAsync(Convert.ToString(SetID));

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
            }
            else
            {
                SetValues();
            }
        }

        public async Task OnSubmit(int answer)
        {
            await Submit(answer);
        }

        private async Task Submit(int answer)
        {
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
