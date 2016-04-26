using System;
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

namespace Excel2DB
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

        private void PaintByCIA()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.C, p.I, p.A };
            foreach (var set in ret)
            {
                uint sum = set.C + set.A + set.I;
                ChangeBackground(set.UniqueId, ColorMaps.CIA[sum]);
            }
        }

        private void PaintByConfidentiality()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.C };
            foreach (var set in ret)
            {
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.C]);
            }
        }

        private void PaintByIntegrety()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.I };
            foreach (var set in ret)
            {
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.I]);
            }
        }

        private void PaintByAvailibility()
        {
            var ret = from p in dbcontext.Capabilities
                      select new { p.UniqueId, p.A };
            foreach (var set in ret)
            {
                ChangeBackground(set.UniqueId, ColorMaps.CIASeparate[set.A]);
            }
        }

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
        private string GetName(string id)
        {
            return Regex.Replace(id, @"[^[0-9a-zA-Z]]*", "_");
        }

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
        private void Apply_Tool_Click(object sender, RoutedEventArgs e)
        {
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

        private void HideLegends()
        {
            this.CIAlegend.Visibility = System.Windows.Visibility.Hidden;
            this.CIASeplegend.Visibility = System.Windows.Visibility.Hidden;
            this.Responsibilitylegend.Visibility = System.Windows.Visibility.Hidden;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            var rtg = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            rtg.Render(canvas);

            JpegBitmapEncoder bitmapper= new JpegBitmapEncoder();
            bitmapper.Frames.Add(BitmapFrame.Create(rtg));
            SaveFileDialog sav = new SaveFileDialog();
            sav.InitialDirectory=Properties.Settings.Default.appFolders;
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
