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
    public class ProjectileTowerData : TowerData
    {
        public double TrackingSpeed;
        public float Range;
        public float RefireDelay;

        public float FiringHeat;
        //public ProjectileType TowerProjectileType;


        public ProjectileTowerData(System.Xml.XPath.XPathNavigator xml) : base(xml) 
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
            RefireDelay = float.Parse(xml.GetAttribute("RefireDelay", ""));
            TrackingSpeed = float.Parse(xml.GetAttribute("TrackingSpeed", ""));
            FiringHeat = float.Parse(xml.GetAttribute("FiringHeat", ""));

            //return x;
        }

    }

    public class ProjectileTower : Tower, IArenaObject
    {
        public Line TargetLaserA = new Line();
        public Line TargetLaserB = new Line();
        public Line TargetLaserC = new Line();
        public Line TargetLaserD = new Line();

        public FrameworkElement VisualObject = null;

        public ProjectileTowerData SubClassData;// = new ProjectileTowerData();
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
                SubClassData = Data as ProjectileTowerData;
            }

            if (ProjectileColor == Colors.Transparent)
            {
                ProjectileColor = System.Windows.Media.Color.FromRgb((byte)R.Next(255), (byte)R.Next(255), (byte)R.Next(255));
               // Arena.instance.AddToArena(TargetLaserD);
                //Arena.instance.AddToArena(TargetLaserB);
            }



            if (RefireCount > 0)
            {
                RefireCount--;
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
               // Body.Fill = Brushes.Blue;
                return;
            }

            Point TargetPoint = new Point(CurrentTarget.XPos, CurrentTarget.YPos);

            //lastsetrotation = new CoercedAngle(StaticHelpers.Angle(PivotPoint, TargetPoint));

            
            if (Target.LockDuration > 0)
            {
                float MuzzleVelocity = 25;

           //     TargetLaserB.X1 = this.PivotPoint.X;
           //     TargetLaserB.Y1 = this.PivotPoint.Y;
            //    TargetLaserB.X2 = CurrentTarget.XPos;
            //    TargetLaserB.Y2 = CurrentTarget.YPos;
            //    TargetLaserB.Stroke = Brushes.Green;
            //    TargetLaserB.StrokeThickness = 1;

                
                

                //calculate the distance to the target
                TXV = (CurrentTarget.XPos - PivotPoint.X);
                TYV = (CurrentTarget.YPos - PivotPoint.Y);
                V = Math.Sqrt((TXV) * (TXV) + (TYV) * (TYV));

                //calculate the targets velocity
                double VelocityX = TXV - Target.LockedTargetPriorX;
                double VelocityY = TYV - Target.LockedTargetPriorY;

                //save the current position to calc velocity next frame
                Target.LockedTargetPriorX = TXV;
                Target.LockedTargetPriorY = TYV;

                
                // calculate where the target will be in the time it takes the projectile to travel to it's current location.
                double ExPosX = CurrentTarget.XPos + (VelocityX * V / MuzzleVelocity);
                double ExPosY = CurrentTarget.YPos + (VelocityY * V / MuzzleVelocity);

                TargetPoint = new Point(ExPosX, ExPosY);

           //     TargetLaserA.X1 = this.PivotPoint.X;
           //     TargetLaserA.Y1 = this.PivotPoint.Y;
          //      TargetLaserA.X2 = ExPosX;
          //      TargetLaserA.Y2 = ExPosY;
          //      TargetLaserA.Stroke = Brushes.Red;
         //       TargetLaserA.StrokeThickness = 1;
                //Arena.instance.AddToArena(TargetLaserA);


                //calculate the distance to the new position.
                TXV = (ExPosX - PivotPoint.X);
                TYV = (ExPosY - PivotPoint.Y);
                V = Math.Sqrt((TXV) * (TXV) + (TYV) * (TYV));

                // re-calculate where the target will be in the time it takes the projectile to travel to it's current location.
                ExPosX = CurrentTarget.XPos + (VelocityX * V / MuzzleVelocity);
                ExPosY = CurrentTarget.YPos + (VelocityY * V / MuzzleVelocity);

         //       TargetLaserC.X1 = this.PivotPoint.X;
         //       TargetLaserC.Y1 = this.PivotPoint.Y;
         //       TargetLaserC.X2 = ExPosX;
         //       TargetLaserC.Y2 = ExPosY;
         //       TargetLaserC.Stroke = Brushes.Blue;
         //       TargetLaserC.StrokeThickness = 1;
                //Arena.instance.AddToArena(TargetLaserC);

                //calculate the distance to the new position.
                TXV = (ExPosX - PivotPoint.X);
                TYV = (ExPosY - PivotPoint.Y);
                V = Math.Sqrt((TXV) * (TXV) + (TYV) * (TYV));

                // re-re-calculate where the target will be in the time it takes the projectile to travel to it's current location.
                ExPosX = CurrentTarget.XPos + (VelocityX * V / MuzzleVelocity);
                ExPosY = CurrentTarget.YPos + (VelocityY * V / MuzzleVelocity);

        //        TargetLaserD.X1 = this.PivotPoint.X;
        //        TargetLaserD.Y1 = this.PivotPoint.Y;
        //        TargetLaserD.X2 = ExPosX;
        //        TargetLaserD.Y2 = ExPosY;
       //         TargetLaserD.Stroke = Brushes.White;
       //         TargetLaserD.StrokeThickness = 1;

          //      DoubleCollection dc = new DoubleCollection();
           //     dc.Add(1);
          //      dc.Add(4);

          //      TargetLaserD.StrokeDashArray = dc;

                

                TargetPoint = new Point(ExPosX, ExPosY);


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
                    if (RefireCount <= 0 && Target.LockDuration > 1 && this.Heat <= this.Data.ShutdownHeat)
                    {
                        //FIRE!!!
                        Projectile P = Projectile.Create(
                            FrameCount,
                            (float)PivotPoint.X,
                            (float)PivotPoint.Y,
                            (float)(TXV / V) * (float)MuzzleVelocity,
                            (float)(TYV / V) * (float)MuzzleVelocity,
                            Projectile.GenericHitCreep
                            );
                        RefireCount = SubClassData.RefireDelay;
                        this.Heat += this.SubClassData.FiringHeat;
                    }
                }
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
