using System.Collections.Generic;

using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Queue
{
    public class ProcessQueue<T> where T: DefaultQueueItem
    {
        public readonly int MaxSize;

        private List<T> _items;

        public int Size { get => _items.Count; }

        public ProcessQueue(int maxSize)
        {
            MaxSize = maxSize;

            _items = new List<T>();
        }

        public ProcessQueue(int maxSize, int startSize)
        {
            MaxSize = maxSize;
            _items = new List<T>();

            for (int i = 0; i < startSize; i++)
            {
                _items.Add(null);
            }
        }

        public T GetItem()
        {
            T item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public bool PutItem(T item)
        {
            if (_items.Count == MaxSize)
                return false;

            _items.Add(item);
            return true;
        }
    }
}
