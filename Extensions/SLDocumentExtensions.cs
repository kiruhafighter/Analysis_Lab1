using Analysis_Lab1.Constants;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace Analysis_Lab1.Extensions;

// ReSharper disable once InconsistentNaming
public static class SLDocumentExtensions<TEnum> 
    where TEnum : Enum
{
    public static void ApplyExcelConfiguration(SLDocument sl)
    {
        SetHeader(sl);
        SetExcelSettings(sl);
    }

    private static void SetHeader(SLDocument sl)
    {
        var enumValues = Enum.GetValues(typeof(TEnum));
        
        var columnIndex = ExcelConstants.ColumnStartIndex;
        
        foreach (TEnum column in enumValues)
        {
            sl.SetCellValue(ExcelConstants.RowStartIndex, columnIndex, column.GetDisplayName());
            sl.SetColumnWidth(columnIndex, ExcelConstants.ColumnMaxWidth);

            columnIndex++;
        }
    }
    
    private static void SetExcelSettings(SLDocument sl)
    {
        var enumValues = Enum.GetValues(typeof(TEnum));

        var style = new SLStyle
        {
            Font = new SLFont { Bold = true },
        };

        style.Fill.SetPatternType(PatternValues.Solid);
        style.Fill.SetPatternForegroundColor(System.Drawing.Color.LightGray);

        sl.SetCellStyle(ExcelConstants.RowStartIndex, ExcelConstants.ColumnStartIndex, 
            ExcelConstants.RowStartIndex, enumValues.Length, style);

        sl.FreezePanes(1, 2);
        sl.ProtectWorksheet(new SLSheetProtection());
    }
}