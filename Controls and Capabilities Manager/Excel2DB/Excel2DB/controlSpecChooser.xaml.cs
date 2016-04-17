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
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using Excel2DB.Models;

namespace Excel2DB
{
    /// <summary>
    /// Interaction logic for controlSpecChooser.xaml.  pick controls snd specs
    /// </summary>
    public partial class controlSpecChooser : Window
    {
        private Context.DataContext dbcontext;
        public List<string> selected;

        //set up lists
        public controlSpecChooser()
        {
            InitializeComponent();
            dbcontext = new Context.DataContext(DataConnecter.EstablishValidConnection());
            selected = new List<string>();
            populateList(this.fullList);

        }

        /// <summary>
        /// insert all controls into list
        /// </summary>
        /// <param name="list"></param>
        private void populateList(TreeView list){
            var contr = from p in dbcontext.Controls
                        select p;
            foreach(var control in contr){
                TreeViewItem top = new TreeViewItem();
                top.Expanded += OnExpanded;
                top.Header = control.Name;
                top.Name = "control" + control.Id.ToString() + list.Name;

                var sp = from p in dbcontext.Specs
                         where p.ControId == control.Id
                         select p;
                foreach(var spec in sp){
                    TreeViewItem specopt = new TreeViewItem();
                    specopt.Expanded += OnExpanded;
                    specopt.Header = control.Name + spec.SpecificationlName;
                    specopt.Name = "control"+control.Id+ "spec" + spec.Id.ToString() + list.Name;
                    top.Items.Add(specopt);
                    RegisterName(specopt.Name, specopt);
                }
                list.Items.Add(top);
                RegisterName(top.Name, top);
            }
        }

        //put all selected together
        private void Done(object sender, RoutedEventArgs e)
        {
            foreach (object obj in final.Items)
            {
                TreeViewItem top = obj as TreeViewItem;
                if (top.Header != null)
                    selected.Add(top.Header.ToString());
                foreach (object sp in top.Items)
                {
                    TreeViewItem spec = sp as TreeViewItem;
                    selected.Add(spec.Header.ToString());
                }
            }
            DialogResult = true;
            Close();
        }

        //move all option to selected
        private void SelectAll(object sender, RoutedEventArgs e)
        {
            UnregisterTree(this.final);
            this.final.Items.Clear();
            UnregisterTree(this.fullList);
            this.fullList.Items.Clear();
            populateList(this.final);

        }

        /// <summary>
        /// delete all selected and return to left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            UnregisterTree(this.final);
            this.final.Items.Clear();
            UnregisterTree(this.fullList);
            this.fullList.Items.Clear();
            populateList(this.fullList);
        }

        /// <summary>
        /// nesecary for select and remove all
        /// </summary>
        /// <param name="tree"></param>
        private void UnregisterTree(TreeView tree)
        {
            foreach (object ob in tree.Items)
            {
                TreeViewItem top = ob as TreeViewItem;
                if (top.Name != null)
                    UnregisterName(top.Name);
                foreach (object s in top.Items)
                {
                    TreeViewItem spec = s as TreeViewItem;
                    UnregisterName(spec.Name);
                }
            }
        }

        //move selected item from left to right
        private void AddOne(object sender, RoutedEventArgs e) 
        {
            try
            {
                TreeViewItem select = this.fullList.SelectedItem as TreeViewItem;
                AddToTree(this.final,select);
                if (isRow4Control(select.Header.ToString()))
                {
                    select.Header = null;
                    select.IsExpanded = true;
                    if (select.Items.Count == 0)
                    {
                        UnregisterName(select.Name);
                        this.fullList.Items.Remove(select);
                    }
                }
                else
                {
                    TreeViewItem par =(select.Parent as TreeViewItem);
                    par.Items.Remove(select);
                    UnregisterName(select.Name);
                    if (par.Items.Count == 0 && par.Header == null)
                    {
                        UnregisterName(par.Name);
                        this.fullList.Items.Remove(par);
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }

        }

        //move right to left
        private void RemoveOne(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem select = this.final.SelectedItem as TreeViewItem;
                AddToTree(this.fullList, select);
                if (isRow4Control(select.Header.ToString()))
                {
                    select.Header = null;
                    select.IsExpanded = true;
                    if (select.Items.Count == 0)
                    {
                        this.final.Items.Remove(select);
                    }
                }
                else
                {
                    TreeViewItem par = (select.Parent as TreeViewItem);
                    par.Items.Remove(select);
                    if (par.Items.Count == 0 && par.Header == null)
                    {
                        this.final.Items.Remove(par);
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                this.fullList.Items.Refresh();
            }
        }

        /// <summary>
        /// helps to move items
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="add"></param>
        private void AddToTree(TreeView tree, TreeViewItem add)
        {
            string control = "", spec = "";
            uint cid = 0, spid = 0;
            if (isRow4Control(add.Header.ToString()))
            {
                //pull info
                control = add.Name.ToString();
                int pos = control.IndexOf('f');
                cid = uint.Parse(control.Substring(7, pos-7));
                string find = "control" + cid + tree.Name;
                //look for tree
                TreeViewItem top = FindName(find) as TreeViewItem;
                if (top == null)
                {
                    //create new tree
                    int i = 0, index = -1;
                    TreeViewItem empty = new TreeViewItem();
                    empty.Expanded += OnExpanded;
                    empty.Name = "control" + cid + tree.Name;
                    RegisterName(empty.Name, empty);
                    empty.Header = add.Header;
                    foreach (object itm in tree.Items)
                    {
                        TreeViewItem tre = itm as TreeViewItem;
                        int fpos = tre.Name.ToString().IndexOf('f');
                        string bare = tre.Name.Remove(fpos);
                        uint thisid = uint.Parse(bare.Substring(7));
                        if (thisid == cid)
                        {
                            tre.Header = add.Header;
                            return;
                        }
                        else if (thisid > cid)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    if (index == -1)
                        tree.Items.Add(empty);
                    else
                        tree.Items.Insert(index, empty);
                }
                else 
                {
                    //already has specs
                    top.Header = add.Header;
                }
            }
            else
            {
                //get info
                int specplace = add.Name.ToString().IndexOf("spec");
                string specpiec = add.Name.ToString().Substring(specplace);
                string piec = add.Name.ToString().Remove(specplace);
                control = piec;
                spec = add.Name.ToString();
                cid = uint.Parse(control.Substring(7));
                int pos = specpiec.IndexOf('f');
                spid=uint.Parse(specpiec.Substring(4,pos-4));
                string check = control + tree.Name;
                TreeViewItem top = FindName(check) as TreeViewItem;
                if(top==null){
                    //new spec
                    TreeViewItem newt = new TreeViewItem();
                    newt.IsExpanded = true;
                    newt.Expanded += OnExpanded;
                    newt.Name = check;
                    RegisterName(check, newt);
                    int i = 0, index=-1;
                    bool added = false;
                    //find olacement
                    foreach (object topcr in tree.Items)
                    {
                        TreeViewItem look = topcr as TreeViewItem;
                        int fpos = look.Name.ToString().IndexOf('f');
                        string bare = look.Name.Remove(fpos);
                        uint thisid = uint.Parse(bare.Substring(7));
                        if (thisid > cid)
                        {
                            index = i;
                            added = true;
                            break;
                        }
                        i++;
                    }
                    if (!added)
                        tree.Items.Add(newt);
                    else
                        tree.Items.Insert(index, newt);
                    top = newt;
                }
                spec = control + "spec" + spid + tree.Name;
                if (top.FindName(spec) == null)
                {
                    top.IsExpanded = true;
                    TreeViewItem empty = new TreeViewItem();
                    empty.Expanded += OnExpanded;
                    empty.Name = control + "spec" + spid + tree.Name;
                    empty.Header = add.Header;
                    RegisterName(empty.Name, empty);
                    int i = 0;
                    //find placement
                    foreach (object sp in top.Items) {
                        TreeViewItem specview = sp as TreeViewItem;
                        specplace = add.Name.ToString().IndexOf("spec");
                        specpiec = specview.Name.ToString().Substring(specplace);
                        pos = specpiec.IndexOf('f');
                        uint specid = uint.Parse(specpiec.Substring(4, pos - 4));
                        if (specid > spid)
                        {
                            top.Items.Insert(i, empty);
                            return;
                        }
                        i++;
                    }
                    top.Items.Add(empty);
                }
            }

            
        }

        /// <summary>
        /// compares name to pattermn
        /// </summary>
        /// <param name="name"></param>
        /// <returns>if namr matches control patternn</returns>
        protected static bool isRow4Control(string name)
        {
            string pattern = @"[A-Z]{2}-([0-9]{1,2})";
            string test = Regex.Replace(name, pattern, "");
            return test.Length == 0;
        }

        /// <summary>
        /// small bug fix
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem send = sender as TreeViewItem;
            send.IsSelected = true;
        }
    }
}
