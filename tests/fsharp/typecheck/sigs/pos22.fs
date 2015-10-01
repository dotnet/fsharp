// https://github.com/Microsoft/visualfsharp/issues/583

let rec recValNeverUsedAtRuntime = recFuncIgnoresFirstArg (fun _ -> recValNeverUsedAtRuntime) 1
and recFuncIgnoresFirstArg g v = v