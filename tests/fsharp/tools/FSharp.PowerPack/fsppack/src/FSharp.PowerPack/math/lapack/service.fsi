namespace Microsoft.FSharp.Math.Experimental

type Provider<'a> =
    new : name:string * requiredDLLs:string [] * provide:(unit -> 'a) -> Provider<'a>
    member Provide : unit -> 'a
    member Name : string
    member RequiredDLLs : string []
    member Check : unit -> string  
  
type Service<'a> =
    new : providers:seq<Provider<'a>> -> Service<'a>
    member Start     : unit -> bool
    member StartWith : p:Provider<'a> -> unit
    member Stop      : unit -> unit
    member Service   : unit -> 'a option
    member Available : unit -> bool
    member Status    : unit -> string
    member Providers : Provider<'a> array with get,set