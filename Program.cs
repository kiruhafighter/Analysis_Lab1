using Analysis_Lab1.Constants;
using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Entities;
using Analysis_Lab1.Enums;
using Analysis_Lab1.Utilities;

var values = DataReader.LoadDataFromFile(@"C:\Users\kiruhafighter\Downloads\data_lab1,2 1\data_lab1,2\500\norm1.txt");

if (values == null)
{
    return;
}

var variationSeries = DataProcessor.GenerateVariationSeries(values);

ExcelProcessor.SaveAsExcel<DataPoint, VariationSeriesColumns>(variationSeries, FilePaths.SaveVariationSeriesPath);

var classIntervals = StatisticsAnalyzer.DivideIntoClasses(variationSeries);

ExcelProcessor.SaveAsExcel<ClassInterval, ClassIntervalColumns>(classIntervals!, FilePaths.SaveClassIntervalPath);

ExcelProcessor.SaveClassesAsHistogram(classIntervals!, variationSeries, FilePaths.SaveClassIntervalHistogramPath);

var analysisCharacteristics = DataProcessor.EstimateQuantitativeCharacteristics(values);

ExcelProcessor.SaveAsExcel<CharacteristicAnalysisResult, AnalysisResultColumns>(analysisCharacteristics, FilePaths.SaveAnalysisCharacteristicsPath);

var probabilityPlotData = DataProcessor.GenerateProbabilityPlotData(values);

ExcelProcessor.DisplayProbabilityPlotInExcel(probabilityPlotData, FilePaths.SaveProbabilityPlotPath);

var normalDistributionCheck = DataProcessor.IsNormalDistribution(values);

Console.WriteLine($"Is normal distribution: {normalDistributionCheck}");

var anomalies = DataProcessor.DetectAnomalies(values, out double lowerBound, out double upperBound);

ExcelProcessor.DisplayDataWithBoundsInExcel(values, lowerBound, upperBound, FilePaths.SaveDataWithBoundsPath);