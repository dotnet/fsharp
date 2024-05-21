// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify AND on incomplete active patterns with exceptions

let (_ : exn & Failure _) = exn ()
let ((_ : exn) & Failure _) = exn ()
let (_ : exn & (Failure _)) = exn ()
let ((_ : exn) & (Failure _)) = exn ()

exception MyExn

let (_ : exn & MyExn) = exn ()
let ((_ : exn) & MyExn) = exn ()
let (_ : exn & (MyExn)) = exn ()
let ((_ : exn) & (MyExn)) = exn ()

exit 0
