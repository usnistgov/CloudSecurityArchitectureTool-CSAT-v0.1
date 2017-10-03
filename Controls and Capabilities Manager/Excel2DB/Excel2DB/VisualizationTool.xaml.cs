using System;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSRC
{
    /// <summary>
    /// Interaction logic for VisualizationTool.xaml
    /// </summary>
    public partial class VisualizationTool : Window
    {
        private double wid = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width, height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 30;

        private Context.DataContext dbcontext;
        public VisualizationTool()
        {
            InitializeComponent();
            ColorMaps.colorAll();
            dbcontext = new Context.DataContext(Models.DataConnecter.EstablishValidConnection());
            
            this.WindowState = System.Windows.WindowState.Maximized;
            
        }

        //**********Painter methods***********//

        /// <summary>
        /// Change the background color of the capabilities to the cia color from colorMap
        /// </summary>
        private void PaintByCIA()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.C, p.I, p.A };
            foreach (var set in ret)
            {
                uint sum = set.C + set.A + set.I;
                ChangeTextColor(set.UniqueId, new int[] { 0, 0, 0 });
                ChangeBackground(set.UniqueId, ColorMaps.CIA[sum]);
            }
        }

        /// <summary>
        /// Add color based only on C
        /// </summary>
        private void PaintByConfidentiality()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.C };
            foreach (var set in ret)
            {
                ChangeTextColor(set.UniqueId, new int[] { 0, 0, 0 });
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.C]);
            }
        }

        /// <summary>
        /// Add color based on I
        /// </summary>
        private void PaintByIntegrety()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.I };
            foreach (var set in ret)
            {
                ChangeTextColor(set.UniqueId, new int[] { 0, 0, 0 });
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.I]);
            }
        }

        /// <summary>
        /// add color based on A
        /// </summary>
        private void PaintByAvailibility()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.A };
            foreach (var set in ret)
            {
                ChangeTextColor(set.UniqueId, new int[] { 0, 0, 0 });
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.A]);
            }
        }

        /// <summary>
        /// Assign text and background color based on responcibility vector maps
        /// </summary>
        private void PaintByResponsibilityVector()
        {
            var caps = from p in dbcontext.Capabilities
                       select new { p.UniqueId, p.ResponsibilityVector };
            foreach (var set in caps)
            {
                double value;
                if (ColorMaps.valueassign.ContainsKey(set.ResponsibilityVector.Replace("*","")))
                {
                    value = ColorMaps.valueassign[set.ResponsibilityVector.Replace("*","")];
                }
                else
                {
                     value = -2;
                }
                ChangeTextColor(set.UniqueId, ColorMaps.responsibilityTxt[value]);
                ChangeBackground(set.UniqueId, ColorMaps.responsibility[value]);
            }
        }

        //*********Utilities Methods*********//
        /// <summary>
        /// change capability id to background rgb
        /// </summary>
        /// <param name="id">id of capability to change</param>
        /// <param name="rgb">new rgb of background of capability</param>
        private void ChangeBackground(string id, int[] rgb){
            object shap = this.canvas.FindName(GetName(id));
            
            try{
                if(shap is Rectangle){
                    Rectangle rect = shap as Rectangle;
                    rect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte) rgb[0], (byte) rgb[1], (byte) rgb[2]));
                }else{
                    Ellipse ell = shap as Ellipse;
                    ell.Fill=new SolidColorBrush(Color.FromArgb((byte)255,(byte) rgb[0], (byte) rgb[1], (byte) rgb[2]));
                }
            }
            catch (Exception e) { }

        }

        /// <summary>
        /// names cannot have special symbols so this changes all to underscores
        /// </summary>
        /// <param name="id">unique id of capability</param>
        /// <returns>name ofshape</returns>
        private string GetName(string id)
        {
            return Regex.Replace(id, @"[^[0-9a-zA-Z]]*", "_");
        }

        /// <summary>
        /// change text color of name to rgb
        /// </summary>
        /// <param name="id"> capability's unique id</param>
        /// <param name="rgb">rgb to change text color to</param>
        private void ChangeTextColor(string id, int[] rgb)
        {
            object tx = this.canvas.FindName("txt_" + GetName(id));
            try
            {
                if (tx is TextBlock)
                {
                    TextBlock txt = tx as TextBlock;
                    txt.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)rgb[0], (byte)rgb[1], (byte)rgb[2]));
                }
            }
            catch (Exception ex) { }
        }
        
        /// <summary>
        /// Main event handler directs call to rightpainter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Tool_Click(object sender, RoutedEventArgs e)
        {
            //hide all legends then show the selected one and make paiter call and show legend
            string tool = this.tools.Text;
            HideLegends();
            switch (tool){
                case "Paint by CIA":
                    this.CIAlegend.Visibility = System.Windows.Visibility.Visible;
                    PaintByCIA();
                    break;
                case "Paint by Confidentiality":
                    this.CIASeplegend.Visibility = System.Windows.Visibility.Visible;
                    PaintByConfidentiality();
                    break;
                case "Paint by Integrety":
                    this.CIASeplegend.Visibility = System.Windows.Visibility.Visible;
                    PaintByIntegrety();
                    break;
                case "Paint by Availibility":
                    this.CIASeplegend.Visibility = System.Windows.Visibility.Visible;
                    PaintByAvailibility();
                    break;
                case "Paint by Responsibility":
                    this.Responsibilitylegend.Visibility = System.Windows.Visibility.Visible;
                    PaintByResponsibilityVector();
                    break;
            }
       }

        /// <summary>
        /// set all legend grids to be invisible
        /// </summary>
        private void HideLegends()
        {
            this.CIAlegend.Visibility = System.Windows.Visibility.Hidden;
            this.CIASeplegend.Visibility = System.Windows.Visibility.Hidden;
            this.Responsibilitylegend.Visibility = System.Windows.Visibility.Hidden;
        }

       /// <summary>
       /// save picture of diagram as jpeg file
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void save_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            JpegBitmapEncoder bitmapper = new JpegBitmapEncoder();
            bitmapper.Frames.Add(BitmapFrame.Create(rtb));
            SaveFileDialog sav = new SaveFileDialog();
            sav.InitialDirectory = Properties.Settings.Default.appFolders + @"\Visualizations";
            sav.Title="Save visualization to Jpeg";
            sav.Filter = "jpeg |*.jpeg";
            if(sav.ShowDialog() == true){
                var fil = System.IO.File.Create(sav.FileName);
                bitmapper.Save(fil);
                fil.Close();
            }
        }
    }
}
