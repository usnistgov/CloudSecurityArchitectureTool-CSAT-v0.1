using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelReporting;

namespace Excel2DB.Models
{
    public class ExcelControlsModel:ExcelCapabilitiesModel
    {
        protected override void buildExcelColumnsModel()
        {
            if (null == innerMap)
            {
                innerMap = new SortedList<int, ColumnsDescriptor>();

                const string tableSupplemantalGuidance = "Specs";
                const string tableRelatedControls = "Related";
                const string tableFamily = "Families";
                const string tablePriority = "Priorities";
                const string tableBaseline = "Baselines";

                Add2Map(1, "Family", tableFamily);
                Add2Map(2, "Name");
                Add2Map(3, "Title");
                Add2Map(4, "Priority", tablePriority);
                Add2Map(5, "BaselineImpact", tableBaseline, FismaControlsParser.ParseBaseline);
                Add2Map(6, "Description");
                Add2Map(7, "SupplementalGuidance", tableSupplemantalGuidance);
                Add2Map(8, "Related",tableRelatedControls , FismaControlsParser.ParseRelatedChildren);
                //Add2Map(9, "", tableImpl, FismaControlsParser.GetControls, ImpactLevels.Low);
                //Add2Map(10, "", tableImpl, FismaControlsParser.GetControls, ImpactLevels.Moderate);
                //Add2Map(11, "", tableImpl, FismaControlsParser.GetControls, ImpactLevels.High);
            }
        }

        protected virtual void Add2Map(int col, string name, string table = "Control", StringArrayParser arrayParser = null, ImpactLevels level = ImpactLevels.Irrelevant)
        {
            innerMap.Add(col, new ColumnsDescriptor(col, name, table, arrayParser, level));
        }
    }
}
