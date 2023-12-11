using Analysis_Lab1.Entities;
using MathNet.Numerics.Distributions;
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


    public static List<CharacteristicAnalysisResult> EstimateQuantitativeCharacteristics(List<double> data)
    {
        var results = new List<CharacteristicAnalysisResult>();

        double confidenceLevel = 0.95;
        
        //Mean
        double mean = data.Mean();
        double standardDeviation = data.StandardDeviation();
        
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Arithmetic Mean",
            Estimate = mean,
            MeanSquareDeviation = standardDeviation,
            ConfidenceInterval = CalculateConfidenceInterval(data, mean, standardDeviation, confidenceLevel)
        });
        
        // Median
        double median = data.Median();
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Median",
            Estimate = median
        });
        
        // Mean Square Deviation
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Mean Square Deviation",
            Estimate = standardDeviation
        });
        
        // Asymmetry Coefficient
        var skewness = CalculateSkewness(data, mean, standardDeviation);
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Asymmetry Coefficient",
            Estimate = skewness
        });
        
        // Kurtosis Coefficient
        var kurtosis = CalculateKurtosis(data, mean, standardDeviation);
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Kurtosis Coefficient",
            Estimate = kurtosis
        });
        
        // Minimum
        var minimum = data.Min();
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Minimum",
            Estimate = minimum
        });
        
        // Maximum
        double maximum = data.Max();
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Maximum",
            Estimate = maximum
        });

        return results;
    }

    private static (double, double) CalculateConfidenceInterval(List<double> data, double mean, double standardDeviation, double confidenceLevel)
    {
        Normal normalDistribution = Normal.WithMeanStdDev(mean, standardDeviation);
        double criticalValue = normalDistribution.InverseCumulativeDistribution((1 + confidenceLevel) / 2);
        
        double lowerLimit = mean - criticalValue * (standardDeviation / Math.Sqrt(data.Count));
        double upperLimit = mean + criticalValue * (standardDeviation / Math.Sqrt(data.Count));
        
        return (lowerLimit, upperLimit);
    }
    
    static double CalculateSkewness(List<double> data, double mean, double standardDeviation)
    {
        var sum = data.Sum(x => Math.Pow((x - mean) / standardDeviation, 3));

        return sum / data.Count;
    }
    
    static double CalculateKurtosis(List<double> data, double mean, double standardDeviation)
    {
        var sum = data.Sum(x => Math.Pow((x - mean) / standardDeviation, 4));

        double fourthMoment = sum / data.Count;

        return fourthMoment - 3;
    }
}