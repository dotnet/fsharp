// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression test for FSHARP1.0:4163
// Setter with no arguments (curried or uncurried?)
// Used to crash the compiler!
//<Expects id="FS0001" span="(15,13-15,16)" status="error">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
//See FSHARP1.0:5456


type Transform (rotation : (float * float * float), position : (float * float *float)) = 
    let  foo  =   1
    member v.init
        with set() =  
            let bar = foo in
              ()
            bar

