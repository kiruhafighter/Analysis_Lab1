using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Utilities;

var values = DataReader.LoadDataFromFile("C:\\Users\\kiruhafighter\\Downloads\\data_lab1,2 1\\data_lab1,2\\25\\veib1.txt");

if (values == null)
{
    return;
}

TableVisualProcessor.GenerateVariationSeries(values);