/**       _______________________________________
*        |Commercial_Controller :Modern Approach |
*        |______________________________________ |
*
*
*   Author: Saad Eddine
*   CodeBoxx 
*/



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
        public List<floorButton> floorButton = new List<floorButton>();
        public String status;


        /** constructor **/

        public Battery(int amountColumns, int amountFloors, int amountBasements, int amountElevatorPerColumn, String status)
        {
            this.amountColumns = amountColumns;
            this.amountFloors = amountFloors;
            this.amountBasements = amountBasements;
            this.amountElevatorPerColumn = amountElevatorPerColumn;
            this.createfloorButton(this.amountFloors);
            this.status = "OnLine";

            // Set List of columns
            this.setColumnsList();
            // Create list of floors for each column
            this.createfloorList(amountBasements, amountFloors, amountColumns);

            // Console.WriteLine("Battery is created: Ok !");
        }
        // calculate amount of floor per column and define limits of service for each column
        public void floorPerColumn(int amountBasements, int amountFloors, int amountColumns)
        {
            // how much floor per column?
            int remainder = 0; // the rest of division
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
            // startZone: first flor of interval of service
            // endZone: last flor of interval of service
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
                // IF we have zero ! basements
                if (this.amountBasements == 0)
                {
                    for (int i = 0; i < columnList.Count; i++)
                    {
                        columnList[i].startZone = initStart;
                        columnList[i].endZone = initStart + (this.floorPercolumn - 1);
                        initStart = columnList[i].endZone + 1;
                    }
                }
                // IF we have basements: will be affected to the first column
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
                columnList[columnList.Count - 1].endZone = this.floorPercolumn + remainder;
            }
        }
        public void createfloorList(int amountBasements, int amountFloors, int amountColumns)
        {
            this.floorPerColumn(amountBasements, amountFloors, amountColumns);
            foreach (Column column in this.columnList)
            {
                if (column.id == 'A')
                {
                    column.floorList.Add(1);
                    for (int i = -1; i >= -amountBasements; i--)
                    {
                        column.floorList.Add(i);
                    }
                }
                else
                {
                    column.floorList.Add(1);
                    for (int i = column.startZone; i <= column.endZone; i++)
                    {
                        column.floorList.Add(i);
                    }
                }
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
        // return the Column who contains requestedFloor
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

        // This method represents an elevator request on a floor or basement.
        public void RequestElevator(int FloorNumber)
        {
            Console.WriteLine(">> User at : " + FloorNumber + " wants to go to RC <<");

            // find the right column and get the best elevator
            Column column = getColumn(FloorNumber);
            Elevator elevator = column.bestElevator(FloorNumber);

            Console.WriteLine("- Elevator  " + column.id + " [" + elevator.id + "]" + " Position: " + elevator.position + " sent to: " + FloorNumber);

            elevator.doors.openDoor();
            // Safety Check
            if (elevator.safetyFirst(elevator.weightmax, elevator.sensor))
            {
                Console.WriteLine("Safety check: is ok !");
                elevator.doors.closeDoor();
            }
            // Move Elevator
            elevator.moveElevator(FloorNumber);

            elevator.doors.openDoor();
            if (elevator.safetyFirst(elevator.weightmax, elevator.sensor))
            {
                Console.WriteLine("Safety check: is ok !");
                elevator.doors.closeDoor();
            }

            // move the elevator to RC
            elevator.moveElevator(1);
            elevator.doors.openDoor();
        }
        // ***************************
        // AssignElevator (RequestedFloor) This method will be used for the requests made on 
        //the first floor.
        public void AssignElevator(int RequestedFloor)
        {
            Console.WriteLine(">>> Someone at RC wants to go to the : " + RequestedFloor + "<<<");

            // find the correespondante column
            Column column = getColumn(RequestedFloor);
            Elevator bestFit = findBestElevator(RequestedFloor, column);

            Console.WriteLine("- The elevetor: " + column.id + " [" + bestFit.id + "]" + " Position: " + bestFit.position + " to RC");
            bestFit.doors.openDoor();

            if (bestFit.safetyFirst(bestFit.weightmax, bestFit.sensor))
            {
                Console.WriteLine("Safety check: is ok !");
                bestFit.doors.closeDoor();
            }

            // To assign a elevator : STEP 1: we move elevator to Rc
            bestFit.moveElevator(1);

            Console.WriteLine("- stopover: " + column.id + " [" + bestFit.id + "]" + " Position : " + bestFit.position + " Is sent to : " + RequestedFloor);

            bestFit.doors.openDoor();

            if (bestFit.safetyFirst(bestFit.weightmax, bestFit.sensor))
            {
                Console.WriteLine("Safety check: is ok !");
                bestFit.doors.closeDoor();
            }
            // STEP 2: we move elevator to RequestedFloor
            bestFit.moveElevator(RequestedFloor);
            bestFit.doors.openDoor();
        }

        // return the best elevator To Assign
        public Elevator findBestElevator(int RequestedFloor, Column column)
        {
            // The priority is for the elevator in mvt !important
            List<Elevator> elevatorsInMvtList = new List<Elevator>();
            // Second priority for the Idle
            List<Elevator> elevatorIdleList = new List<Elevator>();
            // list of others if no much
            List<Elevator> canBeList = new List<Elevator>();
            Elevator bestFitElevator = null;

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
                bestFitElevator = elevatorIdleList.OrderBy(elevator => Math.Abs(elevator.position - 1)).First();
            }
            else
            {   /** Every elevator have a rating (Score): the rating is based in the distance between elevator and the next destination + distance RC **/

                int rating = 0;
                foreach (Elevator elevator in elevatorsInMvtList)
                {
                    rating = Math.Abs(elevator.position - elevator.nextDestination) + Math.Abs(elevator.nextDestination - 1);
                    elevator.rating = rating;
                }
                // we sort the list a take the first one
                bestFitElevator = elevatorsInMvtList.OrderBy(elevator => elevator.rating).First();
            }
            return bestFitElevator;
        }

        public void createfloorButton(int amountFloors)
        {
            for (int i = -this.amountBasements; i < 1; i++)
            {
                this.floorButton.Add(new floorButton(i, "notPressed"));
            }
            for (int i = amountFloors - this.amountBasements; i < 1; i++)
            {
                this.floorButton.Add(new floorButton(i, "notPressed"));
            }

        }

    }
    class Column
    {
        public char id;
        public int amountElevatorPerColumn;
        public int startZone; //Define the first floor of service
        public int endZone;     //Define the last floor of service
        public List<int> floorList = new List<int>();
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
        // create list of elevators
        public void createElevatorsList()
        {
            for (int i = 1; i <= this.amountElevatorPerColumn; i++)
            {
                this.elevatorsList.Add(new Elevator(i, 1, "Idle", 1));
            }
        }


        public Elevator bestElevator(int FloorNumber)
        {
            //  3 list for 3 category of elevators: on mvt , idle and others 
            List<Elevator> elevatorsInMvtList = new List<Elevator>();
            List<Elevator> elevatorIdleList = new List<Elevator>();
            List<Elevator> canBeList = new List<Elevator>();
            Elevator bestFit = null;
            foreach (Elevator elevator in this.elevatorsList)
            {
                // for the direction down 
                if (FloorNumber > 1)
                {
                    if (elevator.direction == "down" && FloorNumber < elevator.position)
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
                // for the direction UP
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
                // Calculate rating(score) and sort the list 
                // return the first one
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
                // if no best elevator in mvt we take the best Idle
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
                /************************************************
                *  If no 1. best elevator on mvt
                *       2. best nearest Idle
                *       ==> take the nearest elevator when he become availlable 
                **/

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
        /** to keep tracking elevator**/
        public int nextDestination;
        /** Rating to classify elevator by priority**/
        public int rating;
        public Doors doors = new Doors();
        public int weightmax = 800;
        public Boolean sensor = true;

        // constructor
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
        // methodes to move Elevator
        public void moveElevator(int destination)
        {

            if (this.position < destination)
            {
                while (this.position < destination)
                {
                    this.position++;
                    if (this.position != 0)
                    {
                        Console.WriteLine("Moving to: [ " + this.position + " ] ");

                    }
                }
            }
            if (this.position > destination)
            {
                while (this.position > destination)
                {
                    this.position--;
                    if (this.position != 0)
                    {
                        Console.WriteLine("Moving to: [ " + this.position + " ] ");
                    }
                }
            }

        }
        public Boolean safetyFirst(int weight, Boolean sensor)
        {
            return (this.weightmax == weight && sensor == true);
        }
    }
    public class Doors
    {
        public void openDoor()
        {
            Console.WriteLine("Doors ]<>[");
        }
        public void closeDoor()
        {
            Console.WriteLine("Doors >[]<");
        }
    }
    public class floorButton
    {
        int numFloor;
        string status;
        public floorButton(int numFloor, string status)
        {
            this.numFloor = numFloor;
            this.status = status;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Battery mainBattery = new Battery(4, 66, 6, 5, "OnLine");


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("---------------------[     Battery is Online         ]---------------------");


            Console.WriteLine("---------------------[ Generic repartition of Columns]---------------------");

            foreach (Column column in mainBattery.columnList)
            {
                Console.WriteLine("Column  " + column.id + " -- Floors in service =  " + String.Join(" | ", column.floorList));
            }

            // ------------------------------ Testing Section -----------------------------------
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------[ Scenario 1 ! ]---------------------");
            Console.ResetColor();

            mainBattery.columnList[1].elevatorsList[0].direction = "down";
            mainBattery.columnList[1].elevatorsList[0].position = 20;
            mainBattery.columnList[1].elevatorsList[0].nextDestination = 5;

            mainBattery.columnList[1].elevatorsList[1].direction = "up";
            mainBattery.columnList[1].elevatorsList[1].position = 3;
            mainBattery.columnList[1].elevatorsList[1].nextDestination = 15;

            mainBattery.columnList[1].elevatorsList[2].direction = "down";
            mainBattery.columnList[1].elevatorsList[2].position = 13;
            mainBattery.columnList[1].elevatorsList[2].nextDestination = 1;

            mainBattery.columnList[1].elevatorsList[3].direction = "down";
            mainBattery.columnList[1].elevatorsList[3].position = 15;
            mainBattery.columnList[1].elevatorsList[3].nextDestination = 2;

            mainBattery.columnList[1].elevatorsList[4].direction = "down";
            mainBattery.columnList[1].elevatorsList[4].position = 6;
            mainBattery.columnList[1].elevatorsList[4].nextDestination = 1;

            mainBattery.AssignElevator(20);

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------[ Scenario 2 ! ]---------------------");
            Console.ResetColor();
            mainBattery.columnList[2].elevatorsList[0].direction = "up";
            mainBattery.columnList[2].elevatorsList[0].position = 1;
            mainBattery.columnList[2].elevatorsList[0].nextDestination = 21;

            mainBattery.columnList[2].elevatorsList[1].direction = "up";
            mainBattery.columnList[2].elevatorsList[1].position = 23;
            mainBattery.columnList[2].elevatorsList[1].nextDestination = 28;

            mainBattery.columnList[2].elevatorsList[2].direction = "down";
            mainBattery.columnList[2].elevatorsList[2].position = 33;
            mainBattery.columnList[2].elevatorsList[2].nextDestination = 1;

            mainBattery.columnList[2].elevatorsList[3].direction = "down";
            mainBattery.columnList[2].elevatorsList[3].position = 40;
            mainBattery.columnList[3].elevatorsList[3].nextDestination = 24;

            mainBattery.columnList[2].elevatorsList[4].direction = "down";
            mainBattery.columnList[2].elevatorsList[4].position = 39;
            mainBattery.columnList[2].elevatorsList[4].nextDestination = 1;

            mainBattery.AssignElevator(36);


            // -------------------SCENARIO 3-------------------------mainBattery
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------[ Scenario 3 ! ]---------------------");
            Console.ResetColor();
            mainBattery.columnList[3].elevatorsList[0].direction = "down";
            mainBattery.columnList[3].elevatorsList[0].position = 58;
            mainBattery.columnList[3].elevatorsList[0].nextDestination = 1;

            mainBattery.columnList[3].elevatorsList[1].direction = "up";
            mainBattery.columnList[3].elevatorsList[1].position = 50;
            mainBattery.columnList[3].elevatorsList[1].nextDestination = 60;

            mainBattery.columnList[3].elevatorsList[2].direction = "up";
            mainBattery.columnList[3].elevatorsList[2].position = 46;
            mainBattery.columnList[3].elevatorsList[2].nextDestination = 58;

            mainBattery.columnList[3].elevatorsList[3].direction = "up";
            mainBattery.columnList[3].elevatorsList[3].position = 1;
            mainBattery.columnList[3].elevatorsList[3].nextDestination = 54;

            mainBattery.columnList[3].elevatorsList[4].direction = "down";
            mainBattery.columnList[3].elevatorsList[4].position = 60;
            mainBattery.columnList[3].elevatorsList[4].nextDestination = 1;

            mainBattery.RequestElevator(54);


            // -------------------SCENARIO 4-------------------------
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("---------------------[ Scenario 4 ! ]---------------------");
            Console.ResetColor();

            mainBattery.columnList[0].elevatorsList[0].direction = "Idle";
            mainBattery.columnList[0].elevatorsList[0].position = -4;
            mainBattery.columnList[0].elevatorsList[0].nextDestination = 0;

            mainBattery.columnList[0].elevatorsList[1].direction = "Idle";
            mainBattery.columnList[0].elevatorsList[1].position = 1;
            mainBattery.columnList[0].elevatorsList[1].nextDestination = 0;

            mainBattery.columnList[0].elevatorsList[2].direction = "down";
            mainBattery.columnList[0].elevatorsList[2].position = -3;
            mainBattery.columnList[0].elevatorsList[2].nextDestination = -5;

            mainBattery.columnList[0].elevatorsList[3].direction = "up";
            mainBattery.columnList[0].elevatorsList[3].position = -6;
            mainBattery.columnList[0].elevatorsList[3].nextDestination = 1;

            mainBattery.columnList[0].elevatorsList[4].direction = "down";
            mainBattery.columnList[0].elevatorsList[4].position = -1;
            mainBattery.columnList[0].elevatorsList[4].nextDestination = -6;

            mainBattery.RequestElevator(-3);

            Console.Write("-----------------------[ END OF TESTING ! ]---------------------");

        }

    }
}
