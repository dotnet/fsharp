// #Warnings
//<Expects status="Error" span="(7,15)" id="FS0039">The type 'Type' is not defined in 'B'.</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>PublicType</Expects>

type E() =
    inherit B.Type()
    member x.Y() = ()
    
exit 0