using Analysis_Lab1.Constants;
using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Entities;
using Analysis_Lab1.Enums;
using Analysis_Lab1.Utilities;

var values = DataReader.LoadDataFromFile(@"C:\Users\kiruhafighter\Downloads\data_lab1,2 1\data_lab1,2\200\EXTR.DAT");

if (values == null)
{
    return;
}

var variationSeries = DataProcessor.GenerateVariationSeries(values);

ExcelProcessor.SaveAsExcel<DataPoint, VariationSeriesColumns>(variationSeries, FilePaths.SaveVariationSeriesPath);

var classIntervals = StatisticsAnalyzer.DivideIntoClasses(variationSeries, 200);

ExcelProcessor.SaveAsExcel<ClassInterval, ClassIntervalColumns>(classIntervals!, FilePaths.SaveClassIntervalPath);