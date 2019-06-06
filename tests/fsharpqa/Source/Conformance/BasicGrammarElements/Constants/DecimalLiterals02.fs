// #Regression #Conformance #BasicGrammarElements #Constants  #NoMono  
// This is a positive test on Dev10, at least until
// FSHARP1.0:4523 gets resolved.
//<Expects status="success"></Expects>

let ok = 1.0E-50M    // parses ok on Dev10

if ok <> 0.0M then 
    exit 1

exit 0
