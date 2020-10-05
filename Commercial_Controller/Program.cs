/**
* Modern Approach:
*Method 1: RequestElevator (FloorNumber)
*This method represents an elevator request on a floor or basement.
*Method 2: AssignElevator (RequestedFloor)
*This method will be used for the requests made on the first floor.
**/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commercial_Controller
{
    class Battery
    {

        public int amountColumns;
        public int amountFloors;
        public int amountBasements;
        public int floorPercolumn;
        public int amountElevatorPerColumn;
        public List<Column> columnList = new List<Column>();


        /** constructor **/

        public Battery(int amountColumns, int amountFloors, int amountBasements, int amountElevatorPerColumn)
        {
            this.amountColumns = amountColumns;
            this.amountFloors = amountFloors;
            this.amountBasements = amountBasements;
            this.amountElevatorPerColumn = amountElevatorPerColumn;

            // Set List of columns
            this.setColumnsList();
            //Set number of floor per column and define service zone (start/end) for each column
            this.setFloorPerColumn(amountBasements, amountFloors, amountColumns);

            Console.WriteLine("Battery is created: Ok !");



        }
        public void setFloorPerColumn(int amountBasements, int amountFloors, int amountColumns)
        {

            // how much floor per column?
            int remainder = 0;
            if (amountBasements == 0)
            {
                this.floorPercolumn = amountFloors / amountColumns;

            }
            else if (amountColumns == 1)
            {
                this.floorPercolumn = amountFloors;
            }
            else
            {
                this.floorPercolumn = (amountFloors - amountBasements) / (amountColumns - 1);
                remainder = (amountFloors - amountBasements) % (amountColumns - 1);

            }

            // Lets define columns zones 
            // first case: one column
            if (amountColumns == 1 && this.amountBasements == 0)
            {
                //List with one column
                columnList[0].startZone = 1;
                columnList[0].endZone = amountFloors;
            }
            else  // second case: many columns
            {
                int initStart = 1;

                //we have zero ! basements
                if (this.amountBasements == 0)
                {
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        columnList[i].startZone = initStart;
                        columnList[i].endZone = initStart + (this.floorPercolumn - 1);
                        initStart = columnList[i].endZone + 1;
                    }
                }

                //we have basements: will be affected to the first column
                else
                {
                    columnList[0].startZone = -amountBasements;
                    columnList[0].endZone = -1;
                    //  overhead floor columns starts from 2 !!
                    initStart = 2;
                    for (int i = 1; i < this.amountColumns; i++)
                    {
                        if (i == 1)
                        {
                            columnList[i].startZone = 2;
                            columnList[i].endZone = this.floorPercolumn;

                        }
                        else
                        {
                            initStart = columnList[i - 1].endZone;
                            columnList[i].startZone = initStart + 1;
                            columnList[i].endZone = initStart + (this.floorPercolumn);

                        }




                    }

                }


            }

            // if we have a extra number of floors we added it to the last column !!
            // we push the end zone of the last column to limit !!!
            if (remainder > 0)
            {
                Console.WriteLine("Aleeeeert" + remainder + "Voila " + amountFloors + "%" + amountColumns);
                columnList[columnList.Count - 1].endZone = this.floorPercolumn + remainder;
            }

        }
        public void setColumnsList()
        {
            for (int i = 1; i <= this.amountColumns; i++)
            {
                this.columnList.Add(new Column(i, this.amountFloors, this.amountBasements, this.amountElevatorPerColumn));
            }
        }

        public Column getColumn(int requestedFloor)
        {
            Column candidate = null;
            foreach (Column column in this.columnList)
            {
                if (requestedFloor >= column.startZone || requestedFloor <= column.endZone)
                {
                    candidate = column;
                }

            }
            return candidate;
        }

        public Elevator RequestElevator(int FloorNumber)
        {
            Console.WriteLine("---------- User at RC  and request the floor : " + FloorNumber + " ----------");
            Column column = getColumn(FloorNumber);
            // Elevator elevator = column.bestElevator(column, FloorNumber);
            elevator.requestList.Add(FloorNumber);
            elevator.manageRequestList(column, FloorNumber);
            //elevator.moveElevator(requestedFloor);
            return elevator;
        }

    }

}
class Column
{
    public int id;
    public int amountElevatorPerColumn;
    public int startZone;
    public int endZone;
    public List<Elevator> elevatorsList = new List<Elevator>();

    public Column(int id, int amountFloors, int amountBasements, int amountElevatorPerColumn)
    {
        this.id = id;
        this.amountElevatorPerColumn = amountElevatorPerColumn;

        for (int i = 0; i < amountElevatorPerColumn; i++)
        {
            Elevator elevator = new Elevator(i + 1, this.startZone, "Stop", "Idle");
            elevatorsList.Add(elevator);
        }

    }
    public void createElevatorsList()
    {
        for (int i = 1; i <= this.amountElevatorPerColumn; i++)
        {
            this.elevatorsList.Add(new Elevator(i, this.startZone, "Stop", "Idle"));
        }
    }
    public Elevator AssignElevator(int RequestedFloor)
    {

        return Elevator;
    }
    public Elevator bestElevator()
    {
        List<Elevator> elevatorsInMvtList = new List<Elevator>();
        List<Elevator> elevatorIdleList = new List<Elevator>();
        Elevator bestFit = null;
        foreach (Elevator elevator in this.elevatorsList)
        {
            if (elevator.direction == "Idle" && elevator.position == 1)
            {
                elevatorIdleList.Add(elevator);
            }
            else elevatorsInMvtList.Add(elevator);
        }


    }
}
class Elevator
{
    public int id;
    public int position;
    public string direction = "Stop";
    public string status = "Idle";
    public List<int> requestList = new List<int>();
    public string doors = "Closed";


    public Elevator(int id, int position, string direction, string status)
    {
        this.id = id;
        this.position = position;
        this.direction = direction;
        this.status = status;


    }


    public void manageRequestList(Column column, int requestedFloor)
    {
        if (column.startZone > 0)
        {
            if (this.direction == "up") { this.requestList.Sort(); }
            else if (this.direction == "down")
            {
                this.requestList.Sort();
                this.requestList.Reverse();
            }

        }
        else if (column.startZone < 0)
        {
            if (this.direction == "down") { this.requestList.Sort(); }
            else if (this.direction == "up")
            {
                this.requestList.Sort();
                this.requestList.Reverse();
            }
        }
    }


}
class Program
{
    static void Main(string[] args)
    {

        Battery mainBattery = new Battery(4, 66, 6, 3);
        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine("-------------------------------");
        Console.WriteLine("{0,-10} | {1,-10} | {2,-10}", " Column ID", "Start ", "End");
        Console.WriteLine("-------------------------------");
        Console.ResetColor();
        for (int i = 0; i < mainBattery.columnList.Count; i++)
        {
            Console.WriteLine(String.Format("{0,-10} | {1,-10} | {2,-10}", mainBattery.columnList[i].id, mainBattery.columnList[i].startZone, mainBattery.columnList[i].endZone));
        }


        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("---------------------[ END OF TESTING ! ]---------------------");
        Console.ResetColor();
    }

}
}
