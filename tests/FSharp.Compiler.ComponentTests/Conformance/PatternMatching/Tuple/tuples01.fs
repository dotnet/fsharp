// #Conformance #PatternMatching #Tuples 
#light

// Match nested tuples

let monsterTuple = ("0", "1", ("2", "2.1", "2.2"), "3", ("4", ("4.1", "4.1.1"), "4.2"))

let result = 
    match monsterTuple with
    | (_, _, (_, _, _), _, (_, (_, _), _)) -> true

// Possible compile time error:
// MonsterTuple doesn't match signature (...) from type inference

// Possible runtime error:
// MatchNotFoundException
if result <> true then exit 1

exit 0    
