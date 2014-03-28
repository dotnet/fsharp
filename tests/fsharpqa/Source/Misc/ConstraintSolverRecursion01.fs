// #Misc #TypeConstraints
// In Vs2008/2010 this caused infinite recursion in the constraint solver leading to a stack overflow
// Dev11:137274

module Monad

open System
open Microsoft.FSharp.Reflection 

type ITypeCons<'tag,'a> = interface end

type IMonad<'tag, 'a when 'tag :> ICMonad<'tag> and 'tag: (new: unit -> 'tag)> = 
    inherit ITypeCons<'tag,'a>

and ICMonad<'tag when 'tag :> ICMonad<'tag> and 'tag: (new: unit -> 'tag)> =
    abstract Value: CMonad<'tag>

and
  [<AbstractClass>]
  CMonad<'tag when 'tag :> ICMonad<'tag> and 'tag: (new: unit -> 'tag)>() =
    abstract Return: 'a -> IMonad<'tag,'a>
    abstract Bind: IMonad<'tag,'a> * ('a -> IMonad<'tag,'b>) -> IMonad<'tag,'b>

    member me.ReturnFrom(m) = m

    abstract Zero: unit -> IMonad<'tag, unit>
    default me.Zero() =
        me.Return(())

    abstract Combine: IMonad<'tag,unit> * IMonad<'tag,'a> -> IMonad<'tag,'a>
    default me.Combine(mu, ma) =
        me.Bind(mu, (fun () -> ma))

    abstract Delay: (unit -> IMonad<'tag,'a>) -> IMonad<'tag,'a>
    default me.Delay(f) =
        me.Bind(me.Return(()), f)

    abstract While: (unit -> bool) * IMonad<'tag,unit> -> IMonad<'tag,unit>
    default me.While(pred, body) =
        if pred() then
            me.Bind(body, (fun() -> me.While(pred, body)))
        else
            me.Zero()

let monad<'m when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>() =
    (new 'm()).Value

type MonadBase<'m when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>() =
    inherit CMonad<'m>()
    let M = monad()

    override me.Return(x) = M.Return(x)
    override me.Bind(m, f) = M.Bind(m, f)
    override me.Zero() = M.Zero()
    override me.Combine(mu, ma) = M.Combine(mu, ma)
    override me.Delay(f) = M.Delay(f)
    override me.While(pred, body) = M.While(pred, body)

[<AbstractClass>]
type
  CMonoid<'a when 'a :> System.Collections.IEnumerable>() =
    abstract mempty: 'a
    abstract mappend: 'a -> 'a -> 'a
    abstract mconcat: seq<'a> -> 'a
    default me.mconcat xs =
        xs |> Seq.cast |> Seq.fold me.mappend me.mempty 

[<AbstractClass>]
type CMonadReader<'r,'m when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>() =
    inherit MonadBase<'m>()
    abstract ask: IMonad<'m, 'r>
    abstract local: ('r -> 'r) -> IMonad<'m,'a> -> IMonad<'m,'a>
    member me.asks f = me {
        let! r = me.ask
        return f r
    }

[<AbstractClass>]
type CMonadWriter<'w,'m when 'm :> ICMonad<'m> and 'w :> System.Collections.IEnumerable and 'm: (new: unit -> 'm)>(Mo: CMonoid<'w>) =
    inherit MonadBase<'m>()
    member me.Monoid = Mo
    abstract tell: 'w -> IMonad<'m,unit>
    abstract listen: IMonad<'m,'a> -> IMonad<'m,'a * 'w>
    abstract pass: IMonad<'m, 'a * ('w -> 'w)> -> IMonad<'m,'a>

    member me.listens f m = me {
        let! (a, w) = me.listen m
        return (a, f w)
    }

    member me.censor f m =
        let maw = me {
            let! a = m
            return (a, f)
        }
        me.pass maw

[<AbstractClass>]
type CMonadState<'s,'m when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>() =
    inherit MonadBase<'m>()
    
    abstract get: IMonad<'m,'s>
    abstract put: 's -> IMonad<'m, unit>

    member me.modify f = me {
        let! s = me.get
        return! me.put (f s)
    }

    member me.gets f = me {
        let! s = me.get
        return f s
    }

type Result<'a> = Error of Exception | Success of 'a
    with
        override me.ToString() =
            match me with
            | Error e -> 
                let et = e.GetType()
                if FSharpType.IsExceptionRepresentation(et) then
                    sprintf "%A" e
                else
                    sprintf "%s %A" et.Name e.Message
            | Success x -> 
                sprintf "Success %A" x

type ErrorT<'m when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>() =
    static let unwrap (m: ITypeCons<ErrorT<'m>,'a>) =
        (m :?> ErrorT<'m,'a>).Value
    static let wrap x =
        ErrorT<'m,_>(x)

    static let M = monad<'m>()

    static let lift m : IMonad<ErrorT<'m>,_> =
        let handler = M {
            let! a = m
            return Success a
        }
        upcast wrap handler

    static member Unwrap(m) = unwrap m
    static member Wrap(x) = wrap x

    static member Monad =
        {new CMonad<ErrorT<'m>>() with
            member me.Return(x) =
                upcast wrap (M.Return (Success x))
            member me.Bind(m, f) =
                try
                    let binder = M {
                        let! a = unwrap m
                        match a with
                        | Error e -> 
                            return Error e
                        | Success x ->
                            return! unwrap (f x)
                    }
                    upcast wrap binder
                with
                | e -> upcast wrap (M.Return (Error e))
            member me.Delay(f) =
                try
                    f()
                with
                | e -> upcast wrap (M.Return (Error e))
        }
    interface ICMonad<ErrorT<'m>> with
        member me.Value = ErrorT<_>.Monad

    static member MonadReader(MR: CMonadReader<'r,'m>) =
        {new CMonadReader<_,_>() with
            member me.ask =
                lift MR.ask
            member me.local f m =
                upcast wrap (MR.local f (unwrap m))
        }

    static member MonadWriter(MW: CMonadWriter<'w,'m>) =
        {new CMonadWriter<_,_>(MW.Monoid) with
            member me.tell w =
                lift (MW.tell w)
            member me.listen m =
                let listener = MW {
                    let! (a, w) = MW.listen (unwrap m)
                    return
                        match a with
                        | Error e -> Error e
                        | Success x -> Success (x, w)
                }
                upcast wrap listener
            member me.pass m =
                let passer = MW {
                    let! a = unwrap m
                    return
                        match a with
                        | Error e -> (Error e, id)
                        | Success (x, f) -> (Success x, f)
                }
                upcast wrap (MW.pass passer)
        }

    static member MonadState(MS: CMonadState<'s,'m>) =
        {new CMonadState<'s,_>() with
            member me.get = lift MS.get
            member me.put s = lift (MS.put s)
        }


and ErrorT<'m,'a when 'm :> ICMonad<'m> and 'm: (new: unit -> 'm)>(value: IMonad<'m, Result<'a>>) =
    member me.Value: IMonad<'m, Result<'a>> = value
    interface IMonad<ErrorT<'m>,'a>
  
