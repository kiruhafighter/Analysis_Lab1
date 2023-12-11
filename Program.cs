using Analysis_Lab1.Constants;
using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Entities;
using Analysis_Lab1.Enums;
using Analysis_Lab1.Utilities;

var values = DataReader.LoadDataFromFile(@"C:\Users\Kirill-PC2\Downloads\data_lab1,2 1\data_lab1,2\500\EXP.TXT");

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