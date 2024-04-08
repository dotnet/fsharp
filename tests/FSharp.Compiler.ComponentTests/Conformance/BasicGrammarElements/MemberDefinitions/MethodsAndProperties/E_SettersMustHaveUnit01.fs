// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify property setters must have type unit
//<Expects id="FS0001" status="error" span="(10,66)">This expression was expected to have type</Expects>

type immut =       
  {
     immutStr:       string ;  
  }
with 
  member x.Str       with get() = x.immutStr and set(v:string) = {immutStr = v}

let oldtInstance = {immutStr = "Old"}
let newInstance  = oldtInstance.Str <- "New"  //this doesn't work 

