using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Huffman_Week4_Doak
{
    public class PriorityQueue<T>
    {
        static ArrayList values = new ArrayList();
        static ArrayList priorities = new ArrayList();
        const int MAX_PRIO = int.MaxValue;
        // min. priority is always 1

        public void Enqueue(T data, int priority)
        {
            if (priority <= MAX_PRIO && priority >= 1)
            {
                values.Add(data);
                priorities.Add(priority);
            }
            else
                Console.WriteLine($"Cannot enqueue a priority greater than {MAX_PRIO} or less than 1.");

        }

        public T Dequeue(int priority)
        {
            object result = null;
            for (int i = 0; i < priorities.Count; i++)
            {
                if (priority == (int)priorities[i])
                {
                    result = (T)values[i];
                    priorities.RemoveAt(i);
                    values.RemoveAt(i);
                    return (T)result;
                }
            }
            Console.WriteLine($"No values of priority {priority} found.");
            // note: cannot return null because T could be a non-nullable value type. 
            // for reference variables that are nullable, the default is null.
            return default(T);
        }

        public T Dequeue()
        {
            int priority = 1;
            object result = null;

            while (priority <= MAX_PRIO)
            {
                for (int i = 0; i < priorities.Count; i++)
                {
                    if (priority == (int)priorities[i])
                    {
                        result = (T)values[i];
                        priorities.RemoveAt(i);
                        values.RemoveAt(i);
                        return (T)result;
                    }
                }
                priority++;
            }
            // note: cannot return null because T could be a non-nullable value type. 
            // for reference variables that are nullable, the default is null. 
            Console.WriteLine("Priority queue is empty.");
            return default(T);
        }

        public int Size()
        {
            return values.Count;
        }

        public bool isEmpty()
        {
            if (Size() == 0)
                return true;
            else
                return false;
        }
    }
}
