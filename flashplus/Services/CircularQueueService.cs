using System.Collections.Generic;
using System.Xml.Linq;

namespace flashplus.Services
{
    public class CircularQueueService<T>
    {
        private T[] circularQueue; //'T' can store any type of data making it flexible to different scenarios
        private int front;
        private int back;
        private string[] currentElement;

        public CircularQueueService(T[] elements, int total)
        {
            circularQueue = new T[total];
            front = 0;
            back = 0;

            IntitializeQueue(elements);
        }

        public void IntitializeQueue(T[] elements)
        {
            foreach(var element in elements)
            {
                Enqueue(element);
                back++;
            }
        }

        public void Enqueue(T element) //Adds an element to the back of the Queue
        {
            circularQueue[back] = element;
        }

        public T Dequeue() //Removes the element at the front of the queue and then enqueues it 
        {
            T currentElement = circularQueue[front];

            for(int i = 0; i != circularQueue.Length; i++)
            {
                circularQueue[i] = circularQueue[i + 1];
            }

            Enqueue(currentElement);
            return currentElement;
        }
    }
}
