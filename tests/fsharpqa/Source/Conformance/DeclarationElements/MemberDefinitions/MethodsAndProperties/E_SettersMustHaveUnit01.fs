// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Verify property setters must have type unit
//<Expects id="FS1129" status="error" span="(10,67)">The type 'unit' does not contain a field 'immutStr'</Expects>

type immut =       
  {
     immutStr:       string ;  
  }
with 
  member x.Str       with get() = x.immutStr and set(v:string) = {immutStr = v}

let oldtInstance = {immutStr = "Old"}
let newInstance  = oldtInstance.Str <- "New"  //this doesn't work 

exit 1
