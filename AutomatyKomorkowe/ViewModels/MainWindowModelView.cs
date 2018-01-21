using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
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
        int ruleNumber = 90;
        int width = 51;
        int length = 50;
        bool isOwnStateEnabled;
        bool isStarted;
        float refreshTime = 0.01f;
        string customStartStateValues;

        CellularAutomaton automaton;
        DispatcherTimer timer;
        START_STATE startState;

        public ICommand Start { get; private set; }
        public ICommand Stop { get; private set; }
        public ICommand IncreaseRuleNumber { get; private set; }
        public ICommand DecreaseRuleNumber { get; private set; }
        public ICommand ResetStartSate { get; private set; }

        public BitmapSource Map
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChange("Map");
            }
        }

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

        public bool IsOwnStateEnabled
        {
            get { return isOwnStateEnabled; }
            private set
            {
                isOwnStateEnabled = value;
                OnPropertyChange("IsOwnStateEnabled");

                if(value)
                {
                    resetStartSate(null);
                }
            }
        }

        public string CustomStartStateValues
        {
            get { return customStartStateValues; }
            set
            {
                customStartStateValues = value;

                OnPropertyChange("CustomStartStateValues");
            }
        }

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

        public bool IsStarted
        {
            get { return isStarted; }
            set
            {
                isStarted = value;
                OnPropertyChange("IsStarted");
            }
        }

        public float RefreshTime
        {
            get { return refreshTime; }
            set
            {
                refreshTime = value;
                OnPropertyChange("RefreshTime");
            }
        }

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
            string customValues = string.Empty;
            for (int i = 0; i < Width; i++)
            {
                customValues += "0;";
            }

            CustomStartStateValues = customValues.TrimEnd(';');
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
            Map = null;

            bool[] startState = null;

            if(StartState == START_STATE.OWN_STATE && IsOwnStateEnabled)
            {
                // parse values from textbox
                var values = CustomStartStateValues.Split(';');
                if (values.Length != Width)
                {
                    MessageBox.Show(String.Format("Niepoprawna ilość stanów początkowych. Jest {0}, a powinno być {1}", values.Length, Width));
                    return;
                }

                startState = new bool[Width];
                try
                {
                    for (int i = 0; i < Width; i++)
                    {
                        startState[i] = (values[i] == "1");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(String.Format("Błąd w trakcie wczytywania wartości początkowych:\n{0}", ex.Message));
                    return;
                }
            }

            automaton = new CellularAutomaton(Width, Length, RuleNumber, startState);
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
                96,
                96,
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
