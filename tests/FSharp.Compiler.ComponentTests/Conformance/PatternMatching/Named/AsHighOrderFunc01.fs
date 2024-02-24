// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify ability to treat active patterns like higher-order functions

// Single Case
let (|ToString|) (x : obj) = x.ToString()

let sc = (|ToString|)
if sc 42 <> "42" then exit 1

// Multi Case
let rec (|Odd|Even|) x =
    match x with
    | 0 -> Even
    | 1 -> Odd
    | _ -> match x - 1 with
           | Even -> Odd
           | Odd  -> Even

let mc = (|Odd|Even|)

let _ = match mc 3, mc 4 with
        | Choice1Of2(_), Choice2Of2(_) -> ()
        | _ -> exit 1

// Partial
let (|IsThree|_|) x = if x = 3 then Some() else None

let pap = (|IsThree|_|)
if pap 4 <> None   then exit 1
if pap 3 <> Some() then exit 1

exit 0
