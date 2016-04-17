using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using ExcelReports.AutoReports.TemplateReports;
using ExcelReports.ExcelInteropReports;

namespace ExcelReports.AutoReports
{

  public enum VQR_ResultQuantizationMode
  {
    RegularAverageOfTheLargest = 0x00,
    AverageSDModifiedOfTheLargest,
    AverageAcrossAllSegments
  }

  public enum ModeOfProximityReporting
  {
    JustVectorScalar = 0x00,
    NormedVectorScalar = 0x01,
    SpecialRating = 0x02,
  }

  public enum SlaeReportColors
  {
    Undefined = 0x00,
    //BackgroundPicked = System.Drawing.Color.Gold.ToExcelColor(),
    //BackgroundTarget = System.Drawing.Color.DarkViolet.ToExcelColor(),
    //BackgroundRegularOrder = System.Drawing.Color.Wheat.ToExcelColor(),
    //TextRegularOrder = System.Drawing.Color.DarkBlue.ToExcelColor()
  }

  public class BaseSlaeExcelReport
  {

    protected const int ColumnsTopLimit = 83; //(249 / 3)
    protected const int ColumnsToReRunPVS = 43; //1.5+(83/2)

    public static ModeOfProximityReporting ProximityCalculationMode { get; set; }
    public static int CountOfPlaysForProximityRatingsCounts { get; set; }
    public static string Name4UI{ get ; protected set; }

    /// <summary>
    /// Creates the Sorted Aray Representation
    /// </summary>
    /// <param name="dataPointInfo"></param>
    /// <returns></returns>
    protected static List<double> SortGivenProbs(double[] dataPointInfo)
    {
      List<double> arrayToSort = new List<double>();
      for (int bIdx = 0; bIdx < dataPointInfo.Length; bIdx++)
      {
        arrayToSort.Add(dataPointInfo[bIdx]);
      }
      arrayToSort.Sort();
      return arrayToSort;
    }


    /// <summary>
    /// Adda a row with stats
    /// </summary>
    /// <param name="b">The ord b nulber</param>
    /// <param name="aeProb">AE probability</param>
    /// <param name="newProb">VQ probability</param>
    /// <param name="dt">The Table</param>
    protected static void AddTableRowToDiffReport(int b, double aeProb, double newProb, ref System.Data.DataTable dt)
    {
      dt.Rows.Add(dt.NewRow());
      int lastRow = dt.Rows.Count - 1;
      // Fill in the VQ probs and so on
      dt.Rows[lastRow]["bVQ"] = b;
      dt.Rows[lastRow]["prbVQ"] = newProb;
      dt.Rows[lastRow]["plVQ"] = 0;
      //Fill in  regular AE probs and so on
      dt.Rows[lastRow]["bAE"] = b;
      dt.Rows[lastRow]["prbAE"] = aeProb;
      dt.Rows[lastRow]["plAE"] = 0;
    }

    /// <summary>
    /// Creates the structure of the Typical VQ report
    /// </summary>
    /// <returns>The structured empty table</returns>
    protected static System.Data.DataTable CreateDiffReportTableStructure()
    {
      System.Data.DataTable theData = new System.Data.DataTable("theDiffReport");
      // VQ(i) original columns
      theData.Columns.Add(new System.Data.DataColumn("bVQ", typeof(int)));
      theData.Columns.Add(new System.Data.DataColumn("prbVQ", typeof(double)));
      theData.Columns.Add(new System.Data.DataColumn("plVQ", typeof(int)));
      // AE(r) original columns
      theData.Columns.Add(new System.Data.DataColumn("bAE", typeof(int)));
      theData.Columns.Add(new System.Data.DataColumn("prbAE", typeof(double)));
      theData.Columns.Add(new System.Data.DataColumn("plAE", typeof(int)));
      // Calculated columns
      theData.Columns.Add(new System.Data.DataColumn("prbDiA", typeof(double)));
      theData.Columns.Add(new System.Data.DataColumn("prbDiD", typeof(double)));
      theData.Columns.Add(new System.Data.DataColumn("plDiA", typeof(int)));
      theData.Columns.Add(new System.Data.DataColumn("plDiI", typeof(int)));
      return theData;
    }



    /// <summary>
    /// Creates a table of the play diffs in EAR vs. VQ(i) report
    /// </summary>
    /// <param name="wsDiff"></param>
    /// <param name="dtM5"></param>
    /// <param name="dtMB"></param>
    /// <param name="targetDate"></param>
    protected static void CreateVQTotalsReport(BaseExcelInteropWorksheet wsDiff, System.Data.DataTable dtM5, System.Data.DataTable dtMB, DateTime targetDate)
    {
      CalculatePlacementForDiffReport(dtM5, "prbVQ Desc", "plVQ");
      CalculatePlacementForDiffReport(dtM5, "prbAE Desc", "plAE", true);
      CalculatePlacementForDiffReport(dtMB, "prbVQ Desc", "plVQ");
      CalculatePlacementForDiffReport(dtMB, "prbAE Desc", "plAE", true);

      ProcessTableForDiffReport(wsDiff, dtM5);
      ProcessTableForDiffReport(wsDiff, dtMB, 1);
    }



    /// <summary>
    /// Creates a table of the play diffs in EAR vs. VQ(i) report
    /// </summary>
    /// <param name="wsDiff"></param>
    /// <param name="dtM5"></param>
    /// <param name="dtMB"></param>
    /// <param name="targetDate"></param>
    protected static void CreatePlaceProbDiffsReport(BaseExcelInteropWorksheet wsDiff, System.Data.DataTable dtM5, System.Data.DataTable dtMB, DateTime targetDate)
    {
      CalculatePlacementForDiffReport(dtM5, "prbVQ Desc", "plVQ");
      CalculatePlacementForDiffReport(dtM5, "prbAE Desc", "plAE", true);
      CalculatePlacementForDiffReport(dtMB, "prbVQ Desc", "plVQ");
      CalculatePlacementForDiffReport(dtMB, "prbAE Desc", "plAE", true);

      ProcessTableForDiffReport(wsDiff, dtM5);
      ProcessTableForDiffReport(wsDiff, dtMB, 1);
    }

    private static void ProcessTableForDiffReport(BaseExcelInteropWorksheet wsDiff, DataTable dt, int block=0)
    {
      DataView dv = new DataView(dt, "", "plDiI Asc, prbDiD Asc", DataViewRowState.CurrentRows);
      for (int colH = 0; colH < dt.Columns.Count; colH++)
      {
        wsDiff.setHeaderCellTo(1, ((dt.Columns.Count * block + 3 * block) + (colH + 1) + 1), dt.Columns[colH].ColumnName);
      }
      // Building a header
      for (int row = 0; row < dv.Count; row++)
      {
        wsDiff.setHeaderCellTo(row + 2, ((dt.Columns.Count * block + 3 * block) + 1), (row + 1).ToString());
        for (int col = 0; col < dt.Columns.Count; col++)
        {
          wsDiff.setCellTo(row + 2, ((dt.Columns.Count * block + 3 * block) + (col + 1)+1), dv[row][col].ToString());
        }
      }
    }


    /// <summary>
    /// Calculates and sorts the places/probs of the AE vs. VQ data
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="sortName"></param>
    /// <param name="fieldName"></param>
    /// <param name="bRecalc"></param>
    private static void CalculatePlacementForDiffReport
    (
      System.Data.DataTable   dt, 
      string                  sortName, 
      string                  fieldName,
      bool                    bRecalc=false
    )
    {
      dt.AcceptChanges();
      DataView dv = new DataView(dt, null, sortName, DataViewRowState.CurrentRows);
      for (int row = 0; row < dv.Count; row++ )
      {
        dv[row][fieldName] = row+1;
        if (bRecalc)
        {
          double diff = (double)(dv[row]["prbVQ"]) - (double)(dv[row]["prbAE"]);
          dv[row]["prbDiA"] = Math.Abs(diff);
          dv[row]["prbDiD"] = diff;

          int pdiff = (int)(dv[row]["plVQ"]) - (int)(dv[row]["plAE"]);
          dv[row]["plDiA"] = (int)(Math.Abs(pdiff));
          dv[row]["plDiI"] = pdiff;
        }
      }
    }
  }
}
