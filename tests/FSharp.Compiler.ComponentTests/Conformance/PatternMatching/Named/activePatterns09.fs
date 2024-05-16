// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify AND on active patterns with exceptions

let (_ : exn & Failure _ | _) = exn ()
let ((_ : exn) & (Failure _) | (_)) = exn ()
let (_ : exn & (Failure _) | _) = exn ()
let ((_ : exn) & Failure _ | (_)) = exn ()

exception MyExn

let (_ : exn & MyExn | _) = exn ()
let ((_ : exn) & (MyExn) | (_)) = exn ()
let (_ : exn & (MyExn) | _) = exn ()
let ((_ : exn) & MyExn | (_)) = exn ()

exit 0
