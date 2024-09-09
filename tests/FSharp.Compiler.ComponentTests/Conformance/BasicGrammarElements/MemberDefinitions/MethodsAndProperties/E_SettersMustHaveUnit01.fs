// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify property setters must have type unit


type immut =       
  {
     immutStr:       string ;  
  }
with 
  member x.Str       with get() = x.immutStr and set(v:string) = {immutStr = v}

let oldtInstance = {immutStr = "Old"}
let newInstance  = oldtInstance.Str <- "New"  //this doesn't work 

