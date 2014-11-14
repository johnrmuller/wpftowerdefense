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
    /// Interaction logic for LongProjectileTower.xaml
    /// </summary>
    public partial class LongProjectileTower : ProjectileTower
    {
        public LongProjectileTower()
        {
            Data.Price = 225;
            InitializeComponent();
            Body = _Body;
            Barrel = _Barrel;
            Data.Range = 450;
            Data.TrackingSpeed = 2;
            Data.RefireDelay = 40;
            Data.TowerProjectileType = ProjectileType.Sniper;
        }
    }
}
