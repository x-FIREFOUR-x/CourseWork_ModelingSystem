using System;
using System.Collections.Generic;
using System.Linq;

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
            if(item is ItemWithType Job)
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
                if (typeof(T) == typeof(ItemWithType))
                {
                    List<ItemWithType> finishItemsWithType = _finishItems.Cast<ItemWithType>().ToList();

                    if (finishItemsWithType.Count == 0)
                        return;

                    int countTypes = finishItemsWithType.Max(item => item.Type);
                    int[] countsTypeItems = new int[countTypes];
                    foreach (var item in finishItemsWithType)
                    {
                        countsTypeItems[item.Type - 1]++;
                    }
                    for (int i = 0; i < countsTypeItems.Length; i++)
                    {
                        Console.WriteLine($"\t\t\t{i + 1}: {countsTypeItems[i]}");
                    }

                    double averageTime = 0;
                    foreach (var item in finishItemsWithType)
                    {
                        averageTime += item.FinishTime - item.StartTime;
                    }
                    averageTime /= finishItemsWithType.Count();
                    Console.WriteLine($"\t\tAverage time complite work: {averageTime}");

                    double averageTimeAwait = 0;
                    foreach (var item in finishItemsWithType)
                    {
                        averageTimeAwait += item.TimeAwait;
                    }
                    averageTimeAwait /= finishItemsWithType.Count();
                    Console.WriteLine($"\t\tAverage time await: {averageTimeAwait}");

                    /*
                    Console.WriteLine("\t\tType   StartTime   FinishTime");
                    foreach (var item in _finishItems)
                    {
                        item.PrintStats();
                    }
                    */
                }
            }
        }
    }
}
