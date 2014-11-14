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
    public class BeamTowerData : TowerData
    {
        public double TrackingSpeed;
        public float Range;
        public float RefireDelay;

        public float FiringHeat;
        //public ProjectileType TowerProjectileType;


        public BeamTowerData(System.Xml.XPath.XPathNavigator xml)
            : base(xml) 
        {
            //ProjectileTowerData x = new ProjectileTowerData();
            //Name = this.Name;
            //XAML = this.XAML;
            //Price = this.Price;


            //TrackingSpeed = this.TrackingSpeed;
            //Range = this.Range;
            //RefireDelay = this.RefireDelay;
            //TowerProjectileType = this.TowerProjectileType;
            //FiringHeat = this.FiringHeat;

            Range = float.Parse(xml.GetAttribute("Range", ""));
            //RefireDelay = float.Parse(xml.GetAttribute("RefireDelay", ""));
            TrackingSpeed = float.Parse(xml.GetAttribute("TrackingSpeed", ""));
            FiringHeat = float.Parse(xml.GetAttribute("FiringHeat", ""));

            //return x;
        }

    }

    public class BeamTower : Tower, IArenaObject
    {
        public Line TargetLaserA = new Line();
        public Line Beam = new Line();
        public Line TargetLaserC = new Line();
        public Line TargetLaserD = new Line();

        public FrameworkElement VisualObject = null;

        public BeamTowerData SubClassData;// = new ProjectileTowerData();
        public TargetLock Target = new TargetLock();

        public Creep CurrentTarget;

        public Shape Barrel;

        public float RefireCount = 0;

        static Random R = new Random();
        System.Windows.Media.Color ProjectileColor = Colors.Transparent;

        public new void Tick(int FrameCount)
        {
            base.Tick(FrameCount);

            if (this.SubClassData == null)
            {
                if (this.Data == null)
                {
                    return;
                }
                SubClassData = Data as BeamTowerData;
                
            }

            //if (ProjectileColor == Colors.Transparent)
            {
               // ProjectileColor = System.Windows.Media.Color.FromRgb((byte)R.Next(255), (byte)R.Next(255), (byte)R.Next(255));
               // Arena.instance.AddToArena(TargetLaserD);
                //Arena.instance.AddToArena(TargetLaserB);
            }

            CurrentTarget = null;
            double TXV;
            double TYV;
            double V;

            //System.Windows.Media.Color bc = Color.FromScRgb(1f, (float)(Heat / Data.ShutdownHeat), 0f, 0f);//(float)(Target.LockDuration / 255)
            //Body.Fill = new System.Windows.Media.SolidColorBrush(HeatColor(Heat, Data.ShutdownHeat));
            Body.Fill = StaticHelpers.HeatBrush(Heat, Data.ShutdownHeat);

            // Are we already locked on?
            if (Target.LockedTarget != null && Target.LockedTarget.IsAlive == true)
            {

                // is the locked target still in range?
                TXV = (Target.LockedTarget.XPos - PivotPoint.X);
                TYV = (Target.LockedTarget.YPos - PivotPoint.Y);
                V = (TXV) * (TXV) + (TYV) * (TYV);
                if (V < (SubClassData.Range * SubClassData.Range))
                {
                    CurrentTarget = Target.LockedTarget;
                    Target.LockDuration++;
                }
            }
            else
            {
                Arena.instance.Children.Remove(Beam);
                Beam.Stroke = Brushes.Transparent;
                Arena.instance.AddToArena(Beam);
            }

            // Find a new target
            if (CurrentTarget == null)
            {
                CurrentTarget = NearestCreep(SubClassData.Range);
                Target.LockedTarget = CurrentTarget;
                Target.LockDuration = 0;
                return;
            }

            // No valid targets
            if (CurrentTarget == null)
            {
                
                return;
            }

            if (RefireCount > 0)
            {
                RefireCount--;
            }
            
            Point TargetPoint = new Point(CurrentTarget.XPos, CurrentTarget.YPos);

            //lastsetrotation = new CoercedAngle(StaticHelpers.Angle(PivotPoint, TargetPoint));


            if (Target.LockDuration > 0)
            {
                // float MuzzleVelocity = 25;

                Beam.X1 = this.PivotPoint.X;
                Beam.Y1 = this.PivotPoint.Y;
                Beam.X2 = CurrentTarget.XPos;
                Beam.Y2 = CurrentTarget.YPos;
                // Beam.Stroke = Brushes.Green;
                Beam.StrokeThickness = 3;




                //calculate the distance to the target
                TXV = (CurrentTarget.XPos - PivotPoint.X);
                TYV = (CurrentTarget.YPos - PivotPoint.Y);
                V = Math.Sqrt((TXV) * (TXV) + (TYV) * (TYV));



                // Rotate barrel if not aligned with leading point.
                if (!IsAlignedTo(TargetPoint, SubClassData.TrackingSpeed))
                {
                    CoercedAngle diff = new CoercedAngle(StaticHelpers.Angle(PivotPoint, TargetPoint) - currentrotation.Angle);
                    if (diff.Angle > 180)
                    {
                        currentrotation.Angle -= SubClassData.TrackingSpeed;
                    }
                    else
                    {
                        currentrotation.Angle += SubClassData.TrackingSpeed;
                    }
                    //  Body.Fill = Brushes.Yellow;
                    Barrel.RenderTransform = new RotateTransform(currentrotation.Angle);
                    //return;

                }
                else
                {
                    // We are aligned (within tracking speed margin of error) with the targets predicted position.
                    currentrotation.Angle = StaticHelpers.Angle(PivotPoint, TargetPoint);
                    this.Barrel.RenderTransform = new RotateTransform(StaticHelpers.Angle(PivotPoint, TargetPoint));

                    //Are we ready to fire?

                    if (this.Heat > this.Data.ShutdownHeat)
                    {
                        RefireCount += 5;
                    }

                    if (RefireCount <= 0 && Target.LockDuration > 1 && this.Heat <= this.Data.ShutdownHeat)
                    {
                        //FIRE!!!
                        /*
                         * Projectile P = Projectile.Create(
                            FrameCount,
                            (float)PivotPoint.X,
                            (float)PivotPoint.Y,
                            (float)(TXV / V) * (float)MuzzleVelocity,
                            (float)(TYV / V) * (float)MuzzleVelocity,
                            Projectile.GenericHitCreep
                            );
                        RefireCount = SubClassData.RefireDelay;
                         */
                        Beam.Stroke = StaticHelpers.HeatBrush((float)Target.LockDuration + 50, 100f);
                        CurrentTarget.TakeDamage(Target.LockDuration / 20);
                        this.Heat += this.SubClassData.FiringHeat;
                    }
                    else
                    {
                        Beam.Stroke = Brushes.Transparent;
                    }
                }
            }
            else
            {
                Beam.Stroke = Brushes.Transparent;
            }
            
        }

        public bool IsAlignedTo(Point TargetPoint, double tolerance)
        {
            CoercedAngle diff = new CoercedAngle(StaticHelpers.Angle(PivotPoint, TargetPoint) - currentrotation.Angle);
            return (diff.Angle < tolerance || diff.Angle > (360 - tolerance));
        }

        public Creep NearestCreep(double WithinRange)
        {
            Creep nearest = null;
            Double NearestCreepDistanceSquared = (WithinRange) * (WithinRange);
            foreach (Object Y in Arena.instance.Children)
            {
                if ((Y as Creep) != null)
                {
                    Creep C = (Y as Creep);
                    {
                        double DX = (PivotPoint.X - C.XPos);
                        double DY = (PivotPoint.Y - C.YPos);
                        double DS = DX * DX + DY * DY;

                        if (DS < NearestCreepDistanceSquared)
                        {
                            NearestCreepDistanceSquared = DS;
                            nearest = C;
                        }
                    }
                }
            }
            return nearest;
        }

        public CoercedAngle lastsetrotation = new CoercedAngle(0);
        public CoercedAngle currentrotation = new CoercedAngle(0);
        public double TurretRotation
        {
            get
            {
                return currentrotation.Angle;
            }
            set
            {
                lastsetrotation.Angle = value;
            }
        }
        public Point PivotPoint
        {
            get
            {

                double X = (this.GetValue(Canvas.LeftProperty) as Double?) ?? 0;
                double Y = (this.GetValue(Canvas.TopProperty) as Double?) ?? 0;

                if (VisualObject == null)
                {
                    return new Point(0, 0);
                }

                X += VisualObject.ActualWidth / 2;
                Y += VisualObject.ActualHeight / 2;
                //X += LayoutRoot.Width / 2;
                //Y += LayoutRoot.Height / 2;
                if (double.IsNaN(X) || double.IsNaN(Y))
                {
                    return new Point(0, 0);
                }
                else
                {
                    return new Point(X, Y);
                }
            }
        }
    }
}
