class Column {
	constructor(floorAmount, elevatorAmount) {
		this.floorAmount = floorAmount;
		this.elevatorAmount = elevatorAmount;
		this.elevatorList = [];
		this.floorButtonList = [];
		this.createElevatorList(elevatorAmount); // create elevatorsList
		this.createFloorButtonList(floorAmount); // floorButtonList
	}

	createFloorButtonList(floorAmount) {
		for (let i = 1; i < this.floorAmount; i++) {
			if (i == 1) {
				this.floorButtonList.push(new floorButton(i, "up"));
			} else {
				this.floorButtonList.push(new floorButton(i, "up"));
				this.floorButtonList.push(new floorButton(i, "down"));
			}
		}
		this.floorButtonList.push(new floorButton(this.floorAmount, "down"));
	}
	createElevatorList(elevatorAmount) {
		for (let i = 1; i <= this.elevatorAmount; i++) {
			let elevator = new Elevator(i, "idle", 1, "up", this.floorAmount);
			this.elevatorList.push(elevator);
		}
	}
	requestElevator(requestedFloor, direction) {
		// let elevator = this.elevatorList[0];
		// console.log("ma liste: ", elevator);
		let elevator = this.bestElevator(requestedFloor, direction);
		elevator.requestList.push(requestedFloor);

		console.log(
			"\nRequest from floor: ",
			requestedFloor,
			"  direction: ",
			direction
		);
		console.log(
			"elevator selected id: ",
			elevator.id,
			"position: ",
			elevator.position,
			"\n"
		);
		elevator.manageRequestList();
		elevator.moveElevator(requestedFloor);
		console.log(
			"End mouvement = ",
			elevator.id,
			"position: ",
			elevator.position,
			"Status: ",
			elevator.status
		);
		return elevator;
	}
	requestFloor(elevator, requestedFloor) {
		console.log("\nlanding request to floor: ", requestedFloor);
		elevator.requestList.push(requestedFloor);
		elevator.manageRequestList();
		elevator.moveElevator(requestedFloor);
	}

	bestElevator(requestedFloor, direction) {
		let bestFit = null;
		let bestDistance = 11;
		let nearestIdle = null;
		for (let i = 0; i < this.elevatorList.length; i++) {
			let elevator = this.elevatorList[i];
			let distance = Math.abs(requestedFloor - elevator.position);
			console.log(
				"elevator ID:  ",
				elevator.id,
				" || position: ",
				elevator.position,
				" || direction: ",
				elevator.direction,
				"      || Status: ",
				elevator.status
			);
			// case1: best condition
			if (
				requestedFloor === elevator.position &&
				(elevator.status === "idle" || elevator.status === "stopped")
			) {
				bestFit = elevator;
			} else if (elevator.direction === direction && bestDistance >= distance) {
				bestFit = elevator;
				bestDistance = distance;
			}
		}

		// The priority is for elevator in movement : if bestFit == null => get the nearest IDLE
		var minDistance = 1000;
		for (let i = 0; i < this.elevatorList.length; i++) {
			let elevator = this.elevatorList[i];
			let distance = Math.abs(requestedFloor - elevator.position);
			if (elevator.status === "idle" && minDistance >= distance) {
				minDistance = distance;
				nearestIdle = elevator;
			}
		}

		if (bestFit !== null) {
			return bestFit;
		} else {
			return nearestIdle;
		}
	}
}
class Elevator {
	constructor(id, status, position, direction, elevatorAmount) {
		this.id = id;
		this.status = status;
		this.position = position;
		this.direction = direction;
		this.requestList = [];
		this.panelButtonList = [];
		// Panel Button
		for (let i = 0; i < elevatorAmount; i++) {
			this.panelButtonList[i] = new panelButton(i + 1);
		}
	}
	moveElevator(requestedFloor) {
		while (this.requestList > 0) {
			if (requestedFloor === this.position) {
				this.status = "stopped";
				console.log(
					"Elevator: ",
					this.id,
					" Status: ",
					this.status,
					" direction: ",
					this.direction
				);
				this.openDoor();
				this.requestList.shift();
			} else if (requestedFloor > this.position) {
				this.status = "moving";
				this.direction = "up";
				console.log(
					"Elevator: ",
					this.id,
					" Status: ",
					this.status,
					" Direction: ",
					this.direction
				);
				this.moveElevatorUp(requestedFloor);
				this.status = "stopped";
				console.log(
					"Elevator: ",
					this.id,
					" position ",
					this.position,
					" Status: ",
					this.status
				);
				this.openDoor();
				this.requestList.shift();
			} else if (requestedFloor < this.position) {
				this.status = "moving";
				this.direction = "down";
				console.log(
					"Elevator: ",
					this.id,
					" Status: ",
					this.status,
					" Direction: ",
					this.direction
				);
				this.moveElevatorDown(requestedFloor);
				this.status = "stopped";
				console.log(
					"Elevator: ",
					this.id,
					" position ",
					this.position,
					" Status: ",
					this.status
				);
				this.openDoor();
				this.requestList.shift();
			}
		}
		if (this.requestList === 0) {
			this.status = "idle";
		}
	}
	moveElevatorUp(requestedFloor) {
		while (this.position !== requestedFloor) {
			this.position += 1;
			console.log("---- Elevator is moving:  " + this.position);
		}
	}

	moveElevatorDown(requestedFloor) {
		while (this.position !== requestedFloor) {
			this.position -= 1;
			console.log("Elevator is moving: " + this.position);
		}
	}
	manageRequestList() {
		if (this.direction === "up") {
			this.requestList.sort();
		} else if (this.direction === "down") {
			this.requestList.sort();
			this.requestList.reverse();
		}
	}
	openDoor() {
		console.log("open door");
		this.closeDoor();
	}

	closeDoor() {
		console.log("close door");
	}
}

class panelButton {
	constructor(destination) {
		this.destination = destination;
	}
}
class floorButton {
	constructor(numFloor, direction) {
		this.numFloor = numFloor;
		this.direction = direction;
	}
}

//**---------------                 Testing Section                     --------------- *** //

function Scenario1() {
	console.dir(" 1. Someone is on floor 3 and wants to go to the 7th floor");
	var column1 = new Column(10, 2);
	column1.elevatorList[0].status = "idle";
	column1.elevatorList[0].position = 2;
	column1.elevatorList[0].direction = "none";
	column1.elevatorList[1].status = "idle";
	column1.elevatorList[1].position = 6;
	column1.elevatorList[1].direction = "none";

	var elevator = column1.requestElevator(3, "up");
	column1.requestFloor(elevator, 7);
}

function Scenario2() {
	console.log("----------- [Scenario 2] ------------------");
	console.dir(" 1. Someone is on the 1st floor and requests the 6th floor. ");
	var column1 = new Column(10, 2);
	column1.elevatorList[0].status = "idle";
	column1.elevatorList[0].position = 10;
	column1.elevatorList[0].direction = "none";
	column1.elevatorList[1].status = "idle";
	column1.elevatorList[1].position = 3;
	column1.elevatorList[1].direction = "none";

	var elevator = column1.requestElevator(1, "up");
	column1.requestFloor(elevator, 6);

	console.dir(
		"2. someone else is on the 3rd floor and requests the 5th floor. "
	);
	var elevator = column1.requestElevator(3, "up");
	column1.requestFloor(elevator, 5);
	console.dir(
		"3. a third person is at floor 9 and wants to go down to the 2nd floor.  "
	);
	var elevator = column1.requestElevator(9, "down");
	column1.requestFloor(elevator, 2);
}
function Scenario3() {
	console.log("----------- [Scenario 3] ------------------");
	console.dir(" 1. Someone is on floor 3 and requests the 2nd floor.  ");
	var column1 = new Column(10, 2);
	column1.elevatorList[0].status = "idle";
	column1.elevatorList[0].position = 10;
	column1.elevatorList[0].direction = "none";
	column1.elevatorList[1].status = "moving";
	column1.elevatorList[1].position = 3;
	column1.elevatorList[1].direction = "up";

	var elevator = column1.requestElevator(3, "down");
	column1.requestFloor(elevator, 2);
	console.dir(
		"2. someone else is on the 10th floor and wants to go to the 3rd. "
	);
	column1.elevatorList[0].status = "idle";
	column1.elevatorList[0].position = 2;
	column1.elevatorList[0].direction = "none";
	column1.elevatorList[1].status = "idle";
	column1.elevatorList[1].position = 6;
	column1.elevatorList[1].direction = "none";

	var elevator = column1.requestElevator(10, "down");
	column1.requestFloor(elevator, 3);
}

Scenario1();
Scenario2();
Scenario3();
