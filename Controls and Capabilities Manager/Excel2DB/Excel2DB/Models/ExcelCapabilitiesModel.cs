using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelReporting;

namespace Excel2DB.Models
{

    public class ExcelCapabilitiesModel
    {
        public ExcelCapabilitiesModel(){
            buildExcelColumnsModel();
        }


        protected SortedList<int, ColumnsDescriptor> innerMap;
        /// <summary>
        /// Excel columns map
        /// </summary>
        public SortedList<int, ColumnsDescriptor> ModelMap{
            get
            {
             if(null==innerMap)
                {
                    buildExcelColumnsModel(); 
                }
             return innerMap;
            }
            protected set {innerMap = value; }
        }

        /// <summary>
        /// Implicit 1-Time constructor of the Columns Model
        /// </summary>
        virtual protected  void buildExcelColumnsModel()
        {
            if(null==innerMap)
            {
                innerMap = new SortedList<int, ColumnsDescriptor>();

                const string    tableImpl = "CapabilityControlsImplementation";
                const string    tableData = "CapabilityControlsDataProtection";

                Add2Map(1, "Domain");// Col A
                Add2Map(2, "Container");// Col B
                Add2Map(3, "Capability");// Col C
                Add2Map(4, "Capability_Details");// Col D
                Add2Map(5, "Family");// Col E
                Add2Map(6, "Unique_Identifier");// Col F
                Add2Map(7, "Description");// Col G
                Add2Map(8, "Scope"); // Col H
                Add2Map(9, "Implementation_Low_Impact", tableImpl, FismaControlsParser.GetControls, ImpactLevels.Low);
                Add2Map(10, "Implementation_Moderate_Impact", tableImpl, FismaControlsParser.GetControls, ImpactLevels.Moderate);
                Add2Map(11, "Implementation_High_Impact", tableImpl, FismaControlsParser.GetControls, ImpactLevels.High);
                Add2Map(12, "PM_Controls"); //???
                Add2Map(13, "Information_Protection_Low_Impact", tableData, FismaControlsParser.GetControls, ImpactLevels.Low);
                Add2Map(14, "Information_Protection_Moderate_Impact", tableData, FismaControlsParser.GetControls, ImpactLevels.Moderate);
                Add2Map(15, "Information_Protection_High_Impact", tableData, FismaControlsParser.GetControls, ImpactLevels.High);
                Add2Map(16, "Notes");   //???
                Add2Map(17, "Empty 17");
                Add2Map(18, "Confidentiality_Index");
                Add2Map(19, "Integrity_Index");
                Add2Map(20, "Availability_Index");
                Add2Map(21, "CIA_Score");
                Add2Map(22, "Empty 22");
                Add2Map(23, "IaaS_Consumer");
                Add2Map(24, "PaaS_Consumer");
                Add2Map(25, "SaaS_Consumer");
                Add2Map(26, "Empty 26");
                Add2Map(27, "IasS_Provider");
                Add2Map(28, "PasS_Provider");
                Add2Map(29, "SasS_Provider");
                Add2Map(30, "Empty 30");
                Add2Map(31, "IasS_Provider");
                Add2Map(32, "PasS_Provider");
                Add2Map(33, "SasS_Provider");
                Add2Map(34, "Empty 34");
                Add2Map(35, "Carrier_All");
                Add2Map(36, "Empty 36");
                Add2Map(37, "Auditor_All");

            }
        }

        protected virtual void Add2Map(int col, string name, string table = "Capability", StringArrayParser arrayParser = null, ImpactLevels level = ImpactLevels.Irrelevant)
        {
            innerMap.Add(col, new ColumnsDescriptor(col, name, table, arrayParser, level));
        }
    }
}
