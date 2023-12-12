using System;
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
            //Model<ItemWithType> model = CreateModel();
            //model.Simulation(100, true);

            //runModel(100, 100);


            //TacticalPlanning(5, 5000, 5);

            runModel(80, 10000, 0);
            runModel(80, 10000, 4000);
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

        private static void runModel(int countRun, double timeSimulation, double startTimeGetStats = 0)
        {
            Dictionary<string, double> stats = new();
            stats[StatName.AverageWorkload + "1"] = 0;
            stats[StatName.AverageWorkload + "2"] = 0;
            stats[StatName.AverageWorkload + "3"] = 0;
            stats[StatName.AverageWorkload + "4"] = 0;
            stats[StatName.AverageWorkload + "5"] = 0;
            stats[StatName.AverageTimeComplite] = 0;
            stats[StatName.AverageTimeAwait] = 0;
            for (int i = 0; i < countRun + 1; i++)
            {
                Model<ItemWithType> model = CreateModelforVerification(
                    0.25,
                    new List<double> { 0.3, 0.5, 0.2 },
                    new List<double> { 0.6, 0.8, 0.7 },
                    new List<double> { 0.85, -1, 1.24 },
                    new List<double> { 0.5, 0.75, 1.0 },
                    new List<double> { -1, 1.1, 0.9 },
                    new List<double> { 0.5, -1, 0.25 },
                    startTimeGetStats);
                model.Simulation(timeSimulation, false);

                if (i == 0)
                    continue;

                Console.WriteLine(i);

                for (int j = 1; j < model.GetElements().Count - 1; j++)
                {
                    stats[StatName.AverageWorkload + j.ToString()] += model.GetElements()[j].GetStatistics()[StatName.AverageWorkload];
                }

                stats[StatName.AverageTimeComplite] += model.GetElements()[6].GetStatistics()[StatName.AverageTimeComplite];
                stats[StatName.AverageTimeAwait] += model.GetElements()[6].GetStatistics()[StatName.AverageTimeAwait];
            }

            foreach (var k in stats.Keys)
            {
               Console.WriteLine($"{k}: { Math.Round((stats[k] / countRun), 5)}");
            }
        }

        private static Model<ItemWithType> CreateModelforVerification(
            double avgCreate,
            List<double> chanseCreate,
            List<double> avgDelayProcess1,
            List<double> avgDelayProcess2,
            List<double> avgDelayProcess3,
            List<double> avgDelayProcess4,
            List<double> avgDelayProcess5,
            double startTimeGetStats
        )
        {
            Create<ItemWithType> create = new Create<ItemWithType>(
                "Create",
                new ExponentialDelayGenerator(avgCreate),
                new ItemWithTypeFactory(chanseCreate),
                false,
                startTimeGetStats);

            Process<ItemWithType> workPlace1 = new Process<ItemWithType>("AutomatedWorkPlace1", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess1[0], 2), new ErlangDelayGenerator(avgDelayProcess1[1], 2), new ErlangDelayGenerator(avgDelayProcess1[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess1[0], 2), new ErlangDelayGenerator(avgDelayProcess1[1], 2), new ErlangDelayGenerator(avgDelayProcess1[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess1[0], 2), new ErlangDelayGenerator(avgDelayProcess1[1], 2), new ErlangDelayGenerator(avgDelayProcess1[2], 2)}, false),
                },
                false,
                startTimeGetStats);

            Process<ItemWithType> workPlace2 = new Process<ItemWithType>("AutomatedWorkPlace2", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess2[0], 2), null, new ErlangDelayGenerator(avgDelayProcess2[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess2[0], 2), null, new ErlangDelayGenerator(avgDelayProcess2[2], 2)}, false),
                },
                false,
                startTimeGetStats);

            Process<ItemWithType> workPlace3 = new Process<ItemWithType>("AutomatedWorkPlace3", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess3[0], 2), new ErlangDelayGenerator(avgDelayProcess3[1], 2), new ErlangDelayGenerator(avgDelayProcess3[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess3[0], 2), new ErlangDelayGenerator(avgDelayProcess3[1], 2), new ErlangDelayGenerator(avgDelayProcess3[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess3[0], 2), new ErlangDelayGenerator(avgDelayProcess3[1], 2), new ErlangDelayGenerator(avgDelayProcess3[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine4", new List<IDelayGenerator>{ new ErlangDelayGenerator(avgDelayProcess3[0], 2), new ErlangDelayGenerator(avgDelayProcess3[1], 2), new ErlangDelayGenerator(avgDelayProcess3[2], 2)}, false),
                },
                false,
                startTimeGetStats);

            Process<ItemWithType> workPlace4 = new Process<ItemWithType>("AutomatedWorkPlace4", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(avgDelayProcess4[1], 2), new ErlangDelayGenerator(avgDelayProcess4[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine2", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(avgDelayProcess4[1], 2), new ErlangDelayGenerator(avgDelayProcess4[2], 2)}, false),
                    new SimpleProcessor<ItemWithType>("machine3", new List<IDelayGenerator>{ null, new ErlangDelayGenerator(avgDelayProcess4[1], 2), new ErlangDelayGenerator(avgDelayProcess4[2], 2)}, false),
                },
                false,
                startTimeGetStats);

            Process<ItemWithType> workPlace5 = new Process<ItemWithType>("AutomatedWorkPlace5", new InfinityProcessQueue<ItemWithType>(),
                new List<Element<ItemWithType>> {
                    new SimpleProcessor<ItemWithType>("machine1", new List<IDelayGenerator>{new ErlangDelayGenerator(avgDelayProcess5[0], 2), null, new ErlangDelayGenerator(avgDelayProcess5[2], 2)}, false)
                },
                false,
                startTimeGetStats);

            Dispose<ItemWithType> dispose = new Dispose<ItemWithType>("Dispose", false, startTimeGetStats);


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


        private static void TacticalPlanning(int countRepeat, int timeSimulation, int stepSimulation)
        {
            Model<ItemWithType> model = CreateModel();
            var avgStats = model.Simulation(timeSimulation, stepSimulation);

            Dictionary<string, Dictionary<string, List<double>>> data = new();
            

            for (int i = 0; i < countRepeat; i++)
            {
                Console.WriteLine(i + 1);

                model = CreateModel();
                var stats = model.Simulation(timeSimulation, stepSimulation);

                foreach (var key in stats.Keys)
                {
                    if (!data.ContainsKey(key))
                        data[key] = new();
                    
                    data[key][(i + 1).ToString() + key] = stats[key];
                }
            }

            foreach (var key in data.Keys)
            {
                StatsSaver.SaveToCsv(data[key], $"{key}.csv", 0, stepSimulation);
            }
            
        }
    }
}
