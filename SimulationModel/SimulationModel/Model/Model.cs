using System.Collections.Generic;
using System.Linq;
using System;

using SimulationModel.Model.Elements;
using SimulationModel.Model.Item;

namespace SimulationModel.Model
{
    public class Model<T> where T : DefaultQueueItem
    {
        private double _currentTime;

        private readonly List<Element<T>> _elements;

        public Model(List<Element<T>> elements)
        {
            _elements = elements;
            _currentTime = 0;
        }

        public void Simulation(double simulationTime, bool finalStats = false, bool stepsStats = false)
        {
            while (_currentTime < simulationTime)
            {
                Element<T> nextElement = _elements.OrderBy(item => item.NextTime()).First();
                _currentTime = nextElement.NextTime();

                foreach (var element in _elements)
                    element.UpdatedCurrentTime(_currentTime);

                if (IsDebug())
                    Console.WriteLine();

                foreach (var element in _elements)
                {
                    if (element.TryFinish())
                    {
                        element.FinishService();
                    }

                }

                if (stepsStats)
                {
                    Console.WriteLine("\n--------------------------- Current Stats -----------------------------");
                    foreach (var element in _elements)
                    {
                        element.PrintStats(false);
                    }
                    Console.WriteLine("-----------------------------------------------------------------------");
                }
            }

            if (finalStats)
            {
                Console.WriteLine("\n========================== Finish Stats ===============================");
                foreach (var element in _elements)
                {
                    element.PrintStats(true);
                }
                Console.WriteLine("========================================================================");
            }

        }

        public Dictionary<String, List<double>> Simulation(double simulationTime, double StepForSaveStats)
        {
            foreach (var element in _elements)
            {
                element.IsDebug = false;

                if (element is Process<T> process)
                {
                    process.setIsDebugForSubProcess(false);
                }
            }

            Dictionary<String, List<double>> stepStats = InitialiseStepStats();
            Dictionary<string, double> currentStats = GetCurrentStats();
            double nextTimeForSaveStats = 0;

            while (_currentTime < simulationTime)
            {
                Element<T> nextElement = _elements.OrderBy(item => item.NextTime()).First();
                _currentTime = nextElement.NextTime();

                foreach (var element in _elements)
                    element.UpdatedCurrentTime(_currentTime);

                foreach (var element in _elements)
                {
                    if (element.TryFinish())
                    {
                        element.FinishService();
                    }

                }

                if (_currentTime > nextTimeForSaveStats)
                {
                    SaveStats(stepStats, currentStats);
                    nextTimeForSaveStats += StepForSaveStats;
                }

                currentStats = GetCurrentStats();

            }

            return stepStats;
        }

        public List<Element<T>> GetElements() { return _elements; }

        private bool IsDebug()
        {
            foreach (var element in _elements)
            {
                if (element.IsDebug)
                    return true;
            }
            return false;
        }

        private Dictionary<String, List<double>> InitialiseStepStats()
        {
            Dictionary<String, List<double>> stepStats = new Dictionary<string, List<double>>();

            for (int j = 1; j < GetElements().Count - 1; j++)
            {
                stepStats[StatName.AverageWorkload + j.ToString()] = new List<double>();
            }
            stepStats[StatName.AverageTimeComplite] = new List<double>();
            stepStats[StatName.AverageTimeAwait] = new List<double>();

            return stepStats;
        }

        private void SaveStats(Dictionary<String, List<double>> stats, Dictionary<string, double> currentStats)
        {
            for (int j = 1; j < GetElements().Count - 1; j++)
            {
                stats[StatName.AverageWorkload + j.ToString()].Add(currentStats[StatName.AverageWorkload + j.ToString()]);
            }
            stats[StatName.AverageTimeComplite].Add(currentStats[StatName.AverageTimeComplite]);
            stats[StatName.AverageTimeAwait].Add(currentStats[StatName.AverageTimeAwait]);
        }

        private Dictionary<string, double> GetCurrentStats()
        {
            Dictionary<string, double> currentStats = new();

            for (int j = 1; j < GetElements().Count - 1; j++)
            {
                currentStats[StatName.AverageWorkload + j.ToString()] = GetElements()[j].GetStatistics()[StatName.AverageWorkload];
            }
            currentStats[StatName.AverageTimeComplite] = GetElements()[6].GetStatistics()[StatName.AverageTimeComplite];
            currentStats[StatName.AverageTimeAwait] = GetElements()[6].GetStatistics()[StatName.AverageTimeAwait];

            return currentStats;
        }
    }
}
