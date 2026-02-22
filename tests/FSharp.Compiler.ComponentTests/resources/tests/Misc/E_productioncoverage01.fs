// #Regression #Misc 
// 390      atomicPatterns -> atomicPattern HIGH_PRECEDENCE_APP atomicPatterns 

// Note, need a test case to check an error is raised here:
//<Expects id="FS0584" status="error">Successive patterns should be separated by spaces or tupled</Expects>

let (|C|_|) x inp = inp

let q x = 
    match 3 with
    | C x(3) -> ""
    |  _ -> ""
    
// Testcase should have build error
exit 1
