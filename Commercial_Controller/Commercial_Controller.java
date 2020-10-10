
/**       _______________________________________
*        |Commercial_Controller :Modern Approach |
*        |______________________________________ |
*
*
*   Author: Saad Eddine
*   SUMMARY:
*   Classes:	- Battery
*				- Column
*				- Elevator
*				- Doors
*               - floorButton
*	Main function: 	
*               - AssignElevator: used for the requests made on the first     floor.
*						|-> getColumn return the correspandant column for destination
*						|-> moveElevator is a function to move elevator to destination
*						|-> findBestElevator return the best fit elevator for assignElevator

*				- RequestElevator: Used to respond to arequest on a floor or basement.
*						|-> getColumn return the correspandant column for destination
*						|-> moveElevator is a function to move elevator to destination
*						|-> bestElevator return the best fit elevator for RequestElevator
*				- RatingComparator: Comparator Compare to object elevator based on rating
*				- openDoors
* Priority is managed by 3 lists and Rating system based on 3 elements : floor number (user)
* elevator current position and RC as origin
 */

import java.util.*;
import java.util.ArrayList;
import java.util.Collections;

class Battery {

    public int amountColumns;
    public int amountFloors;
    public int amountBasements;
    public int floorPercolumn;
    public int amountElevatorPerColumn;
    public ArrayList<Column> columnList = new ArrayList<Column>();
    public ArrayList<floorButton> floorButton = new ArrayList<floorButton>();
    public String status;

    /** constructor **/

    public Battery(int amountColumns, int amountFloors, int amountBasements, int amountElevatorPerColumn,
            String status) {
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

    // calculate amount of floor per column and define limits of service for each
    // column
    public void floorPerColumn(int amountBasements, int amountFloors, int amountColumns) {
        // how much floor per column?
        int remainder = 0; // the rest of division
        if (amountBasements == 0) {
            this.floorPercolumn = amountFloors / amountColumns;
        } else if (amountColumns == 1) {
            this.floorPercolumn = amountFloors;
        } else {
            this.floorPercolumn = (amountFloors - amountBasements) / (amountColumns - 1);
            remainder = (amountFloors - amountBasements) % (amountColumns - 1);

        }

        // Lets define columns zones
        // startZone: first flor of interval of service
        // endZone: last flor of interval of service
        // first case: one column
        if (amountColumns == 1 && this.amountBasements == 0) {
            // List with one column
            columnList.get(0).startZone = 1;
            columnList.get(0).endZone = amountFloors;
        } else // second case: many columns
        {
            int initStart = 1;
            // IF we have zero ! basements
            if (this.amountBasements == 0) {
                for (int i = 0; i < columnList.size(); i++) {
                    columnList.get(i).startZone = initStart;
                    columnList.get(i).endZone = initStart + (this.floorPercolumn - 1);
                    initStart = columnList.get(i).endZone + 1;
                }
            }
            // IF we have basements: will be affected to the first column
            else {
                columnList.get(0).startZone = -amountBasements;
                columnList.get(0).endZone = -1;
                // overhead floor columns starts from 2 !!
                initStart = 2;
                for (int i = 1; i < this.amountColumns; i++) {
                    if (i == 1) {
                        columnList.get(i).startZone = 2;
                        columnList.get(i).endZone = this.floorPercolumn;
                    } else {
                        initStart = columnList.get(i - 1).endZone;
                        columnList.get(i).startZone = initStart + 1;
                        columnList.get(i).endZone = initStart + (this.floorPercolumn);

                    }
                }
            }
        }

        // if we have a extra number of floors we added it to the last column !!
        // we push the end zone of the last column to limit !!!
        if (remainder > 0) {
            columnList.get(columnList.size() - 1).endZone = this.floorPercolumn + remainder;
        }
    }

    public void createfloorList(int amountBasements, int amountFloors, int amountColumns) {
        this.floorPerColumn(amountBasements, amountFloors, amountColumns);
        for (Column column : this.columnList) {
            if (column.id == 'A') {
                column.floorList.add(1);
                for (int i = -1; i >= -amountBasements; i--) {
                    column.floorList.add(i);
                }
            } else {
                column.floorList.add(1);
                for (int i = column.startZone; i <= column.endZone; i++) {
                    column.floorList.add(i);
                }
            }
        }
    }

    public void setColumnsList() {
        char c1 = 'A';
        for (int i = 1; i <= this.amountColumns; i++) {

            this.columnList.add(new Column(c1, this.amountFloors, this.amountBasements, this.amountElevatorPerColumn));
            c1++;
        }
    }

    // return the Column who contains requestedFloor
    public Column getColumn(int requestedFloor) {
        Column candidate = null;
        for (Column column : this.columnList) {
            if (requestedFloor >= column.startZone && requestedFloor <= column.endZone) {
                candidate = column;
            }
        }
        return candidate;
    }

    // This method represents an elevator request on a floor or basement.
    public void RequestElevator(int FloorNumber) {
        System.out.println(">> User at : " + FloorNumber + " wants to go to RC <<");
        System.out.println("----------------------------------------------------------");
        // find the right column and get the best elevator
        Column column = getColumn(FloorNumber);
        Elevator elevator = column.bestElevator(FloorNumber);

        System.out.println("- Select elevator  " + column.id + " [" + elevator.id + "]" + " Position: "
                + elevator.position + " sent to: " + FloorNumber);

        elevator.doors.openDoor();
        // Safety Check
        if (elevator.safetyFirst(elevator.weightmax, elevator.sensor)) {
            System.out.println("Safety check: is ok !");
            elevator.doors.closeDoor();
        }
        // Move Elevator
        elevator.moveElevator(FloorNumber);

        elevator.doors.openDoor();
        if (elevator.safetyFirst(elevator.weightmax, elevator.sensor)) {
            System.out.println("Safety check: is ok !");
            elevator.doors.closeDoor();
        }

        // move the elevator to RC
        elevator.moveElevator(1);
        elevator.doors.openDoor();
    }

    // ***************************
    // AssignElevator (RequestedFloor) This method will be used for the requests
    // made on
    // the first floor.
    public void AssignElevator(int RequestedFloor) {
        System.out.println(">>> Someone at RC wants to go to the : " + RequestedFloor + "<<<");
        System.out.println("----------------------------------------------------------------");
        // find the correespondante column
        Column column = getColumn(RequestedFloor);
        Elevator bestFit = findBestElevator(RequestedFloor, column);

        System.out.println("- Select elevetor: " + column.id + " [" + bestFit.id + "]" + " Position: "
                + bestFit.position + " to RC");
        bestFit.doors.openDoor();

        if (bestFit.safetyFirst(bestFit.weightmax, bestFit.sensor)) {
            System.out.println("Safety check: is ok !");
            bestFit.doors.closeDoor();
        }

        // To assign a elevator : STEP 1: we move elevator to Rc
        bestFit.moveElevator(1);

        System.out.println("- stopover: " + column.id + " [" + bestFit.id + "]" + " Position : " + bestFit.position
                + " Is sent to : " + RequestedFloor);

        bestFit.doors.openDoor();

        if (bestFit.safetyFirst(bestFit.weightmax, bestFit.sensor)) {
            System.out.println("Safety check: is ok !");
            bestFit.doors.closeDoor();
        }
        // STEP 2: we move elevator to RequestedFloor
        bestFit.moveElevator(RequestedFloor);
        bestFit.doors.openDoor();
    }

    // return the best elevator To Assign
    public Elevator findBestElevator(int RequestedFloor, Column column) {
        // The priority is for the elevator in mvt !important
        ArrayList<Elevator> elevatorsInMvtList = new ArrayList<Elevator>();
        // Second priority for the Idle
        ArrayList<Elevator> elevatorIdleList = new ArrayList<Elevator>();
        // list of others if no much
        // ArrayList<Elevator> canBeList = new ArrayList<Elevator>();
        Elevator bestFitElevator = null;

        for (Elevator elevator : column.elevatorsList) {
            if (elevator.direction.equals("Idle") || elevator.position == 1) {
                elevatorIdleList.add(elevator);
            } else {
                elevatorsInMvtList.add(elevator);
            }
        }

        if (!elevatorIdleList.isEmpty()) {
            // sort List

            for (Elevator elevator : elevatorsInMvtList) {
                elevator.rating = Math.abs(elevator.position - 1);
            }

            Collections.sort(elevatorIdleList, new RatingComparator());
            bestFitElevator = elevatorIdleList.get(0);

        } else { // * Every elevator have a rating (Score): the rating is based in the distance
                 // between elevator and the next destination + distance RC *

            for (Elevator elevator : elevatorsInMvtList) {
                elevator.rating = Math.abs(elevator.position - elevator.nextDestination)
                        + Math.abs(elevator.nextDestination - 1);
            }
            // we sort the list a take the first one
            Collections.sort(elevatorsInMvtList, new RatingComparator());
            bestFitElevator = elevatorsInMvtList.get(0);
        }
        return bestFitElevator;
    }

    // create a list of buttons
    public void createfloorButton(int amountFloors) {
        for (int i = -this.amountBasements; i < 1; i++) {
            this.floorButton.add(new floorButton(i, "notPressed"));
        }
        for (int i = amountFloors - this.amountBasements; i < 1; i++) {
            this.floorButton.add(new floorButton(i, "notPressed"));
        }

    }

}

// RatingComparator: Compare to object elevator based on rating
class RatingComparator implements Comparator<Elevator> {
    @Override
    public int compare(Elevator e1, Elevator e2) {
        return Integer.compare(e1.getRating(), e2.getRating());
    }
}

class Column {
    public char id;
    public int amountElevatorPerColumn;
    public int startZone; // Define the first floor of service
    public int endZone; // Define the last floor of service
    public ArrayList<Integer> floorList = new ArrayList<Integer>();
    public ArrayList<Elevator> elevatorsList = new ArrayList<Elevator>();

    public Column(char id, int amountFloors, int amountBasements, int amountElevatorPerColumn) {
        this.id = id;
        this.amountElevatorPerColumn = amountElevatorPerColumn;

        for (int i = 0; i < amountElevatorPerColumn; i++) {
            Elevator elevator = new Elevator(i + 1, 1, "Idle", 1);
            elevatorsList.add(elevator);
        }
    }

    // create list of elevators
    public void createElevatorsList() {
        for (int i = 1; i <= this.amountElevatorPerColumn; i++) {
            this.elevatorsList.add(new Elevator(i, 1, "Idle", 1));
        }
    }

    public Elevator bestElevator(int FloorNumber) {
        // 3 list for 3 category of elevators: on mvt , idle and others
        ArrayList<Elevator> elevatorsInMvtList = new ArrayList<Elevator>();
        ArrayList<Elevator> elevatorIdleList = new ArrayList<Elevator>();
        ArrayList<Elevator> canBeList = new ArrayList<Elevator>();
        Elevator bestFit = null;
        for (Elevator elevator : this.elevatorsList) {
            // for the direction down
            if (FloorNumber > 1) {
                if (elevator.direction.equals("down") && FloorNumber < elevator.position) {
                    elevatorsInMvtList.add(elevator);
                } else if (elevator.direction.equals("Idle")) {
                    elevatorIdleList.add(elevator);
                } else {
                    canBeList.add(elevator);
                }
            }
            // for the direction UP
            if (FloorNumber < 1) {
                if (elevator.direction.equals("up") && FloorNumber > elevator.position) {
                    elevatorsInMvtList.add(elevator);
                } else if (elevator.direction.equals("Idle")) {
                    elevatorIdleList.add(elevator);
                } else {
                    canBeList.add(elevator);
                }
            }

        }

        if (!elevatorsInMvtList.isEmpty()) {
            // Calculate rating(score) and sort the list
            // return the first one
            int rating = 0;
            for (Elevator elevator : elevatorsInMvtList) {
                rating = Math.abs(elevator.nextDestination - elevator.position)
                        + Math.abs(elevator.nextDestination - FloorNumber);
                elevator.rating = rating;
            }
            // we sort the list a take the first one
            Collections.sort(elevatorsInMvtList, new RatingComparator());
            bestFit = elevatorsInMvtList.get(0);
        } else if (!elevatorIdleList.isEmpty()) {
            // if no best elevator in mvt we take the best Idle

            for (Elevator elevator : elevatorIdleList) {
                elevator.rating = Math.abs(elevator.nextDestination - elevator.position)
                        + Math.abs(elevator.nextDestination - FloorNumber);

            }
            Collections.sort(elevatorIdleList, new RatingComparator());
            bestFit = elevatorIdleList.get(0);
        } else {

            /************************************************
             * If no 1. best elevator on mvt 2. best nearest Idle ==> take the nearest
             * elevator when he become availlable
             **/

            int rating = 0;
            for (Elevator elevator : canBeList) {
                rating = Math.abs(elevator.position - FloorNumber);
                elevator.rating = rating;
            }
            Collections.sort(canBeList, new RatingComparator());
            bestFit = canBeList.get(0);
        }
        return bestFit;
    }
}

class Elevator {
    public int id;
    public int position;
    public String direction = "Idle";
    public ArrayList<Integer> requestList = new ArrayList<Integer>();
    /** to keep tracking elevator **/
    public int nextDestination;
    /** Rating to classify elevator by priority **/
    public int rating;
    public Doors doors = new Doors();
    public int weightmax = 800;
    public boolean sensor = true;

    // constructor
    public Elevator(int id, int position, String direction, int Destination) {
        this.id = id;
        this.position = position;
        this.direction = direction;
        this.nextDestination = Destination;
    }

    public void manageRequestList(Column column, int requestedFloor) {
        if (column.startZone > 0) {
            if (this.direction.equals("up")) {
                Collections.sort(this.requestList);
            } else if (this.direction.equals("down")) {
                Collections.sort(this.requestList);
                Collections.reverse(this.requestList);
            }
        } else if (column.startZone < 0) {
            if (this.direction.equals("down")) {
                Collections.sort(this.requestList);
            } else if (this.direction.equals("up")) {
                Collections.sort(this.requestList);
                Collections.reverse(this.requestList);
            }
        }
    }

    public int getRating() {
        return this.rating;
    }

    // methodes to move Elevator
    public void moveElevator(int destination) {
        // Move up
        if (this.position < destination) {
            while (this.position < destination) {
                this.position++;
                if (this.position != 0) {
                    System.out.println("Moving to: [ " + this.position + " ] ");

                }
            }
        }
        // Move down
        if (this.position > destination) {
            while (this.position > destination) {
                this.position--;
                if (this.position != 0) {
                    System.out.println("Moving to: [ " + this.position + " ] ");
                }
            }
        }

    }

    public boolean safetyFirst(int weight, boolean sensor) {
        return (this.weightmax == weight && sensor == true);
    }
}

class floorButton {
    public int numFloor;
    public String status;

    public floorButton(int numFloor, String status) {
        this.numFloor = numFloor;
        this.status = status;
    }

    public void setStatus(String s) {
        this.status = s;
    }

    public void setStatus(int n) {
        this.numFloor = n;
    }
}

class Doors {
    public void openDoor() {
        System.out.println("Doors ]<>[");
    }

    public void closeDoor() {
        System.out.println("Doors >[]<");
    }
}

public class Commercial_Controller {
    public static void main(String[] args) {

        Battery mainBattery = new Battery(4, 66, 6, 5, "OnLine");

        System.out.println("---------------------[     Battery is Online         ]---------------------");

        System.out.println("---------------------[ Generic repartition of Columns]---------------------\n");
        for (int i = 0; i < mainBattery.columnList.size(); i++) {
            System.out.print("Column  " + mainBattery.columnList.get(i).id + " -- Floors in service =  ");
            for (int j = 0; j < mainBattery.columnList.get(i).floorList.size(); j++)
                System.out.print("| " + mainBattery.columnList.get(i).floorList.get(j));
            {

            }
            System.out.println("");
        }

        // ------------------ Testing Section ----------------------------------
        System.out.println("\n");
        System.out.println("---------------------[ Scenario 1 ! ]---------------------------");

        mainBattery.columnList.get(1).elevatorsList.get(0).direction = "down";
        mainBattery.columnList.get(1).elevatorsList.get(0).position = 20;
        mainBattery.columnList.get(1).elevatorsList.get(0).nextDestination = 5;

        mainBattery.columnList.get(1).elevatorsList.get(1).direction = "up";
        mainBattery.columnList.get(1).elevatorsList.get(1).position = 3;
        mainBattery.columnList.get(1).elevatorsList.get(1).nextDestination = 15;

        mainBattery.columnList.get(1).elevatorsList.get(2).direction = "down";
        mainBattery.columnList.get(1).elevatorsList.get(2).position = 13;
        mainBattery.columnList.get(1).elevatorsList.get(2).nextDestination = 1;

        mainBattery.columnList.get(1).elevatorsList.get(3).direction = "down";
        mainBattery.columnList.get(1).elevatorsList.get(3).position = 15;
        mainBattery.columnList.get(1).elevatorsList.get(3).nextDestination = 2;

        mainBattery.columnList.get(1).elevatorsList.get(4).direction = "down";
        mainBattery.columnList.get(1).elevatorsList.get(4).position = 6;
        mainBattery.columnList.get(1).elevatorsList.get(4).nextDestination = 1;

        mainBattery.AssignElevator(20);

        System.out.println("---------------------[ Scenario 2 ! ]---------------------------");

        mainBattery.columnList.get(2).elevatorsList.get(0).direction = "up";
        mainBattery.columnList.get(2).elevatorsList.get(0).position = 1;
        mainBattery.columnList.get(2).elevatorsList.get(0).nextDestination = 21;

        mainBattery.columnList.get(2).elevatorsList.get(1).direction = "up";
        mainBattery.columnList.get(2).elevatorsList.get(1).position = 23;
        mainBattery.columnList.get(2).elevatorsList.get(1).nextDestination = 28;

        mainBattery.columnList.get(2).elevatorsList.get(2).direction = "down";
        mainBattery.columnList.get(2).elevatorsList.get(2).position = 33;
        mainBattery.columnList.get(2).elevatorsList.get(2).nextDestination = 1;

        mainBattery.columnList.get(2).elevatorsList.get(3).direction = "down";
        mainBattery.columnList.get(2).elevatorsList.get(3).position = 40;
        mainBattery.columnList.get(3).elevatorsList.get(3).nextDestination = 24;

        mainBattery.columnList.get(2).elevatorsList.get(4).direction = "down";
        mainBattery.columnList.get(2).elevatorsList.get(4).position = 39;
        mainBattery.columnList.get(2).elevatorsList.get(4).nextDestination = 1;

        mainBattery.AssignElevator(36);

        // -------------------SCENARIO 3-------------------------mainBattery

        System.out.println("---------------------[ Scenario 3 ! ]---------------------");

        mainBattery.columnList.get(3).elevatorsList.get(0).direction = "down";
        mainBattery.columnList.get(3).elevatorsList.get(0).position = 58;
        mainBattery.columnList.get(3).elevatorsList.get(0).nextDestination = 1;

        mainBattery.columnList.get(3).elevatorsList.get(1).direction = "up";
        mainBattery.columnList.get(3).elevatorsList.get(1).position = 50;
        mainBattery.columnList.get(3).elevatorsList.get(1).nextDestination = 60;

        mainBattery.columnList.get(3).elevatorsList.get(2).direction = "up";
        mainBattery.columnList.get(3).elevatorsList.get(2).position = 46;
        mainBattery.columnList.get(3).elevatorsList.get(2).nextDestination = 58;

        mainBattery.columnList.get(3).elevatorsList.get(3).direction = "up";
        mainBattery.columnList.get(3).elevatorsList.get(3).position = 1;
        mainBattery.columnList.get(3).elevatorsList.get(3).nextDestination = 54;

        mainBattery.columnList.get(3).elevatorsList.get(4).direction = "down";
        mainBattery.columnList.get(3).elevatorsList.get(4).position = 60;
        mainBattery.columnList.get(3).elevatorsList.get(4).nextDestination = 1;

        mainBattery.RequestElevator(54);

        // -------------------SCENARIO 4-------------------------

        System.out.println("---------------------[ Scenario 4 ! ]---------------------");

        mainBattery.columnList.get(0).elevatorsList.get(0).direction = "Idle";
        mainBattery.columnList.get(0).elevatorsList.get(0).position = -4;
        mainBattery.columnList.get(0).elevatorsList.get(0).nextDestination = 0;

        mainBattery.columnList.get(0).elevatorsList.get(1).direction = "Idle";
        mainBattery.columnList.get(0).elevatorsList.get(1).position = 1;
        mainBattery.columnList.get(0).elevatorsList.get(1).nextDestination = 0;

        mainBattery.columnList.get(0).elevatorsList.get(2).direction = "down";
        mainBattery.columnList.get(0).elevatorsList.get(2).position = -3;
        mainBattery.columnList.get(0).elevatorsList.get(2).nextDestination = -5;

        mainBattery.columnList.get(0).elevatorsList.get(3).direction = "up";
        mainBattery.columnList.get(0).elevatorsList.get(3).position = -6;
        mainBattery.columnList.get(0).elevatorsList.get(3).nextDestination = 1;

        mainBattery.columnList.get(0).elevatorsList.get(4).direction = "down";
        mainBattery.columnList.get(0).elevatorsList.get(4).position = -1;
        mainBattery.columnList.get(0).elevatorsList.get(4).nextDestination = -6;

        mainBattery.RequestElevator(-3);

        System.out.print("-----------------------[ END OF TESTING ! ]---------------------");

    }

}
