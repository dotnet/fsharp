// extension members put in place to fill in gaps in the portable profile, so that test code does not need to be updated
// extensions either call out to hooks, or if the member is very simple, just implement the functionality locally

[<AutoOpen>]
module PortableExtensions

type System.Threading.Thread with 
    static member Sleep(millisecondsTimeout) =
        Hooks.sleep.Invoke(millisecondsTimeout)

type System.Threading.WaitHandle with
    member this.WaitOne(millisecondsTimeout : int, exitContext : bool) =
        this.WaitOne(millisecondsTimeout)

type System.Array with
    static member FindIndex<'a>( arr : 'a array, pred) =
        Array.findIndex pred arr

type System.IO.MemoryStream with
    member this.Close() = ()

type System.IO.Stream with
    member this.Write(str : string) =
        let bytes = System.Text.Encoding.UTF8.GetBytes(str)
        this.Write(bytes, 0, bytes.Length)

    member this.Close() = ()

type System.IO.StreamReader with
    member this.Close() = ()
