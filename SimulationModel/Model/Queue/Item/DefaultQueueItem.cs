namespace SimulationModel.Model.Queue.Item
{
    public class DefaultQueueItem
    {
        public virtual int GetIndexGenerator() 
        {
            return 0;
        }

        public virtual void PrintStats() { }
    }
}
