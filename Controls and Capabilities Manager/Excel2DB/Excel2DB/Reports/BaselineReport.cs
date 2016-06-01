using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using ExcelReports.ExcelInteropReports;

namespace Excel2DB.Reports
{
    class BaselineReport:BaseReport
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
                    this.activeWorksheet.SetFont(14, 1, 1, 3, 20);
                    this.activeWorksheet.wrapText(1, 1, 3, 40);
                    this.activeWorksheet.Center(1, 1, 3, 20);
                    //set widths of columns
                    this.activeWorksheet.ColumnWidth(col++, 16.43);
                    this.activeWorksheet.ColumnWidth(col++, 26.71);
                    this.activeWorksheet.ColumnWidth(col++, 28.29);
                    this.activeWorksheet.ColumnWidth(col++, 21.71);
                    this.activeWorksheet.ColumnWidth(col++, 33.14); 
                    this.activeWorksheet.ColumnWidth(col++, 10.47);
                    this.activeWorksheet.ColumnWidth(col++, 23.47);
                    
                    for (int i = 1; i <= 9; i++)
                        this.activeWorksheet.ColumnWidth(col++, 17.57);

                    col = 8;
                    //header
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability Implementation\nSP800-53 Rev4", 9);

                    row++;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: Low Impact", 3);
                    col += 3;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: Medium Impact", 3);
                    col += 3;
                    this.activeWorksheet.setMergedCellTo(row, col, "Capability implementation: High Impact", 3);

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
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Entire Security Component", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Nist Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Entire Security Component", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Nist Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "FedRAMP Baseline", mapPalatte[0][0], fg, true);
                    this.activeWorksheet.setCellTo(row, col++, "Entire Security Component", mapPalatte[0][0], fg, true);

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

                        string[] components = GetBaselineDivision(cap.Id);
                        foreach (string component in components)
                        {
                            this.activeWorksheet.setCellTo(row, col++, component, mapPalatte[0][1], fg, false);
                        }
                        total += inc;
                        wd.ReportProgress((int)total);
                    }
                    this.activeWorksheet.fit(4, 3, row, 20);
                    this.activeWorksheet.SetFont(12, 4, 1, row, 20);
                    this.activeWorksheet.Border(1, start, row, start + 8);
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

        private string[] GetBaselineDivision(uint id)
        {
            string[] components = new string[9]{"","","","","","","","",""};
            string[] implements = GetImplements(id);
            for (int i = 1; i <= 3; i++)
            {
                List<string> controlSpecs = new List<string>();
                var ret = from p in dbContext.MapTypesCapabilitiesControls
                          where p.MapTypesId == i && p.CapabilitiesId == id
                          select p;
                components[3 * i - 1] = implements[i - 1];
                foreach (var rec in ret)
                {
                    if (rec.isControlMap)
                    {
                        var contr = from p in dbContext.Controls
                                    where p.Id == rec.ControlsId
                                    select new { p.Name };
                        controlSpecs.Add(contr.First().Name);
                    }
                    else
                    {
                        var sp = from p in dbContext.Specs
                                 where p.Id == rec.specId
                                 select new { p.ControId, p.SpecificationlName };
                        uint conId = sp.First().ControId;

                        var top = from p in dbContext.Controls
                                  where p.Id == conId
                                  select new { p.Name };
                        string name = top.First().Name + sp.First().SpecificationlName;
                        controlSpecs.Add(name);
                    }
                }
                
                foreach(string name in controlSpecs){
                    bool iscontr;
                    uint contrid, specid;
                    if (isRow4Control(name))
                    {
                        iscontr = true;
                        contrid = GetControlIdByName(name);
                        specid = 1;
                    }
                    else
                    {
                        iscontr=false;
                        contrid=1;
                        specid=GetSpecIdByName(name);
                    }
                    var data = from p in dbContext.BaselineSecurityMappings
                              where p.IsControlMap == iscontr && p.ControlsId == contrid && p.SpecsId == specid && p.Level == i
                              select p;
                    int startCol = 3 * (i-1);
                    if(data.Any()){
                        foreach(var rec in data){
                            components[startCol + rec.BaselineAuthor - 1] += name + ", ";
                        }
                    }
                }
            }
            for(int i = 0; i < components.Length; i++){
                components[i].Trim();
                components[i].Trim(',');
            }

                return components;
        }
    }
}
