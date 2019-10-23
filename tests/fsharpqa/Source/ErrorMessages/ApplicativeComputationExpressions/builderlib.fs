namespace ApplicativeComputationExpressions

type Eventually<'T> = 
    | Done of 'T 
    | NotYetDone of (unit -> Eventually<'T>)

type ResultOrException<'tresult> =
    | Result of 'tresult
    | Exception of System.Exception

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Eventually = 
    let rec box e = 
        match e with 
        | Done x -> Done (Operators.box x) 
        | NotYetDone (work) -> NotYetDone (fun () -> box (work()))

    let rec force e = 
        match e with 
        | Done x -> x 
        | NotYetDone (work) -> force (work())

    let repeatedlyProgressUntilDoneOrTimeShareOver timeShareInMilliseconds runner e = 
        let sw = new System.Diagnostics.Stopwatch() 
        let rec runTimeShare e = 
          runner (fun () -> 
            sw.Reset()
            sw.Start(); 
            let rec loop(e) = 
                match e with 
                | Done _ -> e
                | NotYetDone (work) -> 
                    if sw.ElapsedMilliseconds > timeShareInMilliseconds then 
                        sw.Stop();
                        NotYetDone(fun () -> runTimeShare e) 
                    else 
                        loop(work())
            loop(e))
        runTimeShare e

    let rec bind k e = 
        match e with 
        | Done x -> k x 
        | NotYetDone work -> NotYetDone (fun () -> bind k (work()))

    let rec apply f e = 
        match f, e with 
        | Done f, Done x -> Done (f x)
        | Done _, NotYetDone work -> NotYetDone (fun () -> apply f (work()))
        | NotYetDone work, _ -> NotYetDone (fun () -> apply (work()) e)
    
    let merge e1 e2 = e1 |> bind (fun x -> e2 |> bind (fun y -> Done (x,y)))
    
    let rec map f e =
        match e with
        | Done x -> Done (f x)
        | NotYetDone work -> NotYetDone (fun () -> map f (work()))

    let fold f acc seq = 
        (Done acc,seq) ||> Seq.fold  (fun acc x -> acc |> bind (fun acc -> f acc x))
        
    let rec catch e = 
        match e with 
        | Done x -> Done(Result x)
        | NotYetDone work -> 
            NotYetDone (fun () -> 
                let res = try Result(work()) with | e -> Exception e 
                match res with 
                | Result cont -> catch cont
                | Exception e -> Done(Exception e))
    
    let delay f = NotYetDone (fun () -> f())

    let tryFinally e compensation =    
        catch (e) 
        |> bind (fun res ->  compensation();
                             match res with 
                             | Result v -> Eventually.Done v
                             | Exception e -> raise e)

    let tryWith e handler =    
        catch e 
        |> bind (function Result v -> Done v | Exception e -> handler e)

    let applyUsing (resource:System.IDisposable) f =
        try
            f resource
        finally
            resource.Dispose()
    
    let rec doWhile f e =    
        if f() then e |> bind (fun () -> doWhile f e) else Eventually.Done ()
    
    let doFor (xs: seq<_>) f =    
        let rec loop (ie:System.Collections.Generic.IEnumerator<_>) =
            if ie.MoveNext() then f ie.Current |> bind (fun () -> loop ie)
            else Eventually.Done ()
        let ie = xs.GetEnumerator()
        tryFinally (delay (fun () -> loop ie)) (fun _ -> ie.Dispose())

type EventuallyBuilder() = 
    member __.Bind(e,k) = Eventually.bind k e
    member __.MergeSources(x1, x2) = Eventually.merge x1 x2
    member __.Return(v) = Eventually.Done v
    member __.ReturnFrom(e:Eventually<_>) = e
    member __.Combine(e1,e2) = e1 |> Eventually.bind (fun () -> e2)
    member __.TryWith(e,handler) = Eventually.tryWith e handler
    member __.TryFinally(e,compensation) =  Eventually.tryFinally e compensation
    member __.Using(resource:System.IDisposable,e) = Eventually.tryFinally (e resource) resource.Dispose
    member __.While(gd,e) = Eventually.doWhile gd e
    member __.For(xs,f) = Eventually.doFor xs f
    member __.Delay(f) = Eventually.delay f
    member __.Zero() = Eventually.Done ()

type EventuallyNoApplyBuilder() = 
    member __.Bind(e,k) = Eventually.bind k e
    member __.Return(v) = Eventually.Done v
    member __.ReturnFrom(e:Eventually<_>) = e
    member __.Combine(e1,e2) = e1 |> Eventually.bind (fun () -> e2)
    member __.TryWith(e,handler) = Eventually.tryWith e handler
    member __.TryFinally(e,compensation) =  Eventually.tryFinally e compensation
    member __.Using(resource:System.IDisposable,e) = Eventually.tryFinally (e resource) resource.Dispose
    member __.While(gd,e) = Eventually.doWhile gd e
    member __.For(xs,f) = Eventually.doFor xs f
    member __.Delay(f) = Eventually.delay f
    member __.Zero() = Eventually.Done ()


[<AutoOpen>]
module TheEventuallyBuilder =
    let eventually = new EventuallyBuilder()
    let eventuallyNoApply = new EventuallyNoApplyBuilder()

type FakeDisposable =
    | FakeDisposable of int
    interface System.IDisposable with
        member __.Dispose() = () // No-op disposal is precisely what makes this a fake
