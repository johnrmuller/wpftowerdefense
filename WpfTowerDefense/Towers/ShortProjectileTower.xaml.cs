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
    /// Interaction logic for ShortProjectileTower.xaml
    /// </summary>
    public partial class ShortProjectileTower : ProjectileTower
    {
        public ShortProjectileTower()
        {
            Data.Price = 75;
            InitializeComponent();
            Body = _Body;
            Barrel = _Barrel;

            Data.Range = 150;
            Data.TrackingSpeed = 6;
            Data.RefireDelay = 10;
            Data.TowerProjectileType = ProjectileType.Bullet;
        }
    }
}
