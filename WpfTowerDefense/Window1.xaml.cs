using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Animation;


namespace WpfTowerDefense
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public static Window1 instance = null;

        public Window1()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Window1_Loaded);
            ArenaContainer.SizeChanged += new SizeChangedEventHandler(ArenaContainer_SizeChanged);
            Zoom.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Zoom_ValueChanged);
            Rotate.ValueChanged += new RoutedPropertyChangedEventHandler<double>(Rotate_ValueChanged);
            //this.SizeChanged += new SizeChangedEventHandler();
            instance = this;
            Arena.instance = TheArena;
            
            ArenaContainer.MouseMove += new MouseEventHandler(TheArena_MouseMove);
            
        }

        void Rotate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Dispatcher.BeginInvoke(new Action(ResizeArena));
        }

        void TheArena_MouseMove(object sender, MouseEventArgs e)
        {
            Point br = TheArena.TranslatePoint(new Point(Arena.instance.Width, Arena.instance.Height), ArenaContainer);
            Point tl = TheArena.TranslatePoint(new Point(0,0), ArenaContainer);
            Point tr = TheArena.TranslatePoint(new Point(Arena.instance.Width, 0), ArenaContainer);
            Point bl = TheArena.TranslatePoint(new Point(0, Arena.instance.Height), ArenaContainer);

            double hmax = Math.Max(br.Y, bl.Y);
            hmax = Math.Max(hmax, tr.Y);
            hmax = Math.Max(hmax, tl.Y);

            double hmin = Math.Min(br.Y, bl.Y);
            hmin = Math.Min(hmin, tr.Y);
            hmin = Math.Min(hmin, tl.Y);

            double wmax = Math.Max(br.X, bl.X);
            wmax = Math.Max(wmax, tr.X);
            wmax = Math.Max(wmax, tl.X);

            double wmin = Math.Min(br.X, bl.X);
            wmin = Math.Min(wmin, tr.X);
            wmin = Math.Min(wmin, tl.X);


            Window1.instance.Title = (hmax - hmin).ToString() + " " + (hmax - hmin).ToString();

            double xp = e.GetPosition(ArenaContainer).X / ArenaContainer.ActualWidth;

            xp = xp * 1.4 - .2;

            double hoff = (((wmax - wmin) ) - ArenaScroll.ActualWidth) * xp;

            double yp = e.GetPosition(ArenaContainer).Y / ArenaContainer.ActualHeight;

            yp = yp * 1.4 - .2;

            double voff = (((hmax - hmin) ) - ArenaScroll.ActualHeight) * yp;

            ArenaScroll.ScrollToHorizontalOffset(hoff);
            ArenaScroll.ScrollToVerticalOffset(voff);
           
            //_Money.Content = (hoff).ToString();//ArenaScroll.ActualWidth

            

            //ArenaScroll.ScrollToHorizontalOffset();
            // ArenaScroll.ScrollToVerticalOffset(e.GetPosition(ArenaContainer).Y);/// ArenaContainer.ActualHeight

            //ArenaScroll.ScrollToBottom();
            //TheArena.RenderTransformOrigin = new Point(1-, 1-);
           // throw new NotImplementedException();
        }

        void ArenaContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(ResizeArena));
            //e.Handled = false;
        }

        

        void Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           // MessageBox.Show(e.NewValue.ToString());

           // Res = ResizeArena;
            this.Dispatcher.BeginInvoke(new Action(ResizeArena));

            //ResizeArena(
            //e.Handled = true;
        }


        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            
         //   Arena.instance.VerticalAlignment = VerticalAlignment.Stretch;// .sca .RenderTransform = new ScaleTransform(1.5, 1.5);
          //  Arena.instance.HorizontalAlignment = HorizontalAlignment.Stretch;// .sca .RenderTransform = new ScaleTransform(1.5, 1.5);
            
            System.Xml.XPath.XPathDocument GameData = new System.Xml.XPath.XPathDocument(".\\GameData.xml");
            System.Xml.XPath.XPathNavigator GameDataNav = GameData.CreateNavigator();

            Arena.instance.BankBalance = double.Parse(GameDataNav.SelectSingleNode("/GameData/StartingCash").ToString());
            Arena.instance.Lifes = 20;

            //GameDataNav.MoveToRoot();

            //System.Xml.XPath.XPathNodeIterator dx = GameDataNav.Select("/GameData/TowerTypes/RadiatorTowers/RadiatorTower");
            System.Xml.XPath.XPathNodeIterator dx = GameDataNav.Select("/GameData/TowerTypes/*/*/@Name");
            //"/GameData/TowerTypes/*/*/@Price"
            while (dx.MoveNext())
            {
                string q = dx.Current.ToString();
                ListBoxItem x = new ListBoxItem();
                x.Content = q;
                System.Xml.XPath.XPathNavigator n = GameData.CreateNavigator();
                System.Xml.XPath.XPathNavigator o = n.SelectSingleNode("/GameData/TowerTypes/*/*[@Name='" + q + "']");
                TowerData PTD;
                if (o.Name == "ProjectileTower")
                {
                    PTD = new ProjectileTowerData(o);
                }
                else if (o.Name == "RadiatorTower")
                {
                    PTD = new HeatTowerData(o);
                }
                else if (o.Name == "BeamTower")
                {
                    PTD = new BeamTowerData(o);
                }
                else
                {
                    PTD = new TowerData(o);
                }
                x.Tag = PTD;
                Window1.instance.Towers.Items.Add(x);
            }
        }

        double ScaleFactor=1;

        void ResizeArena()
        {
            //TheArena.RenderTransformOrigin = new Point(1, 1);

            double ax = (ArenaContainer.ActualWidth / 909.3);
            double ay = (ArenaContainer.ActualHeight / 707);

            if (ax < ay)
            {
                ay = ax;
            }
            else
            {
                ax = ay;
            }

            ScaleFactor = ax * Zoom.Value;

            System.Windows.Media.TransformGroup tg = new TransformGroup();

            tg.Children.Add(new ScaleTransform(ax * Zoom.Value, ay * Zoom.Value));
            tg.Children.Add(new RotateTransform(Rotate.Value));

            TheArena.LayoutTransform = tg;

            TheArena.RenderTransformOrigin = new Point(.5, .5);
            //TheArena.RenderTransform = ;
            
            
            //TheArena.RenderTransformOrigin = new Point();
            //TheArena.RenderTransform = new RotateTransform(1);


            //TheArena.Width = e.NewSize.Width - 200;
            //TheArena.Height = e.NewSize.Height - 200;
          //  ArenaContainer.Width = 
            // ArenaContainer.Height = e.NewSize.Height - 200;
            //throw new NotImplementedException();
        }
        
        

        bool running = false;

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            Arena.instance.CallWave();
        }
        
        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (running == false)
            {
                Arena.instance.Start();
                running = true;
                this._Go_Stop.Content = "Stop";
            }
            else
            {
                Arena.instance.Stop();
                running = false;
                this._Go_Stop.Content = "Go";
            }
        }

        public static bool AppIsClosing = false;

        
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            AppIsClosing = true;
            Projectile.Stop();
            
            base.OnClosing(e);
        }

    }
}

    
            