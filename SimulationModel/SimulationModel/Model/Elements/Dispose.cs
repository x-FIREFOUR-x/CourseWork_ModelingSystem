using System;
using System.Collections.Generic;
using System.Linq;

using SimulationModel.Model.Item;

namespace SimulationModel.Model.Elements
{
    public class Dispose<T> : Element<T> where T : DefaultQueueItem
    {
        private List<T> _finishItems;

        public Dispose(string name, bool isDebug = true, double timeStartGetStats = 0)
            :base(name, isDebug, timeStartGetStats)
        {
            _currentTime = 0;
            SetNextTime(Double.PositiveInfinity);

            _finishItems = new List<T>();
        }

        public override void StartService(T item) 
        {
            base.FinishService();

            item.Finish(_currentTime);
            _finishItems.Add(item);
        }

        public override void FinishService() => throw new InvalidOperationException();

        public override void PrintStats(bool finalStats)
        {
            base.PrintStats(finalStats);

            Dictionary<String, double> stats = GetStatistics();

            Console.WriteLine($"\t\t{StatName.FinishedItems}: {stats[StatName.FinishedItems]}");

            if (typeof(T) == typeof(ItemWithType))
            {
                List<ItemWithType> finishItemsWithType = _finishItems.Cast<ItemWithType>().ToList();

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
            }

            Console.WriteLine($"\t\t{StatName.AverageTimeComplite}: {stats[StatName.AverageTimeComplite]}");
            Console.WriteLine($"\t\t{StatName.AverageTimeAwait}: {stats[StatName.AverageTimeAwait]}");
        }

        public override Dictionary<String, double> GetStatistics()
        {
            Dictionary<String, double> stats = new Dictionary<String, double>();

            stats[StatName.FinishedItems] = _countProcessed;

            var finishItemsInTimeStartGetStats = _finishItems.Where(a => a.StartTime > _timeStartGetStats).ToList();

            double averageTime = 0;
            foreach (var item in finishItemsInTimeStartGetStats)
            {
                averageTime += item.FinishTime - item.StartTime;
            }
            averageTime /= finishItemsInTimeStartGetStats.Count();
            stats[StatName.AverageTimeComplite] = averageTime;

            double averageTimeAwait = 0;
            foreach (var item in finishItemsInTimeStartGetStats)
            {
                averageTimeAwait += item.TimeAwait;
            }
            averageTimeAwait /= finishItemsInTimeStartGetStats.Count();
            stats[StatName.AverageTimeAwait] = averageTimeAwait;

            return stats;
        }
    }
}
