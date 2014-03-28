// 0=32bit runtime
// 1=otherwise

let rv = 
    match System.IntPtr.Size with
    | 4 -> 0
    | _ -> 1
System.Console.WriteLine(rv)    
exit rv