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
    /// Interaction logic for CreepSpawn.xaml
    /// </summary>
    public partial class CreepSpawn : UserControl, IArenaObject
    {
        double CreepWaveFactor = 1;
        double CreepInterval = 200;
        double CreepSpawnTimer = 0;

        double CreepWaveSize = 1;

        public CreepSpawn()
        {
            InitializeComponent();
        }

        public void Tick(int Frame)
        {
            if (CreepSpawnTimer-- < 0)
            {
                CreepSpawnTimer = CreepInterval;
                SpawnWave();
            }
        }


        public void SpawnWave()
        {
            double xc = CreepWaveSize;

            this.CreepWaveFactor *= 1.1;

            if (CreepWaveFactor > CreepWaveSize)
            {
                CreepWaveFactor = 1;
                CreepInterval *= .8;
                if (CreepInterval < 50)
                {
                    CreepInterval = 1000;
                    CreepWaveSize *= 2;
                }
            }

            while (xc-- > 0)
            {
                
                //TransformGroup TG = new TransformGroup();

                double Progress = -(xc / 2);
                double Speed = 5 / CreepWaveFactor;
                double Health = 100 * CreepWaveFactor;
                double Bounty = 10;

//                C.Health = 5 * CreepWaveFactor;
  //              C.MaxHealth = 5 * CreepWaveFactor;

                //C.MaxHealth = 100 * CreepWaveFactor;

                //C.Speed = 1;

                //double up if too many creeps already on screen.
                //Arena.instance.Creeps.TrimToSize();
                if (xc >= 1 && Creep.Creeps.Count > 10)
                {
                    Health *= 2;
                    //C.MaxHealth *= 2;
                    Bounty *= 2;
                    xc -= 1;

                  //  TG.Children.Add(new ScaleTransform(1.4, 1.4));

                    //TG.Children.Add(new RotateTransform(30));
                }

                if (xc >= 2 && Creep.Creeps.Count > 20)
                {
                    Health *= 2;
                    //C.MaxHealth *= 2;
                    Bounty *= 2;
                    xc -= 2;

                    //TG.Children.Add(new ScaleTransform(1.4, 1.4));

                    //TG.Children.Add(new RotateTransform(30));
                }

                if (xc >= 4 && Creep.Creeps.Count > 30)
                {
                    Health *= 2;
                    //C.MaxHealth *= 2;
                    Bounty *= 2;
                    xc -= 4;

                    //TG.Children.Add(new ScaleTransform(1.4, 1.4));

                    //TG.Children.Add(new RotateTransform(30));
                }


                Creep C = Creep.Spawn(Health, Bounty, Progress, Speed);
                C.Visibility = Visibility.Hidden;

                //C.InArena = Arena.instance;
                C.Fill = new SolidColorBrush(Color.FromScRgb(1, (float)0, (float)(CreepInterval / 200), (float)(CreepWaveFactor - 1)));
                C.StrokeThickness = 3;
                //TG.Children.Add(new ScaleTransform(Math.Sqrt(CreepWaveFactor), Math.Sqrt(CreepWaveFactor)));

               // C.RenderTransform = TG;


               // C.Width = 200;
              //  C.Height = 200;


                // Creep.Creeps.Add(C);

               // C.RenderTransformOrigin = new Point(.5, .5);
                //C.FollowPath = Arena.instance.VisableTrack;
                //C.FollowPathLength = Arena.instance.TrackLength;
               // C.Tick();
               // Window1.instance._Lives.Content = Arena.instance.Creeps;
                
            }
        }
    }
}
