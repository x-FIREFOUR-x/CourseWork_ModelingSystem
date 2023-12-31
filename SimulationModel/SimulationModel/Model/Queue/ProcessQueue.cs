﻿using System.Collections.Generic;

using SimulationModel.Model.Item;

namespace SimulationModel.Model.Queue
{
    public class ProcessQueue<T>: IProcessQueue<T>  where T: DefaultQueueItem
    {
        private readonly int _maxSize;

        private List<T> _items;

        public ProcessQueue(int maxSize)
        {
            _maxSize = maxSize;

            _items = new List<T>();
        }

        public int GetMaxSize()
        {
            return _maxSize;
        }

        public int GetSize()
        {
            return _items.Count;
        }

        public T GetItem()
        {
            T item = _items[0];
            _items.RemoveAt(0);
            return item;
        }

        public bool PutItem(T item)
        {
            if (GetSize() == _maxSize)
                return false;

            _items.Add(item);
            return true;
        }

        public bool CanPutItem()
        {
            return GetSize() < _maxSize;
        }
    }
}
