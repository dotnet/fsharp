// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:6108
// Note: --warnon *WAS* for test purposes only, now it is NOT.
//<Expects status="notin" id="FS0075">The command-line option '--warnon' is for test purposes only</Expects>

//<Expects status="warning" span="(14,9-14,10)" id="FS1182">The value 'u' is unused$</Expects>
//<Expects status="warning" span="(18,9-18,10)" id="FS1182">The value 's' is unused$</Expects>
//<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>

type X = | X of string*int

let x = X("foo",42)
let ff x =
    let u = 0
    match [x] with
    //| [X(s,n) as r] -> ()
    | [] -> ()
    | X(s,n) as r::_ -> ignore r; ()    

