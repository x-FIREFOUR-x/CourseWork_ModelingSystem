using System;

namespace SimulationModel.Model.Item
{
    public class ItemWithType : DefaultQueueItem
    {
        public int Type { get; private set; }

        public ItemWithType(double startTime, int type):
            base(startTime)
        {
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
