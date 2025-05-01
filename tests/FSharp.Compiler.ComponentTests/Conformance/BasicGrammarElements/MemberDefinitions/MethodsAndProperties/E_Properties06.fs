// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4163
// Setter with no arguments (curried or uncurried?)
// Used to crash the compiler!

//See FSHARP1.0:5456


type Transform (rotation : (float * float * float), position : (float * float *float)) = 
    let  foo  =   1
    member v.init
        with set() =  
            let bar = foo in
              ()
            bar

