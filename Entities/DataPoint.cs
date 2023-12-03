namespace Analysis_Lab1.Entities;

public sealed class DataPoint
{
    public int Id { get; init; }

    public double Value { get; init; }

    public int Frequency { get; init; }

    public double RelativeFrequency { get; init; }

    public double EmpiricalDistribution { get; init; }
}