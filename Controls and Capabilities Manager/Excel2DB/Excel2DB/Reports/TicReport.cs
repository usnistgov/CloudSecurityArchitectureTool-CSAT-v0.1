using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2DB.Reports
{
    class TicReport:BaseReport
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
                      select new { p.TICName} ).Distinct();
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
                    this.activeWorksheet.SetFont(18, 1, 1, 2, 20);
                    List<string> tics = GetTics();

                    //header
                    double total = 0, inc = 100.0 / tics.Count;
                    palette = GetPalette();
                    int row = 1, col = 1;
                    this.activeWorksheet.setMergedCellTo(row, ++col, "Capability Structure", 5);
                    col += 5;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation", 3);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row++, col, "Impormation Protection", 3);
                    col = 1;
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capability Mapping");
                    this.activeWorksheet.setCellTo(row, col++, "Domain");
                    this.activeWorksheet.setCellTo(row, col++, "Container");
                    this.activeWorksheet.setCellTo(row, col++, "Capability");
                    this.activeWorksheet.setCellTo(row, col++, "Capability Details");
                    this.activeWorksheet.setCellTo(row, col++, "Description");
                    mapcols = col;
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Low");
                    this.activeWorksheet.setCellTo(row, col++, "Med.");
                    this.activeWorksheet.setCellTo(row, col++, "High");
                    this.activeWorksheet.setCellTo(row, col++, "PM");
                    this.activeWorksheet.setCellTo(row, col++, "Low");
                    this.activeWorksheet.setCellTo(row, col++, "Med.");
                    this.activeWorksheet.setCellTo(row, col++, "High");

                    
                    row++;
                    int bg, fg;
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
                    excelSource.fit();

                    for (int i = 0; i < 7; i++)
                    {
                        this.activeWorksheet.ColumnWidth(mapcols + i, 18);
                    }
                    this.activeWorksheet.ColumnWidth(4, 28);
                    this.activeWorksheet.ColumnWidth(5, 22);
                    this.activeWorksheet.ColumnWidth(6, 33);
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
