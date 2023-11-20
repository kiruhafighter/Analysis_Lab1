using Analysis_Lab1.Entities;

namespace Analysis_Lab1.Utilities
{
    internal static class VisualizeData
    {
        internal static void ShowVariationSeriesInConsole(this List<DataPoint> dataPoints)
        {
            Console.WriteLine("Number\tValue\t\tFrequency\tRelative Frequency\tEmpirical Distribution");

            foreach (var dataPoint in dataPoints)
            {
                Console.WriteLine($"{dataPoint.Id}\t{dataPoint.Value}    \t  {dataPoint.Frequency}   \t\t\t{dataPoint.RelativeFrequency:P2}\t\t {dataPoint.EmpiricalDistribution:P2}");
            }
        }
    }
}