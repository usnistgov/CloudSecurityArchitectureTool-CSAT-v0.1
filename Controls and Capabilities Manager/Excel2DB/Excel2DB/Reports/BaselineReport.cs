using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using ExcelReports.ExcelInteropReports;

namespace CSRC.Reports
{
    class BaselineReport : BaseReport
    {
        public void CreateReport(string fileName, List<Context.Capabilities> caps, BackgroundWorker wd)
        {
            try
            {
                if (InitNewExcelFile(fileName))
                {
                    InitDataModel();

                    int row = 1, col = 1;
                    double total = 0, inc = 100.0 / caps.Count;
                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));

                    mapPalatte = GetMapPallate();
                    domainPalette = GetDomainPalette();
                    //font and spacing
                    this.activeWorksheet.wrapText(1, 1, 3, 45);
                    this.activeWorksheet.Center(1, 1, 3, 45);
                    this.activeWorksheet.SetFont(14, 1, 1, 3, 45);
                    
                    //set widths of columns
                    this.activeWorksheet.ColumnWidth(col++, 16.43);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 21.71);
                    this.activeWorksheet.ColumnWidth(col++, 33.14);
                    this.activeWorksheet.ColumnWidth(col++, 10.47);
                    this.activeWorksheet.ColumnWidth(col++, 23.47);
                    for (int i = 1; i <= 16; i++)
                        this.activeWorksheet.ColumnWidth(col++, 17.57);
                    this.activeWorksheet.ColumnWidth(col++, 32);
                    this.activeWorksheet.ColumnWidth(col++, 2);
                    for (int i = 0; i < 4; i++ )
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
                    this.activeWorksheet.ColumnWidth(col++, 8);
                    this.activeWorksheet.ColumnWidth(col++, 3);
                    this.activeWorksheet.ColumnWidth(col++, 8.5);
                    col = 8;
                    //header
                    int back = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(252, 238, 214));
                    int black = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation\nSP800-53 Rev4", 12, back, black);
                    col += 18;
                    this.activeWorksheet.setMergedCellTo(row, col, "Fictive Examples of Working Spreadsheet - NIST SP 500-299", 20, bg, fg);
                    row++;
                    col = 8;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: Low Impact", 4, back, black);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: Medium Impact", 4, back, black);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: High Impact", 4, back, black);
                    col += 5;
                    this.activeWorksheet.setMergedCellTo(row, col, "Info Protection", 3, mapPalatte[4][0], black);

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
                    this.activeWorksheet.setCellTo(row, col++, "Domain", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Container", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability (proccess or solution)", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability (proccess or solution)", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Description", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Scope", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capabilities Mapping", bg, fg, true);
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Nist Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Nist Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Nist Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Additional Recommended Controls", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "PM Controls", mapPalatte[3][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Low Info Protection", mapPalatte[4][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Moderate Info Protection", mapPalatte[4][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "High Info Protection", mapPalatte[4][0], fg, true);
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

                    foreach (Context.Capabilities cap in caps)
                    {
                        row++;
                        col = 1;

                        this.activeWorksheet.setCellTo(row, col++, cap.Domain, domainPalette[cap.Domain][0], fg, true);
                        this.activeWorksheet.setCellTo(row, col++, cap.Container);
                        this.activeWorksheet.setCellTo(row, col++, cap.Capability);
                        this.activeWorksheet.setCellTo(row, col++, cap.Capability2);
                        this.activeWorksheet.setCellTo(row, col++, cap.Description);
                        this.activeWorksheet.setCellTo(row, col++, cap.Scopes);
                        this.activeWorksheet.setCellTo(row, col++, GetTICString(cap.Id));

                        string[] maps = securityComponents(cap.Id, ref row, ref col);
                        for (int i = 0; i < maps.Length; i++)
                        {
                            int color = 0;
                            if (i < 12)
                            {
                                color = mapPalatte[0][1];
                            }
                            else
                            {
                                color = mapPalatte[i - 9][1];
                            }
                            this.activeWorksheet.setCellTo(row, col++, maps[i], color, fg, false);
                        }
                        this.activeWorksheet.setCellTo(row, col++, cap.Notes);
                        col++;
                        this.activeWorksheet.setCellTo(row, col++, cap.C.ToString());
                        this.activeWorksheet.setCellTo(row, col++, cap.I.ToString());
                        this.activeWorksheet.setCellTo(row, col++, cap.A.ToString());
                        this.activeWorksheet.AddCellFormula(row, col++, "=Z" + row + "+AA" + row + "+AB" + row);
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
                        wd.ReportProgress((int)total);
                    }
                    if (caps.Count > 0)
                    {
                        this.activeWorksheet.fit(4, 1, row, 21);
                        this.activeWorksheet.SetFont(12, 4, 1, row, 60);
                        this.activeWorksheet.Border(1, start, row, start + 15);
                    }
                    wd.ReportProgress(100);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                CloseSaveExcel();
            }

        }

    }
}