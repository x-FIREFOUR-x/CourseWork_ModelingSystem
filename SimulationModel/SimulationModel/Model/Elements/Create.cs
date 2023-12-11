using System;
using System.Collections.Generic;

using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Item;
using SimulationModel.Model.Item.ItemFactory;

namespace SimulationModel.Model.Elements
{
    public class Create<T> : Element<T> where T : DefaultQueueItem
    {
        private ItemFactory<T> _itemFactory;

        private List<T> _createdItems;

        public Create(string name, IDelayGenerator delayGenerator, ItemFactory<T> itemFactory = null, bool isDebug = true, double timeStartGetStats = 0)
           : base(name, delayGenerator, isDebug, timeStartGetStats)
        {
            Processing = true;
            _currentTime = 0;
            SetNextTime(_currentTime + _delayGenerators[0].GetDelay());

            _itemFactory = itemFactory ?? new ItemFactory<T>();
            _createdItems = new();
        }

        public override void StartService(T item) => throw new InvalidOperationException();

        public override void FinishService()
        {
            base.FinishService();

            if (IsDebug)
                Console.WriteLine($"{Name}: finish, time: {_currentTime}");

            SetNextTime(_currentTime + _delayGenerators[0].GetDelay());

            T item = _itemFactory.CreateItem(_currentTime);
            _createdItems.Add(item);

            Element<T> nextElement = NextElementSelector.GetNextElement(item);
            nextElement?.StartService(item);
        }

        public override void PrintStats(bool finalStats)
        {
            base.PrintStats(finalStats);

            Dictionary<String, double> stats = GetStatistics();
            Console.WriteLine($"\t\t{StatName.CreatedItems}: {stats[StatName.CreatedItems]}");
        }

        public override Dictionary<String, double> GetStatistics()
        {
            Dictionary<String, double> stats = new Dictionary<String, double>();

            stats[StatName.CreatedItems] = _countProcessed;

            return stats;
        }
    }
}
