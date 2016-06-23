using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRC.Models
{
    /// <summary>
    /// Hold all of the constants for columns in spreedsheats 
    /// </summary>
    class Constants
    {
        public static int colConFamily = 0, colNumber = 0, colTitle = 0, colImpact = 0, colPriority = 0, colDscription = 0, colGuidance = 0, colRelated = 0, colRest = 0,
            
            colDomain = 0, colContianer = 0, colCapability = 0, colCapability2 = 0, colIdentifier = 0, colDescription = 0,
            colInfoLow = 0,colNotes = 0, colScope = 0, colTIC = 0, capFirstRow = 0, conFirstRow = 0, colCIAC = 0,
            colIncluded = 0, colCapVector = 0, 
            
            baseFirstRow = 0, colNistLow = 0, colFedLow = 0, colNistAnt = 0, colNistMed = 0, colFedMed = 0, colNistHigh = 0, colFedHigh = 0;
            
        public static uint levelLow = 1, levelMed = 2, levelHigh = 3, AuthorNist = 1, AuthorFedRamp = 2;
        public static bool capFile3Cols = false;
            

        /// <summary>
        /// pulls constants from saved user properties
        /// </summary>
        public static void ReadValues(){
            colConFamily = Properties.Settings.Default.colConFamily;
            colNumber = Properties.Settings.Default.colNumber;
            colTitle = Properties.Settings.Default.colTitle;
            colImpact = Properties.Settings.Default.colImpact;
            colPriority = Properties.Settings.Default.colPriority;
            colDscription = Properties.Settings.Default.colDscription;
            colGuidance = Properties.Settings.Default.colGuidance;
            colRelated = Properties.Settings.Default.colRelated;
            colRest = Properties.Settings.Default.colRest;
   
            colDomain = Properties.Settings.Default.colDomain;
            colContianer = Properties.Settings.Default.colContainer;
            colCapability = Properties.Settings.Default.colCapability;
            colCapability2 = Properties.Settings.Default.colCapability2;
            colIdentifier = Properties.Settings.Default.colIdentifier;
            colDescription = Properties.Settings.Default.colDescription;
            colInfoLow = Properties.Settings.Default.colInfoLow;
            colNotes = Properties.Settings.Default.colNotes;
            colScope = Properties.Settings.Default.colScope;
            colTIC = Properties.Settings.Default.colTIC;
            capFirstRow = Properties.Settings.Default.capFirstRow;
            conFirstRow = Properties.Settings.Default.conFirstRow;
            colCapVector = Properties.Settings.Default.colCapVector;
            colCIAC = Properties.Settings.Default.colCIAC;

            baseFirstRow = Properties.Settings.Default.baseFirstRow;
            colNistLow = Properties.Settings.Default.colNistLow;
            colFedLow = Properties.Settings.Default.colFedLow;
            colNistAnt = Properties.Settings.Default.colNistAnt;
            colNistMed = Properties.Settings.Default.colNistMed;
            colFedMed = Properties.Settings.Default.colFedMed;
            colNistHigh = Properties.Settings.Default.colNistHigh;
            colFedHigh = Properties.Settings.Default.colFedHigh;

            capFile3Cols = Properties.Settings.Default.capFile3Cols;
        }
    }
}
