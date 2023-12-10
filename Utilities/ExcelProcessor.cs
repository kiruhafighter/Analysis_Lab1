using Accord.Extensions;
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
                    sl.SetCellValue(rowIndex, columnIndex, cellValue);
                }

                columnIndex++;
            }

            rowIndex++;
        }
        
        sl.SaveAs(savePath);
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

        var kdeChart = worksheet.Drawings.AddChart("KDEChart", eChartType.Line);

        kdeChart.SetPosition(22, 0, 9, 0);
        kdeChart.SetSize(600, 400);

        kdeChart.Series.Add(worksheet.Cells["G2:G" + (kdePoints.Count + 1)],
            worksheet.Cells["F2:F" + (kdePoints.Count + 1)]);
    }
}