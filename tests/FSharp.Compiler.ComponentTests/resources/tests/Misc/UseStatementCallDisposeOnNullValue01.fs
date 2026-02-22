// #Regression #Misc
// Was a regression for 40279 in devdiv2(dev11), bug wasn't fixed so this will catch current behavior.
// Use statement calls Dispose and when bound to a null value will throw NullReferenceException.

let _ =
    try
        let _ =
            use key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("nonexistent")
            ()
        let _ = 
            use key : System.IDisposable = null
            ()
        exit 0
    with
        | :? System.NullReferenceException as nre -> printfn "NullReferenceException caught"
                                                     exit 1