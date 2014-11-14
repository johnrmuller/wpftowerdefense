using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfTowerDefense
{
    /// <summary>
    /// Fources value to be => 0 and < 360
    /// </summary>
    public class CoercedAngle
    {
        public CoercedAngle(double angle)
        {
            Angle = angle;
        }

        private double _Angle;

        public double Angle
        {
            get
            {
                return _Angle;
            }
            set
            {
                _Angle = value;

                if (_Angle < 0)
                {
                    _Angle = 360 - (Math.Abs(0 - _Angle) % 360);
                }
                _Angle = _Angle % 360;

            }
        }
    }

    public class TargetLock
    {
        public Creep LockedTarget = null;
        public double LockDuration = 0;
        public double LockedTargetPriorX = 0;
        public double LockedTargetPriorY = 0;
    }

    public static class StaticHelpers
    {
        public static Random R = new Random();

        static Color[] ThermalColor = new Color[150];
        static Brush[] ThermalBrush = new Brush[150];

        /// <summary>
        /// returns 0-99
        /// </summary>
        /// <param name="num"></param>
        /// <param name="dem"></param>
        /// <returns></returns>
        private static int CappedRatioPercent(float num, float dem)
        {
            int r = (int)((float)((num * 99) / dem));
            r = Math.Max(0, r);
            r = Math.Min(149, r);
            return r;
            
        }

        public static Brush HeatBrush(float xHeat, float xShutdownHeat)
        {
            int ts = CappedRatioPercent(xHeat, xShutdownHeat);
            if (ThermalBrush[ts] == null)
            {
                ThermalBrush[ts] = new SolidColorBrush(HeatColor( xHeat, xShutdownHeat));
                ThermalBrush[ts].Freeze();
            }

            return ThermalBrush[ts];
        }


        private static Color HeatColor(float xHeat, float xShutdownHeat)
        {

            int ts = CappedRatioPercent(xHeat, xShutdownHeat);

           // if (ThermalColor[ts] == null)
            {
                float Third = xShutdownHeat / 2;
                if (xHeat < Third)
                {
                    ThermalColor[ts] = Color.FromScRgb(1f, (float)((xHeat) / Third), 0f, 0f);
                }
                else if (xHeat < Third * 2)
                {
                    ThermalColor[ts] = Color.FromScRgb(1f, 1f, (float)((xHeat - Third) / Third), 0f);
                }
                else if (xHeat < Third * 3)
                {
                    ThermalColor[ts] = Color.FromScRgb(1f, 1f, 1f, (float)(xHeat - Third * 2) / Third);
                }
                else
                {
                    ThermalColor[ts] = Color.FromScRgb(1f, 1f, 1f, 1f);
                }
            }
            return ThermalColor[ts];
        }

        /// <summary>
        /// Adds one to x without using addition or subtraction operators.
        /// </summary>
        /// <param name="x">x</param>
        /// <returns>x+1</returns>
        static public int AddOne(this int x)
        {
            int r = int.MinValue; int c = 0; int n = 1;
            if (x == int.MaxValue)
            { throw new OverflowException("Cannot AddOne to int.MaxValue!"); }
            while ((r = ((x ^ n) & ~(c ^= ((n <<= 1) >> 2)))) < x) ;
            return r;
        }

        public static double Angle(Point p1, Point p2)
        {
            return Angle(p1.X, p1.Y, p2.X, p2.Y);
        }
        
        public static double Angle(double px1, double py1, double px2, double py2)
        {

            // Negate X and Y values 

            double pxRes = px2 - px1;
            double pyRes = py2 - py1;

            double angle = 0.0;
            // Calculate the angle 

            if (pxRes == 0.0)
            {

                if (pxRes == 0.0)
                    angle = 0.0;

                else if (pyRes > 0.0) angle = System.Math.PI / 2.0;
                else

                    angle = System.Math.PI * 3.0 / 2.0;
            }

            else if (pyRes == 0.0)
            {

                if (pxRes > 0.0)
                    angle = 0.0;

                else

                    angle = System.Math.PI;
            }

            else
            {

                if (pxRes < 0.0)
                    angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;

                else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);
                else

                    angle = System.Math.Atan(pyRes / pxRes);
            }

            // Convert to degrees 

            angle = angle * 180 / System.Math.PI;
            
            return (angle + 90) % 360;


        } 

    }
}
