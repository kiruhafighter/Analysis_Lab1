using Analysis_Lab1.Constants;
using Analysis_Lab1.DataProcessors;
using Analysis_Lab1.Entities;
using Analysis_Lab1.Extensions;
using MathNet.Numerics.Statistics;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using SpreadsheetLight;

namespace Analysis_Lab1.Utilities;

public static class ExcelProcessor
{
    public static void SaveAsExcel<T, TEnum>(IList<T> items, string savePath)
        where TEnum : Enum
    {
        using var sl = new SLDocument();
        
        SLDocumentExtensions<TEnum>.ApplyExcelConfiguration(sl);
        
        var rowIndex = ExcelConstants.RowDataStartIndex;

        foreach (var item in items)
        {
            var enumColumns = Enum.GetValues(typeof(TEnum));
            
            var columnIndex = ExcelConstants.ColumnStartIndex;

            foreach (TEnum column in enumColumns)
            {
                string propertyName = column.ToString();

                var property = typeof(T).GetProperty(propertyName);

                if (property is not null)
                {
                    var cellValue = property.GetValue(item)?.ToString();

                    if (cellValue is not null)
                    {
                        sl.SetCellValue(rowIndex, columnIndex, cellValue);
                    }
                }

                columnIndex++;
            }

            rowIndex++;
        }
        
        sl.SaveAs(savePath);
    }
    
    public static void DisplayProbabilityPlotInExcel((double[], double[]) probabilityPlotData, string filePath)
    {
        double[] expectedQuantiles = probabilityPlotData.Item1;
        double[] sortedData = probabilityPlotData.Item2;
        
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("ProbabilityPlot");
            
            for (int i = 0; i < sortedData.Length; i++)
            {
                worksheet.Cells[i + 2, 1].Value = expectedQuantiles[i];
                worksheet.Cells[i + 2, 2].Value = sortedData[i];
            }

            // Create a scatter plot in the worksheet
            var scatterChart = worksheet.Drawings.AddChart("ProbabilityPlot", eChartType.XYScatterLines);
            scatterChart.Title.Text = "Probability Plot";
            scatterChart.SetPosition(0, 0, 4, 0);
            scatterChart.SetSize(600, 400);
            scatterChart.XAxis.Title.Text = "Expected Quantiles (from Normal Distribution)";
            scatterChart.YAxis.Title.Text = "Sorted Data Quantiles";

            var series = scatterChart.Series.Add(worksheet.Cells["A2:A" + (sortedData.Length + 1)], worksheet.Cells["B2:B" + (sortedData.Length + 1)]);
            series.Header = "Probability Plot";
            
            package.SaveAs(new FileInfo(filePath));
        }
    }
    
    public static void DisplayDataWithBoundsInExcel(List<double> data, double lowerBound, double upperBound, string filePath)
    {
        int[] indices = Enumerable.Range(1, data.Count).ToArray();
        double[] values = data.ToArray();
        
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("DataPlot");

            for (int i = 0; i < indices.Length; i++)
            {
                worksheet.Cells[i + 2, 1].Value = indices[i]; // Index
                worksheet.Cells[i + 2, 2].Value = values[i];  // Data value
            }

            // Create a scatter plot in the worksheet
            var scatterChart = worksheet.Drawings.AddChart("DataPlot", eChartType.XYScatterLines);
            scatterChart.Title.Text = "Data Plot";
            scatterChart.SetPosition(0, 0, 4, 0);
            scatterChart.SetSize(600, 400);
            scatterChart.XAxis.Title.Text = "Index";
            scatterChart.YAxis.Title.Text = "Data Value";

            var series = scatterChart.Series.Add(worksheet.Cells["A2:A" + (indices.Length + 1)], worksheet.Cells["B2:B" + (indices.Length + 1)]);
            series.Header = "Data Values";

            // Adding lines for lower and upper bounds
            AddBoundLine(worksheet, scatterChart, lowerBound, indices.Length, "Lower Bound", 3);
            AddBoundLine(worksheet, scatterChart, upperBound, indices.Length, "Upper Bound", 4);

            package.SaveAs(new FileInfo(filePath));
        }
    }

    public static void SaveClassesAsHistogram(List<ClassInterval> classIntervals, List<DataPoint> variationSeries, 
        string savePath,  double bandwidth = 0.0)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("HistogramData");
        
        worksheet.Cells["A1"].Value = "Class Number";
        worksheet.Cells["B1"].Value = "Lower Bound";
        worksheet.Cells["C1"].Value = "Upper Bound";
        worksheet.Cells["D1"].Value = "Frequency";
        worksheet.Cells["F1"].Value = "Value";
        worksheet.Cells["G1"].Value = "KDE Value";
        worksheet.Cells["H1"].Value = "EDF Value";

        // Write data
        for (int i = 0; i < classIntervals.Count; i++)
        {
            worksheet.Cells[i + 2, 1].Value = classIntervals[i].ClassNumber;
            worksheet.Cells[i + 2, 2].Value = classIntervals[i].LowerBound;
            worksheet.Cells[i + 2, 3].Value = classIntervals[i].UpperBound;
            worksheet.Cells[i + 2, 4].Value = classIntervals[i].Frequency;
        }
        
        var chart = worksheet.Drawings.AddChart("HistogramChart", eChartType.ColumnClustered);
        
        chart.SetPosition(0, 0, 9, 0);
        chart.SetSize(600, 400);
        
        // Set chart data range
        chart.Series.Add(worksheet.Cells["D2:D" + (classIntervals.Count + 1)],
            worksheet.Cells["A2:A" + (classIntervals.Count + 1)]);

        #region KDE
        SetKDEChart(variationSeries, bandwidth, worksheet);
        #endregion

        #region EDF
        SetEDFGraph(variationSeries, worksheet);
        #endregion
        
        package.SaveAs(new FileInfo(savePath));
    }

    private static void SetKDEChart(List<DataPoint> variationSeries, double bandwidth, ExcelWorksheet worksheet)
    {
        if (bandwidth <= 0.0)
        {
            // If bandwidth is not specified, calculate it using Scott's rule
            bandwidth = Statistics.StandardDeviation(variationSeries.Select(dp => dp.Value)) *
                        Math.Pow(variationSeries.Count, -1.0 / 5.0) * 1.06;
        }

        var kdePoints = StatisticsAnalyzer.GenerateKDEPoints(variationSeries, bandwidth);

        for (int i = 0; i < kdePoints.Count; i++)
        {
            worksheet.Cells[i + 2, 6].Value = kdePoints[i].Value;
            worksheet.Cells[i + 2, 7].Value = kdePoints[i].EmpiricalDistribution;
        }

        // var kdeLine = chart.PlotArea.ChartTypes.Add(eChartType.Line);

        var kdeChart = worksheet.Drawings.AddChart("KDEChart", eChartType.XYScatterLines);

        kdeChart.SetPosition(22, 0, 9, 0);
        kdeChart.SetSize(600, 400);

        kdeChart.Series.Add(worksheet.Cells["G2:G" + (kdePoints.Count + 1)],
            worksheet.Cells["F2:F" + (kdePoints.Count + 1)]);
    }
    
    private static void SetEDFGraph(List<DataPoint> variationSeries, ExcelWorksheet worksheet)
    {
        for (int i = 0; i < variationSeries.Count; i++)
        {
            worksheet.Cells[i + 2, 8].Value = variationSeries[i].EmpiricalDistribution;
        }

        // var kdeLine = chart.PlotArea.ChartTypes.Add(eChartType.Line);

        var edfGraph = worksheet.Drawings.AddChart("EDFGraph", eChartType.XYScatterLines);

        edfGraph.SetPosition(44, 0, 9, 0);
        edfGraph.SetSize(600, 400);

        edfGraph.Series.Add(worksheet.Cells["H2:H" + (variationSeries.Count + 1)],
            worksheet.Cells["F2:F" + (variationSeries.Count + 1)]);
    }
    
    private static void AddBoundLine(ExcelWorksheet worksheet, ExcelChart scatterChart, double bound, int length, string seriesName, int boundColumn)
    {
        // Adding a column for bound values
        for (int i = 1; i <= length; i++)
        {
            worksheet.Cells[i + 1, boundColumn].Value = bound;
        }

        // The address for the X-values (indices)
        string xAddress = ExcelRange.GetAddress(2, 1, length + 1, 1); // "A2:A{length+1}"

        // The address for the Y-values (constant bound)
        string yAddress = ExcelRange.GetAddress(2, boundColumn, length + 1, boundColumn); // "C2:C{length+1}"

        // Adding the series for the bound line
        var boundSeries = scatterChart.Series.Add(yAddress, xAddress);
        boundSeries.Header = seriesName;
    }
}