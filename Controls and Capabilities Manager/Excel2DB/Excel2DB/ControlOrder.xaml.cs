using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
    /// Interaction logic for ControlOrder.xaml   change order of controls file
    /// </summary>
    public partial class ControlOrder : Window
    {
        private GridView grid;
        //insert grid columns
        public ControlOrder()
        {
            InitializeComponent();
            int[] order = { Constants.colConFamily, Constants.colNumber, Constants.colTitle, Constants.colPriority, Constants.colImpact, Constants.colDescription, Constants.colGuidance };
            string[] tit = { "FAMILY", "NAME", "TITLE", "PRIORITY", "BASELINE-IMPACT", "DESCRIPTION","SUPPLEMENTAL GUIDANCE","RELATED" };
            string[] data = { "ACCESS CONTRO", "AC-1", "ACCESS CONTROL POLICY AND PROCEDURES", "P1", "LOW,MODERATE,HIGH", "The organization:", "This control addresses the establishment of...", 
            "PM-9"};
            
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

        /// <summary>
        /// save values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (GridViewColumn co in grid.Columns)
            {
                string column = co.Header.ToString().Remove(co.Header.ToString().IndexOf("\n"));
                switch (column)
                {
                    case "FAMILY":
                        Properties.Settings.Default.colConFamily = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "NAME":
                        Properties.Settings.Default.colNumber = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "TITLE":
                        Properties.Settings.Default.colTitle = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "PRIORITY":
                        Properties.Settings.Default.colPriority = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "BASELINE-IMPACT":
                        Properties.Settings.Default.colImpact = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "DESCRIPTION":
                        Properties.Settings.Default.colDscription = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "SUPLIMENTAL GUILDANCE":
                        Properties.Settings.Default.colGuidance = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "RELATED":
                        Properties.Settings.Default.colRelated = i;
                        Properties.Settings.Default.Save();
                        break;
                }
                i++;
            }
            try
            {
                Properties.Settings.Default.conFirstRow = int.Parse(this.row.Text);
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
