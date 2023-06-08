open System
open System.Reflection
open FSharp.Collections

let attributes = typeof<List<int>>.GetConstructor(BindingFlags.NonPublic ||| BindingFlags.Instance, Unchecked.defaultof<Binder>, Type.EmptyTypes, [||]).GetCustomAttributes()
for a in attributes do Console.WriteLine(a.ToString())