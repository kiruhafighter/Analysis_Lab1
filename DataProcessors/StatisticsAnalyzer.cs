using Analysis_Lab1.Entities;
using MathNet.Numerics.Distributions;

namespace Analysis_Lab1.DataProcessors;

public static class StatisticsAnalyzer
{
    public static List<ClassInterval>? DivideIntoClasses(List<DataPoint> variationSeries, int? numberOfClasses = null)
    {
        var dataSize = variationSeries.Count;
        
        if (dataSize == 0)
        {
            Console.WriteLine("Error occured while dividing variation series on classes");
            return null;
        }

        if (numberOfClasses is null or <= 0)
        {
            //use Sturges' rule or other methods to calculate the number of classes
            numberOfClasses = (int)Math.Ceiling(Math.Log(dataSize, 2) + 1);
        }

        var firstDataPointValue = variationSeries.First().Value;

        double classWidth = (double)((variationSeries.Last().Value - firstDataPointValue) / numberOfClasses)!;

        double lowerBound = firstDataPointValue;
        double upperBound = lowerBound + classWidth;

        var dividedClasses = new List<ClassInterval>();

        for (int i = 1; i <= numberOfClasses; i++)
        {
            var dataPointsInClass = variationSeries
                .Where(dp => dp.Value >= lowerBound && dp.Value < upperBound)
                .ToList();

            if (i == numberOfClasses)
            {
                var lastElements = variationSeries.Where(vs => vs.Value.Equals(upperBound));
                
                dataPointsInClass.AddRange(lastElements);
            }

            int frequency = dataPointsInClass.Sum(dp => dp.Frequency);
            double relativeFrequency = (double)frequency / dataSize;
            
            double empiricalDistribution = dataPointsInClass.Count > 0
                ? dataPointsInClass.Last().EmpiricalDistribution
                : 0.0;

            var classInterval = new ClassInterval
            {
                ClassNumber = i,
                LowerBound = lowerBound,
                UpperBound = upperBound,
                Frequency = frequency,
                RelativeFrequency = relativeFrequency,
                EmpiricalDistribution = empiricalDistribution,
                DataPoints = dataPointsInClass
            };
            
            dividedClasses.Add(classInterval);

            lowerBound = upperBound;
            upperBound += classWidth;
        }

        return dividedClasses;
    }

    public static List<DataPoint> GenerateKDEPoints(List<DataPoint> variationSeries, double bandwidth)
    {
        if (variationSeries == null || !variationSeries.Any())
            throw new ArgumentException("Variation series must not be null or empty.");

        if (bandwidth <= 0)
            throw new ArgumentException("Bandwidth must be greater than zero.");

        var kdePoints = new List<DataPoint>();

        foreach (var evaluationPoint in variationSeries)
        {
            double kdeValue = 0;
            foreach (var dataPoint in variationSeries)
            {
                var kernel = new Normal(dataPoint.Value, bandwidth);
                kdeValue += kernel.Density(evaluationPoint.Value) * dataPoint.RelativeFrequency;
            }

            kdePoints.Add(new DataPoint
            {
                Value = evaluationPoint.Value,
                EmpiricalDistribution = kdeValue
            });
        }

        return kdePoints;
    }
}