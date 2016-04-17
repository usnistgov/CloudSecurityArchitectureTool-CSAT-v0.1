using ExcelReports.ExcelInteropReports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Excel2DB.Models
{
    /// <summary>
    /// conains worker methods to assist in the manipulation of the data
    /// </summary>
    class Excel2Db
    {
        protected const string generalControlPattern = @"[A-Z]{2}-([0-9]{1,2})(\((\d|\d\d)\)|){0,1}"; 
        protected bool hitCompletelyEmptyRow;
        protected Context.DataContext dbContext;
        protected static Context.Capabilities theCapability;

        /// <summary>
        /// sets up connection
        /// </summary>
        protected void InitDataModel()
        {
            theCapability = new Context.Capabilities();
            if(dbContext == null)
                dbContext = new Context.DataContext(DataConnecter.EstablishValidConnection());
            
        }

        protected BaseExcelInteropReport excelSource;
        protected BaseExcelInteropWorksheet activeWorksheet;
        protected int row2Begin = 0;
        protected int row2Finish = int.MaxValue;
        protected int excelRowLength = 0;

        /// <summary>
        /// Open an existing Excel file
        /// </summary>
        /// <param name="ExcelFileName"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="activeWorksheetNumber"></param>
        /// <returns></returns>
        protected bool InitExcelFile(string ExcelFileName, int firstRow = 1, int lastRow = int.MaxValue, int activeWorksheetNumber = 1)
        {
            FileInfo fi = new FileInfo(ExcelFileName);
            if (null != ExcelFileName && fi.Exists)
            {
                excelSource = new BaseExcelInteropReport(ExcelOpenType.OpenExistingTemplateFile, ExcelFileName, false);
                activeWorksheet = excelSource.getWorksheet(activeWorksheetNumber);
                row2Begin = firstRow;
                row2Finish = lastRow;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a new Excel Report
        /// </summary>
        /// <param name="ExcelFileName">The File Name</param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <param name="activeWorksheetNumber"></param>
        /// <returns></returns>
        protected bool InitNewExcelFile(string ExcelFileName, int firstRow = 1, int lastRow = int.MaxValue, int activeWorksheetNumber = 1)
        {
            FileInfo fi = new FileInfo(ExcelFileName);
            //ExcelCapabilitiesModel theModel = new ExcelCapabilitiesModel();
            if (null != ExcelFileName && !fi.Exists)
            {
                excelSource = new BaseExcelInteropReport(ExcelOpenType.CreateNewFile, ExcelFileName, true);
                activeWorksheet = excelSource.getWorksheet(1);
                activeWorksheet.Name = "report-on-" + DateTime.Now.ToString("yyyy-MM-dd");
                row2Begin = firstRow;
                row2Finish = lastRow;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// close excel properly
        /// </summary>
        protected void CloseExcel()
        {
            excelSource.CloseInput();
        }

        /// <summary>
        /// save report
        /// </summary>
        protected void CloseSaveExcel()
        {
            excelSource.reportDone();
            //excelSource.templateReportClose();
        }

        /// <summary>
        ///  Identifies completely empty row
        /// </summary>
        /// <param name="rowData">the row data collection</param>
        /// <returns>true if the row is EMPTY, false otherwise</returns>
        protected static bool isRowCompletelyEmpty(string[] rowData)
        {
            bool isEmpty = true;
            foreach (string colData in rowData)
            {
                isEmpty = isEmpty && string.IsNullOrEmpty(colData);
            }
            return isEmpty;
        }


        /// <summary>
        /// Looks up the FamilyId by Fmily Prefix (Name) or Description
        /// </summary>
        /// <param name="description">Family Description or NULL</param>
        /// <param name="name">Family Name or NULL</param>
        /// <returns></returns>
        protected uint GetFamilyIdByControlNameOrDescription(string description, string name)
        {
            var retVal =
                from p in dbContext.Families
                where (p.Description == description || null==description ) || (p.Name == name || null==name) //p.Description.Equals(rowData[0], StringComparison.InvariantCultureIgnoreCase)
                select new { p.Id };

            if (retVal.Any())
            {
                uint uintRetVal = retVal.First().Id;
                return uintRetVal;
            }
            else
                return 0;
        }

        /// <summary>
        /// remove spaces
        /// </summary>
        /// <param name="rowData">control name from 800-53</param>
        /// <returns></returns>
        protected string GetCleanControlName(string rowData)
        {
            return rowData.Replace(" ", "");
        }

        /// <summary>
        /// returns spec portion of control
        /// </summary>
        /// <param name="rowData">entire spec name </param>
        /// <returns> the spec name</returns>
        protected string GetCleanTopControlName(string rowData)
        {
            string cleanName = GetCleanControlName(rowData);
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string resultingTail = Regex.Replace(cleanName, pattern, String.Empty);
            return resultingTail;
        }

        /// <summary>
        /// returns the control name without spec
        /// </summary>
        /// <param name="control"></param>
        /// <returns>the top control name</returns>
        protected string RemoveeSpec(string control){
            string tail = GetCleanTopControlName(control);
            if (tail == "")
            {
                return control;
            }
            else
            {
                return control.Remove(control.IndexOf(tail));
            }
        }

        /// <summary>
        ///gets name of family for spec 
        /// </summary>
        /// <param name="spec">a spec object</param>
        /// <returns>the family name</returns>
        protected string GetFamilyNameForSpec(Context.Specs spec)
        {
            var ret = from p in dbContext.Controls
                      where p.Id == spec.ControId
                      select new { p.FamilyId};
            uint id = ret.First().FamilyId;
            var na = from p in dbContext.Families
                     where p.Id == id
                     select new { p.Name };
            return na.First().Name;
        }

        /// <summary>
        /// gets id of spec under the control with name
        /// </summary>
        /// <param name="control">top control id</param>
        /// <param name="spec">the name of just the spec </param>
        /// <returns></returns>
        protected uint GetSpecsId(uint control, string spec)
        {
            var retval
                = from p in dbContext.Specs
                  where (p.ControId == control && p.SpecificationlName == spec)
                  select new { p.Id };

            if (retval.Any())
            {
                return retval.First().Id;
            }
            else
            {
                return 0;
            }
        }
        
        
        /// <summary>
        /// Looks up the FamilyId by Fmily Prefix (Name) or Description
        /// </summary>
        /// <param name="description">Family Description or NULL</param>
        /// <param name="name">Family Name or NULL</param>
        /// <returns></returns>
        protected uint GetFamilyIdByFamilyPrefix(string prefix)
        {
            var retVal =
                from p in dbContext.Families
                where (p.Name == prefix) //p.Description.Equals(rowData[0], StringComparison.InvariantCultureIgnoreCase)
                select new { p.Id };

            if (retVal.Any())
            {
                uint uintRetVal = retVal.First().Id;
                return uintRetVal;
            }
            else
                return 0;
        }

        /// <summary>
        /// returns the id of the cap with the given unique id
        /// </summary>
        /// <param name="theCapId">the unique id</param>
        /// <returns>the numerical id of the capability</returns>
        protected uint GetCapabilityIdByUniqueId(string theCapId)
        {
            var retVal =
                from p in dbContext.Capabilities
                where (p.UniqueId == theCapId) //p.Description.Equals(rowData[0], StringComparison.InvariantCultureIgnoreCase)
                select new { p.Id };

            if (retVal.Any())
            {
                uint uintRetVal = retVal.First().Id;
                return uintRetVal;
            }
            else
                return 0;
        }


        /// <summary>
        /// Gets Control ID by the Control Name 
        /// </summary>
        /// <param name="currentName">Control Name to Look for ID of</param>
        /// <returns>uint Id of the control or ZERO!!!! if not found</returns>
        public uint GetControlIdByName(string currentName)
        {
            var currentControl =
                from p in dbContext.Controls
                where p.Name == currentName
                select new { p.Id };
            if (currentControl.Any())
            {
                return currentControl.First().Id;
            }
            else
            {
                // Make big stink!!!
                return 0;
            }
        }

        /// <summary>
        /// Gets Control name by ID
        /// </summary>
        /// <param name="p">control id</param>
        /// <returns>name of control</returns>
        protected string GetControlName(uint id)
        {
            var currentControl =
                from p in dbContext.Controls
                where p.Id == id
                select new { p.Name };
            if (currentControl.Any())
            {
                return currentControl.First().Name;
            }
            else
            {
                // Make big stink!!!
                return string.Empty;
            }
        }

        /// <summary>
        /// reads in excel row from opened file
        /// </summary>
        /// <param name="row">number of row to read, 1-based</param>
        /// <param name="length">number of cells to read</param>
        /// <returns></returns>
        protected string[] ReadExcelRow(int row, int length=8)
        {
            if (excelRowLength <= 0) 
            { 
                excelRowLength = length; 
            }
            //Read the line from excel spreadsheet into a string array
            string[] rowData = new string[excelRowLength];
            for (int i = 1; i <= excelRowLength; i++)
            {
                rowData[i - 1] = this.activeWorksheet.getCellData(row, i);
            }
            return rowData;
        }

        /// <summary>
        /// debug tool
        /// </summary>
        /// <param name="errorMesage">messege to display in console</param>
        protected static void ReportErrorTrace(string errorMesage)
        {
            Trace.WriteLine("!!! === --- Error ");
            Trace.WriteLine(errorMesage);
            Trace.WriteLine("!!!");
        }

        /// <summary>
        /// splits up controls list from implementation columns of capabilities file
        /// </summary>
        /// <param name="rowData">separated list of control names</param>
        /// <returns></returns>
        protected static string[] GetCleanListOfControls(string controls)
        {
            string cleanControls = controls/*.Replace(" ", string.Empty); */;
            /// Parse out all potential Controls
            string[] potentialControls = cleanControls/*.Replace(" ", string.Empty)*/.Split(new char[] { ',', ';', '\n', ' ', '\t', '*','[',']','{','}' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> result = new List<string>();
            foreach (string suspectControl in potentialControls)
            {
                if (Regex.IsMatch(suspectControl, generalControlPattern))
                {
                    result.Add(suspectControl);
                }
                else 
                {
                    ReportErrorTrace("Pattern mismatch for control - " + suspectControl);
                }
            }
            return result.ToArray();
        }

        public uint GetSpecIdByName(string name)
        {
            string top = RemoveeSpec(name);
            uint controlId = GetControlIdByName(top);
            uint specid = GetSpecsId(controlId, name.Replace(top, ""));
            return specid;
        }
    }
}
