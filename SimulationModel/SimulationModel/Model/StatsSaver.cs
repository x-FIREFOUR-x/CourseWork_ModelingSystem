using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using System.Linq;

namespace SimulationModel.Model
{
    public class StatsSaver
    {
        private static string _folderPath = "D:\\Project\\7Semestr\\CourseWork\\SimulationModel\\SimulationModel\\SaveStats\\";

        public static void SaveToCsv(Dictionary<String, List<double>> data, string fileName, double startX, double stepX)
        {
            double currentX = startX;

            StringBuilder csvContent = new StringBuilder();

            csvContent.AppendLine(" ;" + string.Join(";", data.Keys));

            for (int row = 0; row < data.Values.First().Count; row++)
            {
                csvContent.Append(currentX + ";");
                currentX += stepX;
                foreach (var key in data.Keys)
                {
                    
                    string value = data[key][row].ToString();
                    csvContent.Append(value);

                    
                    if (key != data.Keys.Last())
                    {
                        csvContent.Append(";");
                    }
                }

                csvContent.AppendLine();
            }

            File.WriteAllText(_folderPath + fileName, csvContent.ToString());
        }
    }
}
