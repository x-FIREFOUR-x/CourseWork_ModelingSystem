using System;

namespace SimulationModel.Model.Item
{
    public class ItemWithType : DefaultQueueItem
    {
        public int Type { get; private set; }

        public double StartTime { get; private set; }
        public double FinishTime { get; private set; }

        public ItemWithType(double startTime, int type)
        {
            StartTime = startTime;
            FinishTime = double.NaN;

            Type = type;
        }

        public override int GetIndexGenerator()
        {
            return Type - 1;
        }

        public void Finish(double time)
        {
            FinishTime = time;
        }

        public override void PrintStats()
        {
            Console.WriteLine($"\t\t\t{Type}       {Math.Round(StartTime, 2)}      {Math.Round(FinishTime, 2)}");
        }
    }
}
