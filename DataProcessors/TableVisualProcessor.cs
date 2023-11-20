using Analysis_Lab1.Entities;
using MathNet.Numerics.Statistics;

namespace Analysis_Lab1.DataProcessors
{
    public static class TableVisualProcessor
    {
        public static void GenerateVariationSeries(List<double> data)
        {
            int i = 0;

            var variationSeries = data
                .GroupBy(x => x)
                .OrderBy(x => x.Key)
                .Select(group => new DataPoint
                {
                    Id = ++i,
                    Value = group.Key,
                    Frequency = group.Count(),
                    RelativeFrequency = (double)group.Count() / data.Count(),
                    EmpiricalDistribution = Statistics.EmpiricalCDF(data, group.Key)
                }).ToList();

            Console.WriteLine("Number\tValue\t\tFrequensy\tRelative Frequency\tEmpirical Distribution");

            foreach (var entry in variationSeries)
            {
                Console.WriteLine($"{entry.Id}\t{entry.Value} \t  {entry.Frequency}\t\t\t{entry.RelativeFrequency}\t\t{entry.EmpiricalDistribution:P2}");
            }
        }
    }
}
