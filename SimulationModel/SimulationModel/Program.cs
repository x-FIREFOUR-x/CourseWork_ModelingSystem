using System.Collections.Generic;

using SimulationModel.Model;
using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Elements;
using SimulationModel.Model.NextElementSelector;
using SimulationModel.Model.Item;
using SimulationModel.Model.Item.ItemFactory;
using SimulationModel.Model.Queue;

namespace SimulationModel
{ 
    class Program
    {
        public static void Main(string[] args)
        {
            Model<ItemWithType> model = CreateModel();
            model.Simulation(100);
        }

        private static Model<ItemWithType> CreateModel()
        {
            Create<ItemWithType> create = new Create<ItemWithType>(
                "Create",
                new ExponentialDelayGenerator(0.25),
                new ItemWithTypeFactory(new List<double>{0.3, 0.5, 0.2}));

            Process<ItemWithType> workPlace1 = new Process<ItemWithType>("AutomatedWorkPlace1", new InfinityProcessQueue<ItemWithType>(), 
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.6, 2), new ErlangDelayGenerator(0.8, 2), new ErlangDelayGenerator(0.7, 2)}),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.6, 2), new ErlangDelayGenerator(0.8, 2), new ErlangDelayGenerator(0.7, 2)}),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.6, 2), new ErlangDelayGenerator(0.8, 2), new ErlangDelayGenerator(0.7, 2)}),
                });

            Process<ItemWithType> workPlace2 = new Process<ItemWithType>("AutomatedWorkPlace2", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.85, 2), null, new ErlangDelayGenerator(1.24, 2)}),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.85, 2), null, new ErlangDelayGenerator(1.24, 2)})
                });

            Process<ItemWithType> workPlace3 = new Process<ItemWithType>("AutomatedWorkPlace3", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.5, 2), new ErlangDelayGenerator(0.75, 2), new ErlangDelayGenerator(1.0, 2)}),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.5, 2), new ErlangDelayGenerator(0.75, 2), new ErlangDelayGenerator(1.0, 2)}),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.5, 2), new ErlangDelayGenerator(0.75, 2), new ErlangDelayGenerator(1.0, 2)}),
                    new SimpleProcessor<ItemWithType>("machine4", new List<IDelayGenerator>{ new ErlangDelayGenerator(0.5, 2), new ErlangDelayGenerator(0.75, 2), new ErlangDelayGenerator(1.0, 2)})
                });

            Process<ItemWithType> workPlace4 = new Process<ItemWithType>("AutomatedWorkPlace4", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(1.1, 2), new ErlangDelayGenerator(0.9, 2)}),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(1.1, 2), new ErlangDelayGenerator(0.9, 2)}),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(1.1, 2), new ErlangDelayGenerator(0.9, 2)})
                });

            Process<ItemWithType> workPlace5 = new Process<ItemWithType>("AutomatedWorkPlace5", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{new ErlangDelayGenerator(0.5, 2), null, new ErlangDelayGenerator(0.25, 2)})
                });

            Dispose<ItemWithType> dispose = new Dispose<ItemWithType>("Dispose");

            
            create.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (workPlace3, 1), (workPlace4, 2), (workPlace2, 3) });

            workPlace1.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (workPlace2, 1), (workPlace3, 2), (workPlace4, 3) });

            workPlace2.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (workPlace5, 1), (null, 2), (workPlace5, 3) });

            workPlace3.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (workPlace1, 1), (dispose, 2), (dispose, 3) });

            workPlace4.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (null, 1), (workPlace1, 2), (workPlace3, 3) });

            workPlace5.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (dispose, 1), (null, 2), (workPlace1, 3) });


            List<Element<ItemWithType>> elements = new();
            elements.Add(create);
            elements.Add(workPlace1);
            elements.Add(workPlace2);
            elements.Add(workPlace3);
            elements.Add(workPlace4);
            elements.Add(workPlace5);
            elements.Add(dispose);
            return new Model<ItemWithType>(elements);
        }
    }
}
