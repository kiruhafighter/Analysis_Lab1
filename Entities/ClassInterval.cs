namespace Analysis_Lab1.Entities;

public sealed class ClassInterval
{
    public int ClassNumber { get; set; }
    
    public double LowerBound { get; set; }
    
    public double UpperBound { get; set; }
    
    public int Frequency { get; set; }
    
    public double RelativeFrequency { get; set; }
    
    public double EmpiricalDistribution { get; set; }

    public List<DataPoint> DataPoints { get; set; } = new();
}