type T = Top.Repro.C
T.Run()

let attrs = typeof<T>.GetMethod("Run").GetCustomAttributes(false)
let attr = attrs.[0] :?> System.ServiceModel.FaultContractAttribute

printfn "%A" (attr.ProtectionLevel.ToString())