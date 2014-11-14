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
    public class HeatTowerData : TowerData
    {
        //  public double TrackingSpeed;
        //   public float Range;
        // public float RefireDelay;

        //  public float FiringHeat;
        //public float ShutdownHeat;

        //        public ProjectileType TowerProjectileType;

        public float HeatAbsorb;

        public HeatTowerData(System.Xml.XPath.XPathNavigator xml)
            : base(xml)
        {

            HeatAbsorb = float.Parse(xml.GetAttribute("HeatAbsorb", ""));



            //ProjectileTowerData x = new ProjectileTowerData();
            //Name = this.Name;
            //XAML = this.XAML;
            //Price = this.Price;
            //ShutdownHeat = this.ShutdownHeat;

            //TrackingSpeed = this.TrackingSpeed;
            //Range = this.Range;
            //RefireDelay = this.RefireDelay;
            //TowerProjectileType = this.TowerProjectileType;
            //FiringHeat = this.FiringHeat;



            //return x;
        }

    }

    public class HeatTower : Tower, IArenaObject
    {
        public FrameworkElement VisualObject = null;

        public HeatTowerData SubClassData;// = new ProjectileTowerData();
        //   public TargetLock Target = new TargetLock();

        //    public Creep CurrentTarget;

        // public Shape Barrel;

        //  public float RefireCount = 0;

        //  static Random R = new Random();
        //   System.Windows.Media.Color ProjectileColor = Colors.Transparent;



        public new void Tick(int Frame)
        {
            if (this.SubClassData == null)
            {
                if (this.Data == null)
                {
                    return;
                }
                SubClassData = Data as HeatTowerData;
            }

            
            

            //Shape bb = LogicalTreeHelper.FindLogicalNode(VisualObject, "_Grill") as Shape;

            //Body.Fill = new System.Windows.Media.VisualBrush(this);
            //System.Windows.Media.Color bc = Color.FromScRgb(1f, (float)(Heat / Data.ShutdownHeat), 0f, 0f);
            //Body.Fill = new System.Windows.Media.SolidColorBrush(bc);

            List<Tower> HotNeighbor = new List<Tower>();
            float TotalNeighborHeat = 0;

            float startingheat = this.Heat;
            float HottestNeighborHeat = 0;

            //Make of list of neighbors who's heat level is higher than self.
            foreach (EmptyCell C in this.OwningCell.Adjacent)
            {
                Tower CT = C.Occupant as Tower;

                if (CT != null)
                {
                    if (CT.Heat / CT.Data.ShutdownHeat > startingheat / Data.ShutdownHeat)
                    {
                        HotNeighbor.Add(CT);
                        TotalNeighborHeat += (CT.Heat - startingheat);
                        HottestNeighborHeat = Math.Max(HottestNeighborHeat, CT.Heat);
                    }
                }
            }

            
            
            if (HotNeighbor.Count > 0 && TotalNeighborHeat > 1)
            {
                float RemainingHeatAbsorb = SubClassData.HeatAbsorb;

                // if self is close to overheating, don't take more heat than capacity, and never get hotter than hottest neighbor.

                float ModifiedHeatLimit = Math.Min(this.Data.ShutdownHeat, HottestNeighborHeat);

                if (RemainingHeatAbsorb + startingheat > ModifiedHeatLimit)
                {
                    RemainingHeatAbsorb = ModifiedHeatLimit - startingheat;
                }
                

                

                //ModifiedHeatAbsorb = SubClassData.HeatAbsorb / HotNeighbor.Count;


                foreach (Tower HN in HotNeighbor)
                {
                    float SplitHeatAbsorb;
                    float HeatDiff = (HN.Heat - startingheat);

                    //figure this neighors 'share' of heat absorbtion.
                    SplitHeatAbsorb = (HeatDiff / TotalNeighborHeat) * RemainingHeatAbsorb;

                    //Take no more than half the heat differance.
                    if (HeatDiff < SplitHeatAbsorb * 2)
                    {
                        //float s = (this.Heat +CT.Heat)
                        this.Heat += HeatDiff / 2;
                        HN.Heat -= HeatDiff / 2;
                    }
                    else
                    {
                        this.Heat += SplitHeatAbsorb;
                        HN.Heat -= SplitHeatAbsorb;
                    }

                }
            }

            //Body.Fill = new System.Windows.Media.SolidColorBrush(HeatColor(Heat, Data.ShutdownHeat));
            Body.Fill = StaticHelpers.HeatBrush(Heat, Data.ShutdownHeat);

            base.Tick(Frame);
        }
        /*
        public Point xxxPivotPoint
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
                if (X == double.NaN || Y == double.NaN)
                {
                    return new Point(0, 0);
                }
                else
                {
                    return new Point(X, Y);
                }
            }
        }
         * */
    }
}
