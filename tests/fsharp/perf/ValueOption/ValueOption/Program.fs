open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

type Record = {
  Int: int
  String: string
  Children: Record list
}


[<MemoryDiagnoser>]
type Choose() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int list
    [<DefaultValue>] val mutable recs : Record list

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [1 .. this.N]
      this.recs <- List.init this.N (fun i -> 
        { Int = i
          String = string i
          Children = [ 
            { Int = i
              String = string i
              Children = [] }
          ] }
      )

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> List.choose (fun x -> Some x) |> ignore
      | "record" -> this.recs |> List.choose (fun x -> Some x) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> List.chooseV (fun x -> ValueSome x) |> ignore
      | "record" -> this.recs |> List.chooseV (fun x -> ValueSome x) |> ignore
      | _ -> failwith "Should never happen"


[<MemoryDiagnoser>]
type TryPick() =

    [<Params(10, 1000, 100000)>]
    [<DefaultValue>] val mutable N : int

    [<Params("int", "record")>]
    [<DefaultValue>] val mutable Type : string

    [<DefaultValue>] val mutable ints : int list
    [<DefaultValue>] val mutable recs : Record list

    [<GlobalSetup>]
    member this.Setup () =
      this.ints <- [1 .. this.N]
      this.recs <- List.init this.N (fun i -> 
        { Int = i
          String = string i
          Children = [ 
            { Int = i
              String = string i
              Children = [] }
          ] }
      )

    [<Benchmark(Baseline=true)>]
    member this.Option () =
      match this.Type with
      | "int" -> this.ints |> List.tryPick (fun x -> Some x) |> ignore
      | "record" -> this.recs |> List.tryPick (fun x -> Some x) |> ignore
      | _ -> failwith "Should never happen"

    [<Benchmark>]
    member this.ValueOption () =
      match this.Type with
      | "int" -> this.ints |> List.tryPickV (fun x -> ValueSome x) |> ignore
      | "record" -> this.recs |> List.tryPickV (fun x -> ValueSome x) |> ignore
      | _ -> failwith "Should never happen"


[<EntryPoint>]
let main argv =
    let summaries = BenchmarkRunner.Run(typeof<Choose>.Assembly)
    printfn "%A" summaries
    0
