// #Conformance #PatternMatching #ActivePatterns 
#light

// Single Case
let rec (|SCAP|) (x : obj) =
    match x with
    | :? string as sx -> sx + "-TERM"
    | x               -> (|SCAP|) (x.ToString() + "-AP")
    
if (|SCAP|) 5 <> "5-AP-TERM" then exit 1
let _ = 
    match "foo" with
    | SCAP "foo-TERM" -> ()
    | _ -> exit 1
    
// Multi-Case
let rec (|MCAP|MCAP2|) (ip : string) =
    match ip with
    | "..." -> MCAP(ip)
    | "!!!" -> MCAP2(ip, ip)
    | "test1" -> (|MCAP|MCAP2|) "..."
    | "test2" -> (|MCAP|MCAP2|) "!!!"
    | _ -> failwith ""
    
let _ = match "test1" with MCAP "..."             -> ()  | _ -> exit 1
let _ = match "test2" with MCAP2 ("!!!", "!!!")   -> ()  | _ -> exit 1

// Partial
let rec (|MultOf3|_|) x =
    match x with
    | _ when x < 0 -> None
    | 0 -> Some()
    | x -> (|MultOf3|_|) (x - 3)

let _ = match 3 with MultOf3 -> () | _ -> exit 1
let _ = match 4 with MultOf3 -> exit 1 | _ -> ()
let _ = match 5 with MultOf3 -> exit 1 | _ -> ()
let _ = match 6 with MultOf3 -> () | _ -> exit 1

exit 0
