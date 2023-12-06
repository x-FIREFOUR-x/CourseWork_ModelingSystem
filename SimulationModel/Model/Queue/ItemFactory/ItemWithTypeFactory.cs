using System;
using System.Collections.Generic;

using SimulationModel.Model.Queue.Item;

namespace SimulationModel.Model.Queue.ItemFactory
{
    public class ItemWithTypeFactory : ItemFactory<ItemWithType>
    {
        private List<double> _chancesTypes;

        public ItemWithTypeFactory(List<double> chancesTypes)
        {
            _chancesTypes = chancesTypes;
        }

        public override ItemWithType CreateItem(double currentTime)
        {
            Random rand = new Random();
            float numb = (float)rand.NextDouble();

            int type = 1;
            double interval = 0.0;
            for (int i = 0; i < _chancesTypes.Count; i++)
            {
                if (numb >= interval && numb <= interval + _chancesTypes[i])
                {
                    type = i + 1;
                    break;
                }
                interval += _chancesTypes[i];
            }

            ItemWithType Job = new ItemWithType(currentTime, type);
            return Job;
        }
    }
}
