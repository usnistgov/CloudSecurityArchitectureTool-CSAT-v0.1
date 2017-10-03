using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
namespace CSRC.Models
{
    class UpdateControls : Excel2Db
    {

        //constants for controls file
        public string ExcelFileName { get; set; }        
        public UpdateControls(string theFile)
        {
            ExcelFileName = theFile;
        }

        private SortedList<string, Context.Controls> controlNames = new SortedList<string, Context.Controls>();

        /// <summary>
        /// Specs grouped by the root control name
        /// </summary>
        private SortedList<string, SortedList<string, Context.Specs>> controlSpecs = new SortedList<string, SortedList<string, Context.Specs>>();

        /// <summary>
        /// Related controls grouped by the parent Control Name
        /// </summary>
        private SortedList<string, List<string>> controlRelated = new SortedList<string, List<string>>();

        private List<Context.Controls> listNewControls;
        /// <summary>
        /// Controls list for populating database
        /// </summary>
        protected List<Context.Controls> ListNewControls {
            get{
                if (null == listNewControls)
                { listNewControls = new List<Context.Controls>(); }
                return listNewControls;
            }
            private set
            { listNewControls = value; }
        }
        private List<Context.Controls> listUpdateControls;
        /// <summary>
        /// List of the controls that need to be updated
        /// </summary>
        protected List<Context.Controls> ListUpdateControls 
        {
            get
            {
                if (null == listUpdateControls)
                { listUpdateControls = new List<Context.Controls>(); }
                return listUpdateControls;
            }
            private set
            { listUpdateControls = value; }
        }

        /// <summary>
        /// gets the last line in excel file
        /// </summary>
        /// <returns>returns the last full row</returns>
        public int findEnd()
        {
            int lastRow = 2000;
            int lastEmptyRow = 0, lastFullRow = 1;
            string[] rowData;
            bool foundEnd = false;
            
            while (!foundEnd)
            {
                rowData = ReadExcelRow(lastRow, 15);
                if(isRowCompletelyEmpty(rowData))
                {
                    lastEmptyRow = lastRow;
                }
                else{
                    lastFullRow = lastRow;
                }

                if (lastFullRow == (lastEmptyRow - 1))
                {
                    return lastFullRow;
                }

                lastRow = (int)((lastEmptyRow + lastFullRow) / 2.0);
            }

            return lastRow;
        }

        /// <summary>
        /// reads file, updates progress, and inputs data into database
        /// </summary>
        /// <param name="wd"> progress bar controler </param>
        public void ProcessFile(BackgroundWorker wd)
        {
            int currentRow;
            string[] currentLine;
            try
            {
                if (InitExcelFile(ExcelFileName))
                {
                    InitDataModel();
                    //wd.callWorker();

                    bool isLineSpecsOnly = false;
                    hitCompletelyEmptyRow = false;
                    // Loop over the Excel lines

                    currentRow = Constants.conFirstRow;

                    double ncounter; // = 0.0595238095; // 100/1680 = 0.0595238095; 

                    int rowToEnd = findEnd();
                    ncounter = 100 / rowToEnd;

                    //double counter = 0.485; // 100/206 = 0.485436893
                    while (!hitCompletelyEmptyRow)
                    {                        
                        hitCompletelyEmptyRow = ProcessNextLine(currentRow++, out currentLine);
                        // Identifying the entry type
                        wd.ReportProgress((int)ncounter);
                        ncounter += 0.0595238095; 
                    }

                    if (ListNewControls.Count > 0)
                    {
                        dbContext.Controls.InsertAllOnSubmit(ListNewControls);
                        dbContext.SubmitChanges();
                    }
                    if (ListUpdateControls.Count > 0)
                    {
                        //dbContext.Controls.U
                    }

                    PersistSpecsAndRelated();
                    wd.ReportProgress(100);
                }
            }
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
        /// organizes one control or spec
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="rowData">place to put data</param>
        /// <returns></returns>
        private bool ProcessNextLine(int row, out string[] rowData)
        {
            //use none instead of blank
            rowData = ReadExcelRow(row,10);
            if (isRowCompletelyEmpty(rowData))
            {
                return true;
            }
            if (rowData[Constants.colDscription].StartsWith("[Withdrawn: "))
                return false;

            if (rowData[Constants.colImpact] == "")
                rowData[Constants.colImpact] = "NONE";

            if (rowData[Constants.colPriority] == "")
                rowData[Constants.colPriority] = "NONE";

            string specsName=null;
            string specsTail=null;
            string specsPrefix = null;
            Context.Controls newControl = null;
            //replace - to ,
            rowData[Constants.colImpact] = rowData[Constants.colImpact].Replace("_", ",");

            // Process SPECS row
            if (isRow4SpecsExtension(rowData))
            {
                specsName = GetCleanControlName(rowData[Constants.colNumber]);
                specsTail = GetCleanTopControlName(rowData[Constants.colNumber]);
                specsPrefix = specsName.Replace(specsTail, "");
                // Get base Control for the current Specs
                if (controlNames.Keys.Contains(specsPrefix))
                {
                    /// Found a control in the list and the contol's name is in the specsPrefix variable
                    if (null == controlSpecs) controlSpecs = new SortedList<string, SortedList<string, Context.Specs>>();
                    if (!controlSpecs.ContainsKey(specsPrefix)) 
                    {
                        controlSpecs.Add(specsPrefix, new SortedList<string, Context.Specs>());
                    }
                    Context.Specs newSpecs = new Context.Specs();
                    newSpecs.SpecificationlName = specsTail;
                    newSpecs.Description = rowData[Constants.colDscription];
                    newSpecs.Guidance = rowData[Constants.colGuidance];
                    controlSpecs[specsPrefix].Add(specsTail, newSpecs);
                }
                else if (!string.IsNullOrEmpty(specsTail)) 
                {
                    string s = "";
                }
                else
                {
                    throw (new Exception("Not specs for - " + specsName));
                }
                // Insert the details description
            }
            /// Process a CONTROL row
            else if (isRow4Control(rowData))
            {
                // Stash up the collection of the related controls for the later
                
                newControl = new Context.Controls();
                newControl.BaselineID = FindBaselineId(rowData);
                newControl.FamilyId = FindFamilyId(rowData);
                newControl.PriorityId = FindPriorityId(rowData);
                // Verify that Family, Baseline, Priority already exist or insert them
                if( 0==newControl.BaselineID)
                {
                    // Baseline was not found
                    throw( new Exception("Baseline for - " + rowData[Constants.colImpact] + " does not exist!!!"));
                }
                if( 0==newControl.FamilyId)
                {
                    // Baseline was not found
                    throw (new Exception("Family with Name - " + GetControlsFamilyName(rowData) + " and Description - " + GetControlsFamilyDescription(rowData) + " does not exist!!!"));
                }
                if (0 == newControl.PriorityId)
                {
                    // Baseline was not found
                    throw (new Exception("Priority for - " + rowData[Constants.colPriority] + " does not exist!!!"));
                }
                // TODO: Insert the Actual control with Family, Baseline, Priority verification  (Future)
                newControl.Name = GetCleanControlName(rowData[Constants.colNumber]);
                newControl.Description = rowData[Constants.colDscription];
                newControl.Guidance = rowData[Constants.colGuidance];
                /// >>> dbContext.Controls.GetNewBindingList().Add(newControl)
                if (!ControlExistsInDb(newControl))
                {
                    ListNewControls.Add(newControl);
                }
                // Get the inserted control by name
                if (!controlNames.ContainsKey(newControl.Name))
                {
                    controlNames.Add(newControl.Name, newControl);
                }
                else
                {
                    throw (new Exception("Control - " + newControl.Name + " happens more than once!!!"));
                }
            }
            if (!string.IsNullOrEmpty(rowData[Constants.colRelated]) )
            {
                if (null != newControl)
                {
                    /// Parse out and add all Related Controls
                    /// 
                    string[] relateds = rowData[Constants.colRelated].Replace(" ", string.Empty).Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    /// Add the related list to the collection (SAFELY)

                    if (null == controlRelated) controlRelated = new SortedList<string, List<string>>();
                    if (!controlRelated.ContainsKey(newControl.Name))
                    {
                        controlRelated.Add(newControl.Name, new List<string>(relateds));
                    }
                    
                }
            }
            
            return false;
        }



        /// <summary>
        /// Persists all of the ralated and specs for controls
        /// </summary>
        private void PersistSpecsAndRelated()
        {
            List<Context.Specs> controlSpecsFinal = new List<Context.Specs>();
            // Loop over the Controls that have Specs
            foreach (KeyValuePair<string, SortedList<string, Context.Specs>> kvp in controlSpecs)
            {
                uint controlId = GetControlIdByName(kvp.Key);
                // Loop over the specs for the given control
                foreach (KeyValuePair<string, Context.Specs> specKVP in controlSpecs[kvp.Key])
                {
                    specKVP.Value.ControId = controlId;
                    controlSpecsFinal.Add(specKVP.Value);
                }
            }
            dbContext.Specs.InsertAllOnSubmit(controlSpecsFinal);
            dbContext.SubmitChanges();


            // Handling the realteds //
            List<Context.Related> relateds = new List<Context.Related>();
            // Loop over the related controls
            foreach(KeyValuePair<string, List<string>> outerKvp in controlRelated)
            {
                uint parentId = GetControlIdByName(outerKvp.Key);
                foreach (string childName in outerKvp.Value)
                {
                    uint childId = GetControlIdByName(childName);
                    if (childId > 0)
                    {   // Add only for valid references back to CONTROLS
                        relateds.Add(new Context.Related
                        {
                            ParentId = parentId,
                            ChildId = childId
                        });
                    }
                    else 
                    {
                        string errorMesage = "Reference to the Related Entity Named : " + childName + " in the line with Control : " + outerKvp.Key;
                        ReportErrorTrace(errorMesage);
                    }
                }
            }
            dbContext.Relateds.InsertAllOnSubmit(relateds);
            dbContext.SubmitChanges();
        }





        /// <summary>
        /// Verifies that the control already exists in the Database
        /// </summary>
        /// <param name="newControl"></param>
        /// <returns></returns>
        private bool ControlExistsInDb(Context.Controls newControl)
        {
            bool haveFoundControl = false;
            var retVal =
                from p in dbContext.Controls
                where p.Name == newControl.Name &&  p.Description == newControl.Description && p.Guidance == newControl.Guidance ///   p.Name.Equals(rowData[3], StringComparison.InvariantCultureIgnoreCase) 
                select p;
            haveFoundControl = retVal.Any();
            return haveFoundControl ;
        }

        /// <summary>
        /// Finds matching Priority ID
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private uint FindPriorityId(string[] rowData)
        {
            string priorityValue = (string.IsNullOrEmpty(rowData[Constants.colPriority]))?("-"):(rowData[Constants.colPriority].Trim());
            var retVal = 
                from p in dbContext.Priorities
                where p.Name == priorityValue ///   p.Name.Equals(rowData[3], StringComparison.InvariantCultureIgnoreCase) 
                select new{p.Id};

            if (retVal.Any())
            {
                uint uintRetVal = retVal.First().Id;
                return uintRetVal;
            }
            else
                return 0;
 
        }
        /// <summary>
        /// Finds Matching FamilyID
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private uint FindFamilyId(string[] rowData)
        {
            string description = GetControlsFamilyDescription(rowData);
            string name = GetControlsFamilyName(rowData);

            return GetFamilyIdByControlNameOrDescription(description, name);
        }


        /// <summary>
        /// full family name
        /// </summary>
        /// <param name="rowData">entire control row</param>
        /// <returns></returns>
        private string GetControlsFamilyDescription(string[] rowData)
        {
            return rowData[Constants.colConFamily].Trim();
        }

        /// <summary>
        /// get 2 letter name
        /// </summary>
        /// <param name="rowData">entire control row</param>
        /// <returns></returns>
        private string GetControlsFamilyName(string[] rowData)
        {
            return rowData[Constants.colNumber].Substring( 2);
        }

        /// <summary>
        /// Finds matching BaselineID
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private uint FindBaselineId(string[] rowData)
        {
            string baselineDescription = (string.IsNullOrEmpty(rowData[Constants.colImpact])) ? ("NO IMPACT") : (rowData[Constants.colImpact].Trim());
            string baselineDescriptionCorrected = baselineDescription.Replace("_", ",");
            var retVal =
                 from p in dbContext.Baselines
                 where (p.Description == baselineDescription || p.Description == baselineDescriptionCorrected)
                 select p.Id ;
            
            if (retVal.Any())
            {
                uint uintRetVal = retVal.First();
                return uintRetVal;
            }
            else
                return 0;
        }


        /// <summary>
        /// Determines condition at which the row can be only the Specs extension
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private static bool isRow4SpecsExtension(string[] rowData)
        {
            return !isRow4Control(rowData);
        }

        /// <summary>
        /// checks if row fits a control pattern
        /// </summary>
        /// <param name="rowData">entire control row</param>
        /// <returns></returns>
        private static bool isRow4Control(string[] rowData)
        {
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string test = Regex.Replace(rowData[Constants.colNumber], pattern,"");
            return test.Length == 0;
        }
    }
}
