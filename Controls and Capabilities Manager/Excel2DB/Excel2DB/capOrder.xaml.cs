using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel2DB.Models;
namespace Excel2DB
{
    /// <summary>
    /// Interaction logic for capOrder.xaml.  change order of input
    /// </summary>
    public partial class capOrder : Window
    {

        private GridView grid;
        //populate grid columns
        public capOrder()
        {
            InitializeComponent();
            int[] order = { Constants.colIncluded, Constants.colDomain, Constants.colContianer, Constants.colCapability,  Constants.colCapFamily, Constants.colIdentifier, Constants.colDescription,
                          Constants.colScope, Constants.colTIC, Constants.colInfoLow, Constants.colNotes, Constants.colCIAC};
            string[] tit = { "Included", "DOMAIN", "CONTAINER", "CAPABILITY 1     CAPABILITY 2\n(process or solution)", "Family","UNIQUE IDENTIFIER","DESCRIPTION", "SCOPE", 
                               "TIC CAPABILITIES mapping", "Capability Implementation  PM  Information Protection" ,"NOTES","","C   I   A   CIA"};
 
            string[] data = {"y","BOSS", "Compliance", "Intellectual Property Protection", "AC","Intellectual Property Protection","The main focus here is","TIC","TM.DS.05","AC-1   AC-1a  PM-1...","The purpose of this capability...","     ","2  2  2  6"};

        
            grid = new GridView();
            
            grid.AllowsColumnReorder = true;

            for (int i = 0; i < tit.Length; i++)
            {
                GridViewColumn col = new GridViewColumn() { Header = tit[i] + "\n" + data[i] };
                try
                {
                    grid.Columns.Insert(order[i], col);
                    
                }
                catch (Exception ior)
                {
                    grid.Columns.Add(col);
                }
            }
            this.box.View = grid;
        }

        //save order
        private void Done(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (GridViewColumn co in grid.Columns)
            {
                string column = co.Header.ToString().Remove(co.Header.ToString().IndexOf("\n"));
                switch (column)
                {
                    case "Included":
                        Properties.Settings.Default.colIncluded = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "DOMAIN":
                        Properties.Settings.Default.colDomain = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "CONTAINER":
                        Properties.Settings.Default.colContianer=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "CAPABILITY 1     CAPABILITY 2":
                        Properties.Settings.Default.colCapability=i;
                        i++;
                        Properties.Settings.Default.Save();
                        break;
                    case "Family":
                        Properties.Settings.Default.colCapFamily=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "UNIQUE IDENTIFIER":
                        Properties.Settings.Default.colIdentifier=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "DESCRIPTION":
                        Properties.Settings.Default.colDescription=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "SCOPE":
                        Properties.Settings.Default.colScope=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "TIC CAPABILITIES mapping":
                        Properties.Settings.Default.colTIC=i;
                        Properties.Settings.Default.Save();
                        i++;;
                        break;
                    case "Capability Implementation  PM  Information Protection":
                        Properties.Settings.Default.colInfoLow=i;
                        Properties.Settings.Default.Save();
                        i += 6;
                        break;
                    case "NOTES":
                        Properties.Settings.Default.colScope=i;
                        Properties.Settings.Default.Save();
                        break;
                    case "C   I   A   CIA":
                        Properties.Settings.Default.colCIAC=i;
                        Properties.Settings.Default.Save();
                        i+=3;
                        break;
                }
                i++;
            }
            try
            {
                Properties.Settings.Default.capFirstRow = int.Parse(this.row.Text);
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {

            }
            DialogResult = true;
            Close();

        }
    }
}
