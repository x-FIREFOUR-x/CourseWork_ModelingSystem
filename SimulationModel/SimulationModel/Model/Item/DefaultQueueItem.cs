namespace SimulationModel.Model.Item
{
    public class DefaultQueueItem
    {
        private double _timeStartAwait;
        public double TimeAwait { get; private set; }


        public virtual int GetIndexGenerator() 
        {
            return 0;
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
