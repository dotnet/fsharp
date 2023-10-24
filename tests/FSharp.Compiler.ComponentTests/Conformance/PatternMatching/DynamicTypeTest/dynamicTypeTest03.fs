// #Regression #Conformance #PatternMatching #TypeTests 
// FSB 1034, Downcasting for interface types
// This test verifies that dynamic type test patterns work on objects which cannot
// statically be checked if they implement the interface. (For example, A implements IFoo and IBar
// dynamically checking if an instance of (A :> IFoo) implements IBar.

// NB. Years hard coded to avoid introducing non-determinism
type IAge = interface
    abstract Age : int
    end

type IAges = interface
    abstract GrowOlder : int -> unit
    end

type Car(modelYear : int) =
    let mutable m_miles = 0
    let Odometer = m_miles
    
    interface IAge with
        member this.Age = 2008 - modelYear
    interface IAges with
        member this.GrowOlder x = m_miles <- m_miles + x
    
type Person(yearBorn : int) =
    let mutable m_yearsOld = 2008 - yearBorn
    
    interface IAge with
        member this.Age = m_yearsOld

    interface IAges with
        member this.GrowOlder x = m_yearsOld <- m_yearsOld + x

type Wine(year : int) =
    interface IAge with
        member this.Age = year

// Variables
let me = new Person(1982)
let myCar = new Car(2005)
let myWine = new Wine(1)

let myStuff = [ (myWine :> IAge); (me :> IAge); (myCar :> IAge) ]

// Functions
let totalAge (iAgeList : IAge list) = iAgeList |> List.map (fun iage -> iage.Age) |> List.sum

let rec ageOneYear (l:IAge list) =
    match l with
    | (:? IAges as agingThing) :: tl -> agingThing.GrowOlder 1
                                        ageOneYear tl 
    | hd :: tl -> ageOneYear tl
    | [] -> ()

// Tests
if totalAge myStuff <> 30 then exit 1

ageOneYear myStuff
// (In this test, wine doesn't age, and cars 'grow older' doesn't increment age')
if totalAge myStuff <> 31 then exit 2

exit 0
    
