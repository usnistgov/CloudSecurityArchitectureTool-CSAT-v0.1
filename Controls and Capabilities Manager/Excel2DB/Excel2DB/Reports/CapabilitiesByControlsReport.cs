using ExcelReports.ExcelInteropReports;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSRC.Reports
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

                    int row = 1, col = 1;
                    int numcontrols = input.Count;
                    double total = 0, inc = 100.0 / numcontrols;
                    palette = GetPalette();
                    
                    //format header
                    this.activeWorksheet.SetFont(18, 1,1,2,20);
                    this.activeWorksheet.wrapText(1, 1, 2, 45);
                    this.activeWorksheet.Center(1, 1, 2, 45);
                    this.activeWorksheet.SetFont(14, 1, 1, 2, 45);

                    //column widths
                    this.activeWorksheet.ColumnWidth(col++, 25);
                    this.activeWorksheet.ColumnWidth(col++, 15);
                    this.activeWorksheet.ColumnWidth(col++, 16.43);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 21.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 10.47);
                    this.activeWorksheet.ColumnWidth(col++, 30);
                    col += 7;
                    this.activeWorksheet.ColumnWidth(col, 30);
                    
                    int black = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                    int fg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                    int bg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(217, 217, 217));

                    //header
                    col = 10;
                    //this.activeWorksheet.setMergedCellTo(row,col,"Capability Structure", 5, bg, fg);
                    //col += 7;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation\nSP800-53 Rev4", 3, bg, fg);
                    col += 4;
                    this.activeWorksheet.setMergedCellTo(row, col, "Information Protection\nP800-53 Rev4", 3, bg, fg);
                    row++;
                    col = 1;
                    this.activeWorksheet.setCellTo(row, col++, "Control Family", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Control", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Domain", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Container", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability (proccess or solution)", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Capability (proccess or solution)", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Unique ID", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Scope", bg, fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "TIC Capabilities Mapping", bg, fg, true);
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
                    this.activeWorksheet.setCellTo(row, col++, "Related Controls", bg, fg, true);
                    this.activeWorksheet.Hide(7); 
                    this.activeWorksheet.Border(2, 1, 2, 9);
                    this.activeWorksheet.SetHeight(1, 57);
                    this.activeWorksheet.SetHeight(2, 55);

                    familyPalette = GetFamilyPalette();
                    domainPalette = GetDomainPalette();
                    
                    foreach (string nam in input)
                    {
                        col = 1;
                        row++;
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
                    this.activeWorksheet.Border(1, start, row - 2, start + 6);
                    this.activeWorksheet.fit(3, 1, row - 1, 17);
                    this.activeWorksheet.SetFont(12, 3, 1, row - 1, 17);
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

