using System;
using System.Collections.Generic;
using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Elements
{
    public class SimpleProcessor<T> : Element<T> where T : DefaultQueueItem
    {
        public T ProcessingItem { get; private set; }

        private Action<T> _additionalAction;

        public SimpleProcessor(string name, IDelayGenerator delayGenerator, IDelayGenerator nextDelayGenerator = null)
            :base(name, delayGenerator)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);

            if (nextDelayGenerator != null)
            {
                Processing = true;
                SetNextTime(_delayGenerators[0].GetDelay());
                _delayGenerators[0] = nextDelayGenerator;
            }
        }

        public SimpleProcessor(string name, List<IDelayGenerator> delayGenerators, IDelayGenerator nextDelayGenerator = null)
            : base(name, delayGenerators)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);

            if (nextDelayGenerator != null)
            {
                Processing = true;
                SetNextTime(_delayGenerators[0].GetDelay());
                _delayGenerators[0] = nextDelayGenerator;
            }
        }

        public SimpleProcessor(string name, IDelayGenerator delayGenerator, Action<T> additionalAction)
            : base(name, delayGenerator)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);

            _additionalAction = additionalAction;
        }

        public SimpleProcessor(string name, List<IDelayGenerator> delayGenerators, Action<T> additionalAction)
            : base(name, delayGenerators)
        {
            Processing = false;
            SetNextTime(Double.PositiveInfinity);

            _additionalAction = additionalAction;
        }

        public override void StartService(T item)
        {
            ProcessingItem = item;

            Console.WriteLine($".{Name}: start service, time: {_currentTime}");
            Processing = true;

            int index = item != null ? item.GetIndexGenerator() : 0;
            index = index < _delayGenerators.Count ? index : 0;
            SetNextTime(_currentTime + _delayGenerators[index].GetDelay());
        }

        public override void FinishService()
        {
            base.FinishService();

            if (_additionalAction != null)
            {
                _additionalAction(ProcessingItem);
            }

            Processing = false;
            ProcessingItem = null;
        }

        public void SetStartConditions(IDelayGenerator newDelayGenerator)
        {
            Processing = true;
            SetNextTime(_delayGenerators[0].GetDelay());
            _delayGenerators[0] = newDelayGenerator;
        }
    }
}
