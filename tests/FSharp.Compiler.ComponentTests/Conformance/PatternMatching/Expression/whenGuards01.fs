// #Conformance #PatternMatching 
#light

// Test that when guards only execute once the match value has 
// matched the pattern associated with the rule.

let mutable m_flag = 0
let flagSet() = m_flag = 1
let resetFlag() = m_flag <- 0

let testMatch x = 
    match x with
    | 1 when (m_flag <- 1; true)  -> 1
    | 2 when (m_flag <- 1; false) -> 2
    | _ -> 0
    
    
// 0 should hit the wildcard, and testMatch should return 0
resetFlag()
if testMatch 0 <> 0 then exit 1
if flagSet() then exit 1

// 1 should have been matched, setting the flag to true
// and with the where guard succeeding, the match statement
// returned 1 as well.
resetFlag()
if testMatch 1 <> 1 then exit 1
if not (flagSet()) then exit 1

// 2 should have been matched, setting the flat to true
// but, since the where guard failed, the match statement
// hit the wild card- returning 0
resetFlag()
if testMatch 2 <> 0 then exit 1
if not (flagSet()) then exit 1

exit 0
