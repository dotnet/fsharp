[<AutoOpen>]
module CoreClrUtilities

    open System
    open System.Reflection
    open System.Runtime.InteropServices

    type System.Delegate with
        static member CreateDelegate(delegateType, methodInfo : System.Reflection.MethodInfo) = methodInfo.CreateDelegate(delegateType)
        static member CreateDelegate(delegateType, obj : obj, methodInfo : System.Reflection.MethodInfo) = methodInfo.CreateDelegate(delegateType, obj)            

#if !INTERACTIVE
    module internal UnsafeNativeMethods =
        [<DllImport("kernel32.dll")>]
        extern System.IntPtr GetCommandLine();
#endif

#if !INTERACTIVE
    type System.Environment with 
        static member GetCommandLineArgs() = 
            let cl = 
                let c = UnsafeNativeMethods.GetCommandLine()
                if c = IntPtr.Zero then "" 
                else Marshal.PtrToStringUni(c)
            cl.Split(' ')
#endif

    let commit (results : _[]) = 
        match results with
        | [||] -> null
        | [| m |] -> m
        | _ -> raise (AmbiguousMatchException())

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

        
    let inline hasFlag (flag : BindingFlags) f  = (f &&& flag) = flag
    let isDeclaredFlag  f    = hasFlag BindingFlags.DeclaredOnly f
    let isPublicFlag    f    = hasFlag BindingFlags.Public f
    let isStaticFlag    f    = hasFlag BindingFlags.Static f
    let isInstanceFlag  f    = hasFlag BindingFlags.Instance f
    let isNonPublicFlag f    = hasFlag BindingFlags.NonPublic f

    let isAcceptable bindingFlags isStatic isPublic =
        // 1. check if member kind (static\instance) was specified in flags
        ((isStaticFlag bindingFlags && isStatic) || (isInstanceFlag bindingFlags && not isStatic)) && 
        // 2. check if member accessibility was specified in flags
        ((isPublicFlag bindingFlags && isPublic) || (isNonPublicFlag bindingFlags && not isPublic))

    let publicFlags = BindingFlags.Public ||| BindingFlags.Instance ||| BindingFlags.Static


    type System.Reflection.MemberInfo with
        member this.GetCustomAttributes(inherits:bool) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, inherits) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy:Type) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits) |> Seq.toArray)
        member this.MemberType = mapMemberType this

    type System.Reflection.MethodInfo with
        member this.GetCustomAttributes(inherits:bool) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, inherits) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy:Type) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits) |> Seq.toArray)
        member this.MemberType = mapMemberType this

    type System.Reflection.PropertyInfo with
        member this.GetCustomAttributes(inherits:bool) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, inherits) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy:Type) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy, inherits) |> Seq.toArray)
        member this.MemberType = mapMemberType this

    type System.Reflection.Assembly with
        member this.GetCustomAttributes() : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this) |> Seq.toArray)
        member this.GetCustomAttributes(attrTy, _inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this, attrTy) |> Seq.toArray)
        member this.GetTypes() = 
            this.DefinedTypes 
            |> Seq.map (fun ti -> ti.AsType())
            |> Seq.toArray

    type System.Threading.Thread with 
        member this.CurrentCulture
            with get () = System.Globalization.CultureInfo.CurrentCulture
            and set culture = System.Globalization.CultureInfo.CurrentCulture <-  culture

    type System.Type with

        member this.Assembly = this.GetTypeInfo().Assembly
        // use different sources based on Declared flag
        member this.GetConstructor(_bindingFlags, _binder, argsT:Type[], _parameterModifiers) = this.GetConstructor(argsT)
        member this.GetConstructor(parameterTypes : Type[]) = 
            this.GetTypeInfo().DeclaredConstructors
            |> Seq.filter (fun ci ->
                not ci.IsStatic && //exclude type initializer
                (
                    let parameters = ci.GetParameters()
                    (parameters.Length = parameterTypes.Length) &&
                    (parameterTypes, parameters) ||> Array.forall2 (fun ty pi -> pi.ParameterType.Equals ty) 
                )
            )
            |> Seq.toArray
            |> commit
        member this.GetCustomAttributes(attrTy, inherits) : obj[] = downcast box(CustomAttributeExtensions.GetCustomAttributes(this.GetTypeInfo(), attrTy, inherits) |> Seq.toArray)
        member this.GetGenericParameterConstraints() = this.GetTypeInfo().GetGenericParameterConstraints()
        member this.GetMethods(bindingFlags) =
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredMethods else this.GetRuntimeMethods())
            |> Seq.filter (fun m -> isAcceptable bindingFlags m.IsStatic m.IsPublic)
            |> Seq.toArray
        member this.GetMethods() = this.GetMethods(publicFlags)
        member this.GetMethod(name, ?bindingFlags) =
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetMethods(bindingFlags)
            |> Array.filter(fun m -> m.Name = name)
            |> commit
        member this.GetMethod(name, _bindingFlags, _binder, argsT:Type[], _parameterModifiers) =
            this.GetMethod(name, argsT)
        member this.GetProperties(?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            (if isDeclaredFlag bindingFlags then this.GetTypeInfo().DeclaredProperties else this.GetRuntimeProperties())
            |> Seq.filter (fun pi-> 
                let mi = match pi.GetMethod with | null -> pi.SetMethod | _ -> pi.GetMethod
                if mi = null then false
                else isAcceptable bindingFlags mi.IsStatic mi.IsPublic
                )
            |> Seq.toArray
        member this.GetProperty(name, ?bindingFlags) = 
            let bindingFlags = defaultArg bindingFlags publicFlags
            this.GetProperties(bindingFlags)
            |> Array.filter (fun pi -> pi.Name = name)
            |> commit
        member this.IsGenericType = this.GetTypeInfo().IsGenericType
        member this.IsValueType = this.GetTypeInfo().IsValueType
