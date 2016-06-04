// #Regression #Conformance #DeclarationElements #PInvoke 
// Regression for Dev11:25538, we used to only work with StdCall
// We need to use different C libraries for x64 and x86, hence the ifdefs

// copy of CallingConventions01 but with record structs

open System
open System.Runtime.InteropServices

[<Struct;StructLayout(LayoutKind.Sequential)>]
type ComplexStd =
    {
        mutable re:double
        mutable im:double
    }

module InteropWithNative =
#if AMD64
    [<DllImport(@"CallingConvTestLib_x64", CallingConvention = CallingConvention.StdCall)>]
#else
    [<DllImport(@"CallingConvTestLib_x86", CallingConvention = CallingConvention.StdCall)>]
#endif
    extern IntPtr CreateComplexStd(double, double)

let test1() =
    let addr = InteropWithNative.CreateComplexStd(2.0, 8.0)
    let c = Marshal.PtrToStructure(addr, typeof<ComplexStd>) :?> ComplexStd
    if (c.im <> 8.000000 || c.re <> 2.000000) then exit 1

[<Struct;StructLayout(LayoutKind.Sequential)>]
type ComplexCDecl =
    {
        mutable re:double
        mutable im:double
    }

module InteropWithNative2 =
#if AMD64
    [<DllImport(@"CallingConvTestLib_x64", CallingConvention = CallingConvention.Cdecl)>]
#else
    [<DllImport(@"CallingConvTestLib_x86", CallingConvention = CallingConvention.Cdecl)>]
#endif
    extern IntPtr CreateComplexCDecl(double, double)

let test2() =
    let addr2 = InteropWithNative2.CreateComplexCDecl(2.0, 8.0)
    let c2 = Marshal.PtrToStructure(addr2, typeof<ComplexCDecl>) :?> ComplexCDecl

    if (c2.im <> 8.000000 || c2.re <> 2.000000) then exit 1

// This just automatically passes if we're in an invalid config 
// ex the x86 case on an x64 machine just doesn't execute the test logic (otherwise we'd get a BadImageFormatException)
if IntPtr.Size = 8 then   // we don't use System.Environment.Is64BitOperatingSystem because it is not in NetFx2.0
#if AMD64
    test1()
    test2()
#else
    printfn "Did nothing"
    ()
#endif
else
#if AMD64
    printfn "Did nothing"
    ()
#else
    test1()
    test2()
#endif

exit 0