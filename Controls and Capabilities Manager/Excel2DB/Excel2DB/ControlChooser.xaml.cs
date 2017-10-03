using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Excel2DB.Models;

namespace Excel2DB
{
    /// <summary>
    /// Interaction logic for ControlChooser.xaml
    /// </summary>
    public partial class ControlChooser : Window
    {
        private Context.DataContext dbContext;
        public List<string> selectedCtrls = new List<string>();

        public ControlChooser()
        {
            InitializeComponent();
            if (dbContext == null)
                dbContext = new Context.DataContext(DataConnecter.EstablishValidConnection());

            populateList();
        }

        private void DoneBtn_Click(object sender, RoutedEventArgs e)
        {

            if (this.SelectBtn.Content.ToString() == "Unselect all")
            {
                selectedCtrls.Clear();
                var ctrls = (from p in dbContext.Controls
                             select p);
                int ctrlsCount = ctrls.Count();
                foreach (var c in ctrls)
                {
                    selectedCtrls.Add(c.Name);
                    var sp = from p in dbContext.Specs
                             where p.ControId == c.Id
                             select p;
                    foreach (var sn in sp)
                    {
                        this.selectedCtrls.Add(c.Name + sn.SpecificationlName);
                    }
                }
            }
            else
            {
                //Foreach to save the last specs selected when the user click done
                foreach (Object selecteditem in this.SpecListBox.SelectedItems)
                {
                    //checking if the object is a string or a ListBoxItem
                    var newItem = selecteditem;
                    if (!newItem.GetType().Equals(typeof(String)))
                    {
                        ListBoxItem ListItem = (ListBoxItem)newItem;
                        newItem = ListItem.Content;
                    }

                    if (!selectedCtrls.Contains(newItem.ToString()))
                    {
                        selectedCtrls.Add(newItem.ToString());
                    }

                }
            }
            selectedCtrls.Sort();
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Function to populate the CtrlListBox
        /// </summary>
        private void populateList()
        {
            var ctrls = (from p in dbContext.Controls
                         select p);
            int ctrlsCount = ctrls.Count();
            foreach (var c in ctrls)
            {
                this.CtrlListBox.Items.Add(c.Name);
            }
        }

        private void CancelBtn_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void CtrlListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectBtn.Content.ToString() != "Unselect all")
            {
                //removing the unselected specs
                foreach (Object item in this.SpecListBox.Items)
                {

                    var newItem = item;
                    //checking if the object is a string or a ListBoxItem
                    if (!newItem.GetType().Equals(typeof(String)))
                    {
                        ListBoxItem ListItem = (ListBoxItem)newItem;
                        newItem = ListItem.Content;
                    }

                    if (selectedCtrls.Contains(newItem.ToString()) && !this.SpecListBox.SelectedItems.Contains(newItem.ToString()))
                    {
                        selectedCtrls.Remove(newItem.ToString());
                    }
                }

                //saving the selected specs 
                foreach (Object selecteditem in this.SpecListBox.SelectedItems)
                {
                    //checking if the object is a string or a ListBoxItem
                    var newItem = selecteditem;
                    if (!newItem.GetType().Equals(typeof(String)))
                    {
                        ListBoxItem ListItem = (ListBoxItem)newItem;
                        newItem = ListItem.Content;
                    }

                    if (!selectedCtrls.Contains(newItem.ToString()))
                    {
                        selectedCtrls.Add(newItem.ToString());
                    }
                }


                this.SpecListBox.Items.Clear();

                // Get the currently selected item in the ListBox. 
                string curItem = this.CtrlListBox.SelectedItem.ToString();

                var ctrlId = from p in dbContext.Controls
                             where p.Name == curItem
                             select new { p.Id };

                var sp = from p in dbContext.Specs
                         where p.ControId == ctrlId.First().Id
                         select p;

                if (selectedCtrls.Contains(curItem))
                {
                    ListBoxItem newItem = new ListBoxItem();
                    newItem.Content = curItem;
                    newItem.IsSelected = true;
                    this.SpecListBox.Items.Add(newItem);
                }
                else
                {
                    this.SpecListBox.Items.Add(curItem);
                }

                //Populating the SpecBox with the selected specs highlighted 
                foreach (var sn in sp)
                {
                    if (selectedCtrls.Contains(curItem + sn.SpecificationlName.ToString()))
                    {
                        ListBoxItem newItem = new ListBoxItem();
                        newItem.Content = curItem + sn.SpecificationlName;
                        newItem.IsSelected = true;
                        this.SpecListBox.Items.Add(newItem);
                    }
                    else
                    {
                        this.SpecListBox.Items.Add(curItem + sn.SpecificationlName);
                    }

                }            
            }
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectBtn.Content.ToString() == "Select all")
            {
                this.SelectBtn.Content = "Unselect all";
                this.SpecListBox.Items.Clear();
                this.CtrlListBox.IsEnabled = false;
                this.CtrlListBox.SelectionMode = SelectionMode.Multiple;
                this.CtrlListBox.SelectAll();
            }
            else
            {
                this.SelectBtn.Content = "Select all";
                this.CtrlListBox.IsEnabled = true;
                this.CtrlListBox.SelectionMode = SelectionMode.Single;                
            }
        }
    }
}
