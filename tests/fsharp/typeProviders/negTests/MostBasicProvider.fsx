open System
open System.Reflection

//////////////////////////////////////////////////////

type MostBasicTypeImpl(theModule : Module, rootNamespace, className, baseType) = 
    inherit Type()

    let NYI() = raise <| new NotImplementedException()
    do
        assert(theModule <> null)
        assert(rootNamespace <> null)
        assert(className <> null)
        // baseType might be null

    // Members of Type and parents
    override this.GUID : Guid = 
        NYI()
    override this.InvokeMember(name: string, invokeAttr: BindingFlags, binder: Binder, target: obj, args: obj [], modifiers: ParameterModifier [], 
                               culture: System.Globalization.CultureInfo, namedParameters: string []) : obj =
        NYI()
    override this.Assembly : Assembly = 
        theModule.Assembly
    override this.FullName : string = 
        match rootNamespace with
        | null -> className
        | n -> n + "." + className
    override this.Namespace : string = 
        rootNamespace
    override this.AssemblyQualifiedName : string = 
        NYI()
    override this.BaseType : Type = 
        baseType
    override this.GetConstructorImpl(bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : ConstructorInfo = 
        NYI()
    override this.GetConstructors(bindingAttr: BindingFlags) : ConstructorInfo []= 
        [| |]
    override this.GetMethodImpl(name: string, bindingAttr: BindingFlags, binder: Binder, callConvention: CallingConventions, types: Type[], modifiers: ParameterModifier []) : MethodInfo= 
        null
    override this.GetMethods(bindingAttr: BindingFlags) : MethodInfo [] = 
        [| |]
    override this.GetField(name, bindingAttr) : FieldInfo = 
        null
    override this.GetFields(bindingAttr) : FieldInfo[] = 
        [| |] 
    override this.GetInterface(name, ignoreCase) : Type = 
        this.BaseType.GetInterface(name, ignoreCase)
    override this.GetInterfaces() : Type[] = 
        match this.BaseType with null -> [| |] | _ -> this.BaseType.GetInterfaces()
    override this.GetEvent(name, bindingAttr) = 
        null
    override this.GetEvents(bindingAttr) = 
        [| |]
    override this.GetPropertyImpl(name: string, bindingAttr: BindingFlags, binder: Binder, returnType: Type, types: Type [], modifiers: ParameterModifier []) : PropertyInfo = 
        null
    override this.GetProperties(bindingAttr: BindingFlags) : PropertyInfo []= 
        [| |]
    override this.GetNestedTypes(bindingAttr: BindingFlags) : Type [] = 
        [| |]
    override this.GetNestedType(name: string, bindingAttr: BindingFlags) : Type = 
        null
    override this.GetMembers(bindingAttr: BindingFlags) : MemberInfo [] = 
        [| |]
    override this.GetAttributeFlagsImpl() : TypeAttributes =
        TypeAttributes.Public ||| TypeAttributes.Class ||| TypeAttributes.Sealed
    override this.IsArrayImpl() : bool = 
        false
    override this.IsByRefImpl() : bool = 
        false
    override this.IsPointerImpl() : bool = 
        false
    override this.IsPrimitiveImpl() : bool = 
        false
    override this.IsCOMObjectImpl() : bool = 
        false
    override this.GetElementType() : Type =
        NYI()
    override this.HasElementTypeImpl() : bool = 
        false
    override this.UnderlyingSystemType : Type = 
        typeof<System.Type>
    override this.Name : string = 
        className
    override this.GetCustomAttributes(``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributes(attributeType, ``inherit``) = failwith "not implemented ReflectedType"
    override this.GetCustomAttributesData() = [| |] :> System.Collections.Generic.IList<_>

    override this.IsDefined(attributeType: Type, ``inherit``: bool) : bool = 
        NYI()
    override this.Module : Module = 
        theModule
    override this.DeclaringType : Type = 
        null
    override this.MemberType : MemberTypes =
        if this.IsNested then MemberTypes.NestedType else MemberTypes.TypeInfo      
    override this.GetHashCode() : int = 
        rootNamespace.GetHashCode() ^^^ className.GetHashCode()
    override this.Equals(that:obj) : bool = 
        match that with
        | null -> false
        | :? MostBasicTypeImpl as ti -> String.CompareOrdinal(this.FullName, ti.FullName) = 0
        | _ -> false
    override this.GetMember(name:string,mt:MemberTypes,bindingAttr:BindingFlags) : MemberInfo[] = 
        [| |]
    override this.GetGenericArguments() : Type[] =
        [| |]
    override this.ToString() : string = 
        this.FullName

//////////////////////////////////////////////////////

open Microsoft.FSharp.Core.CompilerServices

[<assembly: TypeProviderAssembly>]
do ()

[<TypeProvider>]
type MostBasicProvider() =
    let modul = typeof<MostBasicProvider>.Assembly.GetModules().[0]
    let namespaceName = "MyProvider"
    let theType : Type = upcast new MostBasicTypeImpl(modul, namespaceName, "TheType", null)
    let types = [|theType|]
    let invalidation = new Event<System.EventHandler,_>()
    interface IProvidedNamespace with
        member this.GetNestedNamespaces() : IProvidedNamespace[] = [| |]
        member this.NamespaceName : string = namespaceName
        member this.GetTypes() : Type[] = types
        member this.ResolveTypeName(s:string) : Type = match s with | "TheType" -> theType | _ -> null
    interface ITypeProvider with
        member this.GetNamespaces() : IProvidedNamespace[] = [| this |]
        member this.GetStaticParameters(typeWithoutArguments:Type) : ParameterInfo[] = [| |]
        member this.ApplyStaticArguments(typeWithoutArguments:Type, typeNameWithArguments:string[], staticArguments:obj[]) : Type = typeWithoutArguments
        member this.GetInvokerExpression(mb,parameters) = failwith "GetInvokerExpression not implemented"
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents - only erased types were provided!!"
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
    interface IDisposable with
        member __.Dispose() = ()
