using System;
using System.Collections.Generic;

using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Item;

namespace SimulationModel.Model.Elements
{
    public class SimpleProcessor<T> : Element<T> where T : DefaultQueueItem
    {
        public T ProcessingItem { get; private set; }

        public SimpleProcessor(string name, IDelayGenerator delayGenerator)
            : base(name, delayGenerator)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);
        }

        public SimpleProcessor(string name, List<IDelayGenerator> delayGenerators)
            : base(name, delayGenerators)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);
        }

        public override void StartService(T item)
        {
            ProcessingItem = item;
            item.EndAwait(_currentTime);

            Console.WriteLine($".{Name}: start service, time: {_currentTime}");
            Processing = true;

            int index = item != null ? item.GetIndexGenerator() : 0;
            index = index < _delayGenerators.Count ? index : 0;
            SetNextTime(_currentTime + _delayGenerators[index].GetDelay());
        }

        public override void FinishService()
        {
            base.FinishService();

            Processing = false;
            ProcessingItem = null;
        }
    }
}
