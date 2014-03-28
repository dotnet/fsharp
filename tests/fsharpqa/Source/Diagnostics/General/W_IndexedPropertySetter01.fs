// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1185
//<Expects id="FS0191" span="(8,18-8,19)" status="warning">indexed property setters should take their arguments in curried form, e\.g\. 'member x\.P with set idx v = \.\.\.'\. Setter properties with tuple type should take their arguments in uncurried form, e\.g\. 'member x\.P with set \(\(idx,v\)\) = \.\.\.'</Expects>

#light

type T =
     member this.X
       with set (indexerParam:int,v:int) = ()
     
exit 0
