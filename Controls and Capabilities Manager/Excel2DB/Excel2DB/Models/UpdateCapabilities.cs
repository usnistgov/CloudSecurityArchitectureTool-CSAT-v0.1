using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ExcelReports.ExcelInteropReports;


namespace Excel2DB.Models
{
    /// <summary>
    /// Reads and parses capabilities file and put data into database
    /// </summary>
    class UpdateCapabilities : Excel2Db
    {
        public string ExcelFileName { get; set; }
        public string DatabaseConnection { get; set; }
        
        
        public UpdateCapabilities(string theFile) {
            ExcelFileName = theFile;
        }

        /// <summary>
        /// finds the end of the opened file denoted by a blank line
        /// </summary>
        /// <returns>row number of last full row</returns>
        public int findEnd()
        {
            int row = 5000, lastKnownFull = 1, firstKnowEmpy = 0;
            bool end = false;
            while (!end)
            {
                string[] rowData = ReadExcelRow(row, 37);
                //clear data on bottom of file
                rowData[0] = string.Empty;
                rowData[1] = string.Empty;
                rowData[2] = string.Empty;

                if (isRowCompletelyEmpty(rowData))
                {
                    firstKnowEmpy = row;
                }
                else
                {
                    lastKnownFull = row;
                }

                if (lastKnownFull == firstKnowEmpy - 1)
                {
                    return firstKnowEmpy;
                }

                row = (int)((firstKnowEmpy + lastKnownFull) / 2.0);
            }
            return row;
        }

        protected List<Context.Capabilities> listCapabilities;
        protected List<Context.MapTypesCapabilitiesControls> listMap;
        protected List<Context.TICMappings> TICFinal;
        protected SortedList<string, List<Context.TICMappings>> listTIC;

        /// <summary>
        /// reads file, updates progress and inputs data
        /// </summary>
        /// <param name="bw">background worker for event from mainwindow</param>
        /// <returns></returns>
        public void ProcessFile(BackgroundWorker bw){
            try
            {
                int firstRow = 4;
                int lastRow = 100000;
                double val = 0, inc = 0; // inc = 0.287; // inc = 100.0/350;
                if (InitExcelFile(ExcelFileName))
                {
                    row2Finish = findEnd();
                    inc = 50.0 / row2Finish;
                    InitDataModel();
                    //linesCount = OldParseFileMethod(linesCount, tableCaps, theModel, firstRow, lastRow);
                    bool isLineSpecsOnly = false;
                    hitCompletelyEmptyRow = false;
                    // Loop over the Excel lines

                    //call function to pull column names 
                    int currentRow = Constants.capFirstRow;
                  
                    while (currentRow<this.row2Finish)
                    {

                        string[] rowData = this.ReadExcelRow(currentRow,37);
                        //hitCompletelyEmptyRow = ProcessNextLine(currentRow++, out currentLine);
                        if (rowData[Constants.colIncluded].ToLower() == "y")
                        {
                            AddEntryToCapabilities(rowData);
                        }
                        val += inc;
                        bw.ReportProgress((int)val);
                        currentRow++;
                    }
                }
                dbContext.Capabilities.InsertAllOnSubmit(listCapabilities);
                dbContext.SubmitChanges();

                //pop cap tics
                PopulateTICData();
                dbContext.TICMappings.InsertAllOnSubmit(TICFinal);
                                
                //Add all the applicable entities to the map 
                PopulateMapData(bw, val, inc);
                dbContext.MapTypesCapabilitiesControls.InsertAllOnSubmit(listMap);
                
                
                dbContext.SubmitChanges();
                bw.ReportProgress(100);
            }// end-of-try-block
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                CloseExcel();
            }
        }

        /// <summary>
        /// matchs tic to capability ids
        /// </summary>
        private void PopulateTICData()
        {
            if (TICFinal == null) TICFinal = new List<Context.TICMappings>();
            foreach (KeyValuePair<string, List<Context.TICMappings>> pair in listTIC)
            {
                var ret =
                    from p in dbContext.Capabilities
                    where p.UniqueId == pair.Key
                    select new { p.Id };
                if(ret.Any()){
                    uint capid = ret.First().Id;
                    foreach(Context.TICMappings tic in pair.Value){
                        tic.CapabilityId = capid;
                        TICFinal.Add(tic);
                    }
                }
            }
        }
        
        /// <summary>
        /// Populates the Map Controls-Capabilities Information
        /// </summary>
        private void PopulateMapData(BackgroundWorker bw, double atotal, double inc)
        {

            if (null == listMap) listMap = new List<Context.MapTypesCapabilitiesControls>();
            int currRow = this.row2Begin;
            while (currRow < this.row2Finish)
            {
                atotal += inc;
                bw.ReportProgress((int)atotal);

                string[] rowData = this.ReadExcelRow(currRow, 37);
                if(rowData[Constants.colIncluded].ToLower() == "y")
                    AddEntrytoMap(currRow, rowData, GetCapabilityFromRow(rowData));
                currRow++;
            }
        }
        
        
        /// <summary>
        /// Create all necessary antities for the given row information
        /// </summary>
        /// <param name="currRow"></param>
        /// <param name="rowData"></param>
        /// <param name="currentCapability"></param>
        private void AddEntrytoMap(int currRow, string[] rowData, Context.Capabilities currentCapability)
        {
            for (int col = Constants.colInfoLow; col < Constants.colInfoLow + 7; col++)
            {
                // 14,15,16 - Capability Implementation (Low(1,1), Moderate(1,2), Hight(1,3)) correspondingly
                // 17 - PM Controls (2,1)
                // 18, 19, 20 - Information Protection (Low(3,1), Moderate(3,2), Hight(3,3)) correspondingly
                uint mapTypeId = (uint)(col - Constants.colInfoLow + 1 ); // Tentative logic loock-up would be much safer    
                uint capId = GetCapabilityIdByUniqueId(currentCapability.UniqueId);
                string[] controls = GetCleanListOfControls(rowData[col].Replace(" ","")); // !!!!!Important to have the dynamic COL instead of the static column number
                foreach (string controlName in controls)
                {
                    try
                    {
                        string topControl = RemoveeSpec(controlName);

                        uint controlId = GetControlIdByName(topControl);
                        uint specid = GetSpecsId(controlId, GetCleanTopControlName(controlName));

                        if (mapTypeId > 0 && capId > 0 && (controlId > 0 || specid > 0))
                        {
                            bool isControl;
                            if (specid == 0)
                            {
                                isControl = true;
                                specid = 1;
                            }
                            else
                                isControl = false;
                            listMap.Add(new Context.MapTypesCapabilitiesControls
                            {
                                CapabilitiesId = capId,
                                ControlsId = controlId,
                                MapTypesId = mapTypeId,
                                specId = specid,
                                isControlMap = isControl
                            });
                        }
                        else
                        {
                            // MAKE STINK ABOUT UNMAPPABLE DATA!!!
                            ReportErrorTrace("Excel cell @row=" + currRow.ToString()
                                + " @col=" + col.ToString()
                                + " had unparsable combination : {mapType=" + mapTypeId.ToString()
                                + ", Cap=" + currentCapability.UniqueId
                                + ", CapId=" + capId
                                + ", Cntrl=" + controlName
                                + ", CntrlId=" + controlId
                                + "}"
                                );
                        }
                    }
                    catch (Exception x)
                    {
                        ReportErrorTrace("Excel cell @row=" + currRow.ToString()
                            + " @col=" + col.ToString()
                            + " had unparsable combination : {mapType=" + mapTypeId.ToString()
                            + ", Cap=" + currentCapability.UniqueId
                            + ", CapId=" + capId
                            + ", Cntrl=" + controlName
                            + "}"
                            );
                    }
                }


            }
        }

        /// <summary>
        /// pulls out all capability info from row
        /// </summary>
        /// <param name="rowData"> capability row</param>
        /// <returns></returns>
        private Context.Capabilities GetCapabilityFromRow(string[] rowData) 
        {
            Context.Capabilities currentCapabilities
                = new Context.Capabilities
                {
                    Domain = rowData[Constants.colDomain],
                    Container = rowData[Constants.colContianer],
                    Capability = rowData[Constants.colCapability],
                    Capability2 = rowData[Constants.colCapability2],
                    FamilyId = GetFamilyIdByFamilyPrefix(rowData[Constants.colCapFamily]),             //PROBEM NO FAMILY ID IN CURRENT VERSION
                    UniqueId = rowData[Constants.colIdentifier],
                    Description = rowData[Constants.colDescription],
                    Notes = rowData[Constants.colNotes],
                    C= uint.Parse(rowData[Constants.colCIAC]),
                    I = uint.Parse(rowData[Constants.colCIAC+1]),
                    A = uint.Parse(rowData[Constants.colCIAC+2]),
                    Scopes = rowData[Constants.colScope],
                    ResponsibilityVector = GetResponceVector(rowData)
                };
            return currentCapabilities;
        }

        private string GetResponceVector(string[] rowdata)
        {
            string vector = "";
            vector += rowdata[Constants.colCapVector];
            vector += rowdata[Constants.colCapVector + 4];
            vector += "," +rowdata[Constants.colCapVector + 1];
            vector += rowdata[Constants.colCapVector + 5];
            vector += "," + rowdata[Constants.colCapVector + 2];
            vector += rowdata[Constants.colCapVector + 6];
            return vector;
        }
        /// <summary>
        /// inserts tic data into temparary list to be finished later
        /// </summary>
        /// <param name="rowData">full capability row</param>
        /// <param name="capName">name of current capability</param>
        private void AddTICs(string[] rowData, string capName)
        {
            if (listTIC == null) listTIC = new SortedList<string, List<Context.TICMappings>>();
            string[] tic = rowData[Constants.colTIC].Replace(" ", string.Empty).Split(new char[] { ';' ,'\n', ','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string ticCap in tic)
            {
                Context.TICMappings ticdata = new Context.TICMappings
                {
                    TICName = ticCap
                };
                if(!listTIC.ContainsKey(capName)) listTIC[capName] = new List<Context.TICMappings>();
                listTIC[capName].Add(ticdata);
            }
        }

        /// <summary>
        /// handles new capabilities
        /// </summary>
        /// <param name="rowData">entire capability row</param>
        private void AddEntryToCapabilities(string[] rowData)
        {
            if (null == listCapabilities) listCapabilities = new List<Context.Capabilities>();

            Context.Capabilities currentCapabilities = GetCapabilityFromRow(rowData);
            AddTICs(rowData, currentCapabilities.UniqueId);

            if (0 == currentCapabilities.FamilyId)
            {
                ReportErrorTrace("Capabilities parser: family id for family " + rowData[Constants.colCapFamily] + " not found");
                return;
            }
            else
            {
                listCapabilities.Add(currentCapabilities);
            }
        }
    }
}
