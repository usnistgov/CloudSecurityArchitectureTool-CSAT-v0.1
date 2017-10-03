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


namespace CSRC.Models
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
        protected int implementationStart;
        protected bool implementation3col;
        protected int uniqueIdCol;
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
                    int currentRow = 4;
                  
                    while (currentRow<this.row2Finish)
                    {

                        string[] rowData = this.ReadExcelRow(currentRow,50);
                        //hitCompletelyEmptyRow = ProcessNextLine(currentRow++, out currentLine);
                        AddEntryToCapabilities(rowData);
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

        private void PopulateMapData(BackgroundWorker bw, double atotal, double inc)
        {

            if (null == listMap) listMap = new List<Context.MapTypesCapabilitiesControls>();
            int currRow = 4;
            while (currRow < this.row2Finish)
            {
                atotal += inc;
                bw.ReportProgress((int)atotal);

                string[] rowData = this.ReadExcelRow(currRow, 50);
                AddEntrytoMap(rowData);
                currRow++;
            }
        }

        private void AddEntrytoMap(string[] rowData)
        {
            var que = from p in dbContext.Capabilities
                      where p.UniqueId == rowData[uniqueIdCol]
                      select p;
            uint capId = que.First().Id;

            for (uint level = 1; level <= 7; level++)
            {
                string implementList = "";
                if (implementation3col)
                {
                    implementList = rowData[implementationStart + (level - 1)];
                }
                else
                {
                    if (level <= 3)
                    {
                        implementList = rowData[implementationStart + 4 * (level - 1)];
                        implementList += "," + rowData[implementationStart + 4 * (level - 1) + 1];
                    }
                    else
                    {
                        int start = implementationStart + 12;
                        implementList = rowData[start + (level - 4)];
                    }
                }
                string[] controls = GetCleanListOfControls(implementList.Replace(" ", ""));
                foreach (string controlName in controls)
                {
                    try
                    {  
                        string topControl = RemoveeSpec(controlName);

                        uint controlId = GetControlIdByName(topControl);
                        uint specid = GetSpecsId(controlId, GetCleanTopControlName(controlName));

                        if (controlId > 0 || specid > 0)
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
                                MapTypesId = level,
                                specId = specid,
                                isControlMap = isControl
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                }
            }
        }

        private void SaveTics(string tics, string uniqueId)
        {
            if (listTIC == null) listTIC = new SortedList<string, List<Context.TICMappings>>();
            string[] entries = tics.Replace(" ", string.Empty).Split(new char[] { ';', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ticCap in entries)
            {
                Context.TICMappings ticdata = new Context.TICMappings
                {
                    TICName = ticCap
                };
                if (!listTIC.ContainsKey(uniqueId)) listTIC[uniqueId] = new List<Context.TICMappings>();
                listTIC[uniqueId].Add(ticdata);
            }

        }

        private string GetResponceVector(string[] rowdata, int col)
        {
            string vector = "";
            vector += rowdata[col];
            vector += "," + rowdata[col + 4];
            vector += "," + rowdata[col + 1];
            vector += "," + rowdata[col + 5];
            vector += "," + rowdata[col + 2];
            vector += "," + rowdata[col + 6];
            return vector;
        }

        private string GetOtherActors(string[] rowdata, ref int col)
        {
            string oths = "";
            oths += rowdata[col++];
            oths += ',' + rowdata[col++];
            oths += ',' + rowdata[col++];
            col++;
            oths += ',' + rowdata[col++];
            col++;
            oths += ',' + rowdata[col++];
            return oths;
        }

        /// <summary>
        /// handles new capabilities
        /// </summary>
        /// <param name="rowData">entire capability row</param>
        private void AddEntryToCapabilities(string[] rowData)
        {
            if (null == listCapabilities) listCapabilities = new List<Context.Capabilities>();

            int col = 0;
            Context.Capabilities newCap = new Context.Capabilities();
            newCap.Domain = rowData[col++];
            newCap.Container = rowData[col++];
            newCap.Capability = rowData[col++];
            newCap.Capability2 = rowData[col++];
            newCap.Description = rowData[col++];
            newCap.CSADescription = rowData[col++];
            uniqueIdCol = col;
            newCap.UniqueId = rowData[col++];
            newCap.Scopes = rowData[col++];

            SaveTics(rowData[col++], newCap.UniqueId);

            implementation3col = Properties.Settings.Default.capFile3Cols;
            implementationStart = col;

            if (implementation3col)
            {
                col += 7;
            }
            else
            {
                col += 16;
            }

            newCap.Notes = rowData[col++];
            col++;
            newCap.C = uint.Parse(rowData[col++]);
            newCap.I = uint.Parse(rowData[col++]);
            newCap.A = uint.Parse(rowData[col++]);
            col += 2;
            newCap.ResponsibilityVector = GetResponceVector(rowData, col);
            col += 8;
            newCap.OtherActors = GetOtherActors(rowData, ref col);

            listCapabilities.Add(newCap);
        }
    }
}
