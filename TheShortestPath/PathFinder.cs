using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace TheShortestPath
{
    class PathFinder
    {
        //input data variables
        bool[,] Field;
        Position Startpoint;
        Position Endpoint;
        //Service variables
        List<Position> OpenedList;
        List<Position> ClosedList;
                
        public struct Position
        {
           public int i,j;           
           public double length { get; set; }
           public Position(int I, int J): this()
           {
               i = I;
               j = J;
           }          
        }

        //Constructor for initializing
        public PathFinder(bool[,] field, Position startpoint, Position endpoint)
        {
            Field = field;
            Startpoint = startpoint;
            Endpoint = endpoint;
            OpenedList = new List<Position>();
            ClosedList = new List<Position>();          
        }
        //entry point
        public List<Position> Get()
        {   
            Position CurrentPoint = new Position();
            Startpoint.length = GetRemainingPathValue(Startpoint);
            OpenedList.Add(Startpoint);
            //Repeat before we destinate the end or recognize
            //that's impossible to get there
            while (OpenedList.Count != 0 && !OpenedList.Contains(Endpoint))
            {
                //figure out the closest to the end
                // position and start checking it
                CurrentPoint = GetMinimalValue();
                ClosedList.Add(CurrentPoint);                
                OpenedList.Remove(CurrentPoint);
                //Checking of  neigbour positions,
                //controlling of borders of the Array
                if (CurrentPoint.i!=Field.GetLength(0)-1)
                if (Field[CurrentPoint.i + 1, CurrentPoint.j] == true)
                    CheckPosition(new Position(CurrentPoint.i + 1, CurrentPoint.j));
                if (CurrentPoint.i!= 0)
                if (Field[CurrentPoint.i - 1, CurrentPoint.j] == true)
                    CheckPosition(new Position(CurrentPoint.i - 1, CurrentPoint.j));
                if (CurrentPoint.j!=Field.GetLength(1)-1)
                if (Field[CurrentPoint.i, CurrentPoint.j + 1] == true)
                    CheckPosition(new Position(CurrentPoint.i, CurrentPoint.j + 1));
                if (CurrentPoint.j!= 0)
                if (Field[CurrentPoint.i, CurrentPoint.j - 1] == true)
                    CheckPosition(new Position(CurrentPoint.i, CurrentPoint.j - 1));               
            }
            //Checking if the weather it is impossible or not
            if (OpenedList.Count == 0)
            {
                OpenedList.Add(new Position(-1, 0)); return OpenedList;
            }
            //else return the clue
            else
            {
                ClosedList.Remove(Startpoint);
                if (ClosedList.Count >= 2) PathFilter();//filtering of array to get the clue
                return ClosedList;
            }
        }

        //this method gets the path from the result of algorithm
        //filtering ClosedList
        private void PathFilter()
        {
            int i=ClosedList.Count-1;
            while (i != 0)
            {
                if ((Math.Abs(ClosedList[i].i - ClosedList[i - 1].i) <= 1 &&
                    Math.Abs(ClosedList[i].j - ClosedList[i - 1].j) == 0) ^
                    (Math.Abs(ClosedList[i].j - ClosedList[i - 1].j) <= 1 &&
                    Math.Abs(ClosedList[i].i - ClosedList[i - 1].i) == 0)) i--;
                else { ClosedList.Remove(ClosedList[i-1]); i--; }
            }
            i = ClosedList.Count - 1;
            while (i >3)
            {
                if ((ClosedList[i].i + ClosedList[i - 1].i -
                    ClosedList[i - 2].i - ClosedList[i - 3].i) == 0 &&
                    ClosedList[i].i != ClosedList[i - 1].i ||
                    (ClosedList[i].j + ClosedList[i - 1].j -
                    ClosedList[i - 2].j - ClosedList[i - 3].j) == 0 && 
                    ClosedList[i].j != ClosedList[i - 1].j)
                {
                    ClosedList.Remove(ClosedList[i - 1]);
                    ClosedList.Remove(ClosedList[i - 2]);
                    i = i - 2;
                }
                i--;   
            }
        }

       //This method adds unprocessed positions
        private void CheckPosition(Position CurrentPosition)
        {
            CurrentPosition.length = GetRemainingPathValue(CurrentPosition);            
            if (!OpenedList.Contains(CurrentPosition) &&
                !ClosedList.Contains(CurrentPosition))
                OpenedList.Add(CurrentPosition);            
        }

        //This Method calculates distance to the end
        private double GetRemainingPathValue(Position CurrentPoint)
        {
            return Math.Pow(CurrentPoint.i-Endpoint.i, 2)+
                Math.Pow(CurrentPoint.j - Endpoint.j, 2);
        }

        //this method figures out which positin is
        //the best to process
        private Position GetMinimalValue()
        {
            Position MinimalValue= OpenedList[0];
            for (int i = 0; i <= OpenedList.Count-1; i++)
            {
                if (OpenedList[i].length<=MinimalValue.length)
                {
                    MinimalValue = OpenedList[i];                    
                }
            }            
            return MinimalValue;
        }
    }
}
