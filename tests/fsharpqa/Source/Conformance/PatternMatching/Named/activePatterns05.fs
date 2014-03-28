// #Conformance #PatternMatching #ActivePatterns 
#light

open System

let (|ExceptionMessageLen|) (ex : #Exception) = ex.Message.Length

let mutable exnLength = 0

try
    raise (NotImplementedException "1234567890")
with
    ExceptionMessageLen len -> exnLength <- len

if exnLength <> 10 then exit 1
exit 0
