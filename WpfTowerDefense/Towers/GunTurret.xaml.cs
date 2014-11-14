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
    /// Interaction logic for GunTurret.xaml
    /// </summary>
    public partial class MachineGunTower : ProjectileTower
    {

        public MachineGunTower()
        {
            Data.Range = 400;
            
            InitializeComponent();
            Data.TowerProjectileType = ProjectileType.Sniper;
        }

        new public Point PivotPoint
        {
            get
            {

                double X = (this.GetValue(Canvas.LeftProperty) as Double?) ?? 0;
                double Y = (this.GetValue(Canvas.TopProperty) as Double?) ?? 0;

                X += this.ActualWidth / 2;
                Y += this.ActualHeight / 2;
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
    }
}

