namespace Analysis_Lab1.Entities;

public sealed class CharacteristicAnalysisResult
{
    public string Characteristic { get; set; }
    
    public double Estimate { get; set; }
    
    public double? MeanSquareDeviation { get; set; }

    public (double, double)? ConfidenceInterval { get; set; }
}