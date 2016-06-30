// #Conformance #Interop #PInvoke #Structs 


#nowarn "9"
open System
open System.Runtime.InteropServices
open System.Windows.Forms
open System.Drawing


[<DllImport("cards.dll")>]
let cdtInit((width: IntPtr), (height: IntPtr)) : unit = ()

let pinned (obj: obj) f = 
  let gch = GCHandle.Alloc(obj,GCHandleType.Pinned) in 
  try f(gch.AddrOfPinnedObject())
  finally
    gch.Free()

//The following types from the System namespace are blittable types: 
//
//System.Byte 
//System.SByte 
//System.Int16 
//System.UInt16 
//System.Int32 
//System.UInt32 
//System.Int64 
//System.IntPtr 
//System.UIntPtr 
//The following complex types are also blittable types: 
//One-dimensional arrays of blittable types, such as an array of integers. 
//Formatted value types that contain only blittable types (and classes if they are marshaled as formatted types). 

//
// assert ((typeof<'a>) == (typeof<int>)  or
//         (typeof<'a>) == (typeof<int64>)  or
// etc.           

type PinBox<'a> = 
    { v : obj }
    static member Create(x) = { v  = box(x) }
    member x.Value = (unbox x.v : 'a)
    member x.Pin(f) = pinned(x.v) f

let card_init () =
  let width = PinBox<_>.Create(300) in
  let height = PinBox<_>.Create(400) in
  width.Pin (fun widthAddress -> 
    height.Pin (fun heightAddress -> 
      cdtInit (widthAddress, heightAddress)));
  Printf.printf "width = %d\n" width.Value;
  Printf.printf "height = %d\n" height.Value;
  ()

do card_init()

let asciiz (pBytes: nativeptr<sbyte>) = new System.String(pBytes)
 
#nowarn "0044";;
#nowarn "0051";;

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

type voidptr = System.IntPtr

//int (*derivs)(double, double [], double [], void *), 
type DerivsFunction = delegate of double * double nativeptr * double nativeptr * voidptr -> int

//int (*outputFn)(double, double*, void*) );
type OutputFunction = delegate of double * double nativeptr * voidptr -> int

[<DllImport("PopDyn.dll")>]
// Wrap the C function with the following signature:
//
extern int SolveODE2(double *ystart, int nvar, double x1, double x2, double eps, double h1,
                     double hmin, double hmax, int *nok, int *nbad, double dx, void *info, 
                     DerivsFunction derivs, 
                     OutputFunction outputFn);
module Array = 
    let inline pinObjUnscoped (obj: obj) =  GCHandle.Alloc(obj,GCHandleType.Pinned) 

    let inline pinObj (obj: obj) f = 
        let gch = pinObjUnscoped obj 
        try f gch
        finally
            gch.Free()
    
    [<NoDynamicInvocation>]
    let inline pin (arr: 'T []) (f : nativeptr<'T> -> 'U) = 
        pinObj (box arr) (fun _ -> f (&&arr.[0]))
    

type NativeArray<'T when 'T : unmanaged>(ptr : nativeptr<'T>, len: int) =
    member x.Ptr = ptr
    [<NoDynamicInvocation>]
    member inline x.Item 
       with get n = NativePtr.get x.Ptr n
       and  set n v = NativePtr.set x.Ptr n v
    member x.Length = len
// Provide a nicer wrapper for use from F# code.  This takes an F# array as input,
// and when the callbacks happen wraps up the returned native arrays in the 
// F# NativeArray thin wrapper which lets you use nice syntax arr.[n] for getting and
// setting values of these arrays.
let solveODE ystart (x1,x2,eps,h1,hmin,hmax) (nok,nbad) dx derivs outputFn = 
    Array.pin ystart (fun ystartAddr -> 
        let nvar = Array.length ystart in
        let mutable nok = nok in 
        let mutable nbad = nbad in 
        let info = 0n in 
        let derivsF  = new DerivsFunction(fun  x arr1 arr2 _ -> derivs x (new NativeArray<_>(arr1,nvar)) (new NativeArray<_>(arr2,nvar))) in 
        let outputFnF = new OutputFunction(fun x pY _ -> outputFn x) in 
        SolveODE2(ystartAddr,nvar,x1,x2,eps,h1,hmin,hmax,&&nok,&&nbad,dx,info,derivsF,outputFnF))

let example1() = 
  solveODE 
     // initial values
     [| 1.0; 2.0 |] 
     // settings
     (1.0,2.0,0.0001,1.0,1.0,1.0) 
     // nok,nbad
     (10,20) 
     // dx
     0.05 
     // Compute the derivatives.  Note outp and inp are both NativeArrays, passed to us from C.
     // So there is no bounds checking on these assignments - be careful!  
     // If it turns out that these arrays are of static known size then we can do better here. 
     (fun x inp outp -> 
         outp.[0] <- inp.[0] + 0.05; 1) 
     // output
     (fun v -> printf "v = %G\n" v; 5)



module GetSystemTimeTest = 
    open System
    open System.Runtime.InteropServices

    [<StructLayout(LayoutKind.Explicit, Size=16, CharSet=CharSet.Ansi)>]
    type MySystemTime = class
       new() = { wYear=0us; wMonth=0us; wDayOfWeek=0us; wDay=0us; wHour=0us; wMinute=0us;wSecond=0us;wMilliseconds=0us }
       [<FieldOffset(0)>] val wYear : uint16; 
       [<FieldOffset(2)>] val wMonth : uint16;
       [<FieldOffset(4)>] val wDayOfWeek : uint16; 
       [<FieldOffset(6)>] val wDay : uint16 ; 
       [<FieldOffset(8)>] val wHour : uint16 ; 
       [<FieldOffset(10)>] val wMinute : uint16 ; 
       [<FieldOffset(12)>] val wSecond : uint16 ; 
       [<FieldOffset(14)>] val wMilliseconds : uint16 ; 
    end

    [<DllImport("kernel32.dll")>]
    extern void GetSystemTime([<MarshalAs(UnmanagedType.LPStruct)>] MySystemTime ct);

    do 
          let sysTime = new MySystemTime()
          GetSystemTime(sysTime);
          printf "The System time is %d/%d/%d %d:%d:%d\n" 
                            (int32 sysTime.wDay)
                            (int32 sysTime.wMonth )
                            (int32 sysTime.wYear )
                            (int32 sysTime.wHour )
                            (int32 sysTime.wMinute )
                            (int32 sysTime.wSecond)


module MemoryStatusTest = 
    open System
    open System.Runtime.InteropServices

    [<StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)>]
    type MEMORYSTATUSEX = class
        val mutable dwLength : uint32
        val dwMemoryLoad : uint32
        val ullTotalPhys : uint64
        val ullAvailPhys : uint64
        val ullTotalPageFile : uint64
        val ullAvailPageFile : uint64
        val ullTotalVirtual : uint64
        val ullAvailVirtual : uint64
        val ullAvailExtendedVirtual : uint64
        new() = {dwLength = Convert.ToUInt32(Marshal.SizeOf(typeof<MEMORYSTATUSEX>));
                 dwMemoryLoad = 0ul;
                 ullTotalPhys = 0UL;
                 ullAvailPhys = 0UL;
                 ullTotalPageFile = 0UL;
                 ullAvailPageFile = 0UL;
                 ullTotalVirtual = 0UL;
                 ullAvailVirtual = 0UL;
                 ullAvailExtendedVirtual  = 0UL}
    end

    [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern [<MarshalAs(UnmanagedType.Bool)>] bool 
          GlobalMemoryStatusEx( [<In; Out>] MEMORYSTATUSEX lpBuffer);

    let main() =
        let mex = new MEMORYSTATUSEX()
        GlobalMemoryStatusEx(mex) |> ignore
        printf "%A\n" mex

    main()


module MemoryStatusTest2 = 
    open System
    open System.Runtime.InteropServices

    [<StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)>]
    type MEMORYSTATUSEX = struct
        val mutable dwLength : uint32
        val dwMemoryLoad : uint32
        val ullTotalPhys : uint64
        val ullAvailPhys : uint64
        val ullTotalPageFile : uint64
        val ullAvailPageFile : uint64
        val ullTotalVirtual : uint64
        val ullAvailVirtual : uint64
        val ullAvailExtendedVirtual : uint64
        new(dummy:int) = {dwLength = Convert.ToUInt32(Marshal.SizeOf(typeof<MEMORYSTATUSEX>));
                 dwMemoryLoad = 0ul;
                 ullTotalPhys = 0UL;
                 ullAvailPhys = 0UL;
                 ullTotalPageFile = 0UL;
                 ullAvailPageFile = 0UL;
                 ullTotalVirtual = 0UL;
                 ullAvailVirtual = 0UL;
                 ullAvailExtendedVirtual  = 0UL}
    end

    [<DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern [<MarshalAs(UnmanagedType.Bool)>] bool 
          GlobalMemoryStatusEx( [<In; Out>] MEMORYSTATUSEX *lpBuffer);

    let main() =
        let mutable mex = new MEMORYSTATUSEX(0)
        GlobalMemoryStatusEx(&& mex) |> ignore
        printf "%A\n" mex

    main()

