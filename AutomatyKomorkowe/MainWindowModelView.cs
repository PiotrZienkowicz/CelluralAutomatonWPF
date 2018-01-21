using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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


        BitmapImage map;
        public BitmapImage Map
        {
            get { return map; }
            set
            {
                map = value;
                OnPropertyChange("Map");
            }
        }

        int ruleNumber = 0;
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

        int sizeX = 50;
        int sizeY = 50;

        public int SizeX
        {
            get { return sizeX; }
            set
            {
                if (value > 0)
                    sizeX = value;
                else
                    sizeX = 1;

                OnPropertyChange("SizeX");
            }
        }

        public int SizeY
        {
            get { return sizeY; }
            set
            {
                if (value > 0)
                    sizeY = value;
                else
                    sizeY = 1;

                OnPropertyChange("SizeY");
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

        float refreshTime = 1;
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
        }

        private void start(object obj)
        {
            IsStarted = true;
            //Map = new BitmapImage(); 
        }
    }
}
