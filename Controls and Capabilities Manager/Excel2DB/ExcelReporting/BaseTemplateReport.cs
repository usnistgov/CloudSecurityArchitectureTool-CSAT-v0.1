using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XL = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Drawing;
using ExcelReports.ExcelInteropReports;
using System.IO;


namespace ExcelReports.AutoReports.TemplateReports
{
    class BaseTemplateReport
    {
        public static void ReadLLFileAndFillInTemplateData(string theFile, BaseExcelInteropWorksheet xlSheet, int beginIncludingFromRow = 2, int endIncludingToRow = -1)
        {
            //DateTime currentRulesDate = theGame.GetLatestRulesDate();
            FileInfo fi = new FileInfo(theFile);
            if (fi.Exists)
            {
                StreamReader sr = new StreamReader(fi.FullName);
                //Skip the headers
                // string input = sr.ReadLine();
                string input = null;
                int fileRow = 1;
                int tableRow = 2;
                while ((input = sr.ReadLine()) != null)
                {
                    if (fileRow >= beginIncludingFromRow)
                    {
                        string[] currentLine = input.Split(new char[] { ';', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int colNum = 1; colNum <= currentLine.Length; colNum++)
                        {
                            DateTime dt = new DateTime();
                            double x;
                            if (DateTime.TryParse(currentLine[colNum - 1], out dt) && !double.TryParse(currentLine[colNum - 1], out x))
                            {
                                currentLine[colNum - 1] = dt.ToString("yyyy-MM-dd");
                            }
                            xlSheet.setCellTo(tableRow, colNum, currentLine[colNum - 1]);
                        }
                        tableRow++;
                    }
                    if (endIncludingToRow > 0 && fileRow > endIncludingToRow)
                    {
                        break;
                    }
                    fileRow++;
                }
            }
            return;
        }

        /// <summary>
        /// Supposed to populate a given DataSheet with A content of a given string
        /// </summary>
        /// <param name="theTextData"></param>
        /// <param name="xlSheet"></param>
        /// <param name="beginIncludingFromTableRow"></param>
        /// <param name="endIncludingToRow"></param>
        public static void ReadStringAndFillInTemplateDataSheet
          (
            string theTextData,
            BaseExcelInteropWorksheet xlSheet,
            int beginIncludingFromTableRow = 1,
            int beginIncludingFromTableColumn = 1,
            int endIncludingToRow = -1
          )
        {
            //DateTime currentRulesDate = theGame.GetLatestRulesDate();
            TextReader tr = new System.IO.StringReader(theTextData);
            //Skip the headers
            // string input = sr.ReadLine();
            string input = null;
            int fileRow = 1;
            int tableRow = beginIncludingFromTableRow;
            while ((input = tr.ReadLine()) != null)
            {
                if (fileRow >= 1)
                {
                    string[] currentLine = input.Split(new char[] { '\t', '\n' });
                    int colNum = beginIncludingFromTableColumn;

                    foreach (string currentEntry in currentLine)
                    {
                        string theEntry = currentEntry;
                        DateTime dt = new DateTime();
                        double x;
                        if (DateTime.TryParse(currentEntry, out dt) && !double.TryParse(currentEntry, out x))
                        {
                            theEntry = dt.ToString("yyyy-MM-dd");
                        }
                        xlSheet.setCellTo(tableRow, colNum, theEntry);
                        colNum++;
                    }

                    tableRow++;
                }
                if (endIncludingToRow > 0 && fileRow > endIncludingToRow)
                {
                    break;
                }

                fileRow++;
            }
            return;
        }


    }
}
