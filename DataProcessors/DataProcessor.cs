using Analysis_Lab1.Entities;
using MathNet.Numerics.Statistics;

namespace Analysis_Lab1.DataProcessors
{
    public static class DataProcessor
    {
        public static List<DataPoint>? GenerateVariationSeries(List<double> data)
        {
            if (data.Count < 1)
            {
                return null;
            }

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

            return variationSeries;
        }
    }
}
