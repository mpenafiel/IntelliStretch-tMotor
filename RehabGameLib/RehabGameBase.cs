using System.Windows;
using System.Windows.Controls;
using System;
using System.ComponentModel;

namespace RehabGameLib
{
    /// <summary>
    /// Rehab game base template
    /// </summary>
    public abstract class RehabGameBase : UserControl, INotifyPropertyChanged
    {
        public RehabGameBase() { }

        public enum GamePlayMode
        {
            Title = 0,
            Instruction = 1,
            Start = 2,
            Playing = 3,
            Paused = 4,
            Target = 5,
            TargetRequest = 6,
            TargetReady = 7,
            Level = 8,
            Over = 9,
            Exit = 10
        }
        public GamePlayMode PlayMode { get; set; }
        public enum Direction
        {
            X = 1,
            Y = 2,
            XY = 3,
            Z = 4,
            XZ = 5,
            YZ = 6,
            XYZ = 7
        }
        private Direction _ctrlDirection;
        public Direction CtrlDirection 
        {
            get { return _ctrlDirection; }
            set
            {
                _ctrlDirection = value;
                OnPropertyChanged("CtrlDirection");
            }
        }
        public int FlexMax { get; set; }
        public int ExtMax { get; set; }
        public int CtrlPosMax { get; set; }
        public bool isCtrlInversed { get; set; }

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Level.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(RehabGameBase), new UIPropertyMetadata(1));

        public int TotalScore
        {
            get { return (int)GetValue(TotalScoreProperty); }
            set { SetValue(TotalScoreProperty, value); }
        }
        public static readonly DependencyProperty TotalScoreProperty =
            DependencyProperty.Register("TotalScore", typeof(int), typeof(RehabGameBase), new UIPropertyMetadata(0));

        

        public int Target
        {
            get { return (int)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(int), typeof(RehabGameBase), new UIPropertyMetadata(0));



        public int TargetReachedCount
        {
            get { return (int)GetValue(TargetReachedCountProperty); }
            set { SetValue(TargetReachedCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetReachedCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetReachedCountProperty =
            DependencyProperty.Register("TargetReachedCount", typeof(int), typeof(RehabGameBase), new UIPropertyMetadata(0));



        public abstract void CreateTarget(double target);
        public abstract void SetLevel(int level);
        public abstract void UpdateUI(double data);
        public abstract void PauseGame();
        public abstract void ResumeGame();
        public abstract void ExitGame();
        protected abstract void StartGame();

        public delegate void GameEvent(GamePlayMode playMode);
        public GameEvent GameEventNotified;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        protected abstract void CollisionTest();

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        protected double Pos2Screen(double dataPos, int flexMax, int extMax, double scrMax, double scrMin, bool isInversed)
        {
            if (isInversed)
                return (scrMax - scrMin) * (flexMax - dataPos) / (flexMax - extMax) + scrMin;
            else
                return (scrMax - scrMin) * (dataPos - extMax) / (flexMax - extMax) + scrMin;
        }

        protected int Screen2Pos(double scrPos, int flexMax, int extMax, double scrMax, double scrMin, bool isInversed)
        {
            // Careful! DSP flexion is < 0, device flexion > 0
            if (isInversed)
                return -(int)((flexMax - extMax) * (scrMax - scrPos) / (scrMax - scrMin) + extMax);
            else
                return -(int)((flexMax - extMax) * (scrPos - scrMin) / (scrMax - scrMin) + extMax);
            //return -(int)(flexMax - (float)((scrPos - scrMin) * (flexMax - extMax) / (scrMax - scrMin)));
        }
    }
}
