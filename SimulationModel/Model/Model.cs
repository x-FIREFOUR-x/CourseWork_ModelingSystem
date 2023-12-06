using System.Collections.Generic;
using System.Linq;
using System;

using SimulationModel.Model.Elements;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model
{
    public class Model<T> where T: DefaultQueueItem
    {
        private double _currentTime;

        private readonly List<Element<T>> _elements;

        private Action<List<Element<T>>> _additionalAction;

        public Model(List<Element<T>> elements, Action<List<Element<T>>> additionalAction = null)
        {
            _elements = elements;
            _currentTime = 0;

            _additionalAction = additionalAction;
        }

        public void Simulation(double simulationTime, bool stepsStats = false)
        {
            while (_currentTime < simulationTime)
            {
                Element<T> nextElement = _elements.OrderBy(item => item.NextTime()).First();
                _currentTime = nextElement.NextTime();

                foreach (var element in _elements)
                    element.UpdatedCurrentTime(_currentTime);

                Console.WriteLine();
                foreach (var element in _elements)
                {
                    if (element.TryFinish())
                    {
                        element.FinishService();
                    }
                    
                }

                if(_additionalAction != null)
                {
                    _additionalAction(_elements);
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

            Console.WriteLine("\n========================== Finish Stats ===============================");
            foreach (var element in _elements)
            {
                element.PrintStats(true);
            }
            Console.WriteLine("========================================================================");
        }
    }
}
