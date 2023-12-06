using System;

namespace SimulationModel.Model.Queue.Item
{
    public class ItemWithType : DefaultQueueItem
    {
        public int Type { get; set; }

        private double _startTime;
        private double _finishTime;

        public ItemWithType(double startTime, int type)
        {
            _startTime = startTime;
            _finishTime = double.NaN;

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
            Console.WriteLine($"\t\t\t{Type}       {Math.Round(_startTime, 2)}      {Math.Round(_finishTime, 2)}");
        }
    }
}
