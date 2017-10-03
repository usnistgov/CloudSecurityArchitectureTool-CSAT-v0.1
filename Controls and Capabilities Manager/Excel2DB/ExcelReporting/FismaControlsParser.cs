using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReporting
{
    /// <summary>
    /// Parses the block of the text formatted with tabs, CR-LFs and comas
    /// Verifies that the FISMA controls are in a verifiable and expected format
    /// </summary>
    public class FismaControlsParser
    {
        public static string[] GetControls(string inputBlock)
        {
            string[] tempArray = inputBlock.Split(new string[] { "\n", "\t", " ", "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> returnArray = new List<string>();
            foreach (string control in tempArray)
            {
                if (IsValidatedControl(control))
                {
                    returnArray.Add(control);
                }
            }
            return returnArray.ToArray();
        }

        static bool IsValidatedControl(string controlText) 
        {
            if (null != controlText && controlText.Length > 0) {
                return true;
            }
            return false;
        }

        public static string[] ParseBaseline(string inputBlock)
        {
            string[] tempArray=null;
            return tempArray;
        }

        public static string[] ParseRelatedChildren(string inputBlock)
        {
            string[] tempArray = null;
            return tempArray;
        }
    }
}
