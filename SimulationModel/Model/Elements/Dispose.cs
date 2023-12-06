using System;
using System.Collections.Generic;

using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Elements
{
    public class Dispose<T> : Element<T> where T : DefaultQueueItem
    {
        private List<T> _finishItems;

        public Dispose(string name)
            :base(name)
        {
            _currentTime = 0;
            SetNextTime(Double.PositiveInfinity);

            _finishItems = new List<T>();
        }

        public override void StartService(T item) 
        {
            if(item is Job Job)
            {
                Job.Finish(_currentTime);
            }
            _countProcessed++;
            _finishItems.Add(item);
        }

        public override void FinishService()
        {
        }

        public override void PrintStats(bool finalStats)
        {
            base.PrintStats(finalStats);
            Console.WriteLine($"\t\tFinished items: {_countProcessed}");

            if(finalStats)
            {
                if (typeof(T) == typeof(Job))
                {
                    Console.WriteLine("\t\tPatients Type   StartTime   FinishTime");
                    foreach (var item in _finishItems)
                    {
                        item.PrintStats();
                    }
                }
            }
        }
    }
}
