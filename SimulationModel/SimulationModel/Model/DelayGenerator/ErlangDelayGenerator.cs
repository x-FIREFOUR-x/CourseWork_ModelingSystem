using System;

namespace SimulationModel.Model.DelayGenerator
{
    public class ErlangDelayGenerator: IDelayGenerator
    {
        private double _averageDelay;
        private double _k;

        private Random _random;

        public ErlangDelayGenerator(double averageDelay, int k)
        {
            _averageDelay = averageDelay;
            _k = k;

            _random = new Random();
        }

        public double GetDelay()
        {
            double erlangRandom = 0;
            for (int j = 0; j < _k; j++)
            {
                double u = _random.NextDouble();
                erlangRandom += -Math.Log(1 - u) * _averageDelay;
            }

            return erlangRandom;
        }
    }
}
