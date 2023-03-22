using System.Collections.Generic;
using System.Xml.Linq;

namespace flashplus.Services
{
    public class CircularQueueService
    {
        private string[][] circularQueue; 
        private int front;
        private int back;
        public string[] currentElement;

        public CircularQueueService(List<string[]> elements, int total) //Constructor for the CircularQueueService
        {
            circularQueue = new string[total][]; 
            front = 0;
            back = -1;

            foreach (var element in elements) //Enqueues each element in elements
            {
                back++; //Back pointer goes up by one
                Enqueue(element);
            }

            GetCurrentElement();
        }

        public void GetCurrentElement()
        {
            currentElement = CurrentElement();
        }

        public void GetNextElement()
        {
            Dequeue();
            GetCurrentElement();
        }

        public void GetPreviousElement()
        {
            ReverseDequeue();
            GetCurrentElement();
        }

        private string[] CurrentElement() //Gets the value of the element at the front of the Queue
        {
            return circularQueue[front];
        }

        private void Enqueue(string[] element) //Adds an element to the back of the Queue
        {
            circularQueue[back] = element;
        }

        private void Dequeue() //Removes the element at the front of the queue and then enqueues it 
        {
            string[] tempElement = circularQueue[front];

            for (int i = 0; i < circularQueue.Length - 1; i++)
            {
                circularQueue[i] = circularQueue[i + 1]; //moves each element forward in the array by one
            }

            Enqueue(tempElement);
        }

        private void ReverseDequeue() //Removes the element at the back of the queue and then puts it at the front
        {
            string[] tempElement = circularQueue[back];

            for (int i = circularQueue.Length - 1; i > 0; i--)
            {
                circularQueue[i] = circularQueue[i - 1]; //moves each element back in the array by one
            }

            circularQueue[front] = tempElement;
        }
    }
}
