using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelReports.ExcelInteropReports;

namespace CSRC.Reports
{
    /// <summary>
    /// input to visio
    /// </summary>
    class VisioReport:BaseReport
    {
        public void CreateReport(string file, List<string> controlsList, BackgroundWorker bw)
        {
            try
            {
                if(InitNewExcelFile(file)){
                    InitDataModel();
                    int row = 1, col = 1;
                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));

                    //font and spacing
                    this.activeWorksheet.SetFont(14, 1, 1, 1, 20);
                    this.activeWorksheet.wrapText(1, 1, 1, 40);
                    this.activeWorksheet.Center(1, 1, 1, 20);
                    //set widths of columns
                    this.activeWorksheet.ColumnWidth(col++, 50);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    col = 7;
                    for (int i = 1; i <= 7; i++)
                        this.activeWorksheet.ColumnWidth(col++, 17.57);
                    this.activeWorksheet.ColumnWidth(col++, 20);
                    this.activeWorksheet.ColumnWidth(col++, 25);

                    mapPalatte = GetMapPallate();
                    domainPalette = GetDomainPalette();

                    col = 1;

                    this.activeWorksheet.setCellTo(row, col++, "Capablity Unique Id", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capabilities Mapping", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "C", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "I", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "A", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "CIA", bg, fg, true);
                    int start = col;
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: Low Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: Moderate Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Capability implementation: High Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "PM Controls", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: Low Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "info protection: Moderate Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Info protection: High Impact", mapPalatte[col - 8][0], fg, false);
                    this.activeWorksheet.setCellTo(row, col++, "Aggregated Security Controls", bg, fg, true); 
                    this.activeWorksheet.setCellTo(row, col++, "Security Controls Families", bg, fg, true);

                    var ret = from p in dbContext.Capabilities
                              select p;

                    double total = 0, inc = 100.0 / ret.Count();
                    List<string> fam = new List<string>();
                    List<string> controlAG = new List<string>();
                    foreach (Context.Capabilities set in ret)
                    {
                        row++;
                        col = 1;
                        fam.Clear();
                        controlAG.Clear();
                        this.activeWorksheet.setCellTo(row, col++, set.UniqueId);
                        uint capid = set.Id;
                        string[] controls = new string[7]{"","","","","","",""};
                        for (int i = 1; i <= 7; i++)
                        {
                            var query = from p in dbContext.MapTypesCapabilitiesControls
                                      where p.CapabilitiesId == capid && p.MapTypesId == i
                                      select p;
                            foreach (var pack in query)
                            {
                                string nam;
                                if (pack.isControlMap)
                                {
                                    uint id = pack.ControlsId;
                                    
                                    nam = GetControlName(id);
                                }else{
                                    uint id = pack.specId;
                                    nam=GetSpecName(id);
                                }
                                if (controlsList.Contains(nam))
                                {
                                    controls[i - 1] += nam + ",";
                                    if (!controlAG.Contains(nam))
                                    {
                                        controlAG.Add(nam);
                                    }
                                    string family = nam.Substring(0, 2);
                                    if (!fam.Contains(family))
                                    {
                                        fam.Add(family);
                                    }
                                }
                            }
                        }
                        string families = "";
                        foreach (string s in fam)
                        {
                            families += s + ",";
                        }
                        families = families.Trim(',');

                        string ag = "";
                        foreach (string a in controlAG)
                        {
                            ag += a + ",";
                        }
                        ag = ag.Trim(',');
                        //pull data to file
                        this.activeWorksheet.setCellTo(row, col++, GetTICString(set.Id));
                        this.activeWorksheet.setCellTo(row, col++, set.C.ToString());
                        this.activeWorksheet.setCellTo(row, col++, set.I.ToString());
                        this.activeWorksheet.setCellTo(row, col++, set.A.ToString());
                        this.activeWorksheet.AddCellFormula(row, col++, "=D" + row + "+E" + row + "+C" + row);
                        
                        for (int i = 0; i < 7; i++)
                        {
                            this.activeWorksheet.setCellTo(row, col++, controls[i].Trim(','),mapPalatte[i][1],false);
                        }
                        this.activeWorksheet.setCellTo(row, col++, ag);
                        this.activeWorksheet.setCellTo(row, col++, families);

                        total += inc;
                        bw.ReportProgress((int)total);
                        
                    }
                    //format
                    this.activeWorksheet.SetFont(12, 2, 1, row, 20);
                    this.activeWorksheet.Border(2, start, row, start + 6);
                    this.activeWorksheet.fit(2, start, row, start + 6);
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
