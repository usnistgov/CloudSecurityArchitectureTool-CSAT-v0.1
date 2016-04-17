using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excel2DB.Models
{

    public enum ImpactLevels { Low = 1, Moderate = 2, High = 3, Irrelevant = 0 };

    public delegate string[] StringArrayParser(string x);
    /// <summary>
    /// Descriptor structure of what column contains what type of data
    /// </summary>
    public class ColumnsDescriptor
    {
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
        public string TableName { get; set; }
        public ImpactLevels ImpactLevelImplementation { get; set; }
        public StringArrayParser ArrayParserDelegate { get; set; }

        public ColumnsDescriptor(int columnIndex, string columnName, string table)
        {
            ColumnIndex = columnIndex;
            ColumnName = columnName;
            TableName = table;
            ArrayParserDelegate = null;
        }


        public ColumnsDescriptor(int columnIndex, string columnName, string table, StringArrayParser arrayParser)
            : this(columnIndex, columnName, table)
        {
            ArrayParserDelegate = arrayParser;
        }


        public ColumnsDescriptor(int columnIndex, string columnName, string table, StringArrayParser arrayParser, ImpactLevels level)
            : this(columnIndex, columnName, table, arrayParser)
        {
            ImpactLevelImplementation = level;
        }
    }
}
