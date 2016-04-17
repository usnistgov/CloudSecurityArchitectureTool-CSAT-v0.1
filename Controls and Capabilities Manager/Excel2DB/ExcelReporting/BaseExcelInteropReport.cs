using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XL = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Drawing;

namespace ExcelReports.ExcelInteropReports
{

  public enum ExcelOpenType
  {
    OpenExistingTemplateFile,
    CreateNewFile
  }
  ///  public static class ColorExtensions
  ///{
  ///   public static int ToExcelColor(this System.Drawing.Color color)
  ///   {
  ///      return System.Drawing.Color.FromArgb(color.B, color.G, color.R).ToArgb();
  ///   }
  ///}then you use it thus:
  ///USAGE::
  ///range.Interior.Color = System.Drawing.Color.Red.ToExcelColor();or:
  ///range.Borders.Color = System.Drawing.Color.FromArgb(102, 102, 153).ToExcelColor();

  /// <summary>
  /// Converts colors to Excel palette according to "Using C# to Create an Excel Document" from the Code Project
  /// </summary>
  public static class ColorExtensions
  {
    public static int ToExcelColor(this System.Drawing.Color color)
    {
      return System.Drawing.Color.FromArgb(color.B, color.G, color.R).ToArgb();
    }

    public static int TranslateToExcelColor(System.Drawing.Color color)
    {
        return System.Drawing.Color.FromArgb(color.B, color.G, color.R).ToArgb();
    }
  }

  /// <summary>
  /// Produces unified interface to work with Excel
  /// </summary>
  public class BaseExcelInteropReport
  {
    private string theFileName = "";
    private XL.Application app = null;
    private XL.Workbook workbook = null;
    private XL.Worksheet worksheet = null;
		private List<XL.Worksheet> subs = null;
		/// <summary>
    /// Creates the instance of the report
    /// </summary>
    /// 
    public BaseExcelInteropReport(string theReportName = null, string worksheetName = null)
    {
      createReport(theReportName);
      //addWorksheet(worksheetName);
    }

    public BaseExcelInteropReport(ExcelOpenType theType, string theReportName, bool createIfNotFound)
    {
      if (createIfNotFound)
      {
        createReport(theReportName);
      }
      else
      {
        openReport(theReportName);
      }
    }

    public void openReport(string theName )
    {
      try
      {
        app = new XL.Application();
        app.Visible = false;
        workbook = app.Workbooks.Open(theName);
        if (!string.IsNullOrEmpty(theName))
        {
          theFileName = theName;
        }
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
      }
      finally
      {
      }
    }


    public void createReport(string theName = null)
    {
      try
      {
        app = new XL.Application();
        app.Visible = false;
        workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
        if (!string.IsNullOrEmpty(theName))
        {
          theFileName = theName;
        }
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
      }
      finally
      {
      }
    }

    /// <summary>
    /// Creates and externaly referencable worksheet
    /// </summary>
    /// <param name="wsName">The name of the worksheet</param>
    /// <returns>BaseExcelInteropWorksheet for further manipulations</returns>
    public BaseExcelInteropWorksheet CreateWorksheet(string wsName, bool setActive = true)
		{
      Worksheet ws = null;
      if (null == subs)
      {
        subs = new List<Worksheet>();
        if (null == workbook) { workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet); }
        ws = (XL.Worksheet)(workbook.Worksheets[workbook.Worksheets.Count]);
        ws.Activate();
        ws.Name = ((string.IsNullOrEmpty(wsName)) ? "ws" + workbook.Worksheets.Count.ToString() : wsName);
        subs.Add(ws);
        return new BaseExcelInteropWorksheet(ws);
      }
      else
      {
        ws = addWorksheet(setActive);
        if (null != ws)
        {
          ws.Activate();
          ws.Name = ((string.IsNullOrEmpty(wsName)) ? "ws" + workbook.Worksheets.Count.ToString() : wsName);
          subs.Add(ws);
          return new BaseExcelInteropWorksheet(ws);
        }
      }
			return null;
		}

    /// <summary>
    /// Creates a new worksheet in the current Report file
    /// </summary>
    /// <param name="wsName">worksheet name</param>
    /// <param name="setActive">flag for setting activity</param>
    /// <returns></returns>
    public Worksheet addWorksheet(bool setActive = true)
    {
      try
      {
        worksheet = (XL.Worksheet)(workbook.Worksheets.Add());
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
				return null;
      }
      finally
      {
      }
			return worksheet;
    }


    /// <summary>
    /// Gets worksheet byt the ord numberord
    /// </summary>
    /// <param name="wsNumber"></param>
    /// <returns></returns>
    public BaseExcelInteropWorksheet getWorksheet(int wsNumber = 1)
    {
      try
      {
        if (wsNumber <=workbook.Worksheets.Count )
        {
          worksheet = (XL.Worksheet)(workbook.Worksheets[wsNumber]);
        }
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
        return null;
      }
      finally
      {
      }
      worksheet.Activate();
      return new BaseExcelInteropWorksheet(worksheet);
    }


    /// <summary>
    /// Gets worksheet by the NAME
    /// </summary>
    /// <param name="wsNumber"></param>
    /// <returns></returns>
    public BaseExcelInteropWorksheet getWorksheet(string theWorksheetName)
    {
      Worksheet foundWS=null;
      try
      {
        for (int wsIndex = 1; wsIndex <= workbook.Worksheets.Count; wsIndex++ )
        {
          worksheet = (XL.Worksheet)(workbook.Worksheets[wsIndex]);
          if (worksheet.Name.Equals(theWorksheetName, StringComparison.InvariantCultureIgnoreCase))
          {
            foundWS = worksheet;
            break;
          }
        }
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
        return null;
      }
      finally
      {
      }
      if (null == foundWS)
      {
        foundWS = (Worksheet)(this.workbook.Worksheets.Add());
      }
      foundWS.Activate();
      return new BaseExcelInteropWorksheet(worksheet);
    }



    /// <summary>
    /// Creates a copy of the current worksheet with a new name pasted after the worksheet listed last.
    /// </summary>
    /// <param name="NewSheetName"></param>
    /// <param name="InsertCopyAfter"></param>
    /// <returns></returns>
    public BaseExcelInteropWorksheet CopyWorksheet(BaseExcelInteropWorksheet Source, string NewSheetName, BaseExcelInteropWorksheet InsertCopyAfter)
    {
      try
      {
        if (null != Source && null != InsertCopyAfter)
        {
          Source.GetOriginalWorksheet().Copy(Type.Missing, InsertCopyAfter.GetOriginalWorksheet());
        }
        Worksheet newWS = this.getWorksheet(Source.GetOriginalWorksheet().Name + " (2)").GetOriginalWorksheet();
        newWS.Name = NewSheetName;
        newWS.Activate();
        return new BaseExcelInteropWorksheet(newWS);
      }
      catch (Exception e)
      {
        Console.Write("Error:" + e.Message);
        return null;
      }
      finally
      {
      }

    }

    public void fit()
    {
        foreach (Worksheet ws in workbook.Worksheets)
        {
            ws.Columns.AutoFit();
            ws.Rows.AutoFit();
        }
    }

    


    /// <summary>
    /// Finalizes the report forming (e.g. Activate(s), AutoFit(s), Font.Size(s))
    /// </summary>
    public void reportDone()
    {
			//.SaveAs("c:\\csharp-Excel.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            workbook.SaveAs(theFileName);
			workbook.Close(true, Type.Missing, Type.Missing);
			app.Quit();

			releaseObject(worksheet);
			releaseObject(workbook);
			releaseObject(app);
    }


    public void templateReportClose()
    {
      workbook.Save();
      workbook.Close(true, Type.Missing, Type.Missing);
      releaseObject(worksheet);
      releaseObject(workbook);
      
      app.Quit();
      releaseObject(app);  
    }

    public void CloseInput()
    {
        workbook.Close(true, Type.Missing, Type.Missing);
        releaseObject(worksheet);
        releaseObject(workbook);

        app.Quit();
        releaseObject(app);  
    }
		private void releaseObject(object obj)
		{
			try
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
				obj = null;
			}
			catch (Exception ex)
			{
				obj = null;
			}
			finally
			{
				GC.Collect();
			}
		}

        

    public void Experiments()
    {
      XL.Application xlApp = new XL.Application();
      if (xlApp != null)
      {
        xlApp.Visible = true;
        Workbook wb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
        Worksheet ws = (Worksheet)wb.Worksheets[1];
        ws.Name = "Test";

        wb.Worksheets.Add();
        Worksheet w2 = (Worksheet)wb.Worksheets[wb.Worksheets.Count];
        w2.Name = "X222";

        ws = (Worksheet)wb.Worksheets[1];
        if (ws != null)
        {
          XL.Range aRange = ws.get_Range("C1", "C7");
          if (aRange != null)
          {
            // Fill the cells in the C1 to C7 range of the worksheet with the number 6.
            Object[] args = new Object[1];
            args[0] = 17;
            aRange.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, aRange, args);

            // Change the cells in the C1 to C7 range of the worksheet to the number 8.
            aRange.Value2 = 8;
            //aRange.Text = "Xc";
          }
          XL.Range bRange = ws.get_Range("C4", "C4");
          bRange.Interior.Color = 52479; // This is GOLD
          XL.Range cRange = ws.get_Range("C7", "C7");
          object cf = wb.Colors;
          cRange.Interior.Color = (int)(19749);
          ws.Cells[1, 1] = "T11";
          ws.Cells[1, 2] = "T12";
          ws.Cells[2, 1] = "T21";

          cRange = (XL.Range)(w2.Cells[3, 3]);
          string a1 = ((XL.Range)(w2.Cells[1, 3])).get_Address();
          string a2 = ((XL.Range)(w2.Cells[2, 3])).get_Address();
          w2.Cells[3, 3] = "=" + a1 + "*" + a2;
          cRange.Interior.Color = System.Drawing.Color.Goldenrod.ToExcelColor();

          cRange = (XL.Range)(ws.Cells[5, 5]);
          cRange.Interior.Color = (int)(43791);
        }
      }
    }
  }
}
