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
    /// Interaction logic for BeamTurret.xaml
    /// </summary>
    public partial class BeamTurret : Tower
    {
        public BeamTurret()
        {

            InitializeComponent();
        }

        public new void Tick(int Frame)
        {
            //else
                        
                            //L.X1 = NearestCreep.TranslatePoint(new Point(15, 15), Arena).X;
                            //L.Y1 = NearestCreep.TranslatePoint(new Point(15, 15), Arena).Y;

                            //L.X2 = T.TranslatePoint(new Point(15, 15), Arena).X;
                            //L.Y2 = T.TranslatePoint(new Point(15, 15), Arena).Y;
                            //L.X2 = T.PivotPoint.X;
                            //L.Y2 = T.PivotPoint.Y;
                            //L.Opacity = 1;// T.Target.LockDuration / 100;

                            //if (BeamCounter++ == long.MaxValue)
                            {
                                //BeamCounter = long.MinValue;
                            }




                            //if (T.Type == TowerType.Laser)
                            //{
                            //  NearestCreep.Health -= T.Damage;
                            //}

                            //TBA.Enqueue(L);
        }
    }
}
