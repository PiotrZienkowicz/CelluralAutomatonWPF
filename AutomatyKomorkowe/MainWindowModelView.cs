using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AutomatyKomorkowe
{
    class MainWindowModelView : INotifyPropertyChanged
    {
        public enum START_STATE { DEFAULT = 0, OWN_STATE = 1 };

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChange(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        BitmapSource map;
        public BitmapSource Map
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChange("Map");
            }
        }

        int ruleNumber = 4;
        public int RuleNumber
        {
            get { return ruleNumber; }
            set
            {
                if (value >= 0 && value <= 255)
                    ruleNumber = value;

                if (value < 0) ruleNumber = 0;
                if (value > 255) ruleNumber = 255;

                OnPropertyChange("RuleNumber");
            }
        }


        START_STATE startState;
        public START_STATE StartState
        {
            get { return startState; }
            set
            {
                startState = value;
                OnPropertyChange("StartState");

                IsOwnStateEnabled = (startState == START_STATE.OWN_STATE);
            }
        }

        bool isOwnStateEnabled;
        public bool IsOwnStateEnabled
        {
            get { return isOwnStateEnabled; }
            private set
            {
                isOwnStateEnabled = value;
                OnPropertyChange("IsOwnStateEnabled");
            }
        }

        int width = 11;
        int length = 10;

        public int Width
        {
            get { return width; }
            set
            {
                if (value > 0)
                    width = value;
                else
                    width = 1;

                OnPropertyChange("Width");
            }
        }

        public int Length
        {
            get { return length; }
            set
            {
                if (value > 0)
                    length = value;
                else
                    length = 1;

                OnPropertyChange("Length");
            }
        }

        bool isStarted;

        public bool IsStarted
        {
            get { return isStarted; }
            set
            {
                isStarted = value;
                OnPropertyChange("IsStarted");
            }
        }

        float refreshTime = 0.1f;
        public float RefreshTime
        {
            get { return refreshTime; }
            set
            {
                refreshTime = value;
                OnPropertyChange("RefreshTime");
            }
        }


        public ICommand Start { get; private set; }
        public ICommand Stop { get; private set; }
        public ICommand IncreaseRuleNumber { get; private set; }
        public ICommand DecreaseRuleNumber { get; private set; }
        public ICommand ResetStartSate { get; private set; }

        CellularAutomaton automaton;
        DispatcherTimer timer;

        public MainWindowModelView()
        {
            startState = START_STATE.DEFAULT;

            Start = new RelayCommand(start);
            Stop = new RelayCommand(stop);
            IncreaseRuleNumber = new RelayCommand(increaseRuleNumber);
            DecreaseRuleNumber = new RelayCommand(decreaseRuleNumber);
            ResetStartSate = new RelayCommand(resetStartSate);
        }

        private void resetStartSate(object obj)
        {
            throw new NotImplementedException();
        }

        private void increaseRuleNumber(object obj)
        {
            RuleNumber = RuleNumber + 1;
        }

        private void decreaseRuleNumber(object obj)
        {
            RuleNumber = RuleNumber - 1;
        }

        private void stop(object obj)
        {
            IsStarted = false;
            if (timer != null)
                timer.Stop();
        }

        private void start(object obj)
        {
            refreshBindedValues();

            automaton = new CellularAutomaton(Width, Length, RuleNumber);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(RefreshTime * 1000));
            timer.Tick += Timer_Tick;
            timer.Start();

            IsStarted = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (automaton.CanMove)
            {
                automaton.NextStep();
                drawMap();
            }
            else
            {
                timer.Stop();
                IsStarted = false;
            }
        }

        private void drawMap()
        {
            byte[] pixelData = new byte[Width * Length];

            BitmapPalette palette = BitmapPalettes.Gray256;
            System.Windows.Media.PixelFormat pf =
                System.Windows.Media.PixelFormats.Gray8;

            int col = Width;
            int row = Length;
            int stride = col;

            byte[] pixels = new byte[stride * row];

            int a = 0, b = 0;
            for (int i = 0; i < pixels.Length; ++i, a++)
            {
                if(a == Width)
                {
                    b++;
                    a = 0;
                }

                if ((automaton.StateMatrix[b][a]))
                    pixels[i] = 0x00;
                else
                    pixels[i] = 0xff;
            }

            BitmapSource image = BitmapSource.Create(
                col,
                row,
                1,
                1,
                pf,
                palette,
                pixels,
                stride);

            Map = image;
        }

        /// <summary>
        /// Refresh all textbox value to last saved.
        /// </summary>
        private void refreshBindedValues()
        {
            Width = width;
            Length = length;
            RefreshTime = refreshTime;
            RuleNumber = ruleNumber;
        }
    }
}
