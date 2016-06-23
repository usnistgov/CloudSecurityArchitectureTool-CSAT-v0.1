using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using prim = System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CSRC.Models;

namespace CSRC
{
    /// <summary>
    /// Interaction logic for CapabilityChooser.xaml
    /// </summary>
    public partial class CapabilityChooser : Window
    {
        private Context.DataContext dbContext;
        public List<Context.Capabilities> capsSelected;
        private double wid = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width, height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 30;
        //private double largestheight = 0;
        private SortedList<string, Color> pallate;

        /// <summary>
        /// sets controls on canvas
        /// </summary>
        public CapabilityChooser()
        {
            //maximized
            InitializeComponent();
            ResizeMode = System.Windows.ResizeMode.NoResize;
            capsSelected = new List<Context.Capabilities>();
            if (dbContext == null)
                dbContext = new Context.DataContext(DataConnecter.EstablishValidConnection());
            this.WindowState = System.Windows.WindowState.Maximized;
            
            //add buttons
            Button can = new Button();            
            can.Content = "Cancel";
            can.IsCancel = true;       

            can.Margin = new Thickness(85,15, 0, 0);
            root.Children.Add(can);

            Button ok = new Button();
            ok.Content = "Done";
            ok.Name = "Done";
            ok.IsDefault = true;
            ok.Margin = new Thickness(15,15, 0, 0);
            ok.Click += Done;
            root.Children.Add(ok);
            root.RegisterName("Done", ok);

            Button all = new Button();
            all.Content = "Selet all";
            all.Name = "all";
            all.Margin = new Thickness(168, 15, 0, 0);
            all.Click += new RoutedEventHandler(SelectAll);
            root.Children.Add(all);
            root.RegisterName("all", all);

            Label directions = new Label();
            directions.Content = "Click once to add to report, again to remove.  Read description by hovering on capability."+
                "Click done to create report.";
            directions.Margin = new Thickness(220, 15, 0, 0);
            root.Children.Add(directions);

            SetUpPallate();
            try
            {
                //place capabilities
                Addcaps();
            }
            catch ( Exception e){
                Close();
            }
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            try
            {
                //pull button
                Button click = (Button)sender;
                TextBlock text = (TextBlock)click.Content;
                string id = text.Text;
                var cap = from p in dbContext.Capabilities
                          where p.UniqueId==id
                          select p;
                //toggle button and in list
                if (capsSelected.Contains(cap.First()))
                {
                    capsSelected.Remove(cap.First());
                    click.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(255,255,255));
                }
                else
                {
                    capsSelected.Add(cap.First());
                    click.Background = new System.Windows.Media.SolidColorBrush(pallate[cap.First().Domain.ToLower()]);
                }
            }
            catch (Exception ex){}
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            capsSelected.Clear();
            var ret = from p in dbContext.Capabilities
                      select p;
            capsSelected = ret.ToList();
            foreach (Context.Capabilities cap in capsSelected)
            {
                Button capbt = (Button)(root.FindName('c' + cap.Id.ToString()));
                capbt.Background = new System.Windows.Media.SolidColorBrush(pallate[cap.Domain.ToLower()]);
            }

        }

        private void Done(object sender, RoutedEventArgs e)
        {
            //sort and replace list by id
            var ret = from p in capsSelected
                      orderby p.Id
                      select p;
            capsSelected = ret.ToList();
            DialogResult = true;
            Close();
        }

        private void Addcaps()
        {
            //get placements for domains
            var doms = (from p in dbContext.Capabilities
                        select new { p.Domain }).Distinct();
            int countDoms = doms.Count();
            List<string> domname = new List<string>();
            foreach(var s in doms){
                domname.Add(s.Domain);
            }

            List<DomainInfo> layout = GetPlaces(domname);
            double domwid = (wid / 7) +5;

            foreach (DomainInfo place in layout)
            {
                //top domains
                GroupBox domholder = new GroupBox();
                domholder.Width = place.width;
                domholder.Height = place.height;
                domholder.Header = place.name;
                domholder.Margin = place.placemant;
                domholder.BorderBrush = new System.Windows.Media.SolidColorBrush(pallate[place.name.ToLower()]);
                root.Children.Add(domholder);

                ScrollViewer domcontain = new ScrollViewer();
                domcontain.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                domcontain.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                Canvas domain = new Canvas();
                domain.Width = place.width;
                double largestwidth = place.width;
                
                domcontain.Content = domain;
                domholder.Content = domcontain;
                double xval= 5, yval = 10;

                var contains = (from p in dbContext.Capabilities
                                where p.Domain == place.name
                                select new { p.Container }).Distinct();
                List<string> containers = new List<string>();
                foreach (var i in contains)
                {
                    containers.Add(i.Container);
                }
                containers.Sort();
                double largestrowheight = 0;
                int count = 0;
                foreach (string container in containers)
                {
                    //contianers
                    count++;
                    GroupBox containHold = new GroupBox();
                    containHold.Header = container;
                    containHold.Width = 230;
                    containHold.Margin = new Thickness(xval, yval, 0, 0);
                    containHold.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(255, 255, 255));
                    containHold.Foreground=new System.Windows.Media.SolidColorBrush(Color.FromRgb(0,0,0));
                    domain.Children.Add(containHold);
                    Canvas contaner = new Canvas();
                    containHold.Content = contaner;
                    
                    double capx = 10, capy = 10;
                    //add caps
                    var cap = from p in dbContext.Capabilities
                              where p.Container == container && p.Domain == place.name
                              select new { p.UniqueId };
                    List<string> uniques = new List<string>();
                    foreach (var a in cap)
                    {
                        uniques.Add(a.UniqueId);
                    }
                    int ccount = 1;
                    foreach (string id in uniques)
                    {
                        //capabilities
                        Button capId = new Button();
                        var retval = from p in dbContext.Capabilities
                                  where p.UniqueId == id
                                  select new { p.Id, p.Description };
                        capId.Name = 'c' + retval.First().Id.ToString();
                        root.RegisterName(capId.Name, capId);
                        string descript = retval.First().Description;
                        TextBlock desc = new TextBlock();
                        desc.Text = descript;
                        desc.TextWrapping = TextWrapping.Wrap;
                        ToolTip tt = new ToolTip();
                        tt.Content = desc;
                        tt.Width = 200;
                        capId.ToolTip = tt;
                        
                        capId.Width = 100;
                        capId.Height = 40;
                        capId.Margin = new Thickness(capx, capy, 0, 0);
                        capId.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(255, 255, 255));
                        TextBlock name = new TextBlock();
                        name.Text = id;
                        name.TextWrapping = TextWrapping.Wrap;
                        capId.Content = name;
                        capId.Click += new RoutedEventHandler(Select);
                        contaner.Children.Add(capId);

                        //rows 
                        capx += capId.Width + 5;
                        if (ccount % 2 == 0 && ccount != uniques.Count) {
                            capx = 10;
                            capy += 45;
                        }
                        else if (ccount == uniques.Count)
                        {
                            capy += 55;
                        }
                        ccount++;
                    }
                    //make as tall as needed
                    containHold.Height = capy + 10;
                    if (capy > largestrowheight)
                    {
                        largestrowheight = capy;
                    }
                    //rows of containers
                    xval += containHold.Width + 20;
                    if (count % place.numContainerperrow == 0)
                    {
                        if (xval > largestwidth)
                        {
                            largestwidth = xval;
                        }
                        xval = 10;
                        yval += largestrowheight + 15;
                        largestrowheight = 0;
                    }
                    else if (count == containers.Count)
                    {
                        yval += largestrowheight + 5;
                    }
                }
                domain.Height = yval + 15;
                domain.Width = largestwidth + 50;
            }
        }

        private List<DomainInfo> GetPlaces(List<string> doms)
        {
            //place domains on screen
            doms.Sort();
            List<string> layers = new List<string>(){ "presentation services", "application services", "information services", "infrastructure services" };
            List<string> domains = new List<string>(){ "boss", "itos", "", "", "", "", "s & rm" };            
            double domwid = (wid / 7 ) +5,domheight = (height - 45);
            double laywid = 0;
            List<DomainInfo> info = new List<DomainInfo>();
            foreach (string s in doms)
            {
                DomainInfo onedom = new DomainInfo();
                onedom.name = s;
                if(layers.Contains(s.ToLower())){
                    onedom.height = domheight / 4 - 15;
                    onedom.placemant = new Thickness(2 * (domwid + 10) + 5, 45 + domheight / 4 * layers.IndexOf(s.ToLower()), 0, 0);
                    onedom.numContainerperrow = (int)((4 * domwid) / (domwid + 5));
                    onedom.width = onedom.numContainerperrow * (domwid+10);
                    laywid = onedom.width;
                }else if(domains.Contains(s.ToLower())){
                    onedom.width = domwid;
                    onedom.height = domheight;
                    onedom.placemant = new Thickness(5+(domwid + 10) * domains.IndexOf(s.ToLower()), 45, 0, 0);
                    if (s.ToLower() == domains[6])
                    {
                        onedom.placemant = new Thickness(5 + (domwid + 10) * 2 + laywid+10, 45, 0, 0);
                    }
                    onedom.numContainerperrow = 1;
                }else{
                    MessageBox.Show("Unknown Domain: " + s +  ".  Fix capabilities file, reload data and try again.");
                    throw (new Exception());
                }
                info.Add(onedom);
            }
            return info;
        }

        //colors
        private void SetUpPallate()
        {
            pallate = new SortedList<string, Color>();
            pallate.Add("boss", Color.FromRgb(153, 204, 0));
            pallate.Add("itos", Color.FromRgb(196, 215, 155));
            pallate.Add("presentation services", Color.FromRgb(0, 204, 255));
            pallate.Add("application services", Color.FromRgb(0, 153, 255));
            pallate.Add("information services",Color.FromRgb(0,204,255));
            pallate.Add("infrastructure services",Color.FromRgb(0,153,255));
            pallate.Add("s & rm", Color.FromRgb(255,204,0));
        }
    }
}