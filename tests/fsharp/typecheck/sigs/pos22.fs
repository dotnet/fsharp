// https://github.com/Microsoft/visualfsharp/issues/583

let rec recValNeverUsedAtRuntime = recFuncIgnoresFirstArg (fun _ -> recValNeverUsedAtRuntime) 1
and recFuncIgnoresFirstArg g v = v

// https://github.com/Microsoft/visualfsharp/issues/671
type Base() =
    abstract member Method : 't -> unit
    default this.Method(t) = ()

and Derived() =
    inherit Base()
    override this.Method(t) = base.Method(t)