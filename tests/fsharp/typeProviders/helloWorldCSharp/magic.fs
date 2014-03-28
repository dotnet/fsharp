
namespace FSharp.TypeMagic 
open System
open System.Text
open System.Reflection
open System.Linq
open System.Collections.Generic
open Microsoft.FSharp.Core.CompilerServices

module Formatting = 
    let genericSomeType = (Some 1).GetType().GetGenericTypeDefinition()
    let rec formatObject (o:obj) = 
        if o = null then "null"
        else
            let typ = o.GetType()
            if typ.IsGenericType then 
                let gt = typ.GetGenericTypeDefinition()
                if genericSomeType = gt then 
                    let getter = typ.GetProperty("Value").GetGetMethod()
                    let result = getter.Invoke(o,[||])
                    formatObject result             
                else
                    o.ToString()
            else 
                o.ToString()


type ReifiedInstance(obj:obj, hostedType:Type) = 
    member __.GetReifiedType() = hostedType
    member __.GetObject() = obj
    override __.ToString() =
        if obj = null then
            "null:" + hostedType.Name
        else 
            let sb = StringBuilder()
            for prop in hostedType.GetProperties() do
                if prop.CanRead then
                    let mi = prop.GetGetMethod()
                    let result = Formatting.formatObject (mi.Invoke(obj,[||]))
                    if sb.Length = 0 then
                        sb.Append("{") |> ignore
                    else
                        sb.Append(", ") |> ignore
                    sb.AppendFormat("{0}={1}", prop.Name, result) |> ignore
            if sb.Length <> 0 then sb.Append("}")|>ignore                  
            (sb.ToString()) + ":" + hostedType.Name

type ITypeStatistics =
    abstract CallsToGetMembers : int

type ConstructorImpl(declaringType, ?getCustomAttributes, ?invoke : obj*BindingFlags*Binder*obj [] *Globalization.CultureInfo -> obj) = 
    inherit ConstructorInfo()

    let getCustomAttributes = 
        match getCustomAttributes with
        | Some(getCustomAttributes)->getCustomAttributes
        | None -> fun _ -> [||]

    override this.Invoke(invokeAttr: BindingFlags, binder: Binder, parameters: obj [], culture: Globalization.CultureInfo) : obj = 
        match invoke with 
        | Some(invoke) -> invoke(null,invokeAttr,binder,parameters,culture)
        | None -> failwith "Invoke is not supported here."
    override this.Invoke(obj:obj, invokeAttr: BindingFlags, binder: Binder, parameters: obj [], culture: Globalization.CultureInfo) : obj = 
        match invoke with 
        | Some(invoke) -> invoke(obj,invokeAttr,binder,parameters,culture)
        | None -> failwith "Invoke is not supported here."
    override this.GetMethodImplementationFlags() = failwith "not implemented GetMethodImplementationFlags"
    override this.GetParameters() = [||]
    override this.MethodHandle = failwith "not implemented MethodHandle"
    override this.Attributes = MethodAttributes.Public ||| MethodAttributes.RTSpecialName
    override this.Name = 
        if this.IsStatic then ".cctor"
        else ".ctor"
    override this.DeclaringType = declaringType
    override this.ReflectedType = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(attributeType, ``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributesData() = getCustomAttributes() :> IList<_>
    override this.IsDefined(attributeType, ``inherit``) = true

[<RequireQualifiedAccess>]
type TypeContainer =
|   Namespace of System.Reflection.Module * string // namespace
|   Type of System.Type

type TypeImpl(container:TypeContainer, 
                       className,
                       baseType,
                       members : Type -> BindingFlags -> MemberTypes -> string option -> MemberInfo array,
                       getCustomAttributes,
                       ?invokeMember,
                       ?attributes,?extras:obj) as this= 
    inherit Type()
    

    // Validate
    do
        match container with
        |   TypeContainer.Namespace (``module``, rootNamespace) ->
                if ``module`` = null then failwith "Null modules not allowed"
                if rootNamespace<>null && rootNamespace.Length=0 then failwith "Use 'null' for global namespace"
        |   TypeContainer.Type _ -> ()
    
    let ``module`` =
        match container with
        |   TypeContainer.Namespace (m, _) -> m
        |   TypeContainer.Type st -> st.Module
    
    // Statistics
    let mutable callsToGetMembers = 0
    let rootNamespace =
        match container with
        |   TypeContainer.Namespace (_,rootNamespace) -> rootNamespace
        |   TypeContainer.Type st -> st.Namespace

    // Other state
    let RootNamespace = rootNamespace
    let ClassName = className

    let defaultInvokeMembers(name: string, invokeAttr: BindingFlags, binder: Binder, target: obj, args: obj [], modifiers: ParameterModifier [], culture: System.Globalization.CultureInfo, namedParameters: string []) =
            let matches = 
                this.GetMethods(invokeAttr) 
                |> Array.filter(fun m->m.Name = name) 
                |> List.ofArray
            match matches with 
            | [oneMatch] -> oneMatch.Invoke(target,args)
            | [] -> failwith (sprintf "Found no method named '%s'." name)
            | _ -> failwith (sprintf "Found multiple overloads of method named '%s'." name)
                    
    let mutable invokeMember = 
        match invokeMember with 
        | Some(invokeMember) -> invokeMember
        | None -> defaultInvokeMembers

    /// Replace the default InvokeMember method.
    member this.ReplaceInvokeMember(newInvokeMember) =        
        invokeMember <- newInvokeMember
       
    // Members of Type and parents
    override this.GUID : Guid= failwith "Not implemented GUID"
    override this.InvokeMember(name: string, invokeAttr: BindingFlags, binder: Binder, target: obj, args: obj [], modifiers: ParameterModifier [], culture: System.Globalization.CultureInfo, namedParameters: string []) : obj =
        invokeMember(name,invokeAttr,binder,target,args,modifiers,culture,namedParameters)
    override this.Assembly : Assembly = ``module``.Assembly
    override this.FullName : string = 
        match container with
        | TypeContainer.Type declaringType -> declaringType.FullName+"+"+ClassName
        | TypeContainer.Namespace (_,``namespace``) -> 
            match ``namespace`` with
            | null -> ClassName
            | _ -> ``namespace``+"."+ClassName
    override this.Namespace : string = RootNamespace
    override this.AssemblyQualifiedName : string = failwith "Not implemented"
    override this.BaseType : Type = baseType
    override this.GetConstructorImpl(bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : ConstructorInfo = 
        ConstructorImpl(this) :> ConstructorInfo
    override this.GetConstructors(bindingAttr: BindingFlags) : ConstructorInfo []= 
        [| for m in members this bindingAttr MemberTypes.Constructor None do
            if m.MemberType = MemberTypes.Constructor then
                yield downcast m |]
    override this.GetMethodImpl(name: string, bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : MethodInfo= 
        let result = 
            [ for m in members this bindingAttr MemberTypes.Method (Some name) do
                if m.MemberType = MemberTypes.Method && m.Name = name then
                    yield  m ]
        match result with 
        | [] -> null
        | [one] -> downcast one
        | several -> 
            System.Diagnostics.Debug.Assert(false, "TypeBuilder does not yet support overload resolution")
            downcast (List.head several)
    override this.GetMethods(bindingAttr: BindingFlags) : MethodInfo [] = 
        this.GetMembers(bindingAttr) |> Array.filter(fun m->m.MemberType = MemberTypes.Method) |> Array.map(fun m->downcast m)        

    override this.GetField(name, bindingAttr) : FieldInfo = 
        let fields = [| for m in this.GetMembers(bindingAttr) do if m.MemberType = MemberTypes.Field && (name = null || m.Name = name) then yield m |] 
        if fields.Length > 0 then downcast fields.[0] else null

    override this.GetFields(bindingAttr) : FieldInfo[] = 
        [| for m in this.GetMembers(bindingAttr) do if m.MemberType = MemberTypes.Field then yield downcast m |] 

    override this.GetInterface(name, ignoreCase) : Type = this.BaseType.GetInterface(name, ignoreCase)
    override this.GetInterfaces() : Type[] = this.BaseType.GetInterfaces()
    override this.GetEvent(name, bindingAttr) = 
        let events = this.GetMembers(bindingAttr) |> Array.filter(fun m->m.MemberType = MemberTypes.Event && (name = null || m.Name = name)) 
        if events.Length > 0 then downcast events.[0] else null
    override this.GetEvents(bindingAttr) = 
        [| for m in this.GetMembers(bindingAttr) do if m.MemberType = MemberTypes.Event then yield downcast m |]
    
    override this.GetPropertyImpl(name: string, bindingAttr: BindingFlags, binder: Binder, returnType: Type, types: Type [], modifiers: ParameterModifier []) : PropertyInfo = 
        if returnType <> null then failwith "Need to handle specified return type in GetPropertyImpl"
        if types <> null then failwith "Need to handle specified parameter types in GetPropertyImpl"
        if modifiers <> null then failwith "Need to handle specified modifiers in GetPropertyImpl"
        let props = this.GetMembers(bindingAttr) |> Array.filter(fun m->m.MemberType = MemberTypes.Property && (name = null || m.Name = name)) 
        if props.Length > 0 then
            downcast props.[0]
        else
            null
    override this.GetProperties(bindingAttr: BindingFlags) : PropertyInfo []= 
        [| for m in this.GetMembers(bindingAttr) do if m.MemberType = MemberTypes.Property then yield downcast m |]
    override this.GetNestedTypes(bindingAttr: BindingFlags) : Type [] = 
        this.GetMembers(bindingAttr) |> Array.filter(fun m->m.MemberType = MemberTypes.NestedType || 
                                                            // Allow 'fake' nested types that are actually real .NET types
                                                            m.MemberType = MemberTypes.TypeInfo) |> Array.map(fun m->downcast m)
    override this.GetNestedType(name: string, bindingAttr: BindingFlags) : Type = 
        let nt = this.GetMember(name, MemberTypes.NestedType ||| MemberTypes.TypeInfo, bindingAttr)
        match nt.Length with
        | 0 -> null
        | 1 -> downcast nt.[0]
        | _ -> failwith (sprintf "There is more than one nested type called %s" name)
    override this.GetMembers(bindingAttr: BindingFlags) : MemberInfo [] = 
        let thisMembers = members (downcast this) bindingAttr MemberTypes.All None
        if (bindingAttr &&& BindingFlags.FlattenHierarchy) = BindingFlags.FlattenHierarchy then
            Array.concat [thisMembers;this.BaseType.GetMembers(bindingAttr) |> Array.filter(fun m -> int (m.MemberType &&& MemberTypes.Constructor) = 0 )]
        else thisMembers
    override this.GetAttributeFlagsImpl() : TypeAttributes = 
        let defaultAttributes = 
            (if this.IsNested then TypeAttributes.NestedPublic else TypeAttributes.Public) ||| 
            TypeAttributes.Class ||| 
            TypeAttributes.Sealed |||
            enum (int32 TypeProviderTypeAttributes.IsErased)
        defaultArg attributes defaultAttributes
    override this.IsArrayImpl() : bool = false
    override this.IsByRefImpl() : bool = false
    override this.IsPointerImpl() : bool= false
    override this.IsPrimitiveImpl() : bool= false
    override this.IsCOMObjectImpl() : bool= false
    override this.GetElementType() : Type= failwith "Not implemented GetElementType" 
    override this.HasElementTypeImpl() : bool= false
    override this.UnderlyingSystemType : Type = typeof<System.Type>
    override this.Name : string = ClassName
    override this.GetCustomAttributes(``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(attributeType, ``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributesData() = getCustomAttributes() :> IList<_>
    override this.IsDefined(attributeType: Type, ``inherit``: bool) : bool= failwith "Not implemented IsDefined"        
    override this.Module : Module = ``module``
    override this.DeclaringType = 
        match container with
        | TypeContainer.Type (declaringType)->declaringType
        | TypeContainer.Namespace _ -> null
    override this.MemberType : MemberTypes =
        if this.IsNested then MemberTypes.NestedType else MemberTypes.TypeInfo      
    override this.GetHashCode() = rootNamespace.GetHashCode() ^^^ className.GetHashCode()
    override this.Equals(that:obj) = 
        match that with
        | null -> false
        | :? TypeImpl as ti -> String.CompareOrdinal(this.FullName, ti.FullName) = 0
        | _ -> false
    override this.GetMember(name:string,mt:MemberTypes,bindingAttr:BindingFlags) = 
        let mt = 
            if mt &&& MemberTypes.NestedType = MemberTypes.NestedType then 
                mt ||| MemberTypes.TypeInfo
            else
                mt
        let thisMember : MemberInfo array = 
            members this bindingAttr mt (Some name)
                |> Array.filter(fun m->0<>(int(m.MemberType &&& mt)) && m.Name = name)
        if (bindingAttr &&& BindingFlags.FlattenHierarchy) = BindingFlags.FlattenHierarchy then
            Array.concat [thisMember;this.BaseType.GetMember(name,mt,bindingAttr)]
        else
            thisMember
    override this.GetGenericArguments() = [||]
    override this.ToString() = 
        this.FullName

    member this.Extras = extras
    interface ITypeStatistics with
        override this.CallsToGetMembers = callsToGetMembers        
    
and PropertyInfoImpl(declaringType,propertyName,propertyType,getCustomAttributes,?propertyGetter,?propertySetter) = 
    inherit System.Reflection.PropertyInfo()
    let getCustomAttributes = 
        match getCustomAttributes with
        | Some(getCustomAttributes)->getCustomAttributes
        | None -> fun _ -> [||]
    override this.PropertyType : Type= propertyType
    override this.SetValue(obj: obj, value: obj, invokeAttr: BindingFlags, binder: Binder, index: obj [], culture: Globalization.CultureInfo) : unit= failwith "Not implemented SetValue"
    override this.GetAccessors(nonPublic: bool) : MethodInfo []= failwith "Not implemented GetAccessors"
    override this.GetGetMethod(nonPublic: bool) : MethodInfo = 
        match propertyGetter with
        | Some(propertyGetter) -> propertyGetter
        | None -> null
    override this.GetSetMethod(nonPublic: bool) : MethodInfo = 
        match propertySetter with
        | Some(propertySetter) -> propertySetter
        | None -> null
    override this.GetIndexParameters() : ParameterInfo []= [||]
    override this.Attributes : PropertyAttributes = PropertyAttributes.None
    override this.CanRead : bool= propertyGetter.IsSome
    override this.CanWrite : bool = propertySetter.IsSome
    override this.GetValue(obj: obj, invokeAttr: BindingFlags, binder: Binder, index: obj [], culture: Globalization.CultureInfo) : obj= failwith "Not implemented GetValue"
    override this.Name : string= propertyName
    override this.DeclaringType : Type= declaringType
    override this.ReflectedType : Type= failwith "Not implemented ReflectedType"
    override this.GetCustomAttributes(``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(attributeType, ``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributesData() = getCustomAttributes() :> IList<_>
    override this.IsDefined(attributeType: Type, ``inherit``: bool) : bool= failwith "Not implemented IsDefined"
    override this.MemberType : MemberTypes = MemberTypes.Property
    
and MethodInfoImpl(declaringType,
                             methodName,
                             returnType,
                             getCustomAttributes,
                             attributes,
                             invoke : obj*BindingFlags*Binder*obj [] *Globalization.CultureInfo -> obj,
                             parameters) = 
    inherit System.Reflection.MethodInfo()

    override this.ReturnTypeCustomAttributes= failwith "Not implemented ReturnTypeCustomAttributes"
    override this.GetBaseDefinition() : MethodInfo= failwith "Not implemented GetBaseDefinition"
    override this.GetParameters() : ParameterInfo []= parameters
    override this.GetMethodImplementationFlags() : MethodImplAttributes= failwith "Not implemented GetMethodImplementationFlags"
    override this.MethodHandle : RuntimeMethodHandle= failwith "Not implemented RuntimeMethodHandle"
    override this.Attributes : MethodAttributes = attributes
    override this.Invoke(obj: obj, invokeAttr: BindingFlags, binder: Binder, parameters: obj [], culture: Globalization.CultureInfo) : obj = 
        invoke(obj,invokeAttr,binder,parameters,culture)
    override this.Name : string = methodName
    override this.DeclaringType : Type= declaringType
    override this.ReflectedType : Type= failwith "Not implemented ReflectedType"
    override this.GetCustomAttributes(``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(attributeType, ``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributesData() = getCustomAttributes() :> IList<_>
    override this.IsDefined(attributeType: Type, ``inherit``: bool) : bool=true
    override this.MemberType : MemberTypes = MemberTypes.Method
    override this.CallingConvention = 
        let mutable cc = CallingConventions.Standard
        if not(this.IsStatic) then 
            cc <- cc ||| CallingConventions.HasThis
        cc
    override this.ReturnType = returnType
    override this.ReturnParameter = null 
    override this.ToString() = 
        "Method " + this.Name

type ParameterInfoImpl(name:string,parameterType:Type) = 
    inherit System.Reflection.ParameterInfo()
    override this.Name = name
    override this.ParameterType = parameterType
    
module Flags = 
    let combineWhenOptionTrue optionalFlag append original =
        match optionalFlag with 
        | Some(true) -> original ||| append
        | _ -> original
        
[<Sealed>]
type TypeBuilder = 
        
   
    static member CreateType(container,className,?baseType,?members,?getProperty,?getCustomAttributes,?extras) : Type = 
        let baseType = 
            match baseType with
            | Some(baseType) -> baseType
            | None -> typeof<obj>
       
        let members = 
            match members with 
            | Some(members) -> members 
            | _ -> fun _ _ _ _ -> [||]
                        
        let getCustomAttributes = 
            match getCustomAttributes with
            | Some(getCustomAttributes) -> getCustomAttributes
            | None -> fun _ -> [||]          
        
        let result = TypeImpl(container,className,baseType,members,getCustomAttributes,?extras=extras)
                            
        upcast result                            

    static member CreateSimpleType(container,className,?baseType,?members,?getCustomAttributes) : Type = 
        let members = defaultArg members (lazy [| |])
        let getTypeMembers _declaringType _bindingFlags _memberTypes nameOption : MemberInfo[] = 
            match nameOption with
            | None -> members.Force()
            | Some name -> members.Force() |> Array.filter(fun m->m.Name=name)

        TypeBuilder.CreateType(container, className, ?baseType=baseType, members=getTypeMembers, ?getCustomAttributes=getCustomAttributes)
    
    static member CreateProperty(declaringType,propertyName,propertyType, ?isStatic, ?getInvoke, ?setInvoke, ?getCustomAttributes, ?getterGetCustomAttributes) : PropertyInfo = 
        let attributes = MethodAttributes.Public
        let attributes = attributes |> Flags.combineWhenOptionTrue isStatic MethodAttributes.Static 
        let miGetter = 
            match getInvoke with
            | Some(getInvoke) -> 
                let attributes = attributes ||| MethodAttributes.SpecialName
                match getterGetCustomAttributes with
                | Some(getCustomAttributes) -> Some(TypeBuilder.CreateMethod(declaringType, "get_"+propertyName, propertyType, attributes = attributes, invoke=getInvoke, getCustomAttributes=getCustomAttributes, ?isStatic=isStatic))
                | None -> Some(TypeBuilder.CreateMethod(declaringType, "get_"+propertyName, propertyType, attributes = attributes, invoke=getInvoke, ?isStatic=isStatic))
            | None -> None
        let miSetter = 
            match setInvoke with
            | Some(setInvoke) -> 
                let attributes = attributes ||| MethodAttributes.SpecialName
                Some(TypeBuilder.CreateMethod(declaringType, "set_"+propertyName, 
                                                  typeof<System.Void>, 
                                                  attributes = attributes, 
                                                  parameters=[|TypeBuilder.CreateParameter("value",propertyType)|],
                                                  invoke=setInvoke, 
                                                  ?isStatic=isStatic))
            | None -> None
        match miGetter,miSetter with
        | Some(miGetter),None-> upcast PropertyInfoImpl(declaringType,propertyName, propertyType, propertyGetter=miGetter,getCustomAttributes=getCustomAttributes)
        | None,Some(miSetter)-> upcast PropertyInfoImpl(declaringType,propertyName, propertyType, propertySetter=miSetter,getCustomAttributes=getCustomAttributes)
        | Some(miGetter),Some(miSetter)-> upcast PropertyInfoImpl(declaringType,propertyName, propertyType, propertyGetter=miGetter, propertySetter=miSetter,getCustomAttributes=getCustomAttributes)
        | None,None -> failwith "Property must have a getter or setter"

    // A synthetic property is one without callable "invoke" methods
    static member CreateSyntheticProperty(declaringType, propertyName, propertyType, ?isStatic,?getter,?setter, ?getCustomAttributes, ?getterGetCustomAttributes) = 
        let getter = defaultArg getter true
        let setter = defaultArg setter false
        if not getter && not setter then invalidArg "getter" "property must have either a getter or a setter"
        let dummyGetInvoke(_this,_bindingFlags,_binder,_,_cultureInfo) : obj = failwith "never called 2"

        TypeBuilder.CreateProperty(declaringType,propertyName,propertyType,?isStatic=isStatic,?getInvoke=(if getter then Some dummyGetInvoke else None), ?setInvoke=(if setter then Some dummyGetInvoke else None), ?getCustomAttributes=getCustomAttributes, ?getterGetCustomAttributes=getterGetCustomAttributes) 
    
    static member CreateMethod(declaringType,methodName,returnType,?getCustomAttributes,?attributes,?invoke,?parameters,?isStatic) : MethodInfo = 
        let attributes = 
            match attributes with 
            | Some(attributes) -> attributes 
            | None -> MethodAttributes.Public 
        let attributes = attributes |> Flags.combineWhenOptionTrue isStatic MethodAttributes.Static             

        let getCustomAttributes = 
            match getCustomAttributes with
            | Some(getCustomAttributes)->getCustomAttributes
            | None -> fun _ -> [||]    

        let invoke = 
            match invoke with
            | Some(invoke)->invoke
            | None -> fun _ -> failwith "No invoke for this method."             
            
        let parameters = 
            match parameters with 
            | Some(parameters) -> parameters 
            | None -> [||]
            
        upcast MethodInfoImpl(declaringType,methodName,returnType,getCustomAttributes,attributes,invoke,parameters)


    // A synthetic method is one without a callable "invoke" method
    static member CreateSyntheticMethod(declaringType,methodName,returnType,?getCustomAttributes,?attributes,?invoke,?parameters,?isStatic) : MethodInfo = 
        let dummyInvoke(_this,_bindingFlags,_binder,_,_cultureInfo) : obj = failwith "never called 2"
        TypeBuilder.CreateMethod(declaringType,methodName,returnType,?isStatic=isStatic,invoke=dummyInvoke, ?attributes=attributes, ?parameters=parameters, ?getCustomAttributes=getCustomAttributes) 

    static member CreateParameter(name:string,parameterType:Type) = upcast ParameterInfoImpl(name,parameterType)
    static member CreateConstructor(declaringType, getCustomAttributes) : ConstructorInfo = upcast ConstructorImpl(declaringType,getCustomAttributes=getCustomAttributes)
    
    static member MemberInfosOfProperty(property:PropertyInfo) =
        [ if property.CanRead then yield property.GetGetMethod() :> MemberInfo
          if property.CanWrite then yield property.GetSetMethod() :> MemberInfo
          yield property :> MemberInfo ]


    static member JoinPropertiesIntoMemberInfos(properties:PropertyInfo seq) =
        // CHANGE TO THIS (though it reorders - does that matter?)
        //    [| for property in properties do yield! MemberInfosOfProperty property |]
        
        let properties = properties |> List.ofSeq
        let accessMethods : MemberInfo list = [for property in properties do 
                                                if property.CanRead then yield property.GetGetMethod() :> MemberInfo
                                                if property.CanWrite then yield property.GetSetMethod() :> MemberInfo] 

        let properties : MemberInfo list = properties |> List.map(fun f->upcast f)
        [accessMethods;properties] |> List.concat |> List.toArray

    static member CacheMembers f = 
        let cache = ref None
        fun this _bindingAttr _kind _nmOpt ->
            match !cache with 
            | Some res -> res
            | None -> let res = f this in cache := Some res; res

    static member CacheWithArg f = 
        let cache = ref None
        fun x ->
            match !cache with 
            | Some res -> res
            | None -> let res = f x in cache := Some res; res

