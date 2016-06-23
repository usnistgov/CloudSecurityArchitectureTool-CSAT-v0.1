using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSRC.Models;
using ExcelReports.ExcelInteropReports;
namespace CSRC.Reports
{
    /// <summary>
    /// contains report helper methods
    /// </summary>
    class BaseReport : Excel2Db
    {
        //Excel2Db operations4report = new Excel2Db(); 
        protected SortedList<string, int[]> domainPalette;
        protected SortedList<string, int[]> familyPalette;
        protected int[][] palette, mapPalatte;
        /// <summary>
        /// Get related controls by control ID
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        protected string[] GetRelatedControls(string controlName)
        {
            uint parentId = GetControlIdByName(controlName);
            var relatedControlsIDs =
                from rels in dbContext.Relateds
                where rels.ParentId == parentId
                select new { rels.ChildId };
            if (relatedControlsIDs.Any())
            {
                List<string> names = new List<string>();
                foreach (var id in relatedControlsIDs)
                {
                    uint theId = uint.Parse(id.ChildId.ToString());
                    names.Add(GetControlName(theId));
                }
                return names.ToArray();
            }
            else
            {
                // Make big stink!!!
                return null;
            }
        }

        /// <summary>
        /// asembles tic that are ; seperrated
        /// </summary>
        /// <param name="capId">id desired</param>
        /// <returns>tics</returns>
        protected string GetTICString(uint capId)
        {
            var ret =
                from p in dbContext.TICMappings
                where p.CapabilityId == capId
                select p;
            string tics = "";
            foreach (Context.TICMappings ticcap in ret)
            {
                tics += ticcap.TICName + ";";
            }
            return tics.Trim(new char[] { ';' });
        }

        /// <summary>
        /// Gets the ralated controls as a comma separated string
        /// </summary>
        /// <param name="controlName">The name of the control to get the RELS for</param>
        /// <returns>the comma separated string</returns>
        protected string GetRelatedControlsString(string controlName)
        {
            StringBuilder theResult = new StringBuilder(string.Empty);
            string[] names = GetRelatedControls(controlName);
            if (null != names)
            {
                foreach (string name in names)
                {
                    theResult.Append(", ");
                    theResult.Append(name);
                }
                return theResult.ToString().Remove(0, 2).Trim();
            }
            return string.Empty;
        }

        /// <summary>
        /// return list of spec under given control id
        /// </summary>
        /// <param name="control">top control id</param>
        /// <returns>list of specs</returns>
        protected List<Context.Specs> GetSpecsByControlId(uint control)
        {
            List<Context.Specs> specs = new List<Context.Specs>();
            var retval =
                from p in dbContext.Specs
                where p.ControId == control
                select p.Id;

            foreach (uint id in retval)
            {
                var spe =
                    (from sp in dbContext.Specs
                     where sp.Id == id
                     select sp);
                if (spe.Any())
                {
                    specs.Add(spe.First());
                }
            }
            return specs;
        }

        /// <summary>
        /// return full spec name
        /// </summary>
        /// <param name="specid">id desired</param>
        /// <returns>spec name</returns>
        protected string GetSpecName(uint specid)
        {
            string name = "";
            var ret =
                from p in dbContext.Specs
                where p.Id == specid
                select p;
            if (ret.Any())
            {
                Context.Specs sp = ret.First();
                name +=
                    (from p in dbContext.Controls
                     where p.Id == sp.ControId
                     select p.Name).First();
                name += sp.SpecificationlName;

            }
            return name;
        }

        /// <summary>
        /// return full spec
        /// </summary>
        /// <param name="fullName">name to search</param>
        /// <returns>full spec</returns>
        protected Context.Specs GetSpecByName(string fullName)
        {
            string control = RemoveeSpec(fullName);
            uint conid = GetControlIdByName(control);
            string spec = GetCleanTopControlName(fullName);
            uint id = GetSpecsId(conid, spec);

            var ret = from p in dbContext.Specs
                      where p.Id == id
                      select p;
            return ret.First();

        }

        /// <summary>
        /// return caps that have spec referenced
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        protected List<Context.Capabilities> GetCapabilitiesForSpecId(uint specId)
        {
            List<Context.Capabilities> needeCaps = new List<Context.Capabilities>();
            var theCapIds =
                (from map in dbContext.MapTypesCapabilitiesControls
                 //join caps in dbContext.Capabilities on (map.CapabilitiesId equals caps.Id) into capsSnap
                 orderby map.CapabilitiesId ascending
                 where (map.specId == specId && map.isControlMap == false)
                 select map.CapabilitiesId);
            foreach (uint theId in theCapIds)
            {
                var res = (from cap in dbContext.Capabilities
                           where (cap.Id == theId)
                           select cap);
                if (res.Any())
                {
                    needeCaps.Add(res.First());
                }
            }
            return needeCaps;
        }

        /// <summary>
        /// tests if name matches control pattern
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static bool isRow4Control(string name)
        {
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string test = Regex.Replace(name, pattern, "");
            return test.Length == 0;
        }

        /// <summary>
        /// Gets the matching capabilities for a control Id
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        protected List<Context.Capabilities> GetCapabilitiesForControlId(uint controlId)
        {
            List<Context.Capabilities> needeCaps = new List<Context.Capabilities>();
            var theCapIds =
                (from map in dbContext.MapTypesCapabilitiesControls
                 //join caps in dbContext.Capabilities on (map.CapabilitiesId equals caps.Id) into capsSnap
                 where (map.ControlsId == controlId && map.isControlMap == true)
                 select map.CapabilitiesId).Distinct();
            foreach (uint theId in theCapIds)
            {
                var res = (from cap in dbContext.Capabilities
                           where (cap.Id == theId)
                           select cap);
                if (res.Any())
                {
                    needeCaps.Add(res.First());
                }
            }
            return needeCaps;
        }

        /// <summary>
        ///  Gets Impacts for the given Capability and Control IDs
        /// </summary>
        /// <param name="capabilityId"></param>
        /// <param name="controlId"></param>
        /// <returns></returns>
        protected string[] GetTabbedImpactsFor(uint capabilityId, uint Id, bool iscontrol)
        {
            string[] returnData = new string[7];
            bool go = false;
            for (uint i = 1; i <= 7; i++)
            {
                if (iscontrol)
                {

                    var association =
                    (from map in dbContext.MapTypesCapabilitiesControls
                     where (map.ControlsId == Id) && (map.CapabilitiesId == capabilityId) && (map.MapTypesId == i)
                     select map.MapTypesId);
                    if (association.Any())
                    {
                        returnData[i - 1] = "X";
                        if (i != 4)
                            go = true;
                    }
                    else if (go)
                    {
                        returnData[i - 1] = "X";
                    }
                    else
                    {
                        returnData[i - 1] = "";
                    }
                    if (i == 3)
                    {
                        go = false;
                    }
                }
                else
                {

                    var association =
                    (from map in dbContext.MapTypesCapabilitiesControls
                     where (map.specId == Id) && (map.CapabilitiesId == capabilityId) && (map.MapTypesId == i)
                     select map.MapTypesId);
                    if (association.Any())
                    {
                        returnData[i - 1] = "x";
                        if (i != 4)
                            go = true;
                    }
                    else if (go)
                    {
                        returnData[i - 1] = "X";
                    }
                    else
                    {
                        returnData[i - 1] = "";
                    }
                    if (i == 3)
                    {
                        go = false;
                    }
                }
            }

            return returnData;
        }

        /// <summary>
        /// return famiily name for spec id
        /// </summary>
        /// <param name="specId"></param>
        /// <returns></returns>
        protected string GetFamilyForSpec(uint specId)
        {

            var ret =
                from p in dbContext.Specs
                where p.Id == specId
                select new { p.ControId };
            if (ret.Any())
            {
                uint id = ret.First().ControId;
                var fam =
                    from p in dbContext.Controls
                    where p.Id == id
                    select new { p.FamilyId };
                uint famid = fam.First().FamilyId;
                var name =
                    from p in dbContext.Families
                    where p.Id == famid
                    select new { p.Description };
                return name.First().Description;

            }
            return "";
        }

        protected static int[][] GetPalette()
        {
            int[][] palette = new int[][]
            { 
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(248, 203, 173))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))        // Dark Green
                },

                new int[]
                {   
                    ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 153, 0))
                  , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))
                },

                new int[]
                {   
                    ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 117, 56))
                  , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0,0,0))
                }

            };
            return palette;
        }

        protected static SortedList<string, int[]> GetFamilyPalette()
        {
            SortedList<string, int[]> palette = new SortedList<string, int[]>();
            palette.Add("AC",
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(51, 51, 153))   // Purple
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 0))   // Yellow
                });

            palette.Add("AT",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(153, 204, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });

            palette.Add("AU",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 183, 61))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("CA",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 255, 128))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("CM",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 128, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("CP",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(128 , 255, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("IA",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 255, 0))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("IR",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 128, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("MA",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(204 , 255, 204))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("MP", //Media Protection
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(210 , 180, 140))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 0))      // Yellow
                });
            palette.Add("PE", //Physical and Environmental Protection
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(204 , 0, 204))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(88, 255, 88))       // Bright green
                });
            palette.Add("PL", //Planning
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(244 , 109, 89))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("PS", //Personnel Security
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(204 , 204, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("RA", // Risk Assessment
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 204, 153))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("SA",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(204 , 128, 153))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("SC",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(153 , 0, 153))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 0))       // Yellow
                });
            palette.Add("SI",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(204 , 204, 128))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("PM",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(128 , 255, 0))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });


            return palette;
        }

        protected static SortedList<string, int[]> GetDomainPalette()
        {
            SortedList<string, int[]> palette = new SortedList<string, int[]>();
            palette.Add("BOSS",
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(153, 204, 0))   // Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))   // Yellow
                });

            palette.Add("ITOS",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(196, 215, 155))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });

            palette.Add("Presentation Services",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 204, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("Application Services",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 153, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("Information Services",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 204, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });
            palette.Add("Infrastructure Services",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0 , 153, 255))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });

            palette.Add("S & RM",
                new int[]
                {   
                      ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255 , 204, 0))
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0))       // Black
                });

            return palette;
        }

        protected int[][] GetMapPallate()
        {
            int[][] palette = new int[][]
            { 
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(252, 213, 180))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(253, 253, 217))        // Dark Green
                },new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(252, 213, 180))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(253, 253, 217))        // Dark Green
                },new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(252, 213, 180))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(253, 253, 217))        // Dark Green
                },
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(216, 238, 180))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(235, 241, 222))        // Dark Gree
                },
                new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(196, 189, 151))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(221, 217, 196))        // Dark Green
                },new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(196, 189, 151))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(221, 217, 196))        // Dark Green
                },new int[]
                {     ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(196, 189, 151))   // Light  Green
                    , ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(221, 217, 196))        // Dark Green
                }
            };
            return palette;
        }

        /// <summary>
        /// print list of capabilities 
        /// </summary>
        /// <param name="caps"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="total"></param>
        /// <param name="inc"></param>
        /// <param name="bw"></param>
        protected void PrintCompactCaps(List<Context.Capabilities> caps, ref int row, ref int col, ref double total, double inc, BackgroundWorker bw)
        {
            domainPalette = GetDomainPalette();
            foreach (Context.Capabilities cap in caps)
            {
                int domBg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                int domFg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                if (domainPalette.Keys.Contains(cap.Domain.Trim()))
                {
                    domBg = domainPalette[cap.Domain.Trim()][0];
                    domFg = domainPalette[cap.Domain.Trim()][1];
                }
                this.activeWorksheet.setCellTo(row, col++, cap.Domain, domBg, domFg);
                this.activeWorksheet.setCellTo(row, col++, cap.Container, domBg, domFg);
                this.activeWorksheet.setCellTo(row, col++, cap.Capability, domBg, domFg);
                this.activeWorksheet.setCellTo(row, col++, cap.Capability2, domBg, domFg);
                this.activeWorksheet.setCellTo(row, col++, cap.Description, domBg, domFg);

                string[] implements = GetImplements(cap.Id);
                for (int i = 0; i < implements.Length; i++)
                {
                    if (i < 3)
                    {
                        this.activeWorksheet.setCellTo(row, col++, implements[i], palette[i][0]);
                    }
                    else if (i == 3)
                    {
                        this.activeWorksheet.setCellTo(row, col++, implements[i]);
                    }
                    else
                    {
                        this.activeWorksheet.setCellTo(row, col++, implements[i], palette[i - 4][0]);
                    }


                }
                total += inc;
                bw.ReportProgress((int)total);
                row++;
                col = 2;
            }
        }

        /// <summary>
        /// asembles full list of implemented controls
        /// </summary>
        /// <param name="capId"></param>
        /// <returns></returns>
        protected string[] GetImplements(uint capId)
        {
            string[] info = new string[7] { "", "", "", "", "", "", "" };
            for (int i = 1; i <= 7; i++)
            {
                var ret = from p in dbContext.MapTypesCapabilitiesControls
                          where p.CapabilitiesId == capId && p.MapTypesId == i
                          select p;
                foreach (var data in ret)
                {
                    if (data.isControlMap)
                    {
                        var contr = from p in dbContext.Controls
                                    where p.Id == data.ControlsId
                                    select new { p.Name };
                        if (contr.Any())
                        {
                            info[i - 1] += contr.First().Name + ", ";
                        }
                    }
                    else
                    {
                        var specid = from p in dbContext.Specs
                                     where p.Id == data.specId
                                     select new { p.Id };
                        if (specid.Any())
                        {
                            info[i - 1] += GetSpecName(specid.First().Id) + ", ";
                        }
                    }
                }
                info[i - 1] = info[i - 1].Trim(new char[] { ' ' });
                info[i - 1] = info[i - 1].Trim(new char[] { ',' });
            }

            return info;
        }

        /// <summary>
        /// print x caps
        /// </summary>
        /// <param name="theCaps"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="id"></param>
        /// <param name="isControl"></param>
        /// <param name="bw"></param>
        /// <param name="inc"></param>
        /// <param name="total"></param>
        protected void PrintCaps(List<Context.Capabilities> theCaps, ref int row, ref int col, uint id, bool isControl, BackgroundWorker bw, double inc, ref double total)
        {

            foreach (Context.Capabilities cap in theCaps)
            {
                col = 3;
                // Two Color Fillers
                int domBg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(255, 255, 255));
                int domFg = ColorExtensions.TranslateToExcelColor(System.Drawing.Color.FromArgb(0, 0, 0));
                if (domainPalette.Keys.Contains(cap.Domain.Trim()))
                {
                    domBg = domainPalette[cap.Domain.Trim()][0];
                    domFg = domainPalette[cap.Domain.Trim()][1];
                }
                activeWorksheet.setCellTo(row, col++, cap.Domain, domBg, domFg, true);
                activeWorksheet.setCellTo(row, col++, cap.Container, domBg, domFg, true);
                activeWorksheet.setCellTo(row, col++, cap.Capability, domBg, domFg, true);
                if (!string.IsNullOrEmpty(cap.Capability2))
                {
                    activeWorksheet.setCellTo(row, col++, cap.Capability2, domBg, domFg, true);
                }
                else
                {
                    col++;
                }
                activeWorksheet.setCellTo(row, col++, cap.UniqueId);
                activeWorksheet.setCellTo(row, col++, cap.Scopes);
                activeWorksheet.setCellTo(row, col++, GetTICString(cap.Id));

                string[] impacts = GetTabbedImpactsFor(cap.Id, id, isControl);
                int dc = 0;
                foreach (string tap in impacts)
                {
                    int localCol = 8 + dc;
                    if (localCol >= 8 && localCol <= 10)
                    {
                        activeWorksheet.setCellTo(row, col++, tap, palette[dc][0], palette[dc][1], true);
                    }
                    else if (localCol >= 12 && localCol <= 14)
                    {
                        activeWorksheet.setCellTo(row, col++, tap, palette[dc - 4][0], palette[dc - 4][1], true);
                    }
                    else
                    {
                        activeWorksheet.setCellTo(row, col++, tap, 0, true);
                    }
                    dc++;
                }

                //The very last thing to do is to begin a new row
                row++;

                total += inc;
                bw.ReportProgress((int)total);
            }
        }
    }
}