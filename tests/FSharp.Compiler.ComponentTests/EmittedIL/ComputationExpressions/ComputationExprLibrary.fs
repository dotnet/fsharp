// #Regression #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
// This is an aux library ("eventually" monad used in the compiler)
// It is used by the ComputationExpr* tests
// See regression test for FSHARP1.0:4972
//

namespace Library

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

    let rec doWhile f e =    
        if f() then e |> bind (fun () -> doWhile f e) else Eventually.Done ()
    
    let doFor (xs: seq<_>) f =    
        let rec loop (ie:System.Collections.Generic.IEnumerator<_>) =
            if ie.MoveNext() then f ie.Current |> bind (fun () -> loop ie)
            else Eventually.Done ()
        let ie = xs.GetEnumerator()
        tryFinally (delay (fun () -> loop ie)) (fun _ -> ie.Dispose())

type EventuallyBuilder() = 
    member x.Bind(e,k) = Eventually.bind k e
    member x.Return(v) = Eventually.Done v
    member x.ReturnFrom(e:Eventually<_>) = e
    member x.Combine(e1,e2) = e1 |> Eventually.bind (fun () -> e2)
    member x.TryWith(e,handler) = Eventually.tryWith e handler
    member x.TryFinally(e,compensation) =  Eventually.tryFinally e compensation
    member x.Using(resource:System.IDisposable,e) = Eventually.tryFinally (e resource)  resource.Dispose
    member x.While(gd,e) = Eventually.doWhile gd e
    member x.For(xs,f) = Eventually.doFor xs f
    member x.Delay(f) = Eventually.delay f
    member x.Zero() = Eventually.Done ()


[<AutoOpen>]
module TheEventuallyBuilder =
    let eventually = new EventuallyBuilder()

