using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using Microsoft.Office.Interop.Excel;
using ExcelReports.AutoReports;

namespace ExcelReports.ExcelInteropReports
{

  public class CellDescriptor
  {
    public CellDescriptor(){}

    public int Row { set;  get;  }
    public int Col { set; get; }
    public string Value { set; get; }

    public int Color { set; get; }
    public int BgColor { set; get; }
    public bool MakeBold { get; set; }

  }

	public class BaseExcelInteropWorksheet
	{
    object _MISSING_ = Type.Missing;

		private Worksheet worksheet = null;
    public Worksheet GetOriginalWorksheet()
    {
      return worksheet; 
    }

		public BaseExcelInteropWorksheet(Worksheet ws)
		{
			worksheet = ws;
		}

    public string Name { get { return worksheet.Name; } set { worksheet.Name = value; } }

    Range setCellValue(CellDescriptor cd)
    {
      Range cRange = (Range)(worksheet.Cells[cd.Row, cd.Col]);
      if (!string.IsNullOrEmpty(cd.Value)) worksheet.Cells[cd.Row, cd.Col] = cd.Value;
      if (0 != cd.Color) cRange.Font.Color = cd.Color;
      if (0 != cd.BgColor) cRange.Interior.Color = cd.BgColor;
      if (cd.MakeBold) cRange.Font.Bold = true;
      return cRange;
    }

    /// <summary>
    /// Gets a Cell Name
    /// </summary>
    /// <param name="cd">Give a name of the cell at cd.Row and cd.Col</param>
    /// <returns>cell name</returns>
    public string GetCellName(CellDescriptor cd)
    {
      Range cRange = (Range)(worksheet.Cells[cd.Row, cd.Col]);
      return cRange.AddressLocal;
    }

    /// <summary>
    /// Gets Span Name
    /// </summary>
    /// <param name="cdFrom"></param>
    /// <param name="cdTo"></param>
    /// <returns></returns>
    public string GetSpanName(CellDescriptor cdFrom, CellDescriptor cdTo)
    {
      Range cRange1 = (Range)(worksheet.Cells[cdFrom.Row, cdFrom.Col]);
      Range cRange2 = (Range)(worksheet.Cells[cdTo.Row, cdTo.Col]);
      Range cRange = (Range)(worksheet.get_Range(cRange1.AddressLocal, cRange2.AddressLocal));
      return cRange.AddressLocal;
    }

    /// <summary>
    /// Gets Span Name
    /// </summary>
    /// <param name="cdFrom"></param>
    /// <param name="cdTo"></param>
    /// <returns></returns>
    public void setFormatSpan(int rowFrom, int colFrom, int rowTo, int colTo/*, SlaeReportColors bgColor = SlaeReportColors.Undefined, SlaeReportColors textColor = SlaeReportColors.Undefined*/)
    {
      Range cRange1 = (Range)(worksheet.Cells[rowFrom, colFrom]);
      Range cRange2 = (Range)(worksheet.Cells[rowTo, colTo]);
      Range workRange = (Range)(worksheet.get_Range(cRange1.AddressLocal, cRange2.AddressLocal));

      //int theColor = System.Drawing.Color.FromArgb(0, 188, 0).ToArgb();
      //workRange.BorderAround(0x8, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, ColorExtensions.ToExcelColor(System.Drawing.Color.FromArgb(0, 188, 0)));
      workRange.Interior.Color = ColorExtensions.ToExcelColor(System.Drawing.Color.FromArgb(88, 188, 88));
      return ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowFrom"></param>
    /// <param name="colFrom"></param>
    /// <param name="rowTo"></param>
    /// <param name="colTo"></param>
    public void setBorderForSpan(int rowFrom, int colFrom, int rowTo, int colTo/*, SlaeReportColors bgColor = SlaeReportColors.Undefined, SlaeReportColors textColor = SlaeReportColors.Undefined*/)
    {
      Range cRange1 = (Range)(worksheet.Cells[rowFrom, colFrom]);
      Range cRange2 = (Range)(worksheet.Cells[rowTo, colTo]);
      Range workRange = (Range)(worksheet.get_Range(cRange1.AddressLocal, cRange2.AddressLocal));
      try
      {
        //int theColor = System.Drawing.Color.FromArgb(0, 188, 0).ToArgb();
        workRange.BorderAround(0x8, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, ColorExtensions.ToExcelColor(System.Drawing.Color.FromArgb(0, 255, 0)));
        //workRange.Interior.Color = ColorExtensions.ToExcelColor(System.Drawing.Color.FromArgb(88, 188, 88));
      }
      catch(Exception e)
      {
        string s = e.Message;
      }
      return;
    }



    /// <summary>
    /// Sets value of a cell
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="cellValue"></param>
		public void setHeaderCellTo(int row, int col, string cellValue)
		{
			Range cRange = setCellValue( new CellDescriptor{ Row=row, Col=col, Value = cellValue});
			cRange.Font.Bold = true;
		}

		public void setCellTo(int row, int col, string cellValue, int theBackground)
		{
      Range cRange = setCellValue( new CellDescriptor { Row = row, Col = col,  Value=cellValue, BgColor = theBackground });
		}

		public void setCellTo(int row, int col, string cellValue, int bgColor, int textColor)
		{
      Range cRange = setCellValue(new CellDescriptor { Row = row, Col = col, Value=cellValue, BgColor = bgColor, Color=textColor });
		}

    public void setCellTo(int row, int col, string cellValue, int bgColor, bool textBold)
    {
      Range cRange = setCellValue(new CellDescriptor { Row = row, Col = col, Value = cellValue, BgColor = bgColor, MakeBold = textBold });
    }


    public void setCellTo(int row, int col, string cellValue, int bgColor, int textColor, bool textBold)
    {
      Range cRange = setCellValue(new CellDescriptor { Row = row, Col = col, Value = cellValue, BgColor = bgColor, Color = textColor, MakeBold = textBold });
    }

    public void setCellTo(int row, int col, string cellValue, SlaeReportColors bgColor = SlaeReportColors.Undefined, SlaeReportColors textColor = SlaeReportColors.Undefined, bool textBold = false)
    {
      Range cRange = setCellValue(new CellDescriptor { Row = row, Col = col, Value = cellValue, BgColor = bgColor.GetHashCode(), Color = textColor.GetHashCode(), MakeBold = textBold });
    }

		public void setCellTo(int row, int col, string cellValue)
		{
      Range cRange = setCellValue(new CellDescriptor { Row = row, Col = col, Value = cellValue });
		}

    public string getCellData(int row, int col)
    {
      Range info = (Range)(worksheet.Cells[row, col]);
      var to = info.get_Value(_MISSING_);
      return (null == to)?(""):(to.ToString());
    }

		public void setMergedCellTo
			(
			int row,
			int col,
			string cellValue,
			int colsToMerge,
			int theBgColor = 0xffffff,
			int theTextColor = 0xffffff
			)
		{
			if (theBgColor == theTextColor && theTextColor == 0xffffff)
			{
				theBgColor = Color.White.ToExcelColor(); //System.Windows.SystemColors.WindowColor.GetHashCode();
				theTextColor = Color.Black.ToExcelColor(); //System.Windows.SystemColors.WindowTextColor.GetHashCode();
			}
			worksheet.Cells[row, col] = cellValue;
			Range cRange = (Range)(worksheet.Cells[row, col]);
			if (colsToMerge > 0)
			{
				Range c2 = (Range)(worksheet.Cells[row, col + colsToMerge - 1]);
				cRange = (Range)(worksheet.get_Range(cRange.AddressLocal, c2.AddressLocal));
				cRange.Merge(colsToMerge);
			}
			//cRange.BorderAround();
			//cRange.ApplyOutlineStyles();
			cRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
			cRange.Interior.Color = theBgColor;
			cRange.Font.Color = theTextColor;
			cRange.Font.Bold = true;
    }


    #region [Sorting]
    public void SortRange(CellDescriptor cd1, CellDescriptor cd2, int SortColumnIdx, bool sortAscending=true)
    {
      _Worksheet ws = (_Worksheet)(this.worksheet);
      ws.Activate();
      Range sortRange = ws.get_Range(GetCellName(cd1), GetCellName(cd2));
      sortRange.Sort
        (sortRange.Columns[SortColumnIdx, _MISSING_], XlSortOrder.xlDescending,
        _MISSING_, _MISSING_, ((sortAscending)?XlSortOrder.xlAscending:XlSortOrder.xlDescending),
        _MISSING_, XlSortOrder.xlAscending,
        XlYesNoGuess.xlNo, _MISSING_, _MISSING_,
        XlSortOrientation.xlSortColumns,
        XlSortMethod.xlPinYin,
        XlSortDataOption.xlSortNormal,
        XlSortDataOption.xlSortNormal,
        XlSortDataOption.xlSortNormal); 
    }

    public void SortRanges(CellDescriptor cd1Top, CellDescriptor cd1Bottom, int SortColumnIdx1, bool sortAscending1, int SortColumnIdx2, bool sortAscending2)
    {
      try
      {
        _Worksheet ws = (_Worksheet)(this.worksheet);
        ws.Activate();
        Range sortRange1 = ws.get_Range(GetCellName(cd1Top), GetCellName(cd1Bottom));
        sortRange1.Sort
          (sortRange1.Columns[SortColumnIdx1, _MISSING_], ((sortAscending1) ? XlSortOrder.xlAscending : XlSortOrder.xlDescending),
          sortRange1.Columns[SortColumnIdx2, _MISSING_], _MISSING_, ((sortAscending2) ? XlSortOrder.xlAscending : XlSortOrder.xlDescending),
          _MISSING_, XlSortOrder.xlAscending,
          XlYesNoGuess.xlNo, _MISSING_, _MISSING_,
          XlSortOrientation.xlSortColumns,
          XlSortMethod.xlPinYin,
          XlSortDataOption.xlSortNormal,
          XlSortDataOption.xlSortNormal,
          XlSortDataOption.xlSortNormal);
        /* 
        SortRange(cd1Top, cd1Bottom, SortColumnIdx1, sortAscending1);
        SortRange(cd1Top, cd1Bottom, SortColumnIdx2, sortAscending2);*/
      }
      catch (Exception e)
      { 
      }
    }
    #endregion [Sorting]

    #region [CopyingWorksheet]
    public void CreateBackupCopy()
    {
      worksheet.Copy(_MISSING_, worksheet);
    }
    #endregion [CopyingWorksheet]

    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="forceNumberOfLines"></param>
    /// <summary>
    /// Forces the Text Cell to be a wrapped text cell
    /// </summary>
    /// <param name="forceWidth"></param>
    public void WrapCellText(int row, int col, int forceNumberOfLines = 3, int forceWidth = 80)
    {
      Range cRange = (Range)(worksheet.Cells[row, col]);
      cRange.WrapText = true;
      //cRange. = 7;
    }

    /// <summary>
    /// Adds a comment to the cell in the worksheet
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="commentValue"></param>
    public void AddCellComment(int row, int col, string commentValue)
    {
      Range cRange = (Range)(worksheet.Cells[row, col]);
      cRange.AddComment(commentValue);
    }

    /// <summary>
    /// Adds a comment to the cell in the worksheet
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="commentValue"></param>
    public void AddCellFormula(int row, int col, string FormulaExpression)
    {
      Range cRange = (Range)(worksheet.Cells[row, col]);
      cRange.Formula = FormulaExpression;
    }

    public void Hide(int col)
    {
        worksheet.Cells[1, col].EntireColumn.Hidden = true;
    }

    public void fit(int startx, int starty, int endx, int endy)
    {
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).Cells.HorizontalAlignment = XlHAlign.xlHAlignLeft;
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).Cells.WrapText = true;
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).Cells.VerticalAlignment = XlVAlign.xlVAlignTop;
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).Cells.RowHeight = 15.75;		
   
    }


    public void SetHeight(int row, int size)
    {
        worksheet.Cells[row,1].EntireRow.RowHeight = size;
    }
    public void wrapText(int startx, int starty, int endx, int endy)
    {
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).WrapText = true;
    }

    public void Border(int startx, int starty, int endx, int endy)
    {
        Range oRange = worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy]));
        oRange.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
        oRange.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
        oRange.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
        oRange.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
        oRange.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
        oRange.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
    }

    public void Center(int startx, int starty, int endx, int endy)
    {
        worksheet.get_Range((Range)(worksheet.Cells[startx, starty]), (Range)(worksheet.Cells[endx, endy])).Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
    }

    public void ColumnWidth(int col, double width)
    {
        worksheet.Cells[1, col].EntireColumn.ColumnWidth = width;
    }

    public void SetFont(int siz, int startx, int starty,int endx, int endy)
    {
        worksheet.get_Range((Range)(worksheet.Cells[startx,starty]), (Range)(worksheet.Cells[endx,endy])).Font.Size = siz;
        worksheet.get_Range((Range)worksheet.Cells[startx, starty], (Range)worksheet.Cells[endx, endy]).Font.Name = "arial narrow";
    }

    public void fitRow(int row)
    {
        worksheet.Rows.AutoFit();
    }

    public void addColorText(string text, int row, int col, int color)
    {
        Range cell = (Range)worksheet.Cells[row, col];
        cell.Value += text;
        string celltext = cell.Value2;
        cell.get_Characters(celltext.Length - text.Length + 1, text.Length).Font.Color = color;
    }
  }
}
