using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfTowerDefense
{

    public interface IArenaObject
    {
        void Tick(int Frame);
    }

    
    public class Arena : Canvas
    {
        //public System.Windows.Media.PathGeometry Track;
        //public System.Windows.Shapes.Path VisableTrack = new System.Windows.Shapes.Path();
        public Dictionary<Point, EmptyCell> Cells = new Dictionary<Point, EmptyCell>();
        public List<EmptyCell>[] NACells = new List<EmptyCell>[4];

        System.Windows.Media.Color[] NACellColor = new System.Windows.Media.Color[7];
        //static public System.Windows.Media.DrawingVisual ProjectileLayer = new System.Windows.Media.DrawingVisual();
        static public FrameworkElement ProjectileLayerFE = new ProjectileLayer();
        public System.Collections.ArrayList Towers = new System.Collections.ArrayList();
      

        public static Arena instance;
        private static Queue<UIElement> AddQueue = new Queue<UIElement>();
        private static Queue<UIElement> RemoveQueue = new Queue<UIElement>();


        public System.Windows.Shapes.Ellipse RangePreview = new System.Windows.Shapes.Ellipse();


        public double Lives = 20;

        //public int Creeps = 0;

        System.Windows.Threading.DispatcherTimer DT;// = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal);

        public EmptyCell Begin = null;// Cells[new Point(2, 2)];
        public EmptyCell End = null;//Cells[new Point(21, 20)];

        private EmptyCell _hovering = null;
        public EmptyCell hovering
        {
            get
            {
                return _hovering;
            }
            set
            {
                if (_hovering != value)
                {
                    if (_hovering != null)
                    {
                        _hovering.HoverLeave();
                    }

                    _hovering = value;

                    if (_hovering != null)
                    {
                        _hovering.HoverEnter();
                    }
                }
            }
        }

        private Label[] Debug = new Label[100];
        // Random R = new Random();

        public float gamespeed = 0;

        public void Start()
        {
            gamespeed = 1;
        }
        public void Stop()
        {
            gamespeed = 0;
        }

      //  int ls = 0;
      //  int fc = 0;

     //   public static int hittests = 0;

        public int Frame = 0;
        static bool InTick = false;

        void DT_Tick(object sender, EventArgs e)
        {
            
            if (InTick == true)
            {
                MessageBox.Show("Overlapping Ticks!!!");
            }
            else
            {
                mys.Draw(Frame);

                //ProjectileLayer.Source.InvalidateProperty();
                //InTick = true;

                //if (Track == null)
                //DrawTrack();

                Arena.instance.DoAddRemove();
                if (gamespeed > 0)
                {
                    Frame++;
                    Projectile.Tick(Frame);
                    //ProjectileLayerFE.InvalidateVisual();
                    Creep.Tick(Frame);

                    foreach (EmptyCell ec in NACells[Frame % 4])
                    {
                        if (ec.Occupant != null)
                        {
                            ec.Occupant.Tick(Frame);
                        }
                    }

                    //if (((float)Frame / 10f) == (int)((float)Frame / 10f))
                    //{

                        
                        // TODO: move MVH class to Projectile?

                        
                        // TODO: do to creeps and towers what I did to Projectiles; enumeration logic in static class members.

                       // foreach (Object X in Arena.instance.Children)
                      //  {
                      //      if ((X as IArenaObject) != null)
                    //        {
                      //          (X as IArenaObject).Tick(Frame);
                         //   }
                     //   }
                    //}
                }
                //InTick = false;
            }
        }

        private double _BankBalance;
        public double BankBalance
        {
            get
            {
                return _BankBalance;
            }
            set
            {
                _BankBalance = value;
                Window1.instance._Money.Content = value;

            }
        }

        private double _Lifes;
        public double Lifes
        {
            get
            {
                return _Lifes;
            }
            set
            {
                _Lifes = value;
                Window1.instance._Lives.Content = value;

            }
        }

        // Create a DrawingVisual that contains a rectangle.
//        MyVisualHost mvh = new MyVisualHost();
        
        protected override void OnInitialized(EventArgs e)
        {
            //base.OnInitialized(e);
            Arena.instance = this;

            NACellColor[0] = System.Windows.Media.Color.FromScRgb(.1f, 0f, 0f, 1f);
            NACellColor[1] = System.Windows.Media.Color.FromScRgb(.1f, 0f, 1f, 0f);
            NACellColor[2] = System.Windows.Media.Color.FromScRgb(.1f, 0f, 1f, 1f);
            NACellColor[3] = System.Windows.Media.Color.FromScRgb(.1f, 1f, 0f, 0f);
           // NACellColor[4] = System.Windows.Media.Color.FromScRgb(1f, 1f, 0f, 1f);
          //  NACellColor[5] = System.Windows.Media.Color.FromScRgb(1f, 1f, 1f, 0f);
          //  NACellColor[6] = System.Windows.Media.Color.FromScRgb(1f, 1f, 1f, 1f);

            
  //          mvh.IsHitTestVisible = false;
    //        mvh.SetValue(Canvas.ZIndexProperty, 99);
      //      mvh.SetValue(System.Windows.Media.RenderOptions.EdgeModeProperty, System.Windows.Media.EdgeMode.Aliased);
        //    this.Children.Add(mvh);

            //this

            this.Loaded += new RoutedEventHandler(Arena_Loaded);

       


            //Debug.Content = Arena.instance.Children[0].GetType().ToString();
            bool AltRowToggle = false;
            bool AltColToggle = false;
            int xx = 0;
            int yy = 0;

            NACells[0] = new List<EmptyCell>();
            NACells[1] = new List<EmptyCell>();
            NACells[2] = new List<EmptyCell>();
            NACells[3] = new List<EmptyCell>();

            for (double y = -36.6; y < 707; y += 36.6)
            {
                yy += 1;
                xx = 0;
                AltRowToggle = !AltRowToggle;
                for (double x = -43.3; x < 930.95; x += 43.3)
                {
                    AltColToggle = !AltColToggle;
                    double mx = x;
                    if (!AltRowToggle)
                    {
                        mx += 21.65;
                    }
                    xx += 1;

                    //UIElement b;
                    EmptyCell b = new EmptyCell();

                    int qq = 0;
                    if (AltColToggle) qq += 1;
                    if (AltRowToggle) qq += 2;
                    
                    b.Body.Fill = new System.Windows.Media.SolidColorBrush(NACellColor[qq]);
                    NACells[qq].Add(b);
                    
                    b.CenterPoint = new Point(mx+21.65,y+25);
                    //Height="50" Width="43.3"

                    if (mx < 0 || mx >= 866 || y < 0 || y >= 707 - 36.6)//- 43.3 
                    {
                        b.Occupant = new Block();
                        b.Occupied = true;
                    }
                    else
                    {
                        // b = new EmptyCell();
                        Cells.Add(new Point(xx, yy), b as EmptyCell);
                        Arena.instance.PathInvaliation.Enqueue(b);
                    }



                    b.SetValue(Canvas.TopProperty, y);
                    b.SetValue(Canvas.LeftProperty, mx);
                    Arena.instance.Children.Add(b);
                    if (b.Occupant as UIElement != null)
                    {
                        UIElement bo = b.Occupant as UIElement;
                        bo.SetValue(Canvas.TopProperty, y);
                        bo.SetValue(Canvas.LeftProperty, mx);

                        Arena.instance.Children.Add(b.Occupant as UIElement);
                    }
                    PathInvaliation.Enqueue(b);
                }


            }



            foreach (Point P in Cells.Keys)
            {
                //if (Cells[P].Occupied == false)
                {
                    try
                    {
                        Cells[P].Adjacent.Add(Cells[new Point(P.X + 1, P.Y)]);
                    }
                    catch
                    { }

                    try
                    {
                        Cells[P].Adjacent.Add(Cells[new Point(P.X - 1, P.Y)]);
                    }
                    catch
                    { }
                    try
                    {
                        Cells[P].Adjacent.Add(Cells[new Point(P.X, P.Y - 1)]);
                    }
                    catch
                    { }
                    try
                    {
                        Cells[P].Adjacent.Add(Cells[new Point(P.X, P.Y + 1)]);
                    }
                    catch
                    { }

                    if ((P.Y / 2) == Math.Floor(P.Y / 2))
                    {
                        try
                        {
                            Cells[P].Adjacent.Add(Cells[new Point(P.X + 1, P.Y - 1)]);
                        }
                        catch
                        { }
                        try
                        {
                            Cells[P].Adjacent.Add(Cells[new Point(P.X + 1, P.Y + 1)]);
                        }
                        catch
                        { }

                    }
                    else
                    {
                        try
                        {
                            Cells[P].Adjacent.Add(Cells[new Point(P.X - 1, P.Y - 1)]);
                        }
                        catch
                        { }
                        try
                        {
                            Cells[P].Adjacent.Add(Cells[new Point(P.X - 1, P.Y + 1)]);
                        }
                        catch
                        { }

                    }
                }

                //Arena.instance.MouseMove += new System.Windows.Input.MouseEventHandler(instance_MouseMove);

                
            }


            Begin = Cells[new Point(2, 2)];
            End = Cells[new Point(21, 20)];


            CreepSpawn c = new CreepSpawn();// Tower((TowerType)Enum.Parse(TT.GetType(), Towers.SelectedItem.ToString()));

            TheSpawner = c;
            c.SetValue(Canvas.TopProperty, End.GetValue(Canvas.TopProperty));
            c.SetValue(Canvas.LeftProperty, End.GetValue(Canvas.LeftProperty));

            Begin.Occupant = c as IArenaObject;
            //c.Opacity = .5;
            Arena.instance.Children.Add(c);

            CreepTarget d = new CreepTarget();// Tower((TowerType)Enum.Parse(TT.GetType(), Towers.SelectedItem.ToString()));

            d.SetValue(Canvas.TopProperty, Begin.GetValue(Canvas.TopProperty));
            d.SetValue(Canvas.LeftProperty, Begin.GetValue(Canvas.LeftProperty));

            End.Occupant = d as IArenaObject;
            //d.Opacity = .5;
            Arena.instance.Children.Add(d);

            /*
            System.Windows.Shapes.Line L = new System.Windows.Shapes.Line();
            L.X1 = 0;
            L.Y1 = 0;

            L.X2 = Begin.TranslatePoint(new Point(.5, .5), Arena.instance).X;
            L.Y2 = 500;

            L.Stroke = System.Windows.Media.Brushes.Salmon;
            L.StrokeThickness = 5;
            L.Fill = System.Windows.Media.Brushes.PowderBlue;
            L.Opacity = 1;

            Arena.instance.Children.Add(L);
            */


            Arena.instance.MouseLeave += new System.Windows.Input.MouseEventHandler(instance_MouseLeave);
            Arena.instance.MouseDown += new System.Windows.Input.MouseButtonEventHandler(instance_MouseDown);

            Arena.instance.FindPathCells(Begin, End);

           // this.Dispatcher.Hooks.DispatcherInactive += new EventHandler(Hooks_DispatcherInactive);

            //System.Windows.Media.CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            DT = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Background);

            


            DT.Tick += new EventHandler(DT_Tick);
            
            DT.Interval = TimeSpan.FromSeconds(.01);
            DT.Start();
            

        }
        Image xs;
        MySource mys;
        void Arena_Loaded(object sender, RoutedEventArgs e)
        {
            ProjectileLayerFE.IsHitTestVisible = false;
            ProjectileLayerFE.SetValue(Canvas.ZIndexProperty, 99);
            ProjectileLayerFE.SetValue(System.Windows.Media.RenderOptions.EdgeModeProperty, System.Windows.Media.EdgeMode.Aliased);
            ProjectileLayerFE.Height = 100;
            ProjectileLayerFE.Width = 100;
            this.Children.Add(ProjectileLayerFE);
            xs = new Image();
            xs.IsHitTestVisible = false;
            mys = new MySource();
            xs.Source = MySource.Source;
            xs.Stretch = System.Windows.Media.Stretch.Fill;

            
          //  xs.SetValue(Canvas.TopProperty, 0);
            //xs.SetValue(Canvas.LeftProperty, 0);
            xs.Height = 707;
            xs.Width = 909;
            xs.SetValue(Canvas.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            xs.SetValue(Canvas.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            xs.SetValue(Canvas.ZIndexProperty, 100);
            
            this.Children.Add(xs);
        }

        CreepSpawn TheSpawner;

        public void CallWave()
        {
            TheSpawner.SpawnWave();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
         //   DT_Tick(sender, e);
        }

        void Hooks_DispatcherInactive(object sender, EventArgs e)
        {
           // DT_Tick(sender, e);
        }

        void instance_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_hovering != null)
            {
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    _hovering.Clicked(sender, e);
                }
            }
        }

        void instance_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            hovering = null;
        }


        System.Collections.Generic.Queue<System.Windows.Shapes.Line> PathLines = new Queue<System.Windows.Shapes.Line>();




        public System.Windows.Media.PathFigure FindPath(EmptyCell PathFrom, EmptyCell PathTo)
        {
            //FindPathCells(PathFrom, PathTo);

            EmptyCell backtrace = PathTo;
            EmptyCell Prev1 = PathTo;
            System.Collections.Generic.List<Point> Tracky = new List<Point>();
            System.Windows.Media.PathFigure pf = new System.Windows.Media.PathFigure();
            //Point Prev2 = End;
            //Point Prev3 = End;
            Point p0;
            Point p1;
            Point ppa;

            System.Windows.Media.BezierSegment bs;
            //TrackLength = 0;

            pf.StartPoint = backtrace.CenterPoint;//.TranslatePoint(new Point(21.65, 25), Arena.instance);
            
            //pf.StartPoint = backtrace.TranslatePoint(new Point(21.65, 25), Arena.instance);
            
            foreach (EmptyCell ec in FindPathCells(Begin, End))
//            //while ((backtrace.X != Begin.X || backtrace.Y != Begin.Y) && ((Cells[backtrace].Tag) as Point?).HasValue)
            {
                // ec.Body.Fill = System.Windows.Media.Brushes.Purple;
                backtrace = ec;
              //  TrackLength++;
                // System.Windows.Shapes.Line L = new System.Windows.Shapes.Line();
                // L.X1 = Cells[backtrace].TranslatePoint(new Point(.5, .5), Arena.instance).X + 21.65;
                // L.Y1 = Cells[backtrace].TranslatePoint(new Point(.5, .5), Arena.instance).Y + 25;
                //double xx = 0;
                //double yy = 0;
                //xx = (Cells[backtrace].Tag as Point?).Value.X;
                //yy = (Cells[backtrace].Tag as Point?).Value.Y;

                p0 = backtrace.CenterPoint;// .TranslatePoint(new Point(21.65, 25), Arena.instance);
                p1 = Prev1.CenterPoint;// .TranslatePoint(new Point(21.65, 25), Arena.instance);

                ppa = new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);

                bs = new System.Windows.Media.BezierSegment(p1, p1, ppa, true);
                pf.Segments.Add(bs);


                //backtrace = new Point(xx, yy);

                //Prev3 = Prev2;
                //Prev2 = Prev1;
                Prev1 = ec;

                // L.X2 = Cells[new Point(xx, yy)].TranslatePoint(new Point(.5, .5), Arena.instance).X + 21.65;
                // L.Y2 = Cells[new Point(xx, yy)].TranslatePoint(new Point(.5, .5), Arena.instance).Y + 25;

                // L.Stroke = System.Windows.Media.Brushes.Green;
                // L.StrokeThickness = 4;
                //L.IsHitTestVisible = false;
                //L.Fill = System.Windows.Media.Brushes.Green;
                //L.Opacity = 1;

                //Arena.instance.Children.Add(L);
                //PathLines.Enqueue(L);

            
            }
            p0 = backtrace.CenterPoint;//.TranslatePoint(new Point(21.65, 25), Arena.instance);
            p1 = Prev1.CenterPoint;//.TranslatePoint(new Point(21.65, 25), Arena.instance);

            ppa = new Point((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);

            bs = new System.Windows.Media.BezierSegment(p1, p1, ppa, true);
            pf.Segments.Add(bs);

            return pf;
        }

        public Queue<EmptyCell> PathInvaliation = new Queue<EmptyCell>();



        public List<EmptyCell> FindPathCells(EmptyCell PathFrom, EmptyCell PathTo)
        {

            while (Arena.instance.PathInvaliation.Count > 0)
            {
                EmptyCell xxx = Arena.instance.PathInvaliation.Dequeue();
                xxx.ValidatePath();
            }

            System.Collections.Generic.Queue<EmptyCell> Endpoints = new Queue<EmptyCell>();

            EmptyCell backtrace = End;//new Point(End.X, End.Y);
            List<EmptyCell> PathCells = new List<EmptyCell>();

            while ((backtrace != Begin) && (backtrace.ToTarget != null))
            {
                PathCells.Add(backtrace);
                backtrace = (EmptyCell)backtrace.ToTarget[0];
            }

            return PathCells;
        }
        
        public void AddToArena(UIElement x)
        {
            AddQueue.Enqueue(x);
        }

        public void RemoveFromArena(UIElement x)
        {
            RemoveQueue.Enqueue(x);
        }

        public void DoAddRemove()
        {
            bool Repath = false;
            if (RemoveQueue.Count > 0)
            {
                while (RemoveQueue.Count > 0)
                {
                    if ((RemoveQueue.Peek() as Tower) != null)
                    {
                        Repath = true;
                    }
                    instance.Children.Remove(RemoveQueue.Dequeue());
                }
            }

            if (AddQueue.Count > 0)
            {
                while (AddQueue.Count > 0)
                {
                    if ((AddQueue.Peek() as Tower) != null)
                    {
                        Repath = true;
                    }
                    try
                    {
                        instance.Children.Add(AddQueue.Dequeue());
                    }
                    catch
                    { }
                }
            }
            if (Repath == true)
            {
                Arena.instance.FindPathCells(Begin, End);
            }
        }

        //    foreach (EmptyCell EC in Cells.Values)
        //   {
        //     EC.pc.Content = EC.TargetDistance.ToString();
        //EC.Tag = null;
        //EC.pathlength = int.MaxValue;
        //  }
        //Begin.pathlength = 0;
        /*
        Endpoints.Enqueue(PathFrom);

        bool done = false;

        while (PathLines.Count > 0)
        {
            try
            {
                Arena.instance.Children.Remove(PathLines.Dequeue());
            }
            catch
            {
            }
        }

        while (Endpoints.Count > 0 && done == false)
        {
            EmptyCell cur = Endpoints.Dequeue();

            {
                    
            }
        }
        if (done == false)
        {
         //   return null;
        }
        */
        

        /// <summary>
        /// Setup the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void Arena_Loaded(object sender, RoutedEventArgs e)
        //{

            //DrawTrack();


            //  VisableTrack.Effect = new System.Windows.Media.Effects.BlurEffect();

        //}

        /*
        public void xxDrawTrack()
        {

            Track = new System.Windows.Media.PathGeometry();
            Track.Figures.Add(FindPath(Begin, End));
            VisableTrack.Data = Track;
            VisableTrack.Stroke = System.Windows.Media.Brushes.Red;
            VisableTrack.StrokeThickness = 2;
            VisableTrack.IsHitTestVisible = false;
            try
            {
                Arena.instance.Children.Remove(VisableTrack);
            }
            catch
            { }

            Arena.instance.Children.Add(VisableTrack);


            for (int ee = 0; ee < (TrackLength / 10); ee++)
            {
                if (Debug[ee] == null)
                {
                    Debug[ee] = new Label();
                }
                //Arena.instance.Children.Remove(Debug[ee]);
                //Arena.instance.Children.Add(Debug[ee]);
                Point op;
                Point ta;
                Track.GetPointAtFractionLength((1 / TrackLength) * (10 * ee), out op, out ta);
                Debug[ee].SetValue(Canvas.LeftProperty, op.X);
                Debug[ee].SetValue(Canvas.TopProperty, op.Y);

                Debug[ee].Content = (ee * 10).ToString();
            }



        }
         */

        //public double TrackLength = 0;
        

        //    void Arena_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        //    {
        //       this.Children.Remove(hovering);
        //        hovering = null;
        //    }

        // void Arena_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        //   {
        //    hovering = new MachineGunTower();
        //    this.Children.Add(hovering);
        // }

        //  void Arena_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //  {
        //double SnapPosX = 0;
        //double SnapPosY = 0;

        //SnapPosX = Math.Round(e.GetPosition(this).X / 43.3) * 43.3;
        //SnapPosY = Math.Round(e.GetPosition(this).Y / 43.3) * 43.3;

        //b.SetValue(Canvas.TopProperty, SnapPosY);

        //b.SetValue(Canvas.LeftProperty, SnapPosX);

        //    Tower b = new MachineGunTower();// Tower((TowerType)Enum.Parse(TT.GetType(), Towers.SelectedItem.ToString()));
        //    b.SetValue(Canvas.TopProperty, HexSnap(e.GetPosition(this)).Y);
        //    b.SetValue(Canvas.LeftProperty, HexSnap(e.GetPosition(this)).X);
        ////
        //   Arena.instance.Children.Add(b);

        //   }

        /*
        public Point xHexSnap(Point p)
        {

            double SnapPosX = 0;
            double SnapPosY = 0;

            SnapPosX = Math.Round(p.X / 43.3);
            SnapPosY = Math.Round(p.Y / 36.6);

            if (SnapPosY / 2 == Math.Floor(SnapPosY / 2))
            {
                SnapPosX += .5;
            }

            //Debug.Content = SnapPosX.ToString() + "," + SnapPosY.ToString();

            return new Point((SnapPosX * 43.3) - 21.65, (SnapPosY * 36.6) - 25);
        }
        */
        // void Arena_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        // {

        // hovering.SetValue(Canvas.TopProperty, HexSnap(e.GetPosition(this)).Y);
        //  hovering.SetValue(Canvas.LeftProperty, HexSnap(e.GetPosition(this)).X);
        //throw new NotImplementedException();
        // }
        
    }
}
