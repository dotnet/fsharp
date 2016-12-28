//<Expects status="success" ></Expects>
open System
open System.Runtime.InteropServices

module Test =

    [<DllImport("shell32.dll", CharSet=CharSet.Auto)>]
    extern int32 ExtractIconEx(string szFileName, int nIconIndex,IntPtr[] phiconLarge, IntPtr[] phiconSmall,uint32 nIcons)

    [<DllImport("user32.dll", EntryPoint="DestroyIcon", SetLastError=true)>]
    extern int DestroyIcon(IntPtr hIcon)

[<EntryPoint>]
let main _argv = 
    let _ = Test.DestroyIcon IntPtr.Zero

    let _ = Test.ExtractIconEx("", 0, [| |], [| |], 0u)

    0