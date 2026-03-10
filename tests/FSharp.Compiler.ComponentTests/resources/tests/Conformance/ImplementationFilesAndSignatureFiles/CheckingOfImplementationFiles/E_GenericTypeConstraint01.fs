// #Conformance #SignatureFiles 
module M

open System

type Color = 
   | Red = 0
   | Green = 1
   | Blue = 2

module Enum = 
    let fromString str : 'enumType = Enum.Parse(typeof<'enumType>, str, true) |> unbox
    
    let green : Color = fromString "Green"
    
    if green <> Color.Green then exit 1

    exit 0
