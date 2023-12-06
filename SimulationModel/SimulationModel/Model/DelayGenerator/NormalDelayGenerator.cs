using System;

namespace SimulationModel.Model.DelayGenerator
{
    public class NormalDelayGenerator : IDelayGenerator
    {
        private double _averageDelay;
        private double _devDelay;

        private Random _random;

        public NormalDelayGenerator(double averageDelay, double devDelay)
        {
            if (devDelay == 0)
                throw new ArgumentException("devDelay == 0, it will become constant distribution");

            _averageDelay = averageDelay;
            _devDelay = devDelay;

            _random = new Random();
        }

        public double GetDelay()
        {
            float randNumber = (float)_random.NextDouble();
            return randNumber * _devDelay + _averageDelay;
        }
    }
}
