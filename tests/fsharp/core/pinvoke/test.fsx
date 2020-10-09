// #Conformance #Interop #PInvoke #Structs 

#if TESTS_AS_APP
module Core_csext
#endif

#nowarn "9"
open System
open System.Runtime.InteropServices
open System.Drawing

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

// We currently build targeting netcoreapp3_1, and will continue to do so through this VS cycle
// We will use this api to see if we are running on a netcore which supports pinvoke / refemit
let definePInvokeMethod =
    typeof<System.Reflection.Emit.TypeBuilder>.GetMethod("DefinePInvokeMethod", [|
        typeof<string>
        typeof<string>
        typeof<string>
        typeof<System.Reflection.MethodAttributes>
        typeof<System.Reflection.CallingConventions>
        typeof<Type>
        typeof<Type[]>
        typeof<Type[]>
        typeof<Type[]>
        typeof<Type[][]>
        typeof<Type[][]>
        typeof<System.Runtime.InteropServices.CallingConvention>
        typeof<System.Runtime.InteropServices.CharSet> |])

let enablePInvokeOnCoreClr = definePInvokeMethod <> null

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

    let doTime () = 
        let sysTime = new MySystemTime()
        GetSystemTime(sysTime);
        printf "The System time is %d/%d/%d %d:%d:%d\n" 
               (int32 sysTime.wDay)
               (int32 sysTime.wMonth )
               (int32 sysTime.wYear )
               (int32 sysTime.wHour )
               (int32 sysTime.wMinute )
               (int32 sysTime.wSecond)

    do if enablePInvokeOnCoreClr then doTime () 


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
    extern [<MarshalAs(UnmanagedType.Bool)>] bool GlobalMemoryStatusEx( [<In; Out>] MEMORYSTATUSEX lpBuffer);

    let main() =
        let mex = new MEMORYSTATUSEX()
        GlobalMemoryStatusEx(mex) |> ignore
        printf "%A\n" mex

    if enablePInvokeOnCoreClr then main()

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

    if enablePInvokeOnCoreClr then main()

(*--------------------*)  

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | messages ->
      printfn "%A" messages
      stdout.WriteLine "Test Failed"
      exit 1
#endif



