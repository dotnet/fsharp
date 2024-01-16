// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments 

namespace ConsumeFromCS

open System.Runtime.InteropServices
open System

type Class() =
    // check that all known supported primitive types work.
    static member Method1  ([<Optional;DefaultParameterValue(42y)>]i:sbyte) = i
    static member Method2  ([<Optional;DefaultParameterValue(42uy)>]i:byte) = i
    static member Method3  ([<Optional;DefaultParameterValue(42s)>]i:int16) = i
    static member Method4  ([<Optional;DefaultParameterValue(42us)>]i:uint16) = i
    static member Method5  ([<Optional;DefaultParameterValue(42)>]i:int32) = i
    static member Method6  ([<Optional;DefaultParameterValue(42u)>]i:uint32) = i
    static member Method7  ([<Optional;DefaultParameterValue(42L)>]i:int64) = i
    static member Method8  ([<Optional;DefaultParameterValue(42UL)>]i:uint64) = i
    static member Method9  ([<Optional;DefaultParameterValue(42.42f)>]i:single) = i
    static member Method10 ([<Optional;DefaultParameterValue(42.42)>]i:double) = i
    static member Method11 ([<Optional;DefaultParameterValue(true)>]i:bool) = i
    static member Method12 ([<Optional;DefaultParameterValue('q')>]i:char) = i
    static member Method13 ([<Optional;DefaultParameterValue("ono")>]i:string) = i
    // Check a reference type - only null possible.
    static member Method14 ([<Optional;DefaultParameterValue(null:obj)>]i:obj) = i
    // Check a value type - only default ctor possible.
    static member Method15 ([<Optional;DefaultParameterValue(new DateTime())>]i:DateTime) = i

    // Check nullables. 
    static member MethodNullable1 ([<Optional;DefaultParameterValue(Nullable<int>())>]i:Nullable<int>) = i
    static member MethodNullable2 ([<Optional;DefaultParameterValue(Nullable<bool>())>]i:Nullable<bool>) = i
    static member MethodNullable3 ([<Optional;DefaultParameterValue(Nullable<DateTime>())>]i:Nullable<DateTime>) = i

    // Sanity checks with a mix of optional/non-optional parameters.
    static member Mix1(a:int, b:string, [<Optional;DefaultParameterValue(-12)>]c:int) = c
    // can omit optional in the middle of the arg list; this works in C# too.
    static member Mix2(a:int, b:string, [<Optional;DefaultParameterValue(-12)>]c:int, d: int) = c
    static member Mix3(a:int, [<Optional;DefaultParameterValue("str")>]b:string, 
                       [<Optional;DefaultParameterValue(-12)>]c:int, [<Optional;DefaultParameterValue(-123)>]d: int) = (b,c,d)

    // compiler should be able to figure out default to pass to Optional parameters without DefaultPaarameterValue.
    static member Optional1([<Optional>]a: int) = a
    static member Optional2([<Optional>]a: obj) = a
    static member Optional3([<Optional>]a: DateTime) = a
    static member Optional4([<Optional>]a: Nullable<int>) = a
    static member Optional5([<Optional>]a: string) = a

    // Insanity checks - basically make sure the compiler does not crash.
    static member OnlyDefault([<DefaultParameterValueAttribute(1)>]a: int) = a