namespace SimulationModel.Model.Item
{
    public class DefaultQueueItem
    {
        private double _timeStartAwait;
        public double TimeAwait { get; private set; }

        public double StartTime { get; protected set; }
        public double FinishTime { get; protected set; }

        public DefaultQueueItem(double startTime)
        {
            StartTime = startTime;
            FinishTime = double.NaN;
        }

        public virtual int GetIndexGenerator() 
        {
            return 0;
        }

        public void Finish(double time)
        {
            FinishTime = time;
        }

        public void StartAwait(double timeStartAwait)
        {
            _timeStartAwait = timeStartAwait;
        }

        public void EndAwait(double timeEndAwait)
        {
            TimeAwait += timeEndAwait - _timeStartAwait;
            _timeStartAwait = double.PositiveInfinity;
        }

        public virtual void PrintStats() { }
    }
}
