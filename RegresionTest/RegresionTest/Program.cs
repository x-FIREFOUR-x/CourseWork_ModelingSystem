using System;
using System.Collections.Generic;
using System.Linq;

namespace RegresionTest
{
    class Program
    {
        private static List<List<double>> table1 = new List<List<double>> 
        { 
            new List<double> { 0.5, 0.1, 1.1},
            new List<double> { 0.5, 0.3, 1.3},
            new List<double> { 0.5, 0.2, 1.2}
        };

        private static List<List<double>> table2 = new List<List<double>>
        {
            new List<double> { 2, 2, 2, 2, 2, 2, 2, 2},
            new List<double> { 2, 1, 2, 2, 1, 1, 2, 1},
            new List<double> { 2, 2, 1, 2, 1, 2, 1, 1},
            new List<double> { 2, 1, 1, 2, 2, 1, 1, 2},
            new List<double> { 2, 2, 2, 1, 2, 1, 1, 1},
            new List<double> { 2, 1, 2, 1, 1, 2, 1, 2},
            new List<double> { 2, 2, 1, 1, 1, 1, 2, 1},
            new List<double> { 2, 1, 1, 1, 2, 2, 2, 2}
        };

        private static List<double> y = new List<double> { 457.03, 369.5, 365.92, 272.32, 424.03, 312.01, 311.23, 271.18 };

        static void Main(string[] args)
        {
            for (int i = 0; i < table2[0].Count(); i++)
            {
                double sum = 0;
                for (int k = 0; k < table2.Count(); k++)
                {
                    int q = table2[k][i] == 2 ? 1 : -1;
                    sum += q * y[k];
                }
                Console.WriteLine(sum/8);
            }

            Console.WriteLine();

            List<double> tyt = new List<double>();
            tyt.Add(457.03 + 369.5 + 365.92 + 272.32 + 424.03 + 312.01 + 311.23 + 271.18);
            tyt.Add(457.03 - 369.5 + 365.92 - 272.32 + 424.03 - 312.01 + 311.23 - 271.18);
            tyt.Add(457.03 + 369.5 - 365.92 - 272.32 + 424.03 + 312.01 - 311.23 - 271.18);
            tyt.Add(457.03 + 369.5 + 365.92 + 272.32 - 424.03 - 312.01 - 311.23 - 271.18);
            tyt.Add(457.03 - 369.5 - 365.92 + 272.32 + 424.03 - 312.01 - 311.23 + 271.18);
            tyt.Add(457.03 - 369.5 + 365.92 - 272.32 - 424.03 + 312.01 - 311.23 + 271.18);
            tyt.Add(457.03 + 369.5 - 365.92 - 272.32 - 424.03 - 312.01 + 311.23 + 271.18);
            tyt.Add(457.03 + 369.5 + 365.92 + 272.32 + 424.03 + 312.01 + 311.23 + 271.18);

            foreach (var sum in tyt)
            {
                Console.WriteLine(sum / 8);
            }
        }

        
    }
}
