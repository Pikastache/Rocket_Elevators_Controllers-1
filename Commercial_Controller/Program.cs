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
            char c1 = 'A';
            for (int i = 1; i <= this.amountColumns; i++)
            {

                this.columnList.Add(new Column(c1, this.amountFloors, this.amountBasements, this.amountElevatorPerColumn));
                c1++;
            }
        }

        public Column getColumn(int requestedFloor)
        {
            Column candidate = null;
            foreach (Column column in this.columnList)
            {
                if (requestedFloor >= column.startZone && requestedFloor <= column.endZone)
                {
                    candidate = column;
                }

            }

            return candidate;
        }

        public void RequestElevator(int FloorNumber)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" User at : " + FloorNumber + " wants to go to RC");
            Console.ResetColor();


            Column column = getColumn(FloorNumber);
            Elevator elevator = column.bestElevator(FloorNumber);

            Console.Write("- Elevator  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(column.id);
            Console.Write(" [" + elevator.id + "]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Position: " + elevator.position + " sent to: " + FloorNumber);
            Console.ResetColor();



            elevator.moveElevator(FloorNumber);
            elevator.moveElevator(1);
        }
        // ***************************
        public void AssignElevator(int RequestedFloor)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Someone at RC wants to go to the : ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(RequestedFloor);
            Console.ResetColor();


            Column column = getColumn(RequestedFloor);
            //Console.WriteLine("----------> getColumn return : " + column.id);
            Elevator bestFit = null;
            List<Elevator> elevatorsInMvtList = new List<Elevator>();
            List<Elevator> elevatorIdleList = new List<Elevator>();
            List<Elevator> canBeList = new List<Elevator>();
            foreach (Elevator elevator in column.elevatorsList)
            {
                if (elevator.direction == "Idle" || elevator.position == 1)
                {
                    elevatorIdleList.Add(elevator);
                }
                else
                    elevatorsInMvtList.Add(elevator);

            }

            if (elevatorIdleList.Count > 0)
            {
                bestFit = elevatorIdleList.OrderBy(elevator => Math.Abs(elevator.position - 1)).First();
            }
            else
            {

                int rating = 0;
                foreach (Elevator elevator in elevatorsInMvtList)
                {
                    rating = Math.Abs(elevator.position - elevator.nextDestination) + Math.Abs(elevator.nextDestination - 1);
                    elevator.rating = rating;
                }

                bestFit = elevatorsInMvtList.OrderBy(elevator => elevator.rating).First();

            }

            Console.Write("- Step 1:  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(column.id);

            Console.Write(" [" + bestFit.id + "]");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Position: " + bestFit.position + " to RC");
            Console.ResetColor();
            bestFit.moveElevator(1);


            Console.Write("- Step 2: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(column.id + " [" + bestFit.id + "]");
            Console.ResetColor();
            Console.Write(" Position : ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(bestFit.position);
            Console.ResetColor();
            Console.Write(" Is sent to : ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(RequestedFloor);
            Console.ResetColor();

            bestFit.moveElevator(RequestedFloor);

        }

        public void scenario1()
        {
            this.columnList[1].elevatorsList[0].direction = "down";
            this.columnList[1].elevatorsList[0].position = 20;
            this.columnList[1].elevatorsList[0].nextDestination = 5;

            this.columnList[1].elevatorsList[1].direction = "up";
            this.columnList[1].elevatorsList[1].position = 3;
            this.columnList[1].elevatorsList[1].nextDestination = 15;

            this.columnList[1].elevatorsList[2].direction = "down";
            this.columnList[1].elevatorsList[2].position = 13;
            this.columnList[1].elevatorsList[2].nextDestination = 1;

            this.columnList[1].elevatorsList[3].direction = "down";
            this.columnList[1].elevatorsList[3].position = 15;
            this.columnList[1].elevatorsList[3].nextDestination = 2;

            this.columnList[1].elevatorsList[4].direction = "down";
            this.columnList[1].elevatorsList[4].position = 6;
            this.columnList[1].elevatorsList[4].nextDestination = 1;

            AssignElevator(20);
        }

        public void scenario2()
        {
            this.columnList[2].elevatorsList[0].direction = "up";
            this.columnList[2].elevatorsList[0].position = 1;
            this.columnList[2].elevatorsList[0].nextDestination = 21;

            this.columnList[2].elevatorsList[1].direction = "up";
            this.columnList[2].elevatorsList[1].position = 23;
            this.columnList[2].elevatorsList[1].nextDestination = 28;

            this.columnList[2].elevatorsList[2].direction = "down";
            this.columnList[2].elevatorsList[2].position = 33;
            this.columnList[2].elevatorsList[2].nextDestination = 1;

            this.columnList[2].elevatorsList[3].direction = "down";
            this.columnList[2].elevatorsList[3].position = 40;
            this.columnList[3].elevatorsList[3].nextDestination = 24;

            this.columnList[2].elevatorsList[4].direction = "down";
            this.columnList[2].elevatorsList[4].position = 39;
            this.columnList[2].elevatorsList[4].nextDestination = 1;

            AssignElevator(36);
        }

        // -------------------SCENARIO 3-------------------------
        public void scenario3()
        {
            this.columnList[3].elevatorsList[0].direction = "down";
            this.columnList[3].elevatorsList[0].position = 58;
            this.columnList[3].elevatorsList[0].nextDestination = 1;

            this.columnList[3].elevatorsList[1].direction = "up";
            this.columnList[3].elevatorsList[1].position = 50;
            this.columnList[3].elevatorsList[1].nextDestination = 60;

            this.columnList[3].elevatorsList[2].direction = "up";
            this.columnList[3].elevatorsList[2].position = 46;
            this.columnList[3].elevatorsList[2].nextDestination = 58;

            this.columnList[3].elevatorsList[3].direction = "up";
            this.columnList[3].elevatorsList[3].position = 1;
            this.columnList[3].elevatorsList[3].nextDestination = 54;

            this.columnList[3].elevatorsList[4].direction = "down";
            this.columnList[3].elevatorsList[4].position = 60;
            this.columnList[3].elevatorsList[4].nextDestination = 1;

            RequestElevator(54);
        }

        // // // -------------------SCENARIO 4-------------------------
        public void scenario4()
        {
            this.columnList[0].elevatorsList[0].direction = "Idle";
            this.columnList[0].elevatorsList[0].position = -4;
            this.columnList[0].elevatorsList[0].nextDestination = 0;

            this.columnList[0].elevatorsList[1].direction = "Idle";
            this.columnList[0].elevatorsList[1].position = 1;
            this.columnList[0].elevatorsList[1].nextDestination = 0;

            this.columnList[0].elevatorsList[2].direction = "down";
            this.columnList[0].elevatorsList[2].position = -3;
            this.columnList[0].elevatorsList[2].nextDestination = -5;

            this.columnList[0].elevatorsList[3].direction = "up";
            this.columnList[0].elevatorsList[3].position = -6;
            this.columnList[0].elevatorsList[3].nextDestination = 1;

            this.columnList[0].elevatorsList[4].direction = "down";
            this.columnList[0].elevatorsList[4].position = -1;
            this.columnList[0].elevatorsList[4].nextDestination = -6;

            RequestElevator(-3);
        }


    }


    class Column
    {
        public char id;
        public int amountElevatorPerColumn;
        public int startZone;
        public int endZone;
        public List<Elevator> elevatorsList = new List<Elevator>();

        public Column(char id, int amountFloors, int amountBasements, int amountElevatorPerColumn)
        {
            this.id = id;
            this.amountElevatorPerColumn = amountElevatorPerColumn;

            for (int i = 0; i < amountElevatorPerColumn; i++)
            {
                Elevator elevator = new Elevator(i + 1, 1, "Idle", 1);
                elevatorsList.Add(elevator);
            }

        }
        public void createElevatorsList()
        {
            for (int i = 1; i <= this.amountElevatorPerColumn; i++)
            {
                this.elevatorsList.Add(new Elevator(i, 1, "Idle", 1));
            }
        }


        public Elevator bestElevator(int FloorNumber)
        {
            List<Elevator> elevatorsInMvtList = new List<Elevator>();
            List<Elevator> elevatorIdleList = new List<Elevator>();
            List<Elevator> canBeList = new List<Elevator>();
            Elevator bestFit = null;
            foreach (Elevator elevator in this.elevatorsList)
            {
                if (FloorNumber > 1)
                {
                    if (elevator.direction == "Down" && FloorNumber < elevator.position)
                    {
                        elevatorsInMvtList.Add(elevator);
                    }
                    else if (elevator.direction == "Idle")
                    {
                        elevatorIdleList.Add(elevator);
                    }
                    else
                    {
                        canBeList.Add(elevator);
                    }
                }
                if (FloorNumber < 1)
                {
                    if (elevator.direction == "up" && FloorNumber > elevator.position)
                    {
                        elevatorsInMvtList.Add(elevator);
                    }
                    else if (elevator.direction == "Idle")
                    {
                        elevatorIdleList.Add(elevator);
                    }
                    else
                    {
                        canBeList.Add(elevator);
                    }
                }
                // Console.WriteLine("Elevator " + BestColumn.Id + E.Id + " Direction= " + E.Direction + "| CurrentFloor = " + E.ElevatorCurrentFloor + "| NextStop  = " + E.ElevatorNextStop);
            }
            // BestFit : soit le plus proche dans les GoodElevators  sinon dans les PossibleElevators
            if (elevatorsInMvtList.Count > 0)
            {

                int rating = 0;
                foreach (Elevator elevator in elevatorsInMvtList)
                {
                    rating = Math.Abs(elevator.nextDestination - elevator.position) + Math.Abs(elevator.nextDestination - FloorNumber);
                    elevator.rating = rating;
                }

                bestFit = elevatorsInMvtList.OrderBy(elevator => elevator.rating).First();




            }
            else if (elevatorIdleList.Count > 0)
            {



                int rating = 0;
                foreach (Elevator elevator in elevatorIdleList)
                {
                    rating = Math.Abs(elevator.nextDestination - elevator.position) + Math.Abs(elevator.nextDestination - FloorNumber);
                    elevator.rating = rating;
                }

                bestFit = elevatorIdleList.OrderBy(elevator => elevator.rating).First();




            }
            else
            {
                int rating = 0;
                foreach (Elevator elevator in canBeList)
                {
                    rating = Math.Abs(elevator.position - FloorNumber);
                    elevator.rating = rating;
                }

                bestFit = canBeList.OrderBy(elevator => elevator.rating).First();
            }


            return bestFit;

        }



    }
    class Elevator
    {
        public int id;
        public int position;
        public string direction = "Idle";
        public List<int> requestList = new List<int>();
        public string doors = "Closed";
        public int nextDestination;
        public int rating;


        public Elevator(int id, int position, string direction, int Destination)
        {
            this.id = id;
            this.position = position;
            this.direction = direction;
            this.nextDestination = Destination;


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
        public void moveElevator(int destination)
        {

            if (this.position < destination)
            {
                while (this.position < destination)
                {
                    this.position++;
                    Console.Write(" [ ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(this.position);
                    Console.ResetColor();
                    Console.WriteLine(" ] ");

                }
            }
            if (this.position > destination)
            {
                while (this.position > destination)
                {
                    this.position--;
                    Console.Write(" [ ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(this.position);
                    Console.ResetColor();
                    Console.WriteLine(" ] ");

                }
            }

        }



    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Battery mainBattery = new Battery(4, 66, 6, 5);

            Console.ResetColor();
            for (int i = 0; i < mainBattery.columnList.Count; i++)
            {
                Console.WriteLine(String.Format("{0,-10} | {1,-10} | {2,-10}", mainBattery.columnList[i].id, mainBattery.columnList[i].startZone, mainBattery.columnList[i].endZone));
            }
            mainBattery.scenario1();
            Console.WriteLine("---------------------[ Scenario 2 ! ]---------------------");
            mainBattery.scenario2();
            Console.WriteLine("---------------------[ Scenario 3 ! ]---------------------");
            mainBattery.scenario3();
            Console.WriteLine("---------------------[ Scenario 4 ! ]---------------------");
            mainBattery.scenario4();

            Console.Write("---------------------[ END OF TESTING ! ]---------------------");

        }

    }
}
