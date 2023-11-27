using Analysis_Lab1.Constants;
using Analysis_Lab1.Extensions;
using SpreadsheetLight;

namespace Analysis_Lab1.Utilities;

internal static class ExcelProcessor
{
    internal static void SaveAsExcel<T, TEnum>(IList<T> items, string savePath)
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
}