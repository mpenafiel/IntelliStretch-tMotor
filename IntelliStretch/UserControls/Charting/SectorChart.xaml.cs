using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IntelliStretch.UserControls.Charting
{
    /// <summary>
    /// Interaction logic for SectorChart.xaml
    /// </summary>
    public partial class SectorChart : UserControl
    {
        public SectorChart()
        {
            InitializeComponent();
        }

        #region Dependency Properties



        public SolidColorBrush Stroke
        {
            get { return (SolidColorBrush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(SolidColorBrush), typeof(SectorChart), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));



        public SolidColorBrush Fill
        {
            get { return (SolidColorBrush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(SolidColorBrush), typeof(SectorChart), new UIPropertyMetadata(new SolidColorBrush(Colors.Red)));


        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(SectorChart), new UIPropertyMetadata(200d));




        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(SectorChart), new UIPropertyMetadata(30d));



        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(SectorChart), new UIPropertyMetadata(-30d));



        public Point CenterPoint
        {
            get { return (Point)GetValue(CenterPointProperty); }
            set { SetValue(CenterPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.Register("CenterPoint", typeof(Point), typeof(SectorChart), new UIPropertyMetadata(new Point(0d, 100d)));




        public TimeSpan AnimationBeginTime
        {
            get { return (TimeSpan)GetValue(AnimationBeginTimeProperty); }
            set { SetValue(AnimationBeginTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AnimationBeginTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationBeginTimeProperty =
            DependencyProperty.Register("AnimationBeginTime", typeof(TimeSpan), typeof(SectorChart), new UIPropertyMetadata(new TimeSpan(0,0,0)));




        #endregion

        class ChartDataContext
        {
            public ChartDataContext() { }

            public Point InitialPoint { get; set; }
            public Point StartPoint { get; set; }
            public Point EndPoint { get; set; }
        }
        ChartDataContext chartDataContext = new ChartDataContext();

        private double Deg2Rad(double degree)
        {
            return (degree * Math.PI / 180d);
        }

        private void ResetLayout()
        {
            // R is radius
            double R = this.Radius;
            arcSeg.Size = new Size(R, R);
            double upperHeight = R * Math.Sin(Deg2Rad(this.StartAngle));
            double lowerHeight = R * Math.Sin(Deg2Rad(this.EndAngle));
            this.Width = R;
            double tempHeight;
            if (upperHeight > 0 && lowerHeight < 0)
            {
                tempHeight = (upperHeight > -lowerHeight) ? upperHeight * 2 : -lowerHeight * 2;
                this.CenterPoint = new Point(0, tempHeight / 2);
            }
            else if (upperHeight <= 0 && lowerHeight <= 0)
            {
                tempHeight = -lowerHeight;
                this.CenterPoint = new Point(0, 0);
            }
            else
            {
                tempHeight = upperHeight;
                this.CenterPoint = new Point(0, upperHeight);
            }

            if (!double.IsNaN(this.Height))
            {
                if (this.Height < tempHeight)
                    this.Height = tempHeight;
                else
                    this.CenterPoint = new Point(0, this.Height / 2);
            }


            chartDataContext.InitialPoint = new Point(R, this.CenterPoint.Y);
            chartDataContext.StartPoint = new Point(R * Math.Cos(Deg2Rad(StartAngle)), this.CenterPoint.Y - upperHeight);
            chartDataContext.EndPoint = new Point(R * Math.Cos(Deg2Rad(EndAngle)), this.CenterPoint.Y - lowerHeight);
        }

        public void Refresh()
        {
            ResetLayout();

            PointAnimationUsingPath aniLine = CreatePointAnimationUsingPath(chartDataContext.InitialPoint, chartDataContext.StartPoint, new Size(this.Radius, this.Radius), 
                                                                            SweepDirection.Counterclockwise, this.AnimationBeginTime);
            PointAnimationUsingPath aniArc = CreatePointAnimationUsingPath(chartDataContext.InitialPoint, chartDataContext.EndPoint, new Size(this.Radius, this.Radius), 
                                                                            SweepDirection.Clockwise, this.AnimationBeginTime);

            lineSeg.BeginAnimation(LineSegment.PointProperty, aniLine);
            arcSeg.BeginAnimation(ArcSegment.PointProperty, aniArc);
        }

        private PointAnimationUsingPath CreatePointAnimationUsingPath(Point startPoint, Point point, Size size, SweepDirection sweepDirection, TimeSpan beginTime)
        {
            PointAnimationUsingPath animation = new PointAnimationUsingPath();
            PathFigureCollection pathFigures = new PathFigureCollection();
            PathSegmentCollection pathSegments = new PathSegmentCollection();
            ArcSegment arcPath = new ArcSegment(point, size, 0, false, sweepDirection, false);
            pathSegments.Add(arcPath);
            pathFigures.Add(new PathFigure(startPoint, pathSegments, false));
            animation.PathGeometry = new PathGeometry(pathFigures);
            animation.BeginTime = beginTime;

            return animation;
        }

    }
}
