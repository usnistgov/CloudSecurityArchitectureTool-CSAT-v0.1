using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using ExcelReports.ExcelInteropReports;
namespace Excel2DB.Reports
{
    class CapabilityReport:BaseReport
    {

        /// <summary>
        /// asemble capability report of selected options
        /// </summary>
        /// <param name="file"></param>
        /// <param name="capList"></param>
        /// <param name="bw"></param>
        public void CreateReport(string file, List<Context.Capabilities> capList, BackgroundWorker bw)
        {
            try
            {
                if(InitNewExcelFile(file)){
                    InitDataModel();
                    int count = capList.Count;
                    double total = 0, inc = 100.0 / count;

                    int row = 1, col = 1;
                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));

                    //font and spacing
                    this.activeWorksheet.SetFont(14, 1, 1, 2, 20);
                    this.activeWorksheet.wrapText(1, 1, 2, 40);
                    this.activeWorksheet.Center(1, 1, 2, 20);
                    //set widths of columns
                    this.activeWorksheet.ColumnWidth(col++,16.43);
                    this.activeWorksheet.ColumnWidth(col++,26.71);
                    this.activeWorksheet.ColumnWidth(col++,28.29);
                    this.activeWorksheet.ColumnWidth(col++,21.71);
                    this.activeWorksheet.ColumnWidth(col++,10.47);
                    this.activeWorksheet.ColumnWidth(col++,43.14);
                    for (int i = 1; i <= 7; i++ )
                        this.activeWorksheet.ColumnWidth(col++, 17.57);

                    mapPalatte = GetMapPallate();
                    domainPalette = GetDomainPalette();

                    col = 7;
                    //header
                    this.activeWorksheet.setMergedCellTo(row,col,"Capability Implementation\nSP800-53 Rev4",3);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row,col,"Information Protection\nSP800-53 Rev4",3);
                    row++;
                    col=1;

                    this.activeWorksheet.setCellTo(row,col++,"Domain",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Container",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Capability (proccess or solution)",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Capability (proccess or solution)",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Scope",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"TIC Capabilities Mapping",bg,fg,true);
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: Low Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: Moderate Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: High Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "PM Controls", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Low Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "info protection: Moderate Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: High Impact", mapPalatte[col - 8][0], fg, false);

                    foreach (Context.Capabilities cap in capList)
                    {
                        col = 1;
                        row++;
                        bg = domainPalette[cap.Domain][0];
                        //capability info
                        this.activeWorksheet.setCellTo(row, col++, cap.Domain, bg, fg, true);
                        this.activeWorksheet.setCellTo(row, col++, cap.Container);
                        this.activeWorksheet.setCellTo(row, col++, cap.Capability);
                        this.activeWorksheet.setCellTo(row, col++, cap.Capability2);
                        this.activeWorksheet.setCellTo(row, col++, cap.Scopes);
                        this.activeWorksheet.setCellTo(row, col++, GetTICString(cap.Id));
                        string[] maps = GetImplements(cap.Id);
                        for (int i = 0; i < maps.Length; i++)
                        {
                            this.activeWorksheet.setCellTo(row, col++, maps[i], mapPalatte[i][1], fg, false);
                        }
                        total += inc;
                        bw.ReportProgress((int)total);
                    }
                    this.activeWorksheet.fit(3, 3, row, 20);
                    this.activeWorksheet.SetFont(12, 3, 1, row, 20);
                    this.activeWorksheet.Border(1, start, row, start + 6);
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
            }finally{
                CloseSaveExcel();
            }
        }
    }
}
