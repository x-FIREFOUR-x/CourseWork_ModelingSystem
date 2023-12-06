using System;
using System.Collections.Generic;

using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Queue.ItemFactory;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Elements
{
    public class Create<T> : Element<T> where T : DefaultQueueItem
    {
        private ItemFactory<T> _itemFactory;

        private List<T> _createdItems;

        public Create(string name, IDelayGenerator delayGenerator, ItemFactory<T> itemFactory = null)
           : base(name, delayGenerator)
        {
            Processing = true;
            _currentTime = 0;
            SetNextTime(_currentTime + _delayGenerators[0].GetDelay());

            _itemFactory = itemFactory ?? new ItemFactory<T>();
            _createdItems = new();
        }

        public override void StartService(T item) => throw new NotSupportedException();

        public override void FinishService()
        {
            base.FinishService();

            Console.WriteLine($"{Name}: finish, time: {_currentTime}");

            SetNextTime(_currentTime + _delayGenerators[0].GetDelay());

            T item = _itemFactory.CreateItem(_currentTime);
            _createdItems.Add(item);

            Element<T> nextElement = NextElementSelector.GetNextElement(item);
            nextElement?.StartService(_itemFactory.CreateItem(_currentTime));
        }

        public override void PrintStats(bool finalStats)
        {
            base.PrintStats(finalStats);
            Console.WriteLine($"\t\tCreated items: {_countProcessed}");
        }
    }
}
