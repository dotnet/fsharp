// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:3143

module NonEscapingArguments02
type ListSizeCounter<'t>(somelist: 't list) =    
    let size = List.length somelist     
    member this.Size = size
