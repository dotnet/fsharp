module Pos34

// From https://github.com/dotnet/fsharp/issues/4171#issuecomment-528063764
// This case is where the type gets labelled as Sealed
// This case compiles without complaint
//
// See also neg120.fs
open System.Threading.Tasks

[<Sealed>]
type Id<'t> (v: 't) =
    let value = v
    member __.getValue = value

[<RequireQualifiedAccess>]
module Id =
    let run   (x: Id<_>) = x.getValue
    let map f (x: Id<_>) = Id (f x.getValue)
    let create x = Id x


type Bind =
    static member        (>>=) (source: Lazy<'T>   , f: 'T -> Lazy<'U>    ) = lazy (f source.Value).Value                                   : Lazy<'U>
    static member        (>>=) (source: Task<'T>   , f: 'T -> Task<'U>    ) = source.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap () : Task<'U>
    static member        (>>=) (source             , f: 'T -> _           ) = Option.bind   f source                                        : option<'U>
    static member        (>>=) (source             , f: 'T -> _           ) = async.Bind (source, f)  
    static member        (>>=) (source : Id<_>     , f: 'T -> _           ) = f source.getValue                                 : Id<'U>

    static member inline Invoke (source: '``Monad<'T>``) (binder: 'T -> '``Monad<'U>``) : '``Monad<'U>`` =
        let inline call (_mthd: 'M, input: 'I, _output: 'R, f) = ((^M or ^I or ^R) : (static member (>>=) : _*_ -> _) input, f)
        call (Unchecked.defaultof<Bind>, source, Unchecked.defaultof<'``Monad<'U>``>, binder)

let inline (>>=) (x: '``Monad<'T>``) (f: 'T->'``Monad<'U>``) : '``Monad<'U>`` = Bind.Invoke x f

type Return =
    static member inline Invoke (x: 'T) : '``Applicative<'T>`` =
        let inline call (mthd: ^M, output: ^R) = ((^M or ^R) : (static member Return : _*_ -> _) output, mthd)
        call (Unchecked.defaultof<Return>, Unchecked.defaultof<'``Applicative<'T>``>) x

    static member        Return (_: Lazy<'a>       , _: Return  ) = fun x -> Lazy<_>.CreateFromValue x : Lazy<'a>
    static member        Return (_: 'a Task        , _: Return  ) = fun x -> Task.FromResult x : 'a Task
    static member        Return (_: option<'a>     , _: Return  ) = fun x -> Some x                : option<'a>
    static member        Return (_: 'a Async       , _: Return  ) = fun (x: 'a) -> async.Return x
    static member        Return (_: 'a Id          , _: Return  ) = fun (x: 'a) -> Id x

let inline result (x: 'T) : '``Functor<'T>`` = Return.Invoke x


type TypeT<'``monad<'t>``> = TypeT of obj
type Node<'``monad<'t>``,'t> = A | B of 't * TypeT<'``monad<'t>``>

let inline wrap (mit: 'mit) =
        let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) -> (result Node<'mt,'t>.A ) : 'mit
        TypeT mit : TypeT<'mt>

let inline unwrap (TypeT mit : TypeT<'mt>) =
    let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) ->  (result Node<'mt,'t>.A ) : 'mit
    unbox mit : 'mit

let inline empty () = wrap ((result Node<'mt,'t>.A) : 'mit) : TypeT<'mt>

let inline concat l1 l2 =
        let rec loop (l1: TypeT<'mt>) (lst2: TypeT<'mt>) =
            let (l1, l2) = unwrap l1, unwrap lst2
            TypeT (l1 >>= function A ->  l2 | B (x: 't, xs) -> ((result (B (x, loop xs lst2))) : 'mit))
        loop l1 l2 : TypeT<'mt>


let inline bind f (source: TypeT<'mt>) : TypeT<'mu> =
    // let _mnil = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_: 't) -> (result Unchecked.defaultof<'u>) : 'mu
    let rec loop f input =
        TypeT (
            (unwrap input : 'mit) >>= function
                    | A -> result <| (A : Node<'mu,'u>) : 'miu
                    | B (h:'t, t: TypeT<'mt>) ->
                        let res = concat (f h: TypeT<'mu>) (loop f t)
                        unwrap res  : 'miu) 
    loop f source : TypeT<'mu>


let inline map (f: 'T->'U) (x: '``Monad<'T>`` ) = Bind.Invoke x (f >> Return.Invoke) : '``Monad<'U>``


let inline unfold (f:'State -> '``M<('T * 'State) option>``) (s:'State) : TypeT<'MT> =
        let rec loop f s = f s |> map (function
                | Some (a, s) -> B (a, loop f s)
                | None -> A) |> wrap
        loop f s

let inline create (al: '``Monad<list<'T>>``) : TypeT<'``Monad<'T>``> =
        unfold (fun i -> map (fun (lst:list<_>) -> if lst.Length > i then Some (lst.[i], i+1) else None) al) 0

let inline run (lst: TypeT<'MT>) : '``Monad<list<'T>>`` =
    let rec loop acc x = unwrap x >>= function
        | A         -> result (List.rev acc)
        | B (x, xs) -> loop (x::acc) xs
    loop [] lst

let c0 = create (Id ([1..10]))
let res0 = c0 |> run |> create |> run

