using System.Collections.Generic;
using System;

using SimulationModel.Model;
using SimulationModel.Model.DelayGenerator;
using SimulationModel.Model.Elements;
using SimulationModel.Model.NextElementSelector;
using SimulationModel.Model.Queue.Item;

namespace SimulationModel
{ 
    class Program
    {
        public static void Main(string[] args)
        {
            Model<Job> modelHospital = CreateHospitalModel();
            modelHospital.Simulation(1000, false);
        }
        
        
        private static Model<Job> CreateHospitalModel()
        {
            Process<Job> receptionDepartment = new Process<Job>("ReceptionDepartment", 100, new List<Element<Job>> {
                new SimpleProcessor<Job>("Doctor1", new List<IDelayGenerator>{
                                                            new ExponentialDelayGenerator(15),
                                                            new ExponentialDelayGenerator(40),
                                                            new ExponentialDelayGenerator(30)}),
                new SimpleProcessor<Job>("Doctor2", new List<IDelayGenerator>{
                                                            new ExponentialDelayGenerator(15),
                                                            new ExponentialDelayGenerator(40),
                                                            new ExponentialDelayGenerator(30)}),
            });

            Process<Job> wards = new Process<Job>("Wards", 100, new List<Element<Job>> {
                new SimpleProcessor<Job>("Accompanying1", new UniformDelayGenerator(3, 8)),
                new SimpleProcessor<Job>("Accompanying2", new UniformDelayGenerator(3, 8)),
                new SimpleProcessor<Job>("Accompanying3", new UniformDelayGenerator(3, 8)),
            });

            Process<Job> pathToLab = new Process<Job>("PathToLab", 0, new List<Element<Job>> {
                new SimpleProcessor<Job>("Go1", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go2", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go3", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go4", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go5", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go6", new UniformDelayGenerator(2, 5)),
                new SimpleProcessor<Job>("Go7", new UniformDelayGenerator(2, 5)),
            });

            Process<Job> registryLab = new Process<Job>("RegistryLab", 100, new List<Element<Job>> {
                new SimpleProcessor<Job>("register", new ErlangDelayGenerator(4.5, 3)),
            });


            Action<Job> ActionChangeTypePatientAfterLab = (item) =>
            {
                if (item.Type == 2)
                    item.Type = 1;
            };
            Process<Job> lab = new Process<Job>("Laboratory", 100, new List<Element<Job>> {
                new SimpleProcessor<Job>("lab1", new ErlangDelayGenerator(4, 2), ActionChangeTypePatientAfterLab),
                new SimpleProcessor<Job>("lab2", new ErlangDelayGenerator(4, 2), ActionChangeTypePatientAfterLab)
            });
            lab.PrintTimesIncome = true;

            Dispose<Job> dispose = new Dispose<Job>("Exit");

            receptionDepartment.NextElementSelector = new NextElementItemTypeSelector<Job>(
                new List<(Element<Job>, double)> { (wards, 1), (pathToLab, 2), (pathToLab, 3) });

            wards.NextElementSelector = new NextElementProbabilitySelector<Job> (
                new List<(Element<Job>, double)> { (dispose, 1), });

            pathToLab.NextElementSelector = new NextElementProbabilitySelector<Job>(
                new List<(Element<Job>, double)> { (registryLab, 1.0) });

            registryLab.NextElementSelector = new NextElementProbabilitySelector<Job>(
                new List<(Element<Job>, double)> { (lab, 1.0) });

            lab.NextElementSelector = new NextElementItemTypeSelector<Job>(
                new List<(Element<Job>, double)> { (receptionDepartment, 1), (dispose, 3) });

            Create<Job> create = new Create<Job>("Create", new ExponentialDelayGenerator(15));
            create.NextElementSelector = new NextElementPrioritySelector<Job>(new List<(Element<Job>, double)>{(receptionDepartment, 1)});

            List <Element<Job>> elements = new();
            elements.Add(create);
            elements.Add(receptionDepartment);
            elements.Add(wards);
            elements.Add(pathToLab);
            elements.Add(registryLab);
            elements.Add(lab);
            elements.Add(dispose);
            
            return new Model<Job>(elements);
        }
        
    }
}
