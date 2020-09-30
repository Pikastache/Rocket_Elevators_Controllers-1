//  Objet column

class Column {
	constructor(floorAmount, elevatorAmount) {
		this.floorAmount = floorAmount;
		this.elevatorAmount = elevatorAmount;
		this.elevatorList = [];
		this.floorButtonList = [];
		this.requestList = [];
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
		this.requestList.push(requestedFloor);

		console.log("Request = ", requestedFloor, direction);
		console.log("elevator selected ID = ", elevator.id);
		//elevator.direction = direction;
		//this.manageRequestList(elevator);
		//move elevator to requestedFloor
		return elevator;
	}

	bestElevator(requestedFloor, direction) {
		let bestFit = null;
		let bestDistance = 11;
		let nearestIdle = null;
		for (let i = 0; i < this.elevatorList.length; i++) {
			let elevator = this.elevatorList[i];
			let distance = Math.abs(requestedFloor - elevator.position);
			console.log(
				"elevator ID: ",
				elevator.id,
				"elevator position: ",
				elevator.position,
				"direction",
				elevator.direction
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
				console.log("Same direction: ", elevator.direction === direction);
			}
		}
		var minDistance = 110;
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
	manageRequestList(elevator) {
		if (elevator.direction === "up") {
			this.requestList.sort();
		} else if (elevator.direction === "down") {
			this.requestList.sort();
			this.requestList.reverse();
		}
	}
}
class Elevator {
	constructor(id, status, position, direction, elevatorAmount) {
		this.id = id;
		this.status = status;
		this.position = position;
		this.direction = direction;
		this.requestLandingList = [];
		this.panelButtonList = [];
		// Panel Button
		for (let i = 0; i < elevatorAmount; i++) {
			this.panelButtonList[i] = new panelButton(i + 1);
		}
	}
	// moveElevator(destination) {
	// 	console.log("floor: " + this.liftPosition);
	// 	while (this.liftPosition !== callDestinationFloor) {
	// 		this.liftPosition += 1;
	// 		console.log("floor: " + this.liftPosition);
	// 	}
	// }

	// moveElevatorDown(callDestinationFloor) {
	// 	while (this.liftPosition !== callDestinationFloor) {
	// 		this.liftPosition -= 1;
	// 		console.log("floor: " + this.liftPosition);
	// 	}
	// }
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

//   ** --------------- Testing Section --------------- *** //

console.dir;
console.dir("------------------Start Test-------------------");

var column1 = new Column(10, 2);
// console.dir(column1);

// Scenario 1
console.dir("------------------ Scénario 1 -------------------");
column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 2;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "idle";
column1.elevatorList[1].position = 6;
column1.elevatorList[1].direction = "none";
column1.requestElevator(3, "up");
console.table(column1.requestList);

console.dir("------------------ Scénario 2 -------------------");
column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 10;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "idle";
column1.elevatorList[1].position = 3;
column1.elevatorList[1].direction = "none";
column1.requestElevator(1, "up");
console.table(column1.requestList);
console.dir(
	"2 minutes later, someone else is on the 3rd floor and requests the 5th floor. Elevator B should be sent."
);
column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 10;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "idle";
column1.elevatorList[1].position = 6;
column1.elevatorList[1].direction = "none";
column1.requestElevator(3, "up");
console.table(column1.requestList);

console.dir(
	"Finally, a third person is at floor 9 and wants to go down to the 2nd floor Elevator A should be sent."
);
column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 10;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "idle";
column1.elevatorList[1].position = 5;
column1.elevatorList[1].direction = "none";
column1.requestElevator(9, "down");
console.table(column1.requestList);

console.dir("------------------ Scénario 3 -------------------");
column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 10;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "moving";
column1.elevatorList[1].position = 3;
column1.elevatorList[1].direction = "up";
column1.requestElevator(3, "down");
console.table(column1.requestList);
console.dir(
	"=> 2 minutes plus tard, quelqu’un d’autre est au 10e étage et veut descendre au 3e étage ascenseur B devrait être sélectionné."
);

column1.elevatorList[0].status = "idle";
column1.elevatorList[0].position = 2;
column1.elevatorList[0].direction = "none";
column1.elevatorList[1].status = "idle";
column1.elevatorList[1].position = 6;
column1.elevatorList[1].direction = "none";
column1.requestElevator(10, "down");
console.table(column1.requestList);
