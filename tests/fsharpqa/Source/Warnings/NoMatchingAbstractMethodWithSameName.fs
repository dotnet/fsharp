// #Warnings
//<Expects status="Error" span="(11,14)" id="FS0767">The member 'Function' does not correspond to any abstract or virtual method available to override or implement.</Expects>
//<Expects>MyFunction</Expects>

type IInterface =
    abstract MyFunction : int32 * int32 -> unit
    abstract SomeOtherFunction : int32 * int32 -> unit

let x = 
  { new IInterface with
      member this.Function (i, j) = ()
  }

exit 0