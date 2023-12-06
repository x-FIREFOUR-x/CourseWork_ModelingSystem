using System;
using System.Collections.Generic;

using SimulationModel.Model.Queue;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Elements
{
    class Process<T>: Element<T> where T : DefaultQueueItem
    {
        private ProcessQueue<T> _queue;

        private int _countFailures;

        private double _averageQueueDividend;

        private List<Element<T>> _processors;

        private List<double> _timesIncome;
        public bool PrintTimesIncome { get; set; }

        public int QueueSize { get => _queue.Size; }
        public int QueueMaxSize { get => _queue.MaxSize; }

        public T GetItemWithQueue() { return _queue.GetItem(); }
        public void PutItemToQueue(T item) { _queue.PutItem(item); }

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

        public Process(string name, int maxQueueSize, List<Element<T>> processors, int startQueueSize = 0)
            : base(name)
        {
            _processors = processors;
            SetNextTime(Double.PositiveInfinity);

            _timesIncome = new List<double>();
            PrintTimesIncome = false;

            if (startQueueSize == 0)
            {
                _queue = new ProcessQueue<T>(maxQueueSize);
            }
            else
            {
                _queue = new ProcessQueue<T>(maxQueueSize, startQueueSize);
            }
        }

        public override void StartService(T item)
        {
            _timesIncome.Add(_currentTime);

            item.StartAwait(_currentTime);

            Console.Write(Name);
            if (TryStartService(item))
            {
                return;
            }
            
            if (QueueSize < QueueMaxSize)
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
            _averageQueueDividend += (currentTime - _currentTime) * _queue.Size;
            base.UpdatedCurrentTime(currentTime);

            foreach (var processor in _processors)
            {
                processor.UpdatedCurrentTime(currentTime);
            }
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

            Console.WriteLine($"\t\tQueue size: {_queue.Size}");
            Console.WriteLine($"\t\tFailures: {_countFailures}");
            Console.WriteLine($"\t\tProcessed items: {_countProcessed}");

            if (finalStats)
            {
                Console.WriteLine($"\t\tAverage queue size: {_averageQueueDividend / _currentTime}");
                Console.WriteLine($"\t\tFailure probability: {(float)_countFailures / (_countFailures + _countProcessed)}");
                Console.WriteLine($"\t\tAverage workload: {_timeWorking / _currentTime}");

                if(PrintTimesIncome)
                {
                    Console.Write("\t\tIntervals between incomes: ");
                    for (int i = 1; i < _timesIncome.Count; i++)
                    {
                        Console.Write($"{Math.Round(_timesIncome[i] - _timesIncome[i-1], 2)} ");
                    }
                    Console.WriteLine();
                }
                
            }
        }

        private bool TryStartService(T item)
        {
            foreach(var processor in _processors)
            {
                if(!processor.Processing)
                {
                    base.StartService(item);
                    processor.StartService(item);

                    return true;
                }
            }

            return false;
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
    }
}
