using ExcelReports.ExcelInteropReports;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Excel2DB.Reports
{
    class CapabilitiesByControlsReport: BaseReport

    {
        /// <summary>
        /// COnstructor of the class
        /// </summary>
        /// <param name="reportFileName"></param>
        public CapabilitiesByControlsReport(string reportFileName)
        { 
        }

        /// <summary>
        /// Actual work 
        /// </summary>
        public void CreateReport(string excelReport2Create, List<string> input, BackgroundWorker bw) 
        {
            
            try
            {
                if (InitNewExcelFile(excelReport2Create))
                {
                    InitDataModel();
                    var families = (from fams in dbContext.Families
                                    select fams).ToList<Context.Families>().ToLookup(x => x.Id, x => x);

                    var controls = (from ctrl in dbContext.Controls
                                    select ctrl).ToList<Context.Controls>();
                    int numcontrols = input.Count;
                    double total = 0, inc = 100.0 / numcontrols;
                    this.activeWorksheet.SetFont(18, 1,1,2,20);
                    palette = GetPalette();

                    //header
                    int row = 1, col = 3;
                    this.activeWorksheet.setMergedCellTo(row,col,"Capability Structure", 5);
                    col += 7;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation\nSP800-53 Rev4", 3);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Information Protection\nP800-53 Rev4", 3);
                    row++;
                    col = 1;
                    this.activeWorksheet.setCellTo(row, col++, "Control Family");
                    this.activeWorksheet.setCellTo(row, col++, "Control");
                    this.activeWorksheet.setCellTo(row, col++, "Domain");
                    this.activeWorksheet.setCellTo(row, col++, "Container");
                    this.activeWorksheet.setCellTo(row, col++, "Capability");
                    this.activeWorksheet.setCellTo(row, col++, "Capability Details");
                    this.activeWorksheet.setCellTo(row, col++, "Unique ID");
                    this.activeWorksheet.setCellTo(row, col++, "Scope");
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capabilities Mapping");
                    int start = col;
                    // Scope and Impact Levels
                    this.activeWorksheet.setCellTo(row, col++, "Low", palette[0][0], palette[0][1], true);
                    this.activeWorksheet.setCellTo(row, col++, "Mod.", palette[1][0], palette[1][1], true);
                    this.activeWorksheet.setCellTo(row, col++, "High", palette[2][0], palette[2][1], true);
                    // PM Control
                    this.activeWorksheet.setHeaderCellTo(row, col++, "PM");
                    // Scope and Impact Levels

                    this.activeWorksheet.setCellTo(row, col++, "Low", palette[0][0], palette[0][1], true);
                    this.activeWorksheet.setCellTo(row, col++, "Mod.", palette[1][0], palette[1][1], true);
                    this.activeWorksheet.setCellTo(row, col++, "High", palette[2][0], palette[2][1], true);
                    // Relateds
                    this.activeWorksheet.setHeaderCellTo(row, col++, "Related Controls");
                    
                    familyPalette = GetFamilyPalette();
                    domainPalette = GetDomainPalette();
                    int bg;
                    int fg;
                    
                    //=======================================================================
                    //                      Main Loop of the Report
                    //=======================================================================
                    foreach (string nam in input)
                    {
                        col = 1;
                        row++;
                        bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 255, 255));
                        fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                        //print control info
                        if (isRow4Control(nam))
                        {
                            uint id = GetControlIdByName(nam);
                            var x = from p in dbContext.Controls
                                    where p.Id == id
                                    select p;
                            Context.Controls control = x.First();

                            bg = familyPalette[families[control.FamilyId].First().Name.Trim()][0];
                            fg = familyPalette[families[control.FamilyId].First().Name.Trim()][1];

                            this.activeWorksheet.setCellTo(row, col++, families[control.FamilyId].First().Description.Trim(), bg, fg, true);
                            this.activeWorksheet.setCellTo(row, col++, control.Name, bg, fg, true);
                            List<Context.Capabilities> theCaps = this.GetCapabilitiesForControlId(control.Id);
                            this.activeWorksheet.setCellTo(row, 17, GetRelatedControlsString(control.Name));
                            if (theCaps.Count == 0)
                            {
                                total += inc;
                                bw.ReportProgress((int)total);
                            }
                            else
                            {
                                double capinc = inc / theCaps.Count;

                                PrintCaps(theCaps, ref row, ref col, control.Id, true, bw, capinc, ref total);
                            }
                        }
                        else
                        {
                            //spec info
                            Context.Specs spec = GetSpecByName(nam);
                            bg = familyPalette[GetFamilyNameForSpec(spec)][0];
                            fg = familyPalette[GetFamilyNameForSpec(spec)][1];

                            this.activeWorksheet.setCellTo(row, col++, GetFamilyForSpec(spec.Id), bg, fg, true);
                            this.activeWorksheet.setCellTo(row, col++, GetSpecName(spec.Id), bg, fg, true);

                            List<Context.Capabilities> caps = GetCapabilitiesForSpecId(spec.Id);
                            if (caps.Count == 0)
                            {
                                total += inc;
                                bw.ReportProgress((int)total);
                            }
                            else
                            {
                                double capinc = inc / caps.Count;

                                PrintCaps(caps, ref row, ref col, spec.Id, false, bw, capinc, ref total);
                            }
                        }
                        row++;

                        
                    }
                    bw.ReportProgress(100);
                    this.activeWorksheet.SetFont(12, 3, 1, row, 20);
                    this.activeWorksheet.wrapText(1, 1, 1, 20);
                    excelSource.fit();
                    this.activeWorksheet.SetHeight(1, 70);
                    this.activeWorksheet.Hide(7);
                    this.activeWorksheet.Border(1, start, row - 2, start + 6);
                }
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            finally
            {
                CloseSaveExcel();
            }
        }

        
        
    }
}

