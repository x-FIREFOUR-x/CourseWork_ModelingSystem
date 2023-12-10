using System;
using System.Collections.Generic;

using SimulationModel.Model.Queue;
using SimulationModel.Model.Item;

namespace SimulationModel.Model.Elements
{
    class Process<T>: Element<T> where T : DefaultQueueItem
    {
        private IProcessQueue<T> _queue;
        private List<Element<T>> _processors;

        private int _countFailures;
        private double _averageQueueDividend;
        

        public int QueueSize { get => _queue.GetSize(); }

        public override bool Processing
        {
            get {
                bool isWorking = false;
                foreach (var processors in _processors)
                {
                    if (processors.Processing)
                    {
                        isWorking = true;
                        break;
                    }
                }
                base.Processing = isWorking;
                return base.Processing;
            }
            set => base.Processing = value;
        }
        public override double NextTime()
        {
            double time = Double.PositiveInfinity;
            foreach (var processor in _processors)
            {
                if (time > processor.NextTime())
                {
                    time = processor.NextTime();
                }
            }

            return time;
        }

        public Process(string name, IProcessQueue<T> queue, List<Element<T>> processors)
            : base(name)
        {
            _processors = processors;
            SetNextTime(Double.PositiveInfinity);

            _queue = queue;
        }

        public override void StartService(T item)
        {
            item.StartAwait(_currentTime);

            Console.Write(Name);
            if (TryStartService(item))
            {
                return;
            }
            
            if (_queue.CanPutItem())
            {
                Console.WriteLine($": add item in queue, time: {_currentTime}");

                _queue.PutItem(item);
                return;
            }

            Console.WriteLine($": failure, time: {_currentTime}");
            _countFailures++;
        }

        public override void FinishService()
        {
            base.FinishService();

            foreach (var finishProcessor in _processors)
            {
                if (Math.Abs(finishProcessor.NextTime() - finishProcessor.CurrentTime) < .0001f)
                {
                    T item = ((SimpleProcessor<T>)finishProcessor).ProcessingItem;

                    finishProcessor.FinishService();
                    Console.WriteLine($"{Name}.{finishProcessor.Name}: finish service, time: {_currentTime}");

                    Element<T> nextElement = NextElementSelector?.GetNextElement(item);
                    nextElement?.StartService(item);

                    if (QueueSize > 0)
                    {
                        Console.Write(Name);
                        item = _queue.GetItem();
                        finishProcessor.StartService(item);
                    }
                    else
                    {
                        finishProcessor.SetNextTime(Double.PositiveInfinity);
                    }
                }
            }
        }

        public override void UpdatedCurrentTime(double currentTime)
        {
            _averageQueueDividend += (currentTime - _currentTime) * _queue.GetSize();
            base.UpdatedCurrentTime(currentTime);

            foreach (var processor in _processors)
            {
                processor.UpdatedCurrentTime(currentTime);
            }
        }

        public override bool TryFinish()
        {
            foreach (var processor in _processors)
            {
                if (Math.Abs(processor.NextTime() - processor.CurrentTime) < .0001f)
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryStartService(T item)
        {
            foreach (var processor in _processors)
            {
                if (!processor.Processing)
                {
                    base.StartService(item);
                    processor.StartService(item);

                    return true;
                }
            }

            return false;
        }

        public override void PrintStats(bool finalStats)
        {
            base.PrintStats(finalStats);

            int workingSubProcessors = 0;
            foreach (var processor in _processors)
            {
                if (processor.Processing)
                    workingSubProcessors += 1;
            }
            Console.WriteLine($"\t\tWorking: {workingSubProcessors}");

            Console.WriteLine($"\t\tQueue size: {_queue.GetSize()}");
            Console.WriteLine($"\t\tFailures: {_countFailures}");
            Console.WriteLine($"\t\tProcessed items: {_countProcessed}");

            if (finalStats)
            {
                Console.WriteLine($"\t\tAverage queue size: {_averageQueueDividend / _currentTime}");
                Console.WriteLine($"\t\tFailure probability: {(float)_countFailures / (_countFailures + _countProcessed)}");
                Console.WriteLine($"\t\tAverage workload: {_timeWorking / _currentTime}");
            }
        }
    }
}
