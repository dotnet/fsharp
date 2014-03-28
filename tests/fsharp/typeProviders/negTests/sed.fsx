open System

let fromPatt = fsi.CommandLineArgs.[1]
let toPatt = fsi.CommandLineArgs.[2]

let s = Console.In.ReadToEnd()
let s' = s.Replace(fromPatt, toPatt)
Console.Out.Write(s')