# --------------------------------------
# Residential controller python version
# --------------------------------------

class Column:
    def __init__(self, floorAmount, elevatorAmount):
        self.floorAmount = floorAmount
        self.elevatorAmount = elevatorAmount
        self.elevatorList = []
        for i in range(elevatorAmount):
            elevator = Elevator(i + 1, "idle", 1, "up", floorAmount)
            self.elevatorList.append(elevator)
        self.floorButtonList = []
        for i in range(floorAmount):
            if i == 1:
                self.floorButtonList.append(
                    floorButton(i, "up"))  # first floor
            self.floorButtonList.append(floorButton(i, "up"))
            self.floorButtonList.append(floorButton(i, "down"))
        self.floorButtonList.append(floorButton(
            floorAmount, "down"))  # last floor

    def requestElevator(self, requestedFloor, direction):
        elevator = self.bestElevator(requestedFloor, direction)
        elevator.requestList.append(requestedFloor)

        print(bcolors.OKGREEN+"\nRequest elevator from floor: "+str(requestedFloor) +
              "  direction: " + direction + bcolors.ENDC)
        print(bcolors.UNDERLINE+"elevator selected id: " + bcolors.ENDC, elevator.id,
              "position: ", elevator.position)
        elevator.manageRequestList()
        elevator.moveElevator(requestedFloor)
        print("Elevator = ", elevator.id, "position: ",
              elevator.position,	"Status: ", elevator.status)
        return elevator

    def bestElevator(self, requestedFloor, direction):
        bestfit = None
        minDistance = 11
        nearestIdle = None
        for elevator in (self.elevatorList):
            print("elevator ID [ "+bcolors.WARNING + str(elevator.id) + bcolors.ENDC+" ]  position: [ "+bcolors.WARNING + str(elevator.position)+bcolors.ENDC +
                  " ] direction: [ "+bcolors.WARNING + str(elevator.direction)+bcolors.ENDC+"] Status: [ "+bcolors.WARNING + str(elevator.status) + bcolors.ENDC + "]")
            distance = abs(requestedFloor - elevator.position)
            if (requestedFloor == elevator.position and (elevator.status == "stopped" or elevator.status == "idle")):
                bestfit = elevator
            elif (minDistance > distance and elevator.direction == direction):
                minDistance = distance
                bestfit = elevator
        idleDistance = 11
        for elevator in (self.elevatorList):
            distance = abs(requestedFloor - elevator.position)
            if (elevator.status == "idle" and idleDistance >= distance):
                idleDistance = distance
                nearestIdle = elevator

        if bestfit != None:
            return bestfit
        else:
            return nearestIdle

    def requestFloor(self, elevator, requestedFloor):
        print(bcolors.OKGREEN+"\nlanding request to floor: " +
              str(requestedFloor)+bcolors.ENDC)
        elevator.requestList.append(requestedFloor)
        elevator.manageRequestList()
        elevator.moveElevator(requestedFloor)


class Elevator:
    def __init__(self, id, status, position, direction, floorAmount):
        self.id = id
        self.status = status
        self.position = position
        self.direction = direction
        self.requestList = []
        self.panelButtonList = []
        for i in range(floorAmount):
            button = panelButton(i)
            self.panelButtonList.append(button)

    def manageRequestList(self):
        if self.direction == "up":
            self.requestList.sort()
        elif self.direction == "down":
            self.requestList.sort()
            self.requestList.reverse()

    def moveUp(self, requestedFloor):
        while (self.position != requestedFloor):
            self.position += 1
            print(">> Elevator is moving to :  " +
                  bcolors.OKBLUE + str(self.position)+bcolors.ENDC)

    def moveDown(self, requestedFloor):
        while (self.position != requestedFloor):
            self.position -= 1
            print(">> Elevator is moving to :  " +
                  bcolors.OKBLUE + str(self.position)+bcolors.ENDC)

    def moveElevator(self, requestedFloor):
        while (len(self.requestList) > 0):
            if (requestedFloor == self.position):
                self.status = "stopped"
                print("Elevator: ", self.id, " Status: ",
                      self.status, " direction: ", self.direction)
                self.openDoor()
                self.status = "moving"
                self.requestList.pop()
            elif (requestedFloor < self.position):
                self.status = "moving"
                self.direction = "down"
                print("Elevator: ", self.id, " Status: ",
                      self.status, " direction: ", self.direction)
                self.moveDown(requestedFloor)
                self.status = "stopped"
                print("Elevator: ", self.id, " Status: ",
                      self.status, " direction: ", self.direction)
                self.openDoor()
                self.requestList.pop()
            elif (requestedFloor > self.position):
                self.status = "moving"
                self.direction = "up"
                print("Elevator: ", self.id, " Status: ",
                      self.status, " direction: ", self.direction)
                self.moveUp(requestedFloor)
                self.status = "stopped"
                print("Elevator: ", self.id, " Status: ",
                      self.status)
                self.openDoor()
                self.requestList.pop()

        if self.requestList == 0:
            self.status = "idle"
            print("Elevator: ",  self.status)

    def openDoor(self):
        print(bcolors.WARNING+"[-- open Door -- ]"+bcolors.ENDC)
        self.closeDoor()

    def closeDoor(self):
        print(bcolors.WARNING+"[-- close Door-- ]"+bcolors.ENDC)


class floorButton:
    def __init__(self, numFloor, direction):
        self.numFloor = numFloor
        self.direction = direction


class panelButton:
    def __init__(self, destination):
        self.destination = destination

# ---------------------------------------
# ---------  [ Testing Section ] -------


class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


print(bcolors.HEADER + bcolors.UNDERLINE + bcolors.BOLD +
      "     [ Scenario 1 ]     "+bcolors.ENDC)
print(bcolors.HEADER +
      " 1. Someone is on floor 3 and wants to go to the 7th floor"+bcolors.ENDC)
column1 = Column(10, 2)
column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 2
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 6
column1.elevatorList[1].direction = "none"
elevator = column1.requestElevator(3, "up")
column1.requestFloor(elevator, 7)

print(bcolors.HEADER + bcolors.UNDERLINE + bcolors.BOLD +
      "     [ Scenario 2 ]     "+bcolors.ENDC)
print(" 2.1. Someone is on the 1st floor and requests the 6th floor. ")
column1 = Column(10, 2)
column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 10
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 3
column1.elevatorList[1].direction = "none"

elevator = column1.requestElevator(1, "up")
column1.requestFloor(elevator, 6)

print(bcolors.HEADER+"2.2. someone else is on the 3rd floor and requests the 5th floor. "+bcolors.ENDC)
elevator = column1.requestElevator(3, "up")
column1.requestFloor(elevator, 5)
print(bcolors.HEADER+"2.3. a third person is at floor 9 and wants to go down to the 2nd floor.  "+bcolors.ENDC)
elevator = column1.requestElevator(9, "down")
column1.requestFloor(elevator, 2)

print(bcolors.HEADER + bcolors.UNDERLINE + bcolors.BOLD +
      "     [ Scenario 3 ]     "+bcolors.ENDC)
print(bcolors.HEADER +
      " 3.1. Someone is on floor 3 and requests the 2nd floor.  "+bcolors.ENDC)
column1 = Column(10, 2)
column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 10
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "moving"
column1.elevatorList[1].position = 3
column1.elevatorList[1].direction = "up"

elevator = column1.requestElevator(3, "down")
column1.requestFloor(elevator, 2)
print(bcolors.HEADER+"3.2. someone else is on the 10th floor and wants to go to the 3rd. "+bcolors.ENDC)


column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 2
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 6
column1.elevatorList[1].direction = "none"

elevator = column1.requestElevator(10, "down")
column1.requestFloor(elevator, 3)
