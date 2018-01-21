using System;

namespace AutomatyKomorkowe
{
    class CellularAutomaton
    {
        public int Width { get; private set; }
        public int Length { get; private set; }

        bool [] rules;
        int currentStep;

        public int RuleNumber { get; private set; }

        public bool [][] StateMatrix { get; private set; }
        public bool CanMove { get { return (currentStep < Length); } }

        public CellularAutomaton(int width, int length, int ruleNumber, bool[] startState = null)
        {
            Width = width;
            Length = length;
            RuleNumber = ruleNumber;

            readRules();
            

            StateMatrix = new bool[Length][];

            // fill first row with values
            if (startState == null)
            {
                // default value
                int middleIndex = Width / 2;
                StateMatrix[0] = new bool[Width];
                StateMatrix[0][middleIndex] = true;
            }
            else
            {
                if (startState.Length == Width)
                    StateMatrix[0] = startState;
                else
                    throw new ArgumentException("Wrong array size.");
            }

            // fill rest of rows with false value
            for (int index = 1; index < StateMatrix.Length; index++)
            {
                StateMatrix[index] = new bool[Width];
            }

            currentStep = 1;
        }

        private void readRules()
        {
            rules = new bool[8];

            for (int i = 0; i < 8; i++)
            {
                int position = Convert.ToInt32(Math.Pow(2.0, i));

                rules[i] = ((RuleNumber & position) != 0);
            }
        }

        private int readPreviousStatesValue(int row, int col)
        {
            bool left, middle, right;

            int rowIndex = row - 1;
            if (rowIndex < 0) rowIndex = 0;

            int colIndex;


            // left field
            colIndex = col - 1;
            if (colIndex < 0) left = false;
            else left = StateMatrix[rowIndex][colIndex];

            // middle field
            colIndex = col;
            middle = StateMatrix[rowIndex][colIndex];

            // right field
            colIndex = col + 1;
            if (colIndex >= Width) right = false;
            else right = StateMatrix[rowIndex][colIndex];

            return (((left) ? 4 : 0) + ((middle) ? 2 : 0) + ((right) ? 1 : 0));
        }

        public void NextStep()
        {
            if(currentStep < Length)
            {
                for (int col = 0; col < Width; col++)
                {
                    int rulesIndex = readPreviousStatesValue(currentStep, col);
                    StateMatrix[currentStep][col] = rules[rulesIndex];
                }
                currentStep++;
            }
        }
    }
}
