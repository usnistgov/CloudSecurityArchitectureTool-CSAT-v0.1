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
    /// Interaction logic for BaselineOrder.xaml
    /// </summary>
    public partial class BaselineOrder : Window
    {
        private GridView grid;
        public BaselineOrder()
        {
            InitializeComponent();
            int[] order = { Constants.colNistLow, Constants.colFedLow, Constants.colNistAnt, Constants.colNistMed, Constants.colFedMed, Constants.colNistHigh, Constants.colFedHigh };
            string[] tit = {"Low 800-53 R4","Low FedRAMP", "NIST ANT", "Moderate 800-53 R4", "Moderate FedRAMP", "High 800-53 R4","High FedRAMP"};
            
            grid = new GridView();

            grid.AllowsColumnReorder = true;

            for (int i = 0; i < tit.Length; i++)
            {
                GridViewColumn col = new GridViewColumn() { Header = tit[i]};
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
                string column = co.Header.ToString();
                switch (column)
                {
                    case "Low 800-53 R4":
                        Properties.Settings.Default.colNistLow = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "Low FedRAMP":
                        Properties.Settings.Default.colFedLow = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "NIST ANT":
                        Properties.Settings.Default.colNistAnt = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "Moderate 800-53 R4":
                        Properties.Settings.Default.colNistMed = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "Moderate FedRAMP":
                        Properties.Settings.Default.colFedMed = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "High 800-53 R4":
                        Properties.Settings.Default.colNistHigh = i;
                        Properties.Settings.Default.Save();
                        break;
                    case "High FedRAMP":
                        Properties.Settings.Default.colFedHigh = i;
                        Properties.Settings.Default.Save();
                        break;
                }
                i++;
            }
            try
            {
                Properties.Settings.Default.baseFirstRow = int.Parse(this.row.Text);
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
