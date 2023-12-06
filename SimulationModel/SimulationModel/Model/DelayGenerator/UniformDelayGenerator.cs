using System;

namespace SimulationModel.Model.DelayGenerator
{
    class UniformDelayGenerator: IDelayGenerator
    {
        private double _min;
        private double _max;

        private Random _random;

        public UniformDelayGenerator(double min, double max)
        {
            if (max < min)
                throw new ArgumentException("min > max");

            _min = min;
            _max = max;

            _random = new Random();
        }

        public double GetDelay()
        {
            float randNumber = (float)_random.NextDouble();

            return _min + randNumber * (_max - _min);
        }
    }
}
