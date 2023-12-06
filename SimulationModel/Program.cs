using System.Collections.Generic;
using System;

using SimulationModel.Model;
using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Elements;
using SimulationModel.Model.NextElementSelector;
using SimulationModel.Model.Queue.Item;
using SimulationModel.Model.Queue.ItemFactory;

namespace SimulationModel
{ 
    class Program
    {
        public static void Main(string[] args)
        {
            Model<ItemWithType> modelHospital = CreateHospitalModel();
            modelHospital.Simulation(1000, false);
        }
        
        
        private static Model<ItemWithType> CreateHospitalModel()
        {
            Process<ItemWithType> receptionDepartment = new Process<ItemWithType>("ReceptionDepartment", 100, new List<Element<ItemWithType>> {
                new SimpleProcessor<ItemWithType>("Doctor1", new List<IDelayGenerator>{
                                                            new ExponentialDelayGenerator(15),
                                                            new ExponentialDelayGenerator(40),
                                                            new ExponentialDelayGenerator(30)}),
                new SimpleProcessor<ItemWithType>("Doctor2", new List<IDelayGenerator>{
                                                            new ExponentialDelayGenerator(15),
                                                            new ExponentialDelayGenerator(40),
                                                            new ExponentialDelayGenerator(30)}),
            });

            Process<ItemWithType> wards = new Process<ItemWithType>("Wards", 100, new List<Element<ItemWithType>> {
                new SimpleProcessor<ItemWithType>("Accompanying1", new UniformDelayGenerator(3, 8)),
                new SimpleProcessor<ItemWithType>("Accompanying2", new UniformDelayGenerator(3, 8)),
                new SimpleProcessor<ItemWithType>("Accompanying3", new UniformDelayGenerator(3, 8)),
            });

            Process<ItemWithType> pathToLab = new Process<ItemWithType>("PathToLab", 0, new List<Element<ItemWithType>> {
                new SimpleProcessor<ItemWithType>("Go1", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go2", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go3", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go4", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go5", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go6", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<ItemWithType>("Go7", new UniformDelayGenerator(2, 5)),
            });

            Process<ItemWithType> registryLab = new Process<ItemWithType>("RegistryLab", 100, new List<Element<ItemWithType>> {
                new SimpleProcessor<ItemWithType>("register", new ErlangDelayGenerator(4.5, 3)),
            });


            Action<ItemWithType> ActionChangeTypePatientAfterLab = (item) =>
            {
                if (item.Type == 2)
                    item.Type = 1;
            };
            Process<ItemWithType> lab = new Process<ItemWithType>("Laboratory", 100, new List<Element<ItemWithType>> {
                new SimpleProcessor<ItemWithType>("lab1", new ErlangDelayGenerator(4, 2), ActionChangeTypePatientAfterLab),
                new SimpleProcessor<ItemWithType>("lab2", new ErlangDelayGenerator(4, 2), ActionChangeTypePatientAfterLab)
            });
            lab.PrintTimesIncome = true;

            Dispose<ItemWithType> dispose = new Dispose<ItemWithType>("Exit");

            receptionDepartment.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (wards, 1), (pathToLab, 2), (pathToLab, 3) });

            wards.NextElementSelector = new NextElementProbabilitySelector<ItemWithType> (
                new List<(Element<ItemWithType>, double)> { (dispose, 1), });

            pathToLab.NextElementSelector = new NextElementProbabilitySelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (registryLab, 1.0) });

            registryLab.NextElementSelector = new NextElementProbabilitySelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (lab, 1.0) });

            lab.NextElementSelector = new NextElementItemTypeSelector<ItemWithType>(
                new List<(Element<ItemWithType>, double)> { (receptionDepartment, 1), (dispose, 3) });

            Create<ItemWithType> create = new Create<ItemWithType>("Create", new ExponentialDelayGenerator(15), new ItemWithTypeFactory(new List<double>{0.5, 0.1, 0.4}));
            create.NextElementSelector = new NextElementPrioritySelector<ItemWithType>(new List<(Element<ItemWithType>, double)>{(receptionDepartment, 1)});

            List <Element<ItemWithType>> elements = new();
            elements.Add(create);
            elements.Add(receptionDepartment);
            elements.Add(wards);
            elements.Add(pathToLab);
            elements.Add(registryLab);
            elements.Add(lab);
            elements.Add(dispose);
            
            return new Model<ItemWithType>(elements);
        }
        
    }
}
