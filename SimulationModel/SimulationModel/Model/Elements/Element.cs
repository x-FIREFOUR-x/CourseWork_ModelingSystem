using System;
using System.Collections.Generic;

using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Item;

namespace SimulationModel.Model.Elements
{
    public abstract class Element<T> where T : DefaultQueueItem
    {
        public string Name { get; private set; }

        protected List<IDelayGenerator> _delayGenerators;
        public NextElementSelector.NextElementSelector<T> NextElementSelector { protected get; set; }

        private double _nextTime;
        protected double _currentTime;

        protected double _timeStartGetStats;

        protected int _countProcessed;
        protected double _timeWorking;


        public bool IsDebug { get; set; }

        public virtual bool Processing { get; set; }

        public virtual double CurrentTime => _currentTime;

        public virtual double NextTime() {return _nextTime;}
        
        public void SetNextTime(double nextTime) => _nextTime = nextTime;
        

        public Element(string name, IDelayGenerator delayGenerator, bool isDebug, double timeStartGetStats)
        {
            Name = name;
            _delayGenerators = new List<IDelayGenerator>();
            _delayGenerators.Add(delayGenerator);

            _currentTime = 0;

            IsDebug = isDebug;
            _timeStartGetStats = timeStartGetStats;
        }

        public Element(string name, List<IDelayGenerator> delayGenerators, bool isDebug, double timeStartGetStats)
        {
            Name = name;
            _delayGenerators = delayGenerators;

            _currentTime = 0;

            IsDebug = isDebug;
            _timeStartGetStats = timeStartGetStats;
        }

        public Element(string name, bool isDebug, double timeStartGetStats)
        {
            Name = name;
            _delayGenerators = null;

            _currentTime = 0;

            IsDebug = isDebug;
            _timeStartGetStats = timeStartGetStats;
        }

        public virtual void StartService(T item) { Processing = true; }

        public virtual void FinishService() 
        { 
            if (CurrentTime > _timeStartGetStats)
            {
                _countProcessed++;
            }
        }

        public virtual bool TryFinish() 
        {
            if (Math.Abs(_nextTime - _currentTime) < .0001f)
            {
                return true;
            }

            return false;
        }

        public virtual void UpdatedCurrentTime(double currentTime) 
        {
            if(currentTime > _timeStartGetStats && Processing)
            {
                _timeWorking += currentTime - _currentTime;
            }
            
            _currentTime = currentTime; 
        }

        public virtual void PrintStats(bool finalStats) {
            Console.WriteLine($"\t*{Name}");
        }

        public abstract Dictionary<String, double> GetStatistics();
    }
}
