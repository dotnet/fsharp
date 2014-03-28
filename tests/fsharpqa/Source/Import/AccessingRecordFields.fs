// #Regression #NoMT #Import 
#light

// Regression test for FSharp1.0:4136 - CTP: FieldAccessException is thrown when accessing record fields from other assemblies

open InfoLib

let Dave = { Name = "David"; DoB = new System.DateTime(1980, 1, 1); Age = 28 }

let isAgeCorrect (i : Info) = 
    match (System.DateTime.Today.Year - i.DoB.Year) with
    | n when n = i.Age -> true
    | _                -> false


if not <| isAgeCorrect Dave then
    Dave.Age <- System.DateTime.Today.Year - Dave.DoB.Year

exit 0
