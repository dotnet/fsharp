try
    let ps = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted)
    ps.Demand()
    printfn "You are running in full trust"
    0
with
| :? System.Security.SecurityException -> printfn "You are running in partial trust"
                                         1
| _ -> printfn "Unknown exception!"; 
       2


