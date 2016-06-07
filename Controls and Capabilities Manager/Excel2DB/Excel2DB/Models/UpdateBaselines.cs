using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading.Tasks;
using ExcelReports.ExcelInteropReports;

namespace Excel2DB.Models
{
    class UpdateBaselines:Excel2Db
    {
        private string inputFile;
        private List<Context.BaselineSecurityMapping> newbases = new List<Context.BaselineSecurityMapping>();
        public UpdateBaselines(string file)
        {
            inputFile = file;
        }

        /// <summary>
        /// read in excel and inser all controls into basline table
        /// </summary>
        /// <param name="wd"></param>
        public void ProcessFile(BackgroundWorker wd)
        {
            int row;
            try
            {
                if(InitExcelFile(inputFile))
                {
                    InitDataModel();
                    row = Constants.baseFirstRow;
                    hitCompletelyEmptyRow = false;
                    var ret = from p in dbContext.Families
                              select p;
                    int totalRows = ret.Count();
                    //read file line by line
                    while (!hitCompletelyEmptyRow)
                    {
                        hitCompletelyEmptyRow = InsertLine(row);

                        double frac = (double)(row - Constants.baseFirstRow) / totalRows;
                        wd.ReportProgress((int)(frac * 100));
                        row++;
                    }
                    wd.ReportProgress(100);
                    dbContext.BaselineSecurityMappings.InsertAllOnSubmit(newbases);
                    dbContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            finally
            {
                CloseExcel();
            }

        }

        /// <summary>
        /// read a line of baselines and put in database
        /// </summary>
        /// <param name="row">row number</param>
        /// <returns></returns>
        private bool InsertLine(int row)
        {
            string[] rowdata = ReadExcelRow(row, 10);
            if (!isRowCompletelyEmpty(rowdata))
            {
                InsertComponent(rowdata[Constants.colNistLow], Constants.levelLow, Constants.AuthorNist);
                InsertComponent(rowdata[Constants.colFedLow], Constants.levelLow, Constants.AuthorFedRamp);
                InsertComponent(rowdata[Constants.colNistMed], Constants.levelMed, Constants.AuthorNist);
                InsertComponent(rowdata[Constants.colFedMed], Constants.levelMed, Constants.AuthorFedRamp);
                InsertComponent(rowdata[Constants.colNistHigh], Constants.levelHigh, Constants.AuthorNist);
                InsertComponent(rowdata[Constants.colFedHigh], Constants.levelHigh, Constants.AuthorFedRamp);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// split up component and insert all into database
        /// </summary>
        /// <param name="component">set of controls in baseline for one family</param>
        /// <param name="level">number for low,moderate,high level</param>
        /// <param name="author"> number for nist/fedRAMP</param>
        private void InsertComponent(string component, uint level, uint author)
        {
            string[] controls = GetCleanListOfControls(component);
            foreach(string entry in controls){
                Context.BaselineSecurityMapping baseline = new Context.BaselineSecurityMapping() { Level = level, BaselineAuthor = author };
                if(isRow4Control(entry)){
                    baseline.IsControlMap = true;
                    baseline.ControlsId = GetControlIdByName(entry);
                    baseline.SpecsId = 1;
                }else{
                    baseline.IsControlMap = false;
                    baseline.SpecsId = GetSpecIdByName(entry.Replace(" ",""));
                    baseline.ControlsId = 1;
                    if (baseline.SpecsId == 0)
                    {
                        ReportErrorTrace("Specn ot found:" + entry + " Level " + level + " author " + author);
                        continue;
                    }
                }
                newbases.Add(baseline);
            }


        }

        /// <summary>
        /// check if string is control or spec
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        private static bool isRow4Control(string rowData)
        {
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string test = Regex.Replace(rowData, pattern, "");
            return test.Length == 0;
        }
    }
}
