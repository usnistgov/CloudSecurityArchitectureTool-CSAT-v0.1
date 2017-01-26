using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelReports.ExcelInteropReports;

namespace CSRC.Reports
{
    class TicReport : BaseReport
    {
        /// <summary>
        /// get unique tic list
        /// </summary>
        /// <returns></returns>
        public List<string> GetTics()
        {
            List<string> ticList = new List<string>();
            var ret = (from p in dbContext.TICMappings
                       orderby p.TICName
                       select new { p.TICName }).Distinct();
            foreach (var rec in ret)
            {
                ticList.Add(rec.TICName);
            }
            return ticList;
        }

        /// <summary>
        /// asemble report
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bw"></param>
        public void CreateReport(string fileName, BackgroundWorker bw)
        {
            try
            {
                int mapcols = 0;
                if (InitNewExcelFile(fileName))
                {
                    InitDataModel();

                    List<string> tics = GetTics();

                    double total = 0, inc = 100.0 / tics.Count;
                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));
                    
                    int black = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                    palette = GetPalette();
                    mapPalatte = GetMapPallate();
                    int row = 1, col = 1;

                    //font
                    this.activeWorksheet.wrapText(1, 1, 2, 15);
                    this.activeWorksheet.Center(1, 1, 2, 15);
                    this.activeWorksheet.SetFont(14, 1, 1, 2, 15);

                    //colom widths
                    this.activeWorksheet.ColumnWidth(col++, 20); 
                    this.activeWorksheet.ColumnWidth(col++, 16.43);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 21.71);
                    this.activeWorksheet.ColumnWidth(col++, 33.14);
                    for (int i = 1; i <= 7; i++)
                        this.activeWorksheet.ColumnWidth(col++, 17.57);

                    //header
                    col = 7;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation", 3, back, black);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row++, col, "Information Protection", 3, mapPalatte[4][0], black);
                    col = 1;
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capability Mapping", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Domain", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Container", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability Details", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Description", bg, fg, true);
                    mapcols = col;
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: Low Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: Moderate Impact", mapPalatte[col - 9][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: High Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "PM Controls", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Low Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Moderate Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: High Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.Border(2, 1, 2, 13);
                    this.activeWorksheet.SetHeight(1, 30);
                    
                    row++;
                    foreach (string tic in tics)
                    {
                        //sort by tic
                        col = 1;
                        this.activeWorksheet.setCellTo(row, col++, tic);
                        List<Context.Capabilities> list = GetCaps(tic);
                        double sub = inc / list.Count;
                        PrintCompactCaps(list, ref row, ref col, ref total, sub, bw);

                        row++;
                    }

                    this.activeWorksheet.SetFont(12, 3, 1, row, 20);
                    this.activeWorksheet.Center(3, 1, row, 1);
                    this.activeWorksheet.Border(1, start, row - 2, start + 6);
                    this.activeWorksheet.fit(3, 3, row, 20);

                }
            }
            catch (Exception e)
            {
                string s = e.Message;

            }
            finally
            {
                CloseSaveExcel();
            }

        }

        /// <summary>
        /// get all capabilities with provided tiv
        /// </summary>
        /// <param name="tic"></param>
        /// <returns></returns>
        public List<Context.Capabilities> GetCaps(string tic)
        {
            var first = from p in dbContext.TICMappings
                        where p.TICName == tic.Trim()
                        orderby p.CapabilityId
                        select new { p.CapabilityId };
            List<Context.Capabilities> caps = new List<Context.Capabilities>();
            foreach (var info in first)
            {
                uint id = info.CapabilityId;
                var car = from p in dbContext.Capabilities
                          where p.Id == id
                          select p;
                caps.Add(car.First());
            }
            return caps;
        }
    }
}