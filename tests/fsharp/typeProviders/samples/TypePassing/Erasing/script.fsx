#r "../../../../../../Debug/net40/bin/type_passing_tp.dll"

open FSharp.Reflection
open Test

type MyRecord = 
    { Id: string }
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2

type MyUnion = 
    | A of int
    | B of string
    member x.TestInstanceProperty = 1
    member x.TestInstanceMethod(y:string) = y
    static member TestStaticMethod(y:string) = y
    static member TestStaticProperty = 2


let mutable failures = []

let check nm v1 v2 = 
    if v1 = v2 then printfn "%s: PASSED" nm
    else 
        failures <- failures @ [nm]
        printfn "\n*** %s: FAILED, expected %A, got %A\n" nm v2 v1

let inaccurate nm v1 v2 v3 = 
    if v1 = v2 then printfn "%s: PASSED (was failing, now passing)" nm
    elif v1 = v3 then printfn "%s: PASSED (inaccurate), expected %A, allowing %A" nm v2 v1
    else 
        failures <- failures @ [nm]
        printfn "\n*** %s: FAILED, expected %A, got %A, would have accepted %A\n" nm v2 v1 v3

// Check an F# record type from this assembly
module MyRecord = 
    let T = typeof<MyRecord>
    type S = TypePassing.Summarize<MyRecord>
    check "cnkewcwpo1" S.Name T.Name
    inaccurate "cnkewcwpo2" S.Assembly_DefinedTypes_Count (Seq.length T.Assembly.DefinedTypes) 0  // INACCURACY: this is wrong value, not sure why
    inaccurate "cnkewcwpo3" S.Assembly_FullName T.FullName "script" // INACCURACY: the full name is not returned
    check "cnkewcwpo3" S.IsAbstract T.IsAbstract
    check "cnkewcwpo3" S.IsAnsiClass T.IsAnsiClass
    check "cnkewcwpo3" S.IsArray T.IsArray
    check "cnkewcwpo4" S.IsClass T.IsClass
    inaccurate "cnkewcwpo5a" S.IsPublic T.IsPublic true // INACCURACY: This should report "false", and IsNestedPublic should report "true"
    inaccurate "cnkewcwpo5b" S.IsNestedPublic T.IsNestedPublic false // INACCURACY: This should report "true", and IsPublic should report "false"
    check "cnkewcwpo6" S.IsNotPublic T.IsNotPublic
    check "cnkewcwpo7" S.IsValueType T.IsValueType
    check "cnkewcwpo8" S.IsInterface T.IsInterface
    inaccurate "cnkewcwpo9" S.IsRecord (FSharpType.IsRecord(T)) false // INACCURACY:  Getting FSharp.Core reflection to give the right answer here is a  tricky as it looks for attributes that aren't in the TAST
    check "cnkewcwpo10" S.IsFunction (FSharpType.IsFunction(T))
    check "cnkewcwpo11" S.IsModule (FSharpType.IsModule(T))
    check "cnkewcwpo12" S.IsExceptionRepresentation (FSharpType.IsExceptionRepresentation(T))
    check "cnkewcwpo13" S.IsTuple (FSharpType.IsTuple(T))
    check "cnkewcwpo14" S.IsUnion (FSharpType.IsUnion(T))
    inaccurate "cnkewcwpo15" S.GetPublicProperties_Length (T.GetProperties().Length) 2 // INACCURACY: this should also report the properties for the F# record fields (which are not in the TAST unfortunately)
    inaccurate "cnkewcwpo16" S.GetPublicConstructors_Length (T.GetConstructors().Length)  0  // INACCURACY: this should also report the constructor for the F# record type
    inaccurate "cnkewcwpo17" S.GetPublicMethods_Length (T.GetMethods().Length)  4 // INACCURACY: like GetProperties, this should report the getter methods for the properties for the F# record fields (which are not in the TAST unfortunately)
#if CURRENTLY_GIVES_COMPILATION_ERROR_NEED_TO_CHECK_IF_EXPECTED
    check "cnkewcwpo18" S.Assembly_EntryPoint_isNull true
    check "cnkewcwpo19" S.GUID ""
    check "cnkewcwpo20" (try S.Assembly_CodeBase; false with _ -> true) true
    check "cnkewcwpo21" S.Assembly_CustomAttributes_Count 0
#endif
   // TODO: rest of System.Type properties and methods
   // TODO: reset of FSharp Reflection methods 


// Check an F# record type from this assembly
module MyUnion = 
    let T = typeof<MyUnion>
    type S = TypePassing.Summarize<MyUnion>
    check "unkewcwpo1" S.Name T.Name
    inaccurate "unkewcwpo2" S.Assembly_DefinedTypes_Count (Seq.length T.Assembly.DefinedTypes) 0  // INACCURACY: this is wrong value, not sure why
    inaccurate "unkewcwpo3" S.Assembly_FullName T.FullName "script" // INACCURACY: the full name is not returned
    inaccurate "unkewcwpo3a" S.IsAbstract T.IsAbstract false  // INACCURACY: reports "false", but "true" is expected for the base class of a union type (but depends on representation of union etc.)
    inaccurate "unkewcwpo3b" S.IsAnsiClass T.IsAnsiClass true // INACCURACY: reports "true", but "false" is expected, not at all important though
    check "unkewcwpo3c" S.IsArray T.IsArray
    check "unkewcwpo4" S.IsClass T.IsClass
    inaccurate "unkewcwpo5a" S.IsPublic T.IsPublic true // INACCURACY: This should report "false", and IsNestedPublic should report "true"
    inaccurate "unkewcwpo5b" S.IsNestedPublic T.IsNestedPublic false // INACCURACY: This should report "true", and IsPublic should report "false"
    check "unkewcwpo6" S.IsNotPublic T.IsNotPublic
    check "unkewcwpo7" S.IsValueType T.IsValueType
    check "unkewcwpo8" S.IsInterface T.IsInterface
    check "unkewcwpo9" S.IsRecord (FSharpType.IsRecord(T)) 
    check "unkewcwpo10" S.IsFunction (FSharpType.IsFunction(T))
    check "unkewcwpo11" S.IsModule (FSharpType.IsModule(T))
    check "unkewcwpo12" S.IsExceptionRepresentation (FSharpType.IsExceptionRepresentation(T))
    check "unkewcwpo13" S.IsTuple (FSharpType.IsTuple(T))
    inaccurate "unkewcwpo14" S.IsUnion (FSharpType.IsUnion(T)) false // INACCURACY:  Getting FSharp.Core reflection to give the right answer here is a  tricky as it looks for attributes that aren't in the TAST
    inaccurate "unkewcwpo15" S.GetPublicProperties_Length (T.GetProperties().Length) 2 // INACCURACY: this should also report the properties for the F# record fields (which are not in the TAST unfortunately)
    check "unkewcwpo16" S.GetPublicConstructors_Length (T.GetConstructors().Length)  0  
    inaccurate "unkewcwpo17" S.GetPublicMethods_Length (T.GetMethods().Length)  4 // INACCURACY: like GetProperties, this should report the getter methods for the properties for the F# record fields (which are not in the TAST unfortunately)
#if CURRENTLY_GIVES_COMPILATION_ERROR_NEED_TO_CHECK_IF_EXPECTED
    check "unkewcwpo18" S.Assembly_EntryPoint_isNull true
    check "unkewcwpo19" S.GUID ""
    check "unkewcwpo20" (try S.Assembly_CodeBase; false with _ -> true) true
    check "unkewcwpo21" S.Assembly_CustomAttributes_Count 0
#endif
   // TODO: rest of System.Type properties and methods
   // TODO: reset of FSharp Reflection methods 



// Check a .NET interface from a system assembly
module IComparable = 
    let T = typeof<System.IComparable>
    type S = TypePassing.Summarize<System.IComparable>
    check "inkewcwpo1i1" S.Name T.Name
    check "inkewcwpo1i2" S.FullName T.FullName
    check "inkewcwpo1i3" S.IsInterface T.IsInterface
    check "inkewcwpo1i4" S.IsClass T.IsClass
    check "inkewcwpo1i4" S.IsValueType T.IsValueType
    check "inkewcwpo2i5" S.Assembly_FullName T.Assembly.FullName

#if CURRENTLY_GIVES_COMPILATION_ERROR_NEED_TO_CHECK_IF_EXPECTED
    check "inkewcwpo3i" S.Assembly_DefinedTypes_Count 0
    check "inkewcwpo4i" S.Assembly_EntryPoint_isNull true
    check "inkewcwpo5i" S.GUID ""
    check "inkewcwpo6i" (try S.Assembly_CodeBase; false with _ -> true) true
    check "inkewcwpo7i" S.Assembly_CustomAttributes_Count 0
#endif



// Check a .NET primitive struct type from a system assembly
module Int32 = 
    let T = typeof<System.Int32>
    type S = TypePassing.Summarize<System.Int32>
    check "vnkewcwpo1v" S.Name T.Name
    check "vnkewcwpo2v" S.FullName T.FullName
    check "vnkewcwpo1i3" S.IsInterface T.IsInterface
    check "vnkewcwpo1i4" S.IsClass T.IsClass
    check "vnkewcwpo1i4" S.IsValueType T.IsValueType
    check "vnkewcwpo3v" S.Assembly_FullName T.Assembly.FullName

#if CURRENTLY_GIVES_COMPILATION_ERROR_NEED_TO_CHECK_IF_EXPECTED
    check "vnkewcwpo4v" S.Assembly_DefinedTypes_Count 0
#endif

module int32_abbreviation = 
    let T = typeof<int32>
    type S = TypePassing.Summarize<int32>
    check "ankewcwpo1" S.Name T.Name
    check "ankewcwpo1" S.FullName T.FullName
    check "ankewcwpo1i3" S.IsInterface T.IsInterface
    check "ankewcwpo1i4" S.IsClass T.IsClass
    check "ankewcwpo1i4" S.IsValueType T.IsValueType
    check "ankewcwpo3" S.Assembly_FullName T.Assembly.FullName

#if CURRENTLY_GIVES_COMPILATION_ERROR_NEED_TO_CHECK_IF_EXPECTED
    check "ankewcwpo3" S.Assembly_DefinedTypes_Count 0
#endif


if failures.Length > 0 then 
    printfn "FAILURES: %A" failures
    exit 1
else   
    printfn "TEST PASSED (with some inaccuracies)"
    exit 0
