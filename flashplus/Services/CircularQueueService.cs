using System.Collections.Generic;

namespace flashplus.Services
{
    public class CircularQueueService
    {
        private List<string[]> CircularQueue = new List<string[]>();
        private int front;
        private int back;
        private string[] currentElement;
        private string[] newElement;

        public CircularQueueService(List<string[]> List, int count)
        {
            front = 0;
            back = count - 1;

            foreach (var i in List)
            {
                CircularQueue.Add(i);
            }
        }

        public string[] GetCurrentElement()
        {
            currentElement = CircularQueue [front];
            return currentElement;
        }

        public string[] GetNextElement()
        {
            currentElement = CircularQueue[front];
            CircularQueue.RemoveAt(front);
            CircularQueue.Add(currentElement);

            newElement = CircularQueue[front];

            return newElement;
        }

        public string[] GetPreviousElement()
        {
            newElement = CircularQueue[back];

            currentElement = CircularQueue[back];
            CircularQueue.RemoveAt(back);
            CircularQueue.Insert(0, currentElement);

            return newElement;
        }
    }
}
