// #Warnings
//<Expects status="Error" span="(10,16)" id="FS0768">The member 'Function' does not accept the correct number of arguments.</Expects>
//<Expects>A tuple type is required for one or more arguments</Expects>

type IInterface =
    abstract Function : (int32 * int32) -> unit

let x = 
  { new IInterface with
        member this.Function (i, j) = ()
  }
    
exit 0