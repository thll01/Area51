using System;
using System.Threading;

enum Floor
{
    Ground, Secret1, Secret2, TopSecret
}

enum ClearanceLevel
{
    Low, Medium, High
}

class Elevator
{
    private Floor currentFloor;
    private SemaphoreSlim elevatorSemaphore;
    private SemaphoreSlim doorSemaphore;

    public Elevator()
    {
        currentFloor = Floor.Ground;
        elevatorSemaphore = new SemaphoreSlim(1, 1);
        doorSemaphore = new SemaphoreSlim(1, 1);
    }

    public void CallElevator(Floor floor)
    {
        elevatorSemaphore.Wait();
        Console.WriteLine($"Person presses the button on floor {floor} to call the elevator");
        doorSemaphore.Wait();
        Console.WriteLine("Elevator doors are open");
        Thread.Sleep(1000);
        Console.WriteLine("Elevator doors are closed");
        doorSemaphore.Release();
        elevatorSemaphore.Release();
    }

    public void EnterElevator(ClearanceLevel personClearanceLevel, Floor targetFloor)
    {
        doorSemaphore.Wait();
        Console.WriteLine($"Person enters the elevator on floor {currentFloor}");
        if (personClearanceLevel >= GetFloorClearanceLevel(targetFloor))
        {
            Console.WriteLine($"Elevator moves to floor {targetFloor}");
            Thread.Sleep(1000);
            currentFloor = targetFloor;
        }
        else
        {
            Console.WriteLine($"Person does not have access to floor {targetFloor}");
        }
        doorSemaphore.Release();
    }

    private ClearanceLevel GetFloorClearanceLevel(Floor floor)
    {
        switch (floor)
        {
            case Floor.Ground:
                return ClearanceLevel.Low;
            case Floor.Secret1:
                return ClearanceLevel.Medium;
            case Floor.Secret2:
            case Floor.TopSecret:
                return ClearanceLevel.High;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

class Person
{
    private ClearanceLevel clearanceLevel;
    private Floor currentFloor;
    private Elevator elevator;

    public Person(ClearanceLevel clearanceLevel, Elevator elevator)
    {
        this.clearanceLevel = clearanceLevel;
        this.currentFloor = Floor.Ground;
        this.elevator = elevator;
    }

    public void MoveAround()
    {
        Random random = new Random();

        while (true)
        {
            Floor targetFloor = (Floor)random.Next(4);
            elevator.CallElevator(currentFloor);
            elevator.EnterElevator(clearanceLevel, targetFloor);
            currentFloor = targetFloor;
            Thread.Sleep(2000); // Simulate time spent on the floor
        }
    }
}

class Program
{
    static void Main()
    {
        Elevator elevator = new Elevator();

        Person personLowClearance = new Person(ClearanceLevel.Low, elevator);
        Person personMediumClearance = new Person(ClearanceLevel.Medium, elevator);
        Person personHighClearance = new Person(ClearanceLevel.High, elevator);

        Thread lowClearanceThread = new Thread(personLowClearance.MoveAround);
        Thread mediumClearanceThread = new Thread(personMediumClearance.MoveAround);
        Thread highClearanceThread = new Thread(personHighClearance.MoveAround);

        lowClearanceThread.Start();
        mediumClearanceThread.Start();
        highClearanceThread.Start();

        lowClearanceThread.Join();
        mediumClearanceThread.Join();
        highClearanceThread.Join();
    }
}
