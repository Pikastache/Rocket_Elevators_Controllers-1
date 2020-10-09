class Column
    attr_accessor :floorAmount, :elevatorAmount, :elevatorList
    attr_reader :floorButtonList, :elevatorList, :elevatorAmount, :floorAmount
    def initialize(floorAmount, elevatorAmount)
        @floorAmount= floorAmount
        @elevatorAmount =elevatorAmount
        @elevatorList = Array.new
        @floorButtonList = Array.new 
       
        self.createElevatorList() 
        self.createFloorButtonList() 
    end
    def elevatorList
     @elevatorList
    end

    def createElevatorList()
        for i in 1..@elevatorAmount        
            elevator = Elevator.new(i, "idle", 1, "up", @floorAmount) 
            @elevatorList.push(elevator) 
        end
    end
    def createFloorButtonList ()
        button = FloorButton.new(1, "up") 
        @floorButtonList.push(button)
        for i in 2..(@floorAmount-1)        
            button = FloorButton.new(i, "up") 
            @floorButtonList.push(button) 
            button = FloorButton.new(i, "down") 
            @floorButtonList.push(button)
        end
        button = FloorButton.new(@floorAmount, "up") 
        @floorButtonList.push(button)
    end
   
    def requestElevator(requestedFloor, direction) 
        elevator = self.bestElevator(requestedFloor, direction)
      
        elevator.requestList.push(requestedFloor)
        print("----------------------------------\n")
        puts("\nRequest elevator from floor: "+ requestedFloor.to_s+ "  direction: "+ direction )
        puts("Selected elevator id: "+elevator.id.to_s+
        "  position: "+ elevator.position.to_s)
        print("----------------------------------\n")
        elevator.manageRequestList()
        elevator.moveElevator(requestedFloor)
        # puts("Elevator = ", elevator.id, "position: ", elevator.position,	"Status: ", elevator.status)
        return elevator
    end
    def bestElevator(requestedFloor, direction)
        bestfit = nil
        minDistance = 11
        nearestIdle = nil
        for elevator in (self.elevatorList)
            puts("elevator ID: "+ (elevator.id).to_s+ "  position:  "+ (elevator.position).to_s+ "  direction:  "+ elevator.direction+ "Status:  "+ elevator.status)
            distance = (requestedFloor - elevator.position).abs
            if (requestedFloor == elevator.position and (elevator.status == "stopped" or elevator.status == "idle"))
                bestfit = elevator
            elsif (minDistance > distance and elevator.direction == direction)
                minDistance = distance
                bestfit = elevator
            end
        end
        idleDistance = 11
        for elevator in (self.elevatorList)
            distance = (requestedFloor - elevator.position).abs
            if (elevator.status == "idle" and idleDistance >= distance)
                idleDistance = distance
                nearestIdle = elevator
            end
        end

        if bestfit != nil
            return bestfit
        else
            return nearestIdle
        end
    
    end
    def requestFloor(elevator, requestedFloor)
        puts("\nlanding request to floor: [ "+ requestedFloor.to_s+ "]")
        elevator.requestList.push(requestedFloor)
        elevator.manageRequestList()
        elevator.moveElevator(requestedFloor) 
           
    end
end
class Elevator
    attr_reader :id, :status, :position, :direction, :panelButtonList, :requestList
    attr_accessor :id, :status, :position, :direction, :panelButtonList, :requestList
    def initialize(id, status, position, direction, floorAmount)
        @id = id
        @status = status
        @position = position
        @direction = direction
        @requestList = Array.new
        @panelButtonList = Array.new 
        
        def status
     @status
    end
        for i in 1..9        
            button = PanelButton.new(i) 
            @panelButtonList.push(button) 
        end
    end
    def manageRequestList()
            if (self.direction == "up")
                self.requestList.sort()
            elsif (self.direction == "down")
                self.requestList.sort()
                self.requestList.reverse() 
            end
    end
    def moveUp(requestedFloor)
            while (self.position != requestedFloor)
                self.position += 1
                puts(">> Elevator is moving to : [ "+ self.position.to_s+" ]")
            end
    end
    def moveDown(requestedFloor)
        while (self.position != requestedFloor)
            self.position -= 1
            puts(">> Elevator is moving to : [ "+ self.position.to_s+" ]")
        end
    end

    def moveElevator(requestedFloor)
        while (self.requestList.length() > 0)
            if (requestedFloor == self.position)
                self.status = "stopped"
                puts("Elevator: [ "+ self.id.to_s+ " ] sent to [ " +requestedFloor.to_s+ " ]")
                self.openDoor()
                self.status = "moving"
                self.requestList.pop()
                elsif (requestedFloor < self.position)
                self.status = "moving"
                self.direction = "down"
                puts("Elevator: ["+ self.id.to_s+ " ] Status: ["+
                      self.status+ " ] direction: [ "+ self.direction+" ]")
                self.moveDown(requestedFloor)
                self.status = "stopped"
                puts("Elevator: ["+ self.id.to_s+ " ] Status: ["+
                    self.status+ " ] direction: [ "+ self.direction+" ]")
                self.openDoor()
                self.requestList.pop()
                elsif (requestedFloor > self.position)
                self.status = "moving"
                self.direction = "up"
                puts("Elevator: [ "+ self.id.to_s+ " ] Status: [ "+
                      self.status+ " ] direction: ["+ self.direction+ "]")
                self.moveUp(requestedFloor)
                self.status = "stopped"
                puts("Elevator: [ "+ self.id.to_s+ " ] Status: [ "+
                    self.status)
                self.openDoor()
                self.requestList.pop()
            end
        end

        if (self.requestList == 0)
            self.status = "idle"
            puts("Elevator: "+  self.status)
        end
    end
    def openDoor()
        print("[ open Door ]\n")
        self.closeDoor()
    end

    def closeDoor()
        print("[close Door ]\n")
    end
end
class FloorButton
    def initialize(numFloor, direction)
        @numFloor = numFloor
        @direction = direction
    end
end


class PanelButton
    def initialize(destination)
        @destination = destination
    end
end


# testing 

print("\n-----------     [ Scenario 1 ] --------------   \n ")
print(" 1. Someone is on floor 3 and wants to go to the 7th floor\n")
print(" \n ")

column1 = Column.new(10, 2)

column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 2
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 6
column1.elevatorList[1].direction = "none"
elevator = column1.requestElevator(3, "up")
column1.requestFloor(elevator, 7)
puts("\n")
print("\n-----------     [ Scenario 2 ] --------------   \n ")
print(" 2.1. Someone is on the 1st floor and requests the 6th floor. \n\n")
print(" \n ")

column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 10
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 3
column1.elevatorList[1].direction = "none"

elevator = column1.requestElevator(1, "up")
column1.requestFloor(elevator, 6)
print("  \n ")
print("\n2.2. someone else is on the 3rd floor and requests the 5th floor. \n")
print(" \n ")
elevator = column1.requestElevator(3, "up")
column1.requestFloor(elevator, 5)
print(" \n ")
print("\n 2.3. a third person is at floor 9 and wants to go down to the 2nd floor.  \n")
print(" \n ")
elevator = column1.requestElevator(9, "down")
column1.requestFloor(elevator, 2)
puts("\n")
print("\n-----------     [ Scenario 3 ] --------------   \n ")
print(" 3.1. Someone is on floor 3 and requests the 2nd floor.  \n")
print("  \n ")

column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 10
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "moving"
column1.elevatorList[1].position = 3
column1.elevatorList[1].direction = "up"

elevator = column1.requestElevator(3, "down")
column1.requestFloor(elevator, 2)
print("\n**********  \n ")
print("\n3.2. someone else is on the 10th floor and wants to go to the 3rd.\n ")
print("  \n ")


column1.elevatorList[0].status = "idle"
column1.elevatorList[0].position = 2
column1.elevatorList[0].direction = "none"
column1.elevatorList[1].status = "idle"
column1.elevatorList[1].position = 6
column1.elevatorList[1].direction = "none"

elevator = column1.requestElevator(10, "down")
column1.requestFloor(elevator, 3)
