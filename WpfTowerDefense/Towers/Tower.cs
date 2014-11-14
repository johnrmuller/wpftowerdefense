using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WpfTowerDefense
{
    public class TowerData 
    {

        public double Price;
        public string Name;
        public string XAML;
        
        public float HeatDecay;
        public float ShutdownHeat;


        public TowerData(System.Xml.XPath.XPathNavigator xml) 
        {
            Price = float.Parse(xml.GetAttribute("Price", ""));
            Name = xml.GetAttribute("Name", "");
            
            ShutdownHeat = float.Parse(xml.GetAttribute("ShutdownHeat", ""));
            HeatDecay = float.Parse(xml.GetAttribute("HeatDecay", ""));
            
            XAML = xml.InnerXml;
        }
    }

    public class Tower : UserControl, IArenaObject
    {
        public float Heat = 0;

        public TowerData Data;
        public EmptyCell OwningCell;
        public System.Windows.Shapes.Shape Body;

     //   protected override void OnInitialized(EventArgs e)
     //   {
        //    base.OnInitialized(e);
         //   ContextMenu cm = new ContextMenu();
            //conte cmi
         //   cm.Items.Add("Upgrade");
         //   cm.Items.Add("Remove");
          //  cm.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(cm_PreviewMouseUp);
          //  cm.Closed += new RoutedEventHandler(cm_Closed);
           // this.ContextMenu = cm;
            //this.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(Tower_MouseRightButtonDown);
            //System.Windows.FrameworkTemplate q = new;
            //q.FindName("_Body",this);
            //Body = 
                //this.ge  _Body;
      //  }

        void cm_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Arena.instance.BankBalance += 1;
           
        }

        void cm_Closed(object sender, RoutedEventArgs e)
        {
            this.OwningCell.Occupied = false;
            this.OwningCell.Occupant = null;

            Arena.instance.BankBalance += 50;
            Arena.instance.RemoveFromArena(this);// .RemoveFromArena(this);
            
            
        }

       // void Tower_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
       // {
            
         //   throw new NotImplementedException();
       // }


        //public System.Windows.Media.Color HeatColor;

        
        public void Tick(int Frame)
        {


            if (Heat > 0)
            {
                Heat-=Data.HeatDecay ;
            }

        }
        public Tower()
        {
            //Type = TT;
            /*
            switch (TT)
            {

                case TowerType.Gun:
                    {
                        Damage = 3;
                        Range = 100;
                        RefireDelay = 5;
                        Projectile = ProjectileType.Bullet;
                        break;
                    }
                case TowerType.Cannon:
                    {
                        Projectile = ProjectileType.CannonBall;
                        RefireDelay = 20;
                        BeamColor = Colors.Orange;
                        BeamWidth = 2;
                        Damage = 2;
                        Range = 200;
                        break;
                    }
                case TowerType.Laser:
                    {
                        Projectile = ProjectileType.None;
                        BeamColor = Colors.Green;
                        BeamWidth = 2;
                        Damage = 2;
                        Range = 200;
                        break;
                    }
                case TowerType.Sniper:
                    {
                        Projectile = ProjectileType.Sniper;
                        RefireDelay = 40;
                        BeamColor = Colors.Yellow;
                        BeamWidth = 1;
                        Damage = 1;
                        Range = 400;
                        break;
                    }
                case TowerType.Missile:
                    {
                        BeamColor = Colors.Transparent;
                        BeamWidth = 0;
                        Damage = 0;
                        Range = 400;
                        break;
                    }
                default:
                    break;

            }
             * */
            
        }

      //  protected override System.Windows.Media.Geometry DefiningGeometry
      //  {
       //     get
        //    {
        //        System.Windows.Media.RectangleGeometry g = new System.Windows.Media.RectangleGeometry(new System.Windows.Rect(0, 0, 20, 20));
        //        return g;
        //    }
       // }

     //   public float Damage = 1;
     //   public ProjectileType Projectile = ProjectileType.None;
       // public TowerType Type = TowerType.Gun;
      //  public Color BeamColor = Colors.Red;
     //   public float BeamWidth = 1;


        

    }

    
}
