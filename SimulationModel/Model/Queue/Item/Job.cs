using System;

namespace SimulationModel.Model.Queue.Item
{
    public class Job : DefaultQueueItem
    {
        public int Type { get; set; }
        public int StartTypeJob { get; private set; }

        private double _startTime;
        private double _finishTime;

        public Job(double startTime)
        {
            _startTime = startTime;
            _finishTime = double.NaN;

            Random rand = new Random();
            float numb = (float)rand.NextDouble();

            int type;
            if(numb <= 0.5)
            {
                type = 1;
            }
            else if (numb <= 0.6)
            {
                type = 2;
            }
            else
            {
                type = 3;
            }

            StartTypeJob = type;
            Type = type;
        }

        public override int GetIndexGenerator()
        {
            return Type - 1;
        }

        public void Finish(double time)
        {
            _finishTime = time;
        }

        public override void PrintStats()
        {
            Console.WriteLine($"\t\t\t{StartTypeJob}       {Math.Round(_startTime, 2)}      {Math.Round(_finishTime, 2)}");
        }
    }
}
