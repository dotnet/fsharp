open System
open WbemScripting

let doStuff() =
    let locator =
            let comTy = Type.GetTypeFromProgID("WbemScripting.SWbemLocator")
            Activator.CreateInstance(comTy) :?> SWbemLocator

    // SWBemLocator.ConnectServer https://msdn.microsoft.com/en-us/library/windows/desktop/aa393720(v=vs.85).aspx
    // SWbemServices ConnectServer(
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strServer = ".",
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strNamespace = "",
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strUser = "",
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strPassword = "",
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strLocale = "",
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strAuthority = "",
    //      [In] int iSecurityFlags = 0,
    //      [IDispatchConstant] [MarshalAs(UnmanagedType.IDispatch)] [In] object objWbemNamedValueSet = null);
    let services = locator.ConnectServer()

    // SWbemServices.ExecQuery https://msdn.microsoft.com/en-us/library/windows/desktop/aa393866(v=vs.85).aspx
    // SWbemObjectSet ExecQuery(
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strQuery,
    //      [MarshalAs(UnmanagedType.BStr)] [In] string strQueryLanguage = "WQL",
    //      [In] int iFlags = 16,
    //      [IDispatchConstant] [MarshalAs(UnmanagedType.IDispatch)] [In] object objWbemNamedValueSet = null);
    let resultSet = services.ExecQuery("select * from Win32_Processor")

    resultSet
    |> Seq.cast<SWbemObject>
    |> Seq.map (fun o -> o.GetObjectText_(0))
    |> Seq.iter (printfn "%s")

doStuff()
