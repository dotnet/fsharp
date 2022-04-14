// #Regression #Conformance #TypesAndModules #Records 
// Field labels have module scope
//<Expects id="FS0039" span="(9,15-9,16)" status="error">The record label 'a' is not defined</Expects>
#light

    module M0 =
        type T1 = { a : int;}

    let x = { a = 10 }              // error

