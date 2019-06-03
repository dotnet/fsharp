// #Conformance #TypesAndModules #Exceptions 
// This is the sample code that appears in the specs under 9.4
//<Expects status="success"></Expects>
#light

exception ThatWentBadlyWrong of string * int
exception ThatWentWrongBadly = ThatWentBadlyWrong

let checkForBadDay() = 
    if System.DateTime.Today.DayOfWeek = System.DayOfWeek.Monday then
        raise (ThatWentWrongBadly("yes indeed",123))
