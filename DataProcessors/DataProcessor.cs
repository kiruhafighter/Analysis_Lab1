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
    
    public static List<int> DetectAnomalies(List<double> data, out double lowerBound, out double upperBound, double stdDevMultiplier = 2.0)
    {
        var anomalies = new List<int>();

        if (data == null || data.Count == 0)
            throw new ArgumentException("Data cannot be null or empty.");

        double mean = data.Average();
        double stdDev = data.StandardDeviation();

        for (int i = 0; i < data.Count; i++)
        {
            if (Math.Abs(data[i] - mean) > stdDevMultiplier * stdDev)
            {
                anomalies.Add(i);
            }
        }
        
        lowerBound = mean - stdDevMultiplier * stdDev;
        upperBound = mean + stdDevMultiplier * stdDev;

        return anomalies;
    }
    
    public static bool IsNormalDistribution(List<double> data, double tolerance = 0.01)
    {
        if (data == null || data.Count < 3)
            throw new ArgumentException("Data should not be null and should contain at least three elements.");

        double mean = data.Average();
        double stdDev = data.StandardDeviation();

        double skewness = CalculateSkewness(data, mean, stdDev);
        double kurtosis = CalculateExcessKurtosis(data, mean, stdDev);

        return Math.Abs(skewness) < tolerance && Math.Abs(kurtosis) < tolerance;
    }
    
    public static (double[] theoreticalQuantiles, double[] dataQuantiles) GenerateProbabilityPlotData(List<double> data)
    {
        if (data == null || data.Count == 0)
            throw new ArgumentException("Data cannot be null or empty.");

        var sortedData = data.OrderBy(x => x).ToArray();
        double[] theoreticalQuantiles = new double[sortedData.Length];
        double[] dataQuantiles = new double[sortedData.Length];

        double mean = sortedData.Average();
        double stdDev = sortedData.StandardDeviation();

        // Generate theoretical and data quantiles
        for (int i = 0; i < sortedData.Length; i++)
        {
            // Normal distribution quantile
            double percentile = (i - 0.5) / sortedData.Length;
            theoreticalQuantiles[i] = Normal.InvCDF(mean, stdDev, percentile);

            // Data quantile
            dataQuantiles[i] = sortedData[i];
        }

        return (theoreticalQuantiles, dataQuantiles);
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
            Estimate = median,
            ConfidenceInterval = CalculateMedianConfidenceInterval(data, confidenceLevel)
        });
        
        // Mean Square Deviation(Variance)
        var variance = data.Variance();
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Variance",
            Estimate = variance,
            ConfidenceInterval = CalculateVarianceConfidenceInterval(data, variance, confidenceLevel)
        });
        
        // Asymmetry Coefficient
        var skewness = CalculateSkewness(data, mean, standardDeviation);
        var skewnessMSD = CalculateMeanSquareDeviation(data, skewness);
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Asymmetry Coefficient",
            Estimate = skewness,
            MeanSquareDeviation = skewnessMSD,
            ConfidenceInterval = CalculateSkewnessConfidenceInterval(data, mean, standardDeviation, confidenceLevel)
        });
        
        // Excess Kurtosis Coefficient
        var kurtosis = CalculateExcessKurtosis(data, mean, standardDeviation);
        var kurtosisMSD = CalculateMeanSquareDeviation(data, kurtosis);
        results.Add(new CharacteristicAnalysisResult
        {
            Characteristic = "Kurtosis Coefficient",
            Estimate = kurtosis,
            MeanSquareDeviation = kurtosisMSD,
            ConfidenceInterval = CalculateKurtosisConfidenceInterval(data, mean, standardDeviation, confidenceLevel)
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
        
        double marginOfError = criticalValue * (standardDeviation / Math.Sqrt(data.Count));
        double lowerLimit = mean - marginOfError;
        double upperLimit = mean + marginOfError;
        
        return (lowerLimit, upperLimit);
    }
    
    private static double CalculateSkewness(List<double> data, double mean, double stdDev)
    {
        var sum = data.Sum(x => Math.Pow((x - mean) / stdDev, 3));
        return (sum / data.Count) * Math.Sqrt(data.Count * (data.Count - 1)) / (data.Count - 2);
    }
    
    private static double CalculateExcessKurtosis(List<double> data, double mean, double stdDev)
    {
        var sum = data.Sum(x => Math.Pow((x - mean) / stdDev, 4));
        double n = data.Count;
        double kurtosis = (n * (n + 1) / ((n - 1) * (n - 2) * (n - 3))) * sum - (3 * Math.Pow(n - 1, 2)) / ((n - 2) * (n - 3));
        return kurtosis - 3;
    }
    
    static double CalculateMeanSquareDeviation(List<double> data, double value)
    {
        var sumOfSquares = data.Sum(x => Math.Pow(x - value, 2));
        return Math.Sqrt(sumOfSquares / data.Count);
    }
    
    private static (double, double) CalculateMedianConfidenceInterval(List<double> data, double confidenceLevel)
    {
        int count = data.Count;
        var sortedData = data.OrderBy(x => x).ToList();
        double lowerLimit = CalculateMedian(sortedData.Take(count / 2 + 1).ToList());
        double upperLimit = CalculateMedian(sortedData.Skip(count / 2).ToList());

        return (lowerLimit, upperLimit);
    }

    private static (double, double) CalculateSkewnessConfidenceInterval(List<double> data, double mean, double standardDeviation, double confidenceLevel)
    {
        Normal normalDistribution = Normal.WithMeanStdDev(mean, standardDeviation);
        double criticalValue = normalDistribution.InverseCumulativeDistribution((1 + confidenceLevel) / 2);

        double skewness = CalculateSkewness(data, mean, standardDeviation);
        double standardError = Math.Sqrt(6.0 / data.Count);
        double lowerLimit = skewness - criticalValue * standardError;
        double upperLimit = skewness + criticalValue * standardError;

        return (lowerLimit, upperLimit);
    }

    private static (double, double) CalculateKurtosisConfidenceInterval(List<double> data, double mean, double standardDeviation, double confidenceLevel)
    {
        Normal normalDistribution = Normal.WithMeanStdDev(mean, standardDeviation);
        double criticalValue = normalDistribution.InverseCumulativeDistribution((1 + confidenceLevel) / 2);

        double kurtosis = CalculateExcessKurtosis(data, mean, standardDeviation);
        double standardError = Math.Sqrt(24.0 / data.Count);
        double lowerLimit = kurtosis - criticalValue * standardError;
        double upperLimit = kurtosis + criticalValue * standardError;

        return (lowerLimit, upperLimit);
    }
    
    private static (double, double) CalculateVarianceConfidenceInterval(List<double> data, double variance, double confidenceLevel)
    {
        int degreesOfFreedom = data.Count - 1;
        var chiSquareDistribution = new ChiSquared(degreesOfFreedom);

        double lowerBound = chiSquareDistribution.InverseCumulativeDistribution(1 - (1 - confidenceLevel) / 2);
        double upperBound = chiSquareDistribution.InverseCumulativeDistribution((1 - confidenceLevel) / 2);

        double lowerLimit = (degreesOfFreedom * variance) / upperBound;
        double upperLimit = (degreesOfFreedom * variance) / lowerBound;

        return (lowerLimit, upperLimit);
    }
    
    private static double CalculateMedian(List<double> data)
    {
        int count = data.Count;
        var sortedData = data.OrderBy(x => x).ToList();
        if (count % 2 == 0)
        {
            int middle = count / 2;
            return (sortedData[middle - 1] + sortedData[middle]) / 2.0;
        }
        else
        {
            return sortedData[count / 2];
        }
    }
}