using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace WpfTowerDefense
{
    public class MySource 
    {
        [DllImport("kernel32", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            FileMapAccess dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            uint dwNumberOfBytesToMap);

        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020,
        }
        public MySource()
        {
            PixelFormat format = PixelFormats.Bgra32;
            int max = format.BitsPerPixel;
            int sWidth = 450, sHeight = 350;
            stride = sWidth * 4;
            int count = (int)(sWidth * sHeight * format.BitsPerPixel / 8);
            pixels = new byte[count];

            section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, count, null);

            map = MapViewOfFile(section, FileMapAccess.FileMapAllAccess, (uint)0, (uint)0, (uint)count);
            //Marshal.Copy(pixels, 0, map, (int)count);
            source = System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(section, (int)sWidth, (int)sHeight, format, (int)(sWidth * format.BitsPerPixel / 8), 0) as System.Windows.Interop.InteropBitmap;
            unsafe
            {
                //ulong
                uint* buf = (uint*)map;


                for (int i = 1; i < (350 * 450); i++)
                {
                    byte[] xx = new byte[4];
                    StaticHelpers.R.NextBytes(xx);
                    uint vv = (uint)15 << 24 | (uint)xx[1] << 16 | (uint)xx[2] << 8 | (uint)xx[3];

                    buf[i] = vv;
                }
               
            }
            
        }

        public void Draw(int FrameCount)
        {
            if (source != null)
            {
                unsafe
                {
                    //ulong
                    uint* buf = (uint*)map;
                    uint vv;

                    for (int i = 1; i < (350*450); i++)
                    {
                        byte[] xx = new byte[4];
                        //StaticHelpers.R.NextBytes(xx);

                        //uint AlphaIsolation = (buf[i] & ((uint)255 << 24)) >> 24;
                        uint AlphaIsolation = (buf[i] & ((uint)255));

                        AlphaIsolation = AlphaIsolation - 1;

                        //uint AlphaShifted = ((AlphaIsolation & (uint)255) << 24);
                        uint AlphaShifted = ((AlphaIsolation & (uint)255));



                        vv = (buf[i] & (uint)((uint)256 << 16) | 65535) | AlphaShifted; // (uint)xx[0] << 24 | (uint)xx[1] << 16 | (uint)xx[2] << 8 | (uint)xx[3];


                        buf[i] = vv;
                    }

                    
                    foreach (Projectile P in Projectile.Projectiles)
                    {
                        int Frames = FrameCount - P.AquiredFrame;
                        vv = (uint)255 << 24;
                        int offset = (int)(P.XPos + P.XVel * Frames) / 2 + ((int)(P.YPos + P.YVel * Frames) / 2 * 450);
                        if (offset < (350 * 450) && offset > 0)
                        {
                            buf[offset] = vv;
                        }
                        vv = (uint)((uint)65535 << 16) | 65535;
                        offset = (int)(P.XPos + P.XVel * (Frames - 1)) / 2 + ((int)(P.YPos + P.YVel * (Frames - 1)) / 2 * 450);
                        if (offset < (350 * 450) && offset > 0)
                        {
                            buf[offset] = vv;
                        }
                    }
                }   
                source.Invalidate();
            }
            
        }


        static int stride;
        static byte[] pixels;
        static IntPtr map;
        static IntPtr section;
        static System.Windows.Interop.InteropBitmap source;

        static public System.Windows.Media.Imaging.BitmapSource Source
        {
            get
            {
                source.Invalidate();
                return (System.Windows.Media.Imaging.BitmapSource)source;//.GetAsFrozen()
            }
        }


    }

    public class ProjectileLayer : FrameworkElement
    {

        Brush ivbrush = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
        static Rect rect = new Rect(new Point(0, 0), new Size(909.3, 707));
       // protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
      //  {
            
                
           // Projectile.OnRender(drawingContext);
            
       // }
    }

    public class Projectile
    {
        //Instead of updating projectile 'position', get position by adding Vel*Frames to starting pos?


        public double ImpactDamage = 0;
        public double Flight = 0;
        public double Speed = 0;

        public float XPos = 0;
        public float YPos = 0;
        public float XVel = 0;
        public float YVel = 0;

        /// <summary>
        /// Active projectiles
        /// </summary>
        static public List<Projectile> Projectiles = new List<Projectile>();

        /// <summary>
        /// Inactive projectile objets ready for reuse
        /// </summary>
        static Queue<Projectile> ProjectilePool = new Queue<Projectile>();

        /// <summary>
        /// Delegate to be called when the the projectile hits a creep (for adding effects like splash damage)
        /// </summary>
        Action<Creep> HitCreepDelegate;

        //System.Windows.Media.DrawingVisual
        /*
        public static System.Windows.Media.DrawingVisual Render()
        {
            System.Windows.Media.DrawingVisual drawingVisual = new System.Windows.Media.DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            System.Windows.Media.DrawingContext drawingContext = drawingVisual.RenderOpen();

            System.Windows.Media.Pen MyPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);

            // Create a rectangle and draw it in the DrawingContext.
            foreach (Projectile P in Projectiles)
            {
                Rect rect = new Rect(new System.Windows.Point(P.XPos - 1, P.YPos - 1), new System.Windows.Size(2, 2));
                drawingContext.DrawRectangle(System.Windows.Media.Brushes.Black, (System.Windows.Media.Pen)null, rect);
                //drawingContext.DrawLine(MyPen, new Point(P.XPos, P.YPos), new Point(P.XPos + P.XVel, P.YPos + P.YVel));

            }
            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }
         * */
        
        //System.Windows.Media.Pen MyPen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1);
        //static Rect rect = new Rect(new Point(0, 0), new Size(2, 2));

        //public static void OnRender(System.Windows.Media.DrawingContext drawingContext)
        //{
            //foreach (Projectile P in Projectiles)
            //{
                //  rect.Location = new Point(P.XPos - 1, P.YPos - 1);
                //  drawingContext.DrawRectangle(System.Windows.Media.Brushes.Black, null, rect);//(System.Windows.Media.Pen)



                //   int T = Arena.instance.Frame - P.AquiredFrame;
                //   Point O = new Point((P.XPos + (P.XVel * T)) - 1, (P.YPos+(P.YVel*T) - 1));

                //   rect = new Rect(O, new System.Windows.Size(2, 2));
                //   drawingContext.DrawRectangle(System.Windows.Media.Brushes.Black, (System.Windows.Media.Pen)null, rect);



                //rect = new Rect(new System.Windows.Point((P.XPos - P.XVel / 10) - 1, (P.YPos - P.YVel / 10) - 1), new System.Windows.Size(2, 2));
                //drawingContext.DrawRectangle(System.Windows.Media.Brushes.Gray, (System.Windows.Media.Pen)null, rect);

                //rect = new Rect(new System.Windows.Point((P.XPos - P.XVel / 5) - 1, (P.YPos - P.YVel / 5) - 1), new System.Windows.Size(2, 2));
                //drawingContext.DrawRectangle(System.Windows.Media.Brushes.Gray, (System.Windows.Media.Pen)null, rect);

                //rect = new Rect(new System.Windows., new System.Windows.Size(2, 2));

              

                //drawingContext.DrawLine(MyPen, new Point(P.XPos, P.YPos), new Point(P.XPos + P.XVel, P.YPos + P.YVel));
            //}
        //}

        static public void GenericHitCreep(Creep C)
        {
            C.TakeDamage(10);
        }

        
        public static Projectile Create(int FrameCount, float xPos, float yPos, float xVel, float yVel, Action<Creep> hitCreepDelegate)
        {
            Projectile P = null;

            if (ProjectilePool.Count > 0)
            {
                P = ProjectilePool.Dequeue();
                if (P.RelFrame == Arena.instance.Frame)
                {
                    ProjectilePool.Enqueue(P);
                    P = new Projectile();
                }
            }
            else
            {
                P = new Projectile();
            }
            P.XPos = xPos;
            P.YPos = yPos;
            P.XVel = xVel/4;
            P.YVel = yVel/4;
            P.HitCreepDelegate = hitCreepDelegate;
            //P.Speed = speed;
            //P.Flight = 1000 / P.Speed;
            //P.ImpactDamage = 0;
            P.AquiredFrame = FrameCount;


            double F = 1000;

            if (P.XVel > 0)
            {
                F = Math.Min(F, (909 - P.XPos) / P.XVel);
            }
            else if (P.XVel < 0)
            {
                F = Math.Min(F, (P.XPos) / -P.XVel);
            }

            if (P.YVel > 0)
            {
                F = Math.Min(F, (707 - P.YPos) / P.YVel);
            }
            else if (P.YVel < 0)
            {
                F = Math.Min(F, (P.YPos) / -P.YVel);
            }

            P.Flight = F;

            Added.Enqueue(P);
            //P.Visibility = Visibility.Visible;
            return P;
        }

        /// <summary>
        /// Attemped to make projectile updates multithreaded, but had sync issues.
        /// </summary>
        const int ThreadCount = 1;

        public static Queue<Projectile>[] Released = new Queue<Projectile>[ThreadCount];
        public static Queue<Projectile> Added = new Queue<Projectile>();

        public int AquiredFrame = 0;
        public int RelFrame = 0;

        static System.Threading.Thread[] ProjectilePhysics = new System.Threading.Thread[ThreadCount];
        static ManualResetEvent[] SyncIn = new ManualResetEvent[ThreadCount];
        static AutoResetEvent[] SyncOut = new AutoResetEvent[ThreadCount];
        static ParameterizedThreadStart[] PPThreadStart = new ParameterizedThreadStart[ThreadCount];
        
        public static void Stop()
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                if (SyncOut[i] != null)
                    SyncOut[i].Set();
                //ProjectilePhysics[i].Abort();

            }
        }


        public static void Tick(int FrameCount)
        {
            if (SyncIn[0] == null)
            {
                PPThreadDelegate = PPThread;

                for (int i = 0; i < ThreadCount; i++)
                {
                    SyncIn[i] = new ManualResetEvent(false);
                    SyncOut[i] = new AutoResetEvent(false);
                    PPThreadStart[i] = new ParameterizedThreadStart(PPThreadDelegate);
                    ProjectilePhysics[i] = new Thread(PPThreadStart[i]);
                    ProjectilePhysics[i].Start(i);
                    Released[i] = new Queue<Projectile>();
                    SyncOut[i].Set();
                }
            }


            // ManualResetEvent.WaitAny(SyncIn);

            for (int i = 0; i < ThreadCount; i++)
            {

                if (SyncIn[i].WaitOne(1000) == false)
                {
                    MessageBox.Show("Thread failed to signal, resetting");
                    ProjectilePhysics[i].Abort();
                    ProjectilePhysics[i] = new Thread(PPThreadStart[i]);
                    ProjectilePhysics[i].Start(i);
                }
            }

            foreach (Projectile P in Projectiles)
            {
                P.InnerTick(FrameCount);
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                while (Released[i].Count > 0)
                {
                    Projectile P = Released[i].Dequeue();
                    ProjectilePool.Enqueue(P);
                    Projectiles.Remove(P);
                }


            }

            while (Added.Count > 0)
            {
                Projectile P = Added.Dequeue();
                //ProjectilePool.Enqueue(P);
                Projectiles.Add(P);
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                SyncIn[i].Reset();
            }

            for (int i = 0; i < ThreadCount; i++)
            {
                SyncOut[i].Set();
            }


        }
        static Action<object> PPThreadDelegate;

        static private void PPThread(object whic)
        {
            int? w = whic as int?;
            if (w == null)
            {
                return;
            }

            while (Window1.AppIsClosing == false)
            {
                SyncIn[(int)w].Reset();
                //int x = 0;
                foreach (Projectile P in Projectiles)
                {
                    if ((P.pIndex % ThreadCount) == w)
                    {
                       // lock (P)
                        {
                            EmptyCell OverCell = null;
                            //ValidTargets

                            P.Flight--;
                            //P.XPos += (P.XVel );
                            //P.YPos += (P.YVel );
                            int Frames = Arena.instance.Frame - P.AquiredFrame; // bad reaching out of scope
                            int cxPos = (int)(P.XPos + P.XVel * Frames);
                            int cyPos = (int)(P.YPos + P.YVel * Frames);

                            if (P.Flight <= 0)
                            {
                                Released[(int)w].Enqueue(P);
                            }
                            else
                            {


                                int py = (int)Math.Round((cyPos / 36.6) + 1);
                                int px;
                                if ((py) % 2 == 1)
                                {
                                    px = (int)Math.Round(((cxPos + 21.65) / 43.3) + 1);
                                }
                                else
                                {
                                    px = (int)Math.Round((cxPos / 43.3) + 1);
                                }

                                Point p = new Point(px, py);


                                OverCell = null;
                                P.ValidTargets = new List<Creep>();

                                if (Arena.instance.Cells.ContainsKey(p))
                                {
                                    OverCell = Arena.instance.Cells[p];
                                }
                                if (OverCell != null)
                                {
                                    //lock (OverCell)
                                    {
                                        lock (OverCell.CreepsInCell)
                                        {
                                            if (OverCell.CreepsInCell.Count > 0)
                                            {
                                                P.ValidTargets.AddRange(OverCell.CreepsInCell);
                                            }
                                        }
                                        foreach (EmptyCell NearCell in OverCell.Adjacent)
                                        {
                                            lock (NearCell.CreepsInCell)
                                            {
                                                if (NearCell.CreepsInCell.Count > 0)
                                                {
                                                    P.ValidTargets.AddRange(NearCell.CreepsInCell);
                                                }
                                            }
                                        }
                                    }
                                }
                                Double NearestCreepDistanceSquared = (10) * (10);
                                P.NearestCreep = null;

                                foreach (Creep C in P.ValidTargets)
                                {
                                    lock (C)
                                    {
                                        if (C != null)
                                        {
                                            if (C.IsAlive == true)
                                            {
                                                float XD = (cxPos - C.XPos);
                                                float YD = (cyPos - C.YPos);
                                                float DS = (XD) * (XD) + (YD) * (YD);

                                                if (DS < NearestCreepDistanceSquared)
                                                {
                                                    NearestCreepDistanceSquared = DS;
                                                    P.NearestCreep = C;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        //end per projectile

                    }
                }
                SyncIn[(int)w].Set();
                SyncOut[(int)w].Reset();
                SyncOut[(int)w].WaitOne();
                //lock (SyncIn[(int)w])
                //System.Threading.AutoResetEvent.SignalAndWait(SyncIn[(int)w], SyncIn[(int)w]);
            }

        }

        List<Creep> ValidTargets = new List<Creep>();
        Creep NearestCreep = null;

        private void InnerTick(int FrameCount)
        {
            Projectile P = this;

            

            if (NearestCreep != null)
            {

                HitCreepDelegate(NearestCreep);
                Released[0].Enqueue(P);

            }
        }

        private static int pCount = int.MaxValue;
        public int pIndex;

        private Projectile()
        {
            pIndex = pCount++;
        }
    }
}
