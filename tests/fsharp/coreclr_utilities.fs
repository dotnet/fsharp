[<AutoOpen>]
module CoreClrUtilities

    open System
    open System.Reflection
    open System.Runtime.InteropServices

    type System.Delegate with
        static member CreateDelegate(delegateType, methodInfo : System.Reflection.MethodInfo) = methodInfo.CreateDelegate(delegateType)
        static member CreateDelegate(delegateType, obj : obj, methodInfo : System.Reflection.MethodInfo) = methodInfo.CreateDelegate(delegateType, obj)            

    // Completely not portable:  change to Environment.Exit() when netfx implements it for coreclr
    module internal UnsafeNativeMethods =
        [<DllImport("kernel32.dll")>]
        extern void ExitProcess(int _exitCode)

        [<DllImport("kernel32.dll")>]
        extern System.IntPtr GetCommandLine();


    [<CompiledName("Exit")>]
    let exit (exitCode:int) =
        UnsafeNativeMethods.ExitProcess(exitCode); failwith "UnsafeNativeMethods.ExitProcess did not exit!!"; ()

    type System.Environment with 
        static member GetCommandLineArgs() = 
            let cl = 
                let c = UnsafeNativeMethods.GetCommandLine()
                if c = IntPtr.Zero then "" 
                else Marshal.PtrToStringUni(c)
            cl.Split(' ')

    [<Flags>]
    type BindingFlags =
        | DeclaredOnly = 2
        | Instance = 4 
        | Static = 8
        | Public = 16
        | NonPublic = 32

    [<Flags>]
    type MemberType =
        | None        = 0x00
        | Constructor = 0x01
        | Event       = 0x02
        | Field       = 0x04
        | Method      = 0x08
        | Property    = 0x10
        | TypeInfo    = 0x20
        | NestedType  = 0x80


    let mapMemberType (c:System.Reflection.MemberInfo) = 
        let mapIsMethodOrConstructor = 
            match c with 
            | :? System.Reflection.MethodBase as c -> if c.IsConstructor then MemberType.Constructor else MemberType.Method 
            |_ -> MemberType.None

        let mapIsEvent = match c with | :? System.Reflection.EventInfo as c -> MemberType.Event |_ -> MemberType.None
        let mapIsField = match c with | :? System.Reflection.FieldInfo as c -> MemberType.Field |_ -> MemberType.None
        let mapIsProperty = match c with | :? System.Reflection.PropertyInfo as c -> MemberType.Property |_ -> MemberType.None
        let mapIsTypeInfoOrNested = 
            match c with 
            | :? System.Reflection.TypeInfo as c -> if c.IsNested then MemberType.NestedType else MemberType.TypeInfo 
            |_ -> MemberType.None
        mapIsMethodOrConstructor ||| mapIsEvent ||| mapIsField ||| mapIsProperty ||| mapIsTypeInfoOrNested
        
    type System.Reflection.MemberInfo with
        member this.MemberType = mapMemberType this

    type System.Reflection.MethodInfo with
        member this.MemberType = mapMemberType this

    type System.Reflection.PropertyInfo with
        member this.MemberType = mapMemberType this

    type System.Reflection.Assembly with
        member this.GetTypes() = 
            this.DefinedTypes 
            |> Seq.map (fun ti -> ti.AsType())
            |> Seq.toArray

    type System.Threading.Thread with 
        member this.CurrentCulture
            with get () = System.Globalization.CultureInfo.CurrentCulture
            and set culture = System.Globalization.CultureInfo.CurrentCulture <-  culture

