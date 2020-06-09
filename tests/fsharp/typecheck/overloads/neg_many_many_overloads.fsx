(**)
let types0 = [
    "int8"
    "int16"
    "int32"
    "int64"
    "uint8"
    "uint16"
    "uint32"
    "uint64"
    "string"
    "decimal"
    "float"
    "float32"
    "DateTime"
    "Task"
    
]

let types = [
  yield! types0
  for t in types0 do
    yield sprintf "Task<%s>" t
    yield sprintf "Dictionary<%s,%s>" t t
    yield sprintf "HashSet<%s>" t
    yield sprintf "List<%s>" t
    yield sprintf "Queue<%s>" t
]

let generate () =
  [ for t in types -> sprintf "    static member A (a:%s) = a" t ]
  |> String.concat "\n"
printfn "%s" (generate ())
(**)
open System
open System.Collections.Generic
open System.Threading.Tasks
type Class() =
    static member A (a:int8) = a
    static member A (a:int16) = a
    static member A (a:int32) = a
    static member A (a:int64) = a
    static member A (a:uint8) = a
    static member A (a:uint16) = a
    static member A (a:uint32) = a
    static member A (a:uint64) = a
    static member A (a:string) = a
    static member A (a:decimal) = a
    static member A (a:float) = a
    static member A (a:float32) = a
    static member A (a:DateTime) = a
    static member A (a:Task) = a
    static member A (a:Task<int8>) = a
    static member A (a:Dictionary<int8,int8>) = a
    static member A (a:HashSet<int8>) = a
    static member A (a:List<int8>) = a
    static member A (a:Queue<int8>) = a
    static member A (a:Task<int16>) = a
    static member A (a:Dictionary<int16,int16>) = a
    static member A (a:HashSet<int16>) = a
    static member A (a:List<int16>) = a
    static member A (a:Queue<int16>) = a
    static member A (a:Task<int32>) = a
    static member A (a:Dictionary<int32,int32>) = a
    static member A (a:HashSet<int32>) = a
    static member A (a:List<int32>) = a
    static member A (a:Queue<int32>) = a
    static member A (a:Task<int64>) = a
    static member A (a:Dictionary<int64,int64>) = a
    static member A (a:HashSet<int64>) = a
    static member A (a:List<int64>) = a
    static member A (a:Queue<int64>) = a
    static member A (a:Task<uint8>) = a
    static member A (a:Dictionary<uint8,uint8>) = a
    static member A (a:HashSet<uint8>) = a
    static member A (a:List<uint8>) = a
    static member A (a:Queue<uint8>) = a
    static member A (a:Task<uint16>) = a
    static member A (a:Dictionary<uint16,uint16>) = a
    static member A (a:HashSet<uint16>) = a
    static member A (a:List<uint16>) = a
    static member A (a:Queue<uint16>) = a
    static member A (a:Task<uint32>) = a
    static member A (a:Dictionary<uint32,uint32>) = a
    static member A (a:HashSet<uint32>) = a
    static member A (a:List<uint32>) = a
    static member A (a:Queue<uint32>) = a
    static member A (a:Task<uint64>) = a
    static member A (a:Dictionary<uint64,uint64>) = a
    static member A (a:HashSet<uint64>) = a
    static member A (a:List<uint64>) = a
    static member A (a:Queue<uint64>) = a
    static member A (a:Task<string>) = a
    static member A (a:Dictionary<string,string>) = a
    static member A (a:HashSet<string>) = a
    static member A (a:List<string>) = a
    static member A (a:Queue<string>) = a
    static member A (a:Task<decimal>) = a
    static member A (a:Dictionary<decimal,decimal>) = a
    static member A (a:HashSet<decimal>) = a
    static member A (a:List<decimal>) = a
    static member A (a:Queue<decimal>) = a
    static member A (a:Task<float>) = a
    static member A (a:Dictionary<float,float>) = a
    static member A (a:HashSet<float>) = a
    static member A (a:List<float>) = a
    static member A (a:Queue<float>) = a
    static member A (a:Task<float32>) = a
    static member A (a:Dictionary<float32,float32>) = a
    static member A (a:HashSet<float32>) = a
    static member A (a:List<float32>) = a
    static member A (a:Queue<float32>) = a
    static member A (a:Task<DateTime>) = a
    static member A (a:Dictionary<DateTime,DateTime>) = a
    static member A (a:HashSet<DateTime>) = a
    static member A (a:List<DateTime>) = a
    static member A (a:Queue<DateTime>) = a
    static member A (a:Task<Task>) = a
    static member A (a:Dictionary<Task,Task>) = a
    static member A (a:HashSet<Task>) = a
    static member A (a:List<Task>) = a
    static member A (a:Queue<Task>) = a
Class.A null // does filter only reference type
Class.A typedefof<ResizeArray<_>>
Class.A (Guid.NewGuid())
Class.A (Unchecked.defaultof<System.DayOfWeek>)
