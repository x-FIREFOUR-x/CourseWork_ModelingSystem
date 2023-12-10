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

            Dictionary<String, double> stats = GetStatistics();

            Console.WriteLine($"\t\t{StatName.Working}: {stats[StatName.Working]}");

            Console.WriteLine($"\t\t{StatName.QueueSize}: {stats[StatName.QueueSize]}");
            Console.WriteLine($"\t\t{StatName.Failures}: {stats[StatName.Failures]}");
            Console.WriteLine($"\t\t{StatName.Processed}: {stats[StatName.Processed]}");

            if (finalStats)
            {
                Console.WriteLine($"\t\t{StatName.AverageQueueSize}: {stats[StatName.AverageQueueSize]}");
                Console.WriteLine($"\t\t{StatName.FailureProbability}: {stats[StatName.FailureProbability]}");
                Console.WriteLine($"\t\t{StatName.AverageWorkload}: {stats[StatName.AverageWorkload]}");
            }
        }

        public override Dictionary<String, double> GetStatistics()
        {
            Dictionary<String, double> stats = new Dictionary<String, double>();

            int workingSubProcessors = 0;
            foreach (var processor in _processors)
            {
                if (processor.Processing)
                    workingSubProcessors += 1;
            }
            stats[StatName.Working] = workingSubProcessors;

            stats[StatName.QueueSize] = _queue.GetSize();
            stats[StatName.Failures] = _countFailures;
            stats[StatName.Processed] = _countProcessed;

            stats[StatName.AverageQueueSize] = _averageQueueDividend / _currentTime;
            stats[StatName.FailureProbability] = (float)_countFailures / (_countFailures + _countProcessed);
            stats[StatName.AverageWorkload] = _timeWorking / _currentTime;

            return stats;
        }
    }
}
