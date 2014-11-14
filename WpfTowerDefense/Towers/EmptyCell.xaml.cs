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

namespace WpfTowerDefense
{
    /// <summary>
    /// Interaction logic for EmptyCell.xaml
    /// </summary>
    public partial class EmptyCell : UserControl, IArenaObject
    {
        
        public System.Collections.Generic.List<Creep> CreepsInCell = new List<Creep>();

        public System.Collections.Generic.List<EmptyCell> Adjacent = new List<EmptyCell>(6);

        public System.Collections.Generic.List<EmptyCell> ToTarget = new List<EmptyCell>(6);
        

        private double _TargetDistance = 1000;
 //       public bool InvalidateDistance = true;


//        public double pathlength = 1000000;

        public bool Occupied = false;
        public IArenaObject Occupant = null;
        public double TravelCost = 1;


        public void Tick(int FrameCount)
        {
          
            //pc.Content = CreepsInCell.Count.ToString();
        }

        public void ValidatePath()
        {
            if (Occupied == true || Occupant != null)
            {
                TravelCost = 1000;
            }
            else
            {
                TravelCost = 1;
            }

            foreach (EmptyCell C in Adjacent)
            {
                if ((C.Occupant as Tower != null))// || C==Arena.instance.Begin)
                {
                    TravelCost*=2;
                }
            }


            double best = int.MaxValue;
            if (this == Arena.instance.Begin)
            {
                //ToTarget = null;
                best = -TravelCost;
                //return;
            }
            else
            {

             

                foreach (EmptyCell C in Adjacent)
                {
                    //   if (C.Occupant != null && C.Occupant.GetType().Equals(typeof(CreepTarget)))
                    //   {
                    //     best = 0;
                    //     ToTarget = C;
                    // }

                    if (C == Arena.instance.Begin)
                    {
                        best = 0;
                        //ToTarget = C;
                    }


                    //if (Arena.instance.PathInvaliation.Contains(C) == false)
                    {
                        //if (C.Occupied == false)
                        {

                            if (C.TargetDistance < best)
                            {
                                best = C.TargetDistance;
                                //ToTarget = C;
                            }
                        }
                    }

                }
            }
            if (best == int.MaxValue)
            {
                //Arena.instance.PathInvaliation.Enqueue(this);
                return;
            }

            ToTarget = new List<EmptyCell>();

            foreach (EmptyCell C in Adjacent)
            {
                if ((C.TargetDistance == best))// || C==Arena.instance.Begin)
                {
                    ToTarget.Add(C);
                }
            }
         //   foreach (EmptyCell C in Adjacent)
            {
               // if (( C.TargetDistance <= best+1))// || C==Arena.instance.Begin)
                {
            //        ToTarget.Add(C);
                }
            }



            if (TargetDistance != best + TravelCost)
            {
                TargetDistance = best + TravelCost;
                foreach (EmptyCell C in Adjacent)
                {
                    //if (C.TargetDistance >= _TargetDistance + 1)
                    {
                        if (Arena.instance.PathInvaliation.Contains(C) == false)
                        {
                            if (C.TargetDistance > TargetDistance + TravelCost)
                            {
                                //C.TargetDistance = TargetDistance + 1;
                                Arena.instance.PathInvaliation.Enqueue(C);
                            }
                        }
                    }
                }
            }
        }

        public double TargetDistance
        {
            get
            {
                
                    return _TargetDistance;
                }


            set
            {
                if (_TargetDistance != value)
                {
                _TargetDistance = value;
                //this.pc.Content = _TargetDistance.ToString();
                foreach (EmptyCell C in Adjacent)
                {
                    //if (C.TargetDistance >= _TargetDistance + 1)
                    {
                        if (Arena.instance.PathInvaliation.Contains(C) == false)
                        {
                            Arena.instance.PathInvaliation.Enqueue(C);
                        }
                    }
                }
                }
                //InvalidateDistance = true;
            }

        }


        /*
                    //if (Cells[C].pathlength >= (Cells[cur].pathlength + 1) && Cells[C].Occupied == false)
                    //if (C.TargetDistance >= (cur.pathlength + 1) && C.Occupied == false)
                    //{
                        //  System.Windows.Shapes.Line L = new System.Windows.Shapes.Line();
                        //  L.X1 = C.TranslatePoint(new Point(.5, .5), Arena.instance).X + 21.65;
                        //  L.Y1 = C.TranslatePoint(new Point(.5, .5), Arena.instance).Y + 25;

                        //  if (Endpoints.Contains(C) == false)
                        //  {
                        //      Endpoints.Enqueue(C);
                        //  }

                        //  L.X2 = cur.TranslatePoint(new Point(.5, .5), Arena.instance).X + 21.65;
                        //  L.Y2 = cur.TranslatePoint(new Point(.5, .5), Arena.instance).Y + 25;

                        //  L.Stroke = System.Windows.Media.Brushes.White;
                        //  L.StrokeThickness = 1;
                        //  L.IsHitTestVisible = false;
                        //  L.Fill = System.Windows.Media.Brushes.White;
                        //  L.Opacity = .25;

                        //  Arena.instance.Children.Add(L);
                        //  PathLines.Enqueue(L);
                        //  C.Tag = cur;
                   //     C.pathlength = cur.pathlength + 1;
                     //   C.pc.Content = C.ToString();
                    //}
                    //Cells[C].Body.Fill = System.Windows.Media.Brushes.White;
                    */



        public EmptyCell()
        {
            InitializeComponent();
            //this.Body.MouseDown += new MouseButtonEventHandler(EmptyCell_MouseDown);
            this.MouseEnter += new MouseEventHandler(EmptyCell_MouseMove);
            this.MouseLeave += new MouseEventHandler(EmptyCell_MouseMove);
            this.MouseMove += new MouseEventHandler(EmptyCell_MouseMove);
        }

        public void HoverEnter()
        {
            this.Body.Stroke = System.Windows.Media.Brushes.LightGreen;
            this.Body.StrokeThickness = 5;

            if (Arena.instance.Children.Contains(Arena.instance.RangePreview))
            {
                Arena.instance.Children.Remove(Arena.instance.RangePreview);
            }

            double r = 0;

            switch (Window1.instance.Towers.SelectedIndex)
            {
                case 0:
                    r = 150;

                    break;
                case 1:
                    r = 300;
                    break;
                case 2:
                    r = 450;
                    break;
            }


            Arena.instance.RangePreview.IsHitTestVisible = false;
            Arena.instance.RangePreview.Height = r*2;
            Arena.instance.RangePreview.Width = r*2;
            Arena.instance.RangePreview.Fill = Brushes.Red;
            Arena.instance.RangePreview.Opacity = 0.01;



            Arena.instance.RangePreview.SetValue(Canvas.TopProperty, this.CenterPoint.Y-r);
            Arena.instance.RangePreview.SetValue(Canvas.LeftProperty, this.CenterPoint.X-r);

            Arena.instance.Children.Add(Arena.instance.RangePreview);



        }

        public void HoverLeave()
        {
            Arena.instance.Children.Remove(Arena.instance.RangePreview);
            this.Body.Stroke = System.Windows.Media.Brushes.Transparent;
            this.Body.StrokeThickness = 0;
        }

        public void Clicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EmptyCell_MouseDown(sender, e);
        }


        void EmptyCell_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            List<EmptyCell> cand = new List<EmptyCell>();
            cand.Add(this);
            {
                System.Windows.Point qq = e.GetPosition(this);
                if (qq.X > 0 && qq.Y > 0 && qq.X < this.Width && qq.Y < this.Height)
                {
                }
                else
                {
                    //this.Body.Fill = System.Windows.Media.Brushes.DarkGreen;
                    // this.Body.StrokeThickness = 0;
                }
            }
            foreach (EmptyCell ec in Adjacent)
            {
                System.Windows.Point qq = e.GetPosition(ec);
                if (qq.X > 0 && qq.Y > 0 && qq.X < ec.Width && qq.Y < ec.Height)
                {
                    cand.Add(ec);
                    //      ec.Body.Stroke = System.Windows.Media.Brushes.LightGreen;
                    //    ec.Body.StrokeThickness = 2;
                }
                else
                {
                    // ec.Body.Fill = System.Windows.Media.Brushes.DarkGreen;
                    //ec.Body.StrokeThickness = 0;
                }
            }

            EmptyCell result = null;

            if (cand.Count == 1)
            {
                result = cand[0];
            }
            else
            {
                double dd = double.MaxValue;

                foreach (EmptyCell pr in cand)
                {
                    Point p1 = e.GetPosition(Arena.instance);
                    Point p2 = pr.CenterPoint;
                    double d = (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
                    if (d < dd)
                    {
                        result = pr;
                        dd = d;
                    }
                    //pr.pc.Content = d.ToString();
                }
            }

            Arena.instance.hovering = result;
            //result.Body.Stroke = Brushes.Blue;
            //this.

        }
        Point _CenterPoint;
        public Point CenterPoint
        {
            get
            {
                //Point p;
                //p = this.TranslatePoint(new Point(this.Width / 2, this.Height / 2), Arena.instance);

                return _CenterPoint;
            }

            set
            {
                _CenterPoint = value;
                
            }

        }

        void EmptyCell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Occupied == true || Occupant != null)
            { return; }

            ListBoxItem lbi = (Window1.instance.Towers.SelectedItem as ListBoxItem);

            if (lbi == null)
            { return; }

            Occupied = true;

            Arena.instance.PathInvaliation.Enqueue(this);

            foreach (EmptyCell C in Adjacent)
            {
                if (Arena.instance.PathInvaliation.Contains(C) == false)
                {
                    Arena.instance.PathInvaliation.Enqueue(C);
                }

            }

           // if (Arena.instance.FindPathCells(Arena.instance.Begin, Arena.instance.End) != null)
            {
                Tower b = null;

                //System.Xml.XmlParserContext xpc = new System.Xml.XmlParserContext(

                System.Windows.Markup.ParserContext parserContext;
                parserContext = new System.Windows.Markup.ParserContext();
                parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");


                TowerData PTD = lbi.Tag as TowerData;

                if ((PTD as HeatTowerData) != null)
                {
                    HeatTower ht = new HeatTower();
                    ht.Data = (PTD as HeatTowerData);

                    object tf = System.Windows.Markup.XamlReader.Parse(ht.Data.XAML, parserContext);
                    FrameworkElement tb = tf as FrameworkElement;

                    Shape bc = LogicalTreeHelper.FindLogicalNode(tb, "_Body") as Shape;
                    ht.Body = bc;

                    tb.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                    tb.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                    ht.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                    ht.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                    ht.VisualObject = tb;
                    Arena.instance.AddToArena(tb);
                    Arena.instance.AddToArena(ht);
                    ht.OwningCell = this;
                    this.Occupant = ht;
                    this.Occupied = true;
                }

                if ((PTD as ProjectileTowerData) != null)
                {
                    ProjectileTower pt = new ProjectileTower();
                    pt.Data = (PTD as ProjectileTowerData);

                    object tf = System.Windows.Markup.XamlReader.Parse(pt.Data.XAML, parserContext);

                    FrameworkElement tb = tf as FrameworkElement;
                    if (tb != null)
                    {
                        Shape bb = LogicalTreeHelper.FindLogicalNode(tb, "_Barrel") as Shape;
                        pt.Barrel = bb;
                        Shape bc = LogicalTreeHelper.FindLogicalNode(tb, "_Body") as Shape;
                        pt.Body = bc;

                        tb.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                        tb.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                        pt.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                        pt.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                        pt.VisualObject = tb;
                        Arena.instance.AddToArena(tb);
                        Arena.instance.AddToArena(pt);
                        pt.OwningCell = this;
                        this.Occupant = pt;
                        this.Occupied = true;
                        //Arena.instance.Children.Add(pt);
                        //Arena.instance.Children.Add(tb);
                    }

                    //b.OwningCell = this;
                    //Arena.instance.BankBalance -= b.Data.Price;

                    //FrameworkElement fe = new FrameworkElement();


                }
                if ((PTD as BeamTowerData) != null)
                {
                    BeamTower pt = new BeamTower();
                    pt.Data = (PTD as BeamTowerData);

                    object tf = System.Windows.Markup.XamlReader.Parse(pt.Data.XAML, parserContext);

                    FrameworkElement tb = tf as FrameworkElement;
                    if (tb != null)
                    {
                        Shape bb = LogicalTreeHelper.FindLogicalNode(tb, "_Barrel") as Shape;
                        pt.Barrel = bb;
                        Shape bc = LogicalTreeHelper.FindLogicalNode(tb, "_Body") as Shape;
                        pt.Body = bc;

                        tb.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                        tb.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                        pt.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                        pt.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                        pt.VisualObject = tb;
                        Arena.instance.AddToArena(tb);
                        Arena.instance.AddToArena(pt);
                        pt.OwningCell = this;
                        this.Occupant = pt;
                        this.Occupied = true;
                        //Arena.instance.Children.Add(pt);
                        //Arena.instance.Children.Add(tb);
                    }

                    //b.OwningCell = this;
                    //Arena.instance.BankBalance -= b.Data.Price;

                    //FrameworkElement fe = new FrameworkElement();


                }

                //TowerData
                //
              //  MessageBox.Show((lbi.Tag as System.Xml.XPath.XPathItem).Value);
                /*
                switch (Window1.instance.Towers.SelectedIndex)
                {
                    case 0:
                        b = new ShortProjectileTower();

                        break;
                    case 1:

                        b = new MediumProjectileTower();
                        break;
                    case 2:
                        b = new LongProjectileTower();
                        break;
                    default:
                        Occupied = false;
                        break;

                }
                */
                if (b != null)
                {
                    if (Arena.instance.BankBalance >= b.Data.Price)
                    {
                        b.SetValue(Canvas.TopProperty, this.GetValue(Canvas.TopProperty));
                        b.SetValue(Canvas.LeftProperty, this.GetValue(Canvas.LeftProperty));
                        b.OwningCell = this;
                        Arena.instance.Children.Add(b);
                        Arena.instance.BankBalance -= b.Data.Price;
                        Occupant = b;
                    }
                    else
                    {
                        Occupied = false;
                    }


                }
                //Occupied = true;


            }
           // else
            {
              //  Occupied = false;

            }
            foreach (EmptyCell C in Adjacent)
            {
                if (Arena.instance.PathInvaliation.Contains(C) == false)
                {
                    Arena.instance.PathInvaliation.Enqueue(C);
                }

            }

            //Arena.instance.DrawTrack();
            //Arena.instance.FindPath();
        }
    }
}
