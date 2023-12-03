using Analysis_Lab1.Entities;
using MathNet.Numerics.Statistics;

namespace Analysis_Lab1.DataProcessors;

public static class DataProcessor
{
    public static List<DataPoint> GenerateVariationSeries(IList<double> data)
    {
        var variationSeries = data
            .GroupBy(x => x)
            .OrderBy(x => x.Key)
            .Select((group, index) => new DataPoint
            {
                Id = index + 1,
                Value = group.Key,
                Frequency = group.Count(),
                RelativeFrequency = (double)group.Count() / data.Count,
                EmpiricalDistribution = data.EmpiricalCDF(group.Key)
            }).ToList();

        return variationSeries;
    }
}