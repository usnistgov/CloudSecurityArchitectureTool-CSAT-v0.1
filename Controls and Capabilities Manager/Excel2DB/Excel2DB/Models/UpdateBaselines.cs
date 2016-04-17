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

        private static bool isRow4Control(string rowData)
        {
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string test = Regex.Replace(rowData, pattern, "");
            return test.Length == 0;
        }
    }
}
