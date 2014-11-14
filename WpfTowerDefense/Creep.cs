using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;


namespace WpfTowerDefense
{
    public class Creep : System.Windows.Shapes.Shape 
    {
        static public System.Collections.Generic.List<Creep> Creeps = new System.Collections.Generic.List<Creep>();
        // Used as a seed for pathing
        static int CreepCounter = 715827882;
        public int Counter;

        public List<EmptyCell> CellPath = null;


        public EmptyCell PreviousCell = null;
        public EmptyCell CurrentCell = null;
        public EmptyCell NextCell = null;

        // System.Windows.Media.PathGeometry CellTrack = null;

        public double Bounty = 50;
        double MaxHealth = 500;
        double Health = 500;
        public double Speed = 1;
        public double Progress = 0;

        public static Queue<Creep> CreepPool = new Queue<Creep>();

        public System.Windows.Media.PathGeometry Track;// = new System.Windows.Media.PathGeometry();

        public double FollowPathLength;

        public static Queue<Creep> Released = new Queue<Creep>();

        public int RelFrame = 0;

        public static void Tick(int Frame)
        {
            foreach (Creep P in Creeps)
            {
                P.InnerTick(Frame);
            }

            while (Released.Count > 0)
            {
                Creep C = Released.Dequeue();

                lock (C.CurrentCell.CreepsInCell) C.CurrentCell.CreepsInCell.Remove(C);
                Arena.instance.RemoveFromArena(C);
                Creeps.Remove(C);
                CreepPool.Enqueue(C);
                
            }

            Window1.instance.Title = Creeps.Count.ToString();

        }

        Creep(double health)
        {
            this.Visibility = Visibility.Hidden;
            Counter = CreepCounter++;
            Health = health;
            MaxHealth = health;

            XPos = 10000;
            YPos = 10000;
            SetValue(System.Windows.Controls.Canvas.LeftProperty, (double)10000);
            SetValue(System.Windows.Controls.Canvas.TopProperty, (double)10000);

          //  this.RenderTransformOrigin = new Point(0, 0);
            PreviousCell = Arena.instance.End;
            CurrentCell = Arena.instance.End;
            //NextCell = Arena.instance.End;
            NextCell = CurrentCell.ToTarget[Counter % CurrentCell.ToTarget.Count];

            //Speed = R.Next(1,4);
            //XPos = R.Next(1, 15);
            //YPos = R.Next(1, 15);
        }

        System.Windows.Media.Geometry g;

        protected override System.Windows.Media.Geometry DefiningGeometry
        {
            get
            {
               
                if (g == null)
                {
                    g = new System.Windows.Media.EllipseGeometry(new System.Windows.Point(0, 0), 10, 10);

                    //                g = new System.Windows.Media.RectangleGeometry(new System.Windows.Rect(new System.Windows.Point(-10, -5), new System.Windows.Point(10, 5)));
                }
                return g;
            }
        }


        /// <summary>
        /// Ouch!, called when the creep takes damage.
        /// </summary>
        /// <param name="DamageAmount"></param>
        public void TakeDamage(double DamageAmount)
        {
            Health -= DamageAmount;
            if (Health < 0)
            {
                Die(this);
            }
            
        }

        public bool IsAlive = false;
        //Creep C = Creep.Spawn(Health, Bounty, Progress, Speed);
        static public Creep Spawn(double health, double bounty, double progress, double speed)
        {
            Creep C = new Creep(health);
            lock (C.CurrentCell.CreepsInCell)
            {
                C.CurrentCell.CreepsInCell.Add(C);
            }
            C.Bounty = bounty;
            C.Progress = progress;
            C.Speed = speed;
            Creeps.Add(C);
            C.IsAlive = true;
            return C;

            //C.Health = ;
            //Arena.instance.BankBalance += Bounty;
            //Arena.instance.AddToArena(C);
            //C.Track = null;
        }

        /// <summary>
        /// Called when a creep is killed
        /// </summary>
        static public void Die(Creep C)
        {
            lock (C)
            {
                if (C.IsAlive == true)
                {
                    C.IsAlive = false;
                    Arena.instance.BankBalance += C.Bounty;
                    C.Bounty = 0;
                    C.Health = -1;
                    C.Track = null;
                    Released.Enqueue(C);
                }
            }
        }

        public System.Windows.Shapes.Path CreepTrack = new System.Windows.Shapes.Path();

        public void InnerTick(int Frame)
        {
            if (Health < 0)
            {
                Die(this);
                return;
            }

            Progress += 0.01;

            if (Progress < 0)
            {
                this.Visibility = Visibility.Hidden;

            }
            else
            {
                
                if (this.Visibility == Visibility.Hidden)
                {
                    this.Visibility = Visibility.Visible;    
                    Arena.instance.AddToArena(this);
                }
                

                if (Progress >= .75)//|| NextCell != CurrentCell.ToTarget[Counter % CurrentCell.ToTarget.Count]
                {
                    PreviousCell = CurrentCell;

                    lock (CurrentCell.CreepsInCell)
                    {
                        CurrentCell.CreepsInCell.Remove(this);
                    }
                    CurrentCell = NextCell;
                    lock (CurrentCell.CreepsInCell) CurrentCell.CreepsInCell.Add(this);
                    if (CurrentCell.ToTarget.Count == 0)
                    {
                        // no prize for failure.
                        Bounty = 0;

                        // remove the creep from play.
                        Die(this);

                        if (Arena.instance.Lifes-- <= 0)
                        {
                            // Game Over.
                            //MessageBox.Show("Game Over");
                        }
                    }
                    else// if (CurrentCell.ToTarget.Count == 1)
                    {
                        // NextCell = CurrentCell.ToTarget[0];
                        //}
                        //else
                        //{

                        if ((Counter & 1) == 0)
                        {
                            Counter >>= 2;
                        }
                        else
                        {
                            Counter >>= 2;
                            Counter = Counter | 1073741824;
                        }
                        NextCell = CurrentCell.ToTarget[Counter % CurrentCell.ToTarget.Count];
                    }
                    Progress -= 0.5;

                    // PERF:

                    // Generating Bezier's is expensive; some kind of cache...? or not generate per creep*per frame...

                    // should only be 30 possible curves
                    // 

                    Track = new System.Windows.Media.PathGeometry();
                    System.Windows.Media.PathFigure pf = new System.Windows.Media.PathFigure();
                    pf.StartPoint = new Point(PreviousCell.CenterPoint.X, PreviousCell.CenterPoint.Y);
                    Point p0 = new Point(CurrentCell.CenterPoint.X, CurrentCell.CenterPoint.Y);
                    //Point p1 = new Point(CurrentCell.CenterPoint.X, CurrentCell.CenterPoint.Y);
                    Point ppa = new Point(NextCell.CenterPoint.X, NextCell.CenterPoint.Y);
                    //System.Windows.Media.BezierSegment bs;
                    //bs = ;
                    pf.Segments.Add(new System.Windows.Media.BezierSegment(p0, p0, ppa, true));
                    Track.Figures.Add(pf);
                    //CreepTrack.Data = Track;
                    //CreepTrack.Stroke = System.Windows.Media.Brushes.Blue;
                    //CreepTrack.StrokeThickness = .5;
                    //CreepTrack.IsHitTestVisible = false;
                    //Arena.instance.AddToArena(CreepTrack);
                }



                Point op;
                Point ta;

                //Track.GetPointAtFractionLength((1 / Arena.instance.TrackLength) * (Progress), out op, out ta);

                //once/if I have a bezier cache, maybe cache the point-at-fraction-lenghts?
                if (Track!=null)
                {
                    
                
                Track.GetPointAtFractionLength((Progress), out op, out ta);


                XPos = (float)op.X;
                YPos = (float)op.Y;
                SetValue(System.Windows.Controls.Canvas.LeftProperty, op.X);
                SetValue(System.Windows.Controls.Canvas.TopProperty, op.Y);

                    // this.RenderTransformOrigin = new Point(0, 0);
                    //this.RenderTransform = null;

                    //this.RenderTransform = new System.Windows.Media.RotateTransform(Math.Atan2(ta.Y, ta.X) * 180 / Math.PI, 0, 0);

                }

                System.Windows.Media.Brush B = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)((Health / MaxHealth) * 255), 0, 0));
                Stroke = B;
                this.InvalidateVisual();
                
               

                
            }

        }
        public float XPos = 1000000;
        public float YPos = 1000000;

        public float _XPos
        {
            get
            {
                //return this.TranslatePoint(new Point(0, 0), (Arena.instance as UIElement)).X;
                return _XPos;
            }
            set
            {
                _XPos = value;
            }
        }

        
        public float _YPos
        {
            get
            {
                //return this.TranslatePoint(new Point(0, 0), (Arena.instance as UIElement)).Y;
                return _YPos;
            }
            set
            {
                _YPos = value;
            }
        }
    }
}
