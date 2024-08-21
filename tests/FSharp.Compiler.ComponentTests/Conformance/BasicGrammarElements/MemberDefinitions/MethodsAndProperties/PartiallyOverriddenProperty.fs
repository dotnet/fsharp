module MyLib

type BaseType() =
    abstract Msg : string with get,set
    default this.Msg 
        with get() = ""            
        and  set x = printfn "%s" x

type DerivedType() =
    inherit BaseType()
    override this.Msg with get() = "getterOnly"

let d = new DerivedType()
d.Msg <- "" //invoking setter
printfn "%s" d.Msg //invoking getter
