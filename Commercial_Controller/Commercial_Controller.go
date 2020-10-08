/**       _______________________________________
*        |Commercial_Controller :Modern Approach |
*        |______________________________________ |
*
*
*   Author: Saad Eddine
*   SUMMARY:
*   STRUCTS:	- Battery
*				- Column
*				- Elevator
*				- Doors
*	Function: 	- makeBattery : Create a Battery and fill the lists of columns, floors and elevators
*				- toChar convert int (Code ASCII) to char
*				- conatains :function return True if int exist in list of int
*				- AssignElevator: This method will be used for the requests made on the first floor.
*						|-> getColumn return the correspandant column for destination
*						|-> moveElevator is a function to move elevator to destination
*						|-> findBestElevator return the best fit elevator for assignElevator

*				- RequestElevator: This method represents an elevator request on a floor or basement.
*						|-> getColumn return the correspandant column for destination
*						|-> moveElevator is a function to move elevator to destination
*						|-> getRequestElevator return the best fit elevator for RequestElevator
*				- Abs: return absolute value
*				- closeDoors
*				- openDoors
*
 */

package main

import (
	"fmt"
	"sort"
)

// Battery structure
type Battery struct {
	amountColumns, amountFloors, amountBasements, floorPercolumn, amountElevator int
	status                                                                       string
	columnsList                                                                  []Column
}

// Column structure
type Column struct {
	elevatorList                                  []Elevator
	floorList                                     []int
	amountFloors, amountBasements, amountElevator int
	id                                            string
}

// Elevator structure
type Elevator struct {
	id, position, nextDestination, rating int
	direction                             string
	doors                                 Doors
}

// makeBattery is a kind of constructor
func makeBattery(amountColumns, amountFloors, amountBasements, amountElevator int, status string) *Battery {

	battery := Battery{
		amountColumns:   amountColumns,
		amountFloors:    amountFloors,
		amountBasements: amountBasements,
		amountElevator:  amountElevator,
		status:          status,
	}

	// Create ListColumns
	alpha := "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
	for i := 1; i <= battery.amountColumns; i++ {
		column := Column{id: string(alpha[i-1]), amountFloors: amountFloors, amountBasements: amountBasements, amountElevator: amountElevator}
		battery.columnsList = append(battery.columnsList, column)

	}

	//Create ListFloors and ListElevators for each column

	numberFloors := int((battery.amountFloors - battery.amountBasements) / (battery.amountColumns - 1))
	cpt := 0
	start := 2
	end := numberFloors
	for i, column := range battery.columnsList {

		if column.id == string("A") {
			battery.columnsList[i].floorList = append(battery.columnsList[i].floorList, 1)
			for k := -1; k >= -column.amountBasements; k-- {
				battery.columnsList[i].floorList = append(battery.columnsList[i].floorList, k)
			}
		}
		if column.id != string("A") {
			cpt++
			battery.columnsList[i].floorList = append(battery.columnsList[i].floorList, 1)
			for k := start; k <= end; k++ {
				battery.columnsList[i].floorList = append(battery.columnsList[i].floorList, k)
			}
			start = end + 1
			end = end + numberFloors
		}
		// create listElevator for each colum
		for iterator := 1; iterator <= battery.amountElevator; iterator++ {
			elevator := Elevator{id: iterator}
			battery.columnsList[i].elevatorList = append(battery.columnsList[i].elevatorList, elevator)
		}
		//fmt.Println("Column  ", column.id, " -- ListFloors =  ", battery.columnsList[i].floorList)
	}
	return &battery
}

// toChar convert int to char
func toChar(i int) rune {
	return rune('A' - 1 + i)
}

// https://stackoverflow.com/questions/13520111/check-whether-a-string-slice-contains-a-certain-value-in-go

//  function return True if int exist in list of int
func contains(s []int, e int) bool {
	for _, a := range s {
		if a == e {
			return true
		}
	}
	return false
}

// AssignElevator function return the best elevator for the passenger at the RC
func (battery *Battery) AssignElevator(RequestedFloor int) {
	fmt.Println(">>> Someone at RC wants to go to the : ", RequestedFloor, "<<<")
	var column = battery.getColumn(RequestedFloor)
	var bestFit = battery.findBestElevator(*column, RequestedFloor)
	fmt.Println("- The elevetor: ", column.id, "[", bestFit.id, "]  position: [", bestFit.position, "] --- To RC ---")

	// send elevator
	if bestFit.position != 1 {
		moveElevator(bestFit, 1)
		bestFit.doors.openDoors()
		bestFit.doors.closeDoors()
		fmt.Println(" --- StopOver ---: Take passenger and sent to", RequestedFloor)
	}
	moveElevator(bestFit, RequestedFloor)
	bestFit.doors.openDoors()

}

//moveElevator is a function to move elevator to destination
func moveElevator(elevator *Elevator, destination int) {
	if elevator.position < destination {
		for elevator.position < destination {
			elevator.position++
			if elevator.position != 0 {
				println("Moving to: [", elevator.position, "] ")
			}
		}
	}
	if elevator.position > destination {
		for elevator.position > destination {
			elevator.position--
			if elevator.position != 0 {
				println("Moving to: [", elevator.position, "] ")
			}
		}
	}
}

// getColumn return the correspandant column for destination
func (battery *Battery) getColumn(destination int) *Column {
	var BestColumn Column
	for i, column := range battery.columnsList {
		if contains(column.floorList, destination) {
			BestColumn = battery.columnsList[i]
		}
	}
	return &BestColumn
}

// findBestElevator return the best fit elevator for assignElevator
func (battery *Battery) findBestElevator(column Column, RequestedFloor int) *Elevator {
	var elevatorsInMvtList, elevatorIdleList []Elevator

	for _, elevator := range column.elevatorList {
		if elevator.direction == "Idle" || elevator.position == 1 {
			elevatorIdleList = append(elevatorIdleList, elevator)
			elevator.rating = Abs(elevator.position - 1)
		} else {
			elevatorsInMvtList = append(elevatorsInMvtList, elevator)
			elevator.rating = Abs(elevator.position-elevator.nextDestination) + Abs(elevator.nextDestination-1)
		}

	}
	var bestFit Elevator
	if len(elevatorIdleList) > 0 {

		sort.Slice(elevatorIdleList, func(i, j int) bool {
			return elevatorIdleList[i].rating < elevatorIdleList[j].rating
		})
		bestFit = elevatorIdleList[len(elevatorIdleList)-1]
		_ = bestFit
	} else {
		sort.Slice(elevatorsInMvtList, func(i, j int) bool {
			return elevatorsInMvtList[i].rating < elevatorsInMvtList[j].rating

		})

		bestFit = elevatorsInMvtList[len(elevatorsInMvtList)-1]
		_ = bestFit
	}
	return &bestFit
}

// RequestElevator function
func (battery *Battery) RequestElevator(FloorNumber int) {

	fmt.Println(">> User at : ", FloorNumber, " wants to go to RC <<")

	// find the right column and get the best elevator
	var column = battery.getColumn(FloorNumber)
	var elevator = battery.getRequestElevator(*column, FloorNumber)

	println("- Elevator  ", column.id+" [", elevator.id, "]", " Position: ", elevator.position, " sent to: ", FloorNumber)
	elevator.doors.closeDoors()
	moveElevator(elevator, FloorNumber)
	elevator.doors.openDoors()
	println("- Elevator  ", column.id+" [", elevator.id, "]", " Position: ", elevator.position, " sent to RC ")
	elevator.doors.closeDoors()

	// move the elevator to RC
	moveElevator(elevator, 1)
	elevator.doors.openDoors()

}

func (battery *Battery) getRequestElevator(column Column, FloorNumber int) *Elevator {
	var elevatorsInMvtList, elevatorIdleList, canBeList []Elevator
	for _, elevator := range column.elevatorList {
		// for the direction down
		if FloorNumber > 1 {
			if elevator.direction == "down" && FloorNumber < elevator.position {
				elevatorsInMvtList = append(elevatorsInMvtList, elevator)
				elevator.rating = Abs(elevator.nextDestination-elevator.position) + Abs(elevator.nextDestination-FloorNumber)
			} else if elevator.direction == "Idle" {
				elevatorIdleList = append(elevatorIdleList, elevator)
				elevator.rating = Abs(elevator.nextDestination-elevator.position) + Abs(elevator.nextDestination-FloorNumber)
			} else {
				canBeList = append(canBeList, elevator)
				elevator.rating = Abs(elevator.position - FloorNumber)
			}
		}
		// for the direction UP
		if FloorNumber < 1 {
			if elevator.direction == "up" && FloorNumber > elevator.position {
				elevatorsInMvtList = append(elevatorsInMvtList, elevator)
				elevator.rating = Abs(elevator.nextDestination-elevator.position) + Abs(elevator.nextDestination-FloorNumber)
			} else if elevator.direction == "Idle" {
				elevatorIdleList = append(elevatorIdleList, elevator)
				elevator.rating = Abs(elevator.nextDestination-elevator.position) + Abs(elevator.nextDestination-FloorNumber)
			}
		}
	}
	// Calculate rating(score) and sort the list
	// return the first one
	var bestFit Elevator
	if len(elevatorsInMvtList) > 0 {
		sort.Slice(elevatorsInMvtList, func(i, j int) bool {
			return elevatorsInMvtList[i].rating < elevatorsInMvtList[j].rating
		})
		bestFit = elevatorsInMvtList[0]
		_ = bestFit
	} else if len(elevatorIdleList) > 0 {
		sort.Slice(elevatorIdleList, func(i, j int) bool {
			return elevatorIdleList[i].rating < elevatorIdleList[j].rating
		})
		bestFit = elevatorsInMvtList[len(elevatorsInMvtList)-1]
		_ = bestFit
	} else {
		/************************************************
		*  If no 1. best elevator on mvt
		*       2. best nearest Idle
		*       ==> take the nearest elevator when he become availlable
		**/
		sort.Slice(elevatorIdleList, func(i, j int) bool {
			return elevatorIdleList[i].rating < elevatorIdleList[j].rating
		})
		bestFit = canBeList[len(canBeList)-1]
		_ = bestFit

	}
	return &bestFit
}

// Abs return abs value because math.Abs accept only float
func Abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

// Doors object
type Doors struct {
	status string
}

// open and close doors
func (door Doors) closeDoors() {
	door.status = "IsClose"
	fmt.Println("Close doors >[]<")
}

func (door Doors) openDoors() {
	door.status = "IsOpen"
	fmt.Println("Open doors ]<>[")
}

func main() {

	// ******************** Add some color to the console
	colorReset := "\033[0m"
	colorRed := "\033[31m"

	/*
		-------------------------------------------------------------------------------------------------------------
															Testing Section
		-------------------------------------------------------------------------------------------------------------
	*/
	// Create a battery
	mainBattery := makeBattery(4, 66, 6, 5, "OnLine")

	// Scenario 1
	fmt.Println(string(colorRed), "---------------------[ Scenario 1 ! ]---------------------", string(colorReset))
	mainBattery.columnsList[1].elevatorList[0].direction = "down"
	mainBattery.columnsList[1].elevatorList[0].position = 20
	mainBattery.columnsList[1].elevatorList[0].nextDestination = 5

	mainBattery.columnsList[1].elevatorList[1].direction = "up"
	mainBattery.columnsList[1].elevatorList[1].position = 3
	mainBattery.columnsList[1].elevatorList[1].nextDestination = 15

	mainBattery.columnsList[1].elevatorList[2].direction = "down"
	mainBattery.columnsList[1].elevatorList[2].position = 13
	mainBattery.columnsList[1].elevatorList[2].nextDestination = 1

	mainBattery.columnsList[1].elevatorList[3].direction = "down"
	mainBattery.columnsList[1].elevatorList[3].position = 15
	mainBattery.columnsList[1].elevatorList[3].nextDestination = 2

	mainBattery.columnsList[1].elevatorList[4].direction = "down"
	mainBattery.columnsList[1].elevatorList[4].position = 6
	mainBattery.columnsList[1].elevatorList[4].nextDestination = 1

	mainBattery.AssignElevator(20)
	// Scenario 2
	fmt.Println(string(colorRed), "---------------------[ Scenario 2 ! ]---------------------", string(colorReset))
	mainBattery.columnsList[2].elevatorList[0].direction = "up"
	mainBattery.columnsList[2].elevatorList[0].position = 1
	mainBattery.columnsList[2].elevatorList[0].nextDestination = 21

	mainBattery.columnsList[2].elevatorList[1].direction = "up"
	mainBattery.columnsList[2].elevatorList[1].position = 23
	mainBattery.columnsList[2].elevatorList[1].nextDestination = 28

	mainBattery.columnsList[2].elevatorList[2].direction = "down"
	mainBattery.columnsList[2].elevatorList[2].position = 33
	mainBattery.columnsList[2].elevatorList[2].nextDestination = 1

	mainBattery.columnsList[2].elevatorList[3].direction = "down"
	mainBattery.columnsList[2].elevatorList[3].position = 40
	mainBattery.columnsList[2].elevatorList[3].nextDestination = 24

	mainBattery.columnsList[2].elevatorList[4].direction = "down"
	mainBattery.columnsList[2].elevatorList[4].position = 39
	mainBattery.columnsList[2].elevatorList[4].nextDestination = 1

	mainBattery.AssignElevator(36)
	// Scenario 3
	fmt.Println(string(colorRed), "---------------------[ Scenario 3! ]---------------------", string(colorReset))
	mainBattery.columnsList[3].elevatorList[0].direction = "down"
	mainBattery.columnsList[3].elevatorList[0].position = 58
	mainBattery.columnsList[3].elevatorList[0].nextDestination = 1

	mainBattery.columnsList[3].elevatorList[1].direction = "up"
	mainBattery.columnsList[3].elevatorList[1].position = 50
	mainBattery.columnsList[3].elevatorList[1].nextDestination = 60

	mainBattery.columnsList[3].elevatorList[2].direction = "up"
	mainBattery.columnsList[3].elevatorList[2].position = 46
	mainBattery.columnsList[3].elevatorList[2].nextDestination = 58

	mainBattery.columnsList[3].elevatorList[3].direction = "up"
	mainBattery.columnsList[3].elevatorList[3].position = 1
	mainBattery.columnsList[3].elevatorList[3].nextDestination = 54

	mainBattery.columnsList[3].elevatorList[4].direction = "down"
	mainBattery.columnsList[3].elevatorList[4].position = 60
	mainBattery.columnsList[3].elevatorList[4].nextDestination = 1

	mainBattery.RequestElevator(54)

	fmt.Println(string(colorRed), "---------------------[ Scenario 4 ! ]---------------------", string(colorReset))
	// Scenario 4
	mainBattery.columnsList[0].elevatorList[0].direction = "Idle"
	mainBattery.columnsList[0].elevatorList[0].position = -4
	mainBattery.columnsList[0].elevatorList[0].nextDestination = 0

	mainBattery.columnsList[0].elevatorList[1].direction = "Idle"
	mainBattery.columnsList[0].elevatorList[1].position = 1
	mainBattery.columnsList[0].elevatorList[1].nextDestination = 0

	mainBattery.columnsList[0].elevatorList[2].direction = "down"
	mainBattery.columnsList[0].elevatorList[2].position = -3
	mainBattery.columnsList[0].elevatorList[2].nextDestination = -5

	mainBattery.columnsList[0].elevatorList[3].direction = "up"
	mainBattery.columnsList[0].elevatorList[3].position = -6
	mainBattery.columnsList[0].elevatorList[3].nextDestination = 1

	mainBattery.columnsList[0].elevatorList[4].direction = "down"
	mainBattery.columnsList[0].elevatorList[4].position = -1
	mainBattery.columnsList[0].elevatorList[4].nextDestination = -6

	mainBattery.RequestElevator(-3)

}
