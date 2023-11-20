using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Utilities;

var values = DataReader.LoadDataFromFile(@"C:\Users\Kirill-PC2\Downloads\data_lab1,2 1\data_lab1,2\25\veib1.dat");

if (values == null)
{
    return;
}

var variationSeries = DataProcessor.GenerateVariationSeries(values);

if (variationSeries is null)
{
    return;
}

variationSeries.ShowVariationSeriesInConsole();