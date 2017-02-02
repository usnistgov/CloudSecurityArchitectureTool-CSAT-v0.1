using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using ExcelReports.ExcelInteropReports;
namespace CSRC.Reports
{
    class CapabilityReport:BaseReport
    {
        protected int row = 1, col = 1;

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

                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));
                    int black = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                    //font and spacing
                    this.activeWorksheet.SetFont(14, 1, 1, 3, 50);
                    this.activeWorksheet.wrapText(1, 1, 3, 50);
                    this.activeWorksheet.Center(1, 1, 3, 50);
                    this.activeWorksheet.SetHeight(1, 54);

                    //set widths of columns
                    this.activeWorksheet.ColumnWidth(col++, 16.43);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 21.71);
                    this.activeWorksheet.ColumnWidth(col++, 33.14);
                    this.activeWorksheet.ColumnWidth(col++, 33.14);
                    this.activeWorksheet.ColumnWidth(col++, 33.14);
                    this.activeWorksheet.ColumnWidth(col++, 10.47);
                    this.activeWorksheet.ColumnWidth(col++, 30);
                    for (int i = 1; i <= 7; i++ )
                        this.activeWorksheet.ColumnWidth(col++, 17.57);
                    this.activeWorksheet.ColumnWidth(col++, 32);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    for (int i = 0; i < 4; i++)
                        this.activeWorksheet.ColumnWidth(col++, 7);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    for (int i = 0; i < 3; i++)
                        this.activeWorksheet.ColumnWidth(col++, 7);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    for (int i = 0; i < 3; i++)
                        this.activeWorksheet.ColumnWidth(col++, 7);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    for (int i = 0; i < 3; i++)
                        this.activeWorksheet.ColumnWidth(col++, 7);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    this.activeWorksheet.ColumnWidth(col++, 8.5);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    this.activeWorksheet.ColumnWidth(col++, 8.5);
                    mapPalatte = GetMapPallate();
                    domainPalette = GetDomainPalette();
                    
                    
                    col = 19;
                    this.activeWorksheet.setMergedCellTo(row, col, "Fictive Examples of Working Spreadsheet - NIST SP 500-299", 20, bg, fg); 
                    col = 10;
                    row++;
                    //header
                    this.activeWorksheet.setMergedCellTo(row,col,"Capability Implementation\nSP800-53 Rev4",3, back, black);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Information Protection\nSP800-53 Rev4", 3, mapPalatte[4][0], black); 
                    col += 5;
                    this.activeWorksheet.setMergedCellTo(row, col, "Security  Index System (example of how to use SIS)", 4, bg, fg);
                    col += 5;
                    this.activeWorksheet.setMergedCellTo(row, col, "Consumer", 3, bg, fg);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Provider", 3, bg, fg);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Broker", 3, bg, fg);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Carrier", 1, bg, fg);
                    col += 2;
                    this.activeWorksheet.setMergedCellTo(row, col, "Auditor", 1, bg, fg);

                    col = 1;
                    row++;
                    this.activeWorksheet.setCellTo(row,col++,"Domain",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Container",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Capability (proccess or solution)",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"Capability (proccess or solution)",bg,fg,true);
                    this.activeWorksheet.setCellTo(row, col++, "Description (NIST updated)", bg, fg, true);
                    this.activeWorksheet.Hide(col);
                    this.activeWorksheet.setCellTo(row, col++, "Description (from CSA)", bg, fg, true);
                    this.activeWorksheet.Hide(col);
                    this.activeWorksheet.setCellTo(row, col++, "Unique Identifier", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Scope",bg,fg,true);
                    this.activeWorksheet.setCellTo(row,col++,"TIC Capabilities Mapping",bg,fg,true);
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: Low Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: Moderate Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability Implementation: High Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "PM Controls", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Low Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Moderate Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: High Impact", mapPalatte[col - start - 1][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Notes", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "C", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "I", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "A", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "CIA", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "IaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "PaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "SaaS", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "IaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "PaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "SaaS", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "IaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "PaaS", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "SaaS", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "ALL", bg, fg, true);
                    col++;
                    this.activeWorksheet.setCellTo(row, col++, "ALL", bg, fg, true);
                    this.activeWorksheet.Border(3, 1, 3, 17);
                    this.activeWorksheet.Border(1, 19, 3, 38);

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
                        this.activeWorksheet.setCellTo(row, col++, cap.Description);
                        this.activeWorksheet.setCellTo(row, col++, cap.CSADescription);
                        this.activeWorksheet.setCellTo(row, col++, cap.UniqueId);
                        this.activeWorksheet.setCellTo(row, col++, cap.Scopes);
                        this.activeWorksheet.setCellTo(row, col++, GetTICString(cap.Id));
                        string[] maps = GetImplements(cap.Id);
                        for (int i = 0; i < maps.Length; i++)
                        {
                            this.activeWorksheet.setCellTo(row, col++, maps[i], mapPalatte[i][1], fg, false);
                        }
                        this.activeWorksheet.setCellTo(row, col++, cap.Notes);
                        col++;
                        this.activeWorksheet.setCellTo(row, col++, cap.C.ToString());
                        this.activeWorksheet.setCellTo(row, col++, cap.I.ToString());
                        this.activeWorksheet.setCellTo(row, col++, cap.A.ToString());
                        this.activeWorksheet.AddCellFormula(row, col++, "=S" + row + "+T" + row + "+U" + row);
                        col++;
                        string[] responce = cap.ResponsibilityVector.Split(',');
                        this.activeWorksheet.setCellTo(row, col++, responce[0]);
                        this.activeWorksheet.setCellTo(row, col++, responce[2]);
                        this.activeWorksheet.setCellTo(row, col++, responce[4]);
                        col++;
                        this.activeWorksheet.setCellTo(row, col++, responce[1]);
                        this.activeWorksheet.setCellTo(row, col++, responce[3]);
                        this.activeWorksheet.setCellTo(row, col++, responce[5]);
                        col++;
                        string[] OtherActors = cap.OtherActors.Split(',');
                        this.activeWorksheet.setCellTo(row, col++, OtherActors[0].ToString());
                        this.activeWorksheet.setCellTo(row, col++, OtherActors[1].ToString());
                        this.activeWorksheet.setCellTo(row, col++, OtherActors[2].ToString());
                        col++;
                        this.activeWorksheet.setCellTo(row, col++, OtherActors[3].ToString());
                        col++;
                        this.activeWorksheet.setCellTo(row, col++, OtherActors[4].ToString());
                        total += inc;
                        bw.ReportProgress((int)total);
                    }
                    if (capList.Count > 0)
                    {
                        this.activeWorksheet.fit(4, 1, row, 45);
                        this.activeWorksheet.SetFont(12, 4, 1, row, 45);
                        this.activeWorksheet.Border(2, start, row, start + 6);
                    }
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
