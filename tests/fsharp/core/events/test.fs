// #Regression #Conformance #Events #Regression #Interop 
#light

module Test

let mutable failures = []
let report_failure s =  failures <- failures @ [s]
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else stderr.WriteLine " NO"; report_failure s
let check s b1 b2 = 
   stderr.Write(s:string);  
   if b1 = b2 then 
       stderr.WriteLine " OK" 
   else 
       printfn ", FAILED: expected %A, got %A" b2 b1;
       report_failure s

open System
open System.Drawing
open System.ComponentModel
open System.Windows.Forms

module EventTest2 = 

    type SomeComponent() = 
        let ev1 = new Event<string>()
        let ev2 = new Event<EventArgs>()
        [<CLIEvent>]    
        member x.SomeEvent = ev1.Publish
        [<CLIEvent>]    
        member x.Paint = ev2.Publish
        member x.Fire() = ev1.Trigger("hello"); ev2.Trigger(new EventArgs())
    
    let mk() = new SomeComponent()
    let fire (c:SomeComponent) = c.Fire()

    check "fewnew0" (typeof<SomeComponent>.GetEvent("SomeEvent").Name) "SomeEvent"
    check "fewnew0" (typeof<SomeComponent>.GetEvent("SomeEvent").GetAddMethod().Name) "add_SomeEvent"
    check "fewnew0" (typeof<SomeComponent>.GetEvent("SomeEvent").GetRaiseMethod()) null
    check "fewnew0" (typeof<SomeComponent>.GetEvent("SomeEvent").GetRemoveMethod().Name) "remove_SomeEvent"

    check "fewnew0" (typeof<SomeComponent>.GetEvent("Paint").Name) "Paint"
    check "fewnew0" (typeof<SomeComponent>.GetEvent("Paint").GetAddMethod().Name) "add_Paint"
    check "fewnew0" (typeof<SomeComponent>.GetEvent("Paint").GetRaiseMethod()) null
    check "fewnew0" (typeof<SomeComponent>.GetEvent("Paint").GetRemoveMethod().Name) "remove_Paint"

module AbstractEventTests =


    type ChannelChangedHandler = delegate of obj * int -> unit

    type C() =  
        let channelChanged = new Event<ChannelChangedHandler,_>()

        static let defaultChannelChanged = new Event<ChannelChangedHandler,_>()

        [<CLIEvent>]    
        member self.ChannelChanged = channelChanged.Publish

        member self.ChangeChannel(n) = channelChanged.Trigger(self,n)

        [<CLIEvent>]    
        static member DefaultChannelChanged = defaultChannelChanged.Publish

        static member ChangeDefaultChannel(n) = defaultChannelChanged.Trigger(null,n)



    type I =  
        [<CLIEvent>]    
        abstract member ChannelChanged : IEvent<ChannelChangedHandler,int>


    type ImplI() =  
        let channelChanged = new Event<ChannelChangedHandler,_>()
        member self.ChangeChannel(n) = channelChanged.Trigger(self,n)
        interface I with 
            [<CLIEvent>]    
            member self.ChannelChanged = channelChanged.Publish


    type FrameworkImplI() =  
        let channelChanged = new Event<System.ComponentModel.PropertyChangedEventHandler,_>()
        member self.ChangeChannel(n) = channelChanged.Trigger(self,n)
        interface System.ComponentModel.INotifyPropertyChanged with 
            [<CLIEvent>]    
            override self.PropertyChanged = channelChanged.Publish


    [<AbstractClass>]
    type Base() =  
        [<CLIEvent>]    
        abstract member ChannelChanged : IEvent<ChannelChangedHandler,int>


    type Derived() =  
        inherit Base()
        let channelChanged = new Event<ChannelChangedHandler,_>()
        member self.ChangeChannel(n) = channelChanged.Trigger(self,n)
        [<CLIEvent>]    
        override self.ChannelChanged = channelChanged.Publish

    let ObjectImplI() =  
        let channelChanged = new Event<ChannelChangedHandler,_>()
        channelChanged.Trigger, 
        { new I with 
            [<CLIEvent>]    
            member self.ChannelChanged = channelChanged.Publish }


    let ObjectFrameworkImplI() =  
        let channelChanged = new Event<System.ComponentModel.PropertyChangedEventHandler,_>()
        channelChanged.Trigger,
        { new System.ComponentModel.INotifyPropertyChanged with 
            [<CLIEvent>]    
            override self.PropertyChanged = channelChanged.Publish }


    let ObjectDerived() =  
        let channelChanged = new Event<ChannelChangedHandler,_>()
        channelChanged.Trigger, 
        { new Base() with
              [<CLIEvent>]    
              override self.ChannelChanged = channelChanged.Publish }




    let test(ev:IEvent<ChannelChangedHandler,_>, change) = 
        let r = ref 0 
        let h1 = ChannelChangedHandler(fun sender channel -> r := channel)
        ev.AddHandler(h1)
        change(3)
        check "02374enw1" !r 3
        ev.RemoveHandler(h1)
        change(4)
        check "02374enw2" !r 3

        let r = ref 0 
        let h1 = ChannelChangedHandler(fun sender channel -> r := channel)
        let h2 = ChannelChangedHandler(fun sender channel -> r := channel+1)
        ev.AddHandler(h1)
        ev.AddHandler(h2)
        change(3)
        check "02374enw1" !r 4
        ev.RemoveHandler(h2)
        change(5)
        check "02374enw2" !r 5
        ev.RemoveHandler(h1)
        change(6)
        check "02374enw2" !r 5

    let c = C()
    let ev1 = c.ChannelChanged
    let ev2 = C.DefaultChannelChanged
    let d = Derived()
    let impl = ImplI()
    let ev3 = d.ChannelChanged
    let ev4 = (impl :> I).ChannelChanged


    test(ev1,c.ChangeChannel)
    test(ev2,C.ChangeDefaultChannel)
    test(ev3,d.ChangeChannel)
    test(ev4,impl.ChangeChannel)

module EventCombinators = 

    type ChannelChangedHandler = delegate of obj * int -> unit

    module Observable = 
        let catch (e: IObservable<'T>) = 
            { new IObservable<_> with  
                member __.Subscribe(o:IObserver<_>) = 
                    e.Subscribe({ new IObserver<_> with  
                                      member x.OnNext(v) = o.OnNext(Choice1Of2 v)
                                      member x.OnError(e) = o.OnNext(Choice2Of2 e)
                                      member x.OnCompleted() = o.OnCompleted() }) }
                                
        let empty () = 
            { new IObservable<_> with  
                member __.Subscribe(o:IObserver<_>) = o.OnCompleted(); { new System.IDisposable with 
                                                                            member __.Dispose() = () }  }
                                
        let error f (e: IObservable<'T>) = 
                    e.Subscribe({ new IObserver<'T> with  
                                      member x.OnNext(v) = ()
                                      member x.OnError(e) = f e
                                      member x.OnCompleted() = () }) 
                         |> ignore
                         
        let completed f (e: IObservable<'T>) = 
                    e.Subscribe({ new IObserver<'T> with  
                                      member x.OnNext(v) = ()
                                      member x.OnError(e) = ()
                                      member x.OnCompleted() = f () }) 
                         |> ignore
                         
        let fail () = 
            { new IObservable<_> with  
                member __.Subscribe(o:IObserver<_>) = 
                              o.OnError( Failure "fail"); 
                              { new System.IDisposable with 
                                      member __.Dispose() = () }  }

        // Observers should ignore subsequent failures
        let failTwice () = 
            { new IObservable<_> with  
                member __.Subscribe(o:IObserver<_>) = 
                              o.OnError( Failure "fail1"); 
                              o.OnError( Failure "fail2"); 
                              { new System.IDisposable with 
                                      member __.Dispose() = () }  }

    type C() =  
        let channelChanged = new Event<ChannelChangedHandler,_>()

        static let defaultChannelChanged = new Event<ChannelChangedHandler,_>()

        [<CLIEvent>]    
        member self.ChannelChanged = channelChanged.Publish

        member self.ChangeChannel(n) = channelChanged.Trigger(self,n)

        [<CLIEvent>]    
        static member DefaultChannelChanged = defaultChannelChanged.Publish

        static member ChangeDefaultChannel(n) = defaultChannelChanged.Trigger(null,n)

    module TestListen = 
        let c1 = C()
       
        let result = ref 0 
        c1.ChannelChanged |> Observable.add (fun x -> result := x)
        c1.ChangeChannel(6)
        check "e89e0jrweoi1" !result 6
        c1.ChannelChanged |> Observable.add (fun x -> result := x + 1)
        c1.ChangeChannel(6)
        check "e89e0jrweoi2" !result 7
    
    module TestChoose = 
        let c2 = C()
       

        let result = ref 0 
        c2.ChannelChanged |> Observable.choose (fun x -> None) |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi3" !result 0

        c2.ChannelChanged |> Observable.choose (fun x -> Some 1) |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4d" !result 1

    
        c2.ChannelChanged |> Observable.choose (fun x -> failwith "bad choice") |> Observable.error (fun e -> result := e.Message.Length)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4e" !result (String.length "bad choice")
    
        result := 0
        Observable.empty() |> Observable.completed (fun e -> result := 101 )
        check "e89e0jrweoi4f" !result 101
    
        result := 0
        Observable.empty() |> Observable.choose (fun x -> Some 1) |> Observable.completed (fun e -> result := 101 )
        check "e89e0jrweoi4g" !result 101
    
        result := 0 
        Observable.fail() |> Observable.choose (fun x -> Some 1) |> Observable.completed (fun e -> result := 101 )
        // completed should not be called on error
        check "e89e0jrweoi4g" !result 0
    
        result := 0 
        Observable.fail() |> Observable.choose (fun x -> Some 1) |> Observable.error (fun e -> result := 101 )
        // OnError should be called on error
        check "e89e0jrweoi4g" !result 101
    
        result := 0 
        Observable.failTwice() |> Observable.choose (fun x -> Some 1) |> Observable.error (fun e -> result := 101 )
        // subsequent errors should be ignored
        check "e89e0jrweoi4g" !result 101
    

    module TestFilter = 
        let c2 = C()
       

        let result = ref 0 
        c2.ChannelChanged |> Observable.filter (fun x -> false) |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi5" !result 0

        c2.ChannelChanged |> Observable.filter (fun x -> true) |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi6" !result 6
    

        c2.ChannelChanged |> Observable.filter (fun x -> failwith "bad choice") |> Observable.error (fun e -> result := e.Message.Length)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4h" !result (String.length "bad choice")
    
        result := 0
        Observable.empty() |> Observable.filter (fun x -> false) |> Observable.completed (fun e -> result := 101 )
        check "e89e0jrweoi4i" !result 101

        result := 0
        Observable.empty() |> Observable.filter (fun x -> true) |> Observable.completed (fun e -> result := 101 )
        check "e89e0jrweoi4jB" !result 101

        result := 0
        Observable.failTwice() |> Observable.filter (fun x -> true) |> Observable.error (fun e -> result := 101 )
        check "e89e0jrweoi4jC" !result 101

        result := 0
        Observable.failTwice() |> Observable.filter (fun x -> true) |> Observable.completed (fun e -> result := 101 )
        // completed should not be called on error
        check "e89e0jrweoi4jC" !result 0

    module TestMap = 
        let c2 = C()
       

        let result = ref ""
        c2.ChannelChanged |> Observable.map string |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi7" !result "6"

        c2.ChannelChanged |> Observable.map (fun x -> string (x + 1)) |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi8" !result "7"
    
    

        c2.ChannelChanged |> Observable.map (fun x -> failwith "bad choice") |> Observable.error (fun e -> result := e.Message)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4k" !result "bad choice"

        result := ""
        Observable.empty() |> Observable.map (fun x -> x + 1) |> Observable.completed (fun e -> result := !result + "101" )
        check "e89e0jrweoi4l" !result "101"

        result := ""
        Observable.failTwice() |> Observable.map (fun x -> x + 1) |> Observable.error (fun e -> result := !result + "101" )
        check "e89e0jrweoi4jD" !result "101"

        result := ""
        Observable.failTwice() |> Observable.map (fun x -> x + 1) |> Observable.completed (fun e -> result := !result + "101" )
        // completed should not be called on error
        check "e89e0jrweoi4jD" !result ""
    
    
    module TestMerge = 
        let c2 = C()
        let c3 = C()
       

        let result = ref 0
        (c2.ChannelChanged,c3.ChannelChanged) ||> Observable.merge |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi9" !result 6
        c3.ChangeChannel(7)
        check "e89e0jrweoi10" !result 7


        result := 0
        (Observable.empty(), Observable.empty()) ||> Observable.merge |> Observable.completed (fun () -> result := !result + 101 )
        // should only get one completed signal
        check "e89e0jrweoi4m" !result 101

        result := 0
        (Observable.fail(), Observable.fail()) ||> Observable.merge |> Observable.error (fun e -> result := !result + 101 )
        // should only get one error signal
        check "e89e0jrweoi4n" !result 101
    
        result := 0
        (Observable.failTwice(), Observable.failTwice()) ||> Observable.merge |> Observable.error (fun e -> result := !result + 101 )
        // should only get one error signal
        check "e89e0jrweoi4n" !result 101
    
        result := 0
        (Observable.failTwice(), Observable.failTwice()) ||> Observable.merge |> Observable.completed (fun e -> result := !result + 101 )
        // completed should not be called on error
        check "e89e0jrweoi4nX" !result 0
    
    module TestPairwise = 
        let c2 = C()

        let result = ref (0,0)
        c2.ChannelChanged |> Observable.pairwise |> Observable.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi11" !result (0,0)
        c2.ChangeChannel(7)
        check "e89e0jrweoi12" !result (6,7)
        c2.ChangeChannel(8)
        check "e89e0jrweoi13" !result (7,8)
    
        result := (0,0)
        Observable.failTwice() |> Observable.pairwise |> Observable.error (fun e -> result := (fst !result + 101, snd !result + 102) )
        check "e89e0jrweoi4jA" !result (101,102)

        result := (0,0)
        Observable.empty() |> Observable.pairwise |> Observable.completed (fun e -> result := (10,11))
        check "e89e0jrweoi4jA1" !result (10,11)

        // completed should not be called on error
        result := (0,0)
        Observable.fail() |> Observable.pairwise |> Observable.completed (fun e -> result := (10,11))
        check "e89e0jrweoi4jA2" !result (0,0)

    
    module TestPartition = 
        let c2 = C()

        let resulta = ref 0
        let resultb = ref 0
        let c2a, c2b = c2.ChannelChanged |> Observable.partition (fun x -> x < 2) 
        c2a |> Observable.add (fun x -> resulta := x)
        c2b |> Observable.add (fun x -> resultb := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi14a" !resulta 0
        check "e89e0jrweoi15" !resultb 6


        c2.ChangeChannel(7)
        check "e89e0jrweoi16" !resulta 0
        check "e89e0jrweoi17" !resultb 7

        c2.ChangeChannel(1)
        check "e89e0jrweoi18" !resulta 1
        check "e89e0jrweoi19" !resultb 7

        c2.ChannelChanged |> Observable.partition (fun x -> failwith "bad choice") |> fst |> Observable.error (fun e -> resulta := e.Message.Length)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4a" !resulta (String.length "bad choice")
    
        c2.ChannelChanged |> Observable.partition (fun x -> failwith "bad choice2") |> snd |> Observable.error (fun e -> resultb := e.Message.Length)
        c2.ChangeChannel(6)
        check "e89e0jrweoi4b" !resultb (String.length "bad choice2")
    
    
    module TestScan = 
        let c2 = C()

        let result = ref 0
        (0,c2.ChannelChanged) ||> Event.scan (fun z x -> z + x)  |> Event.add (fun x -> result := x)
        c2.ChangeChannel(6)
        check "e89e0jrweoi20" !result 6
        c2.ChangeChannel(3)
        check "e89e0jrweoi21" !result 9
        c2.ChangeChannel(4)
        check "e89e0jrweoi22" !result 13

        // Add another listener. It will get trigger after the first listener, wiping out the results of the first
        (0,c2.ChannelChanged) ||> Event.scan (fun z x -> z + x)  |> Event.add (fun x -> result := x)
        c2.ChangeChannel(2)
        check "e89e0jrweoi23" !result 2
        c2.ChangeChannel(3)
        check "e89e0jrweoi24" !result 5
        c2.ChangeChannel(1)
        check "e89e0jrweoi25" !result 6

        // add an event scanner
        let result2 = ref false
        (0,c2.ChannelChanged) ||> Event.scan (fun z x -> failwith "bad choice")  |> ignore
        (try c2.ChangeChannel(6) with e -> result2 := true)
        check "e89e0jrweoi4c" !result2 true

// See FSharp 1.0 6175
module CheckEventMembersAreNotMoreGeneric =
   type T() =
      [<CLIEvent>]
      member x.Event = Event<_>().Publish
    
   (T().Event : IEvent<obj>) |> ignore // this was previously giving an error saying "one type parameter expected"


module CLIEventIsInBaseType_FSHarp_1_0_6381 = 
    module ActualRepro = 
        type IBase =
            abstract BaseProp : string
            [<CLIEvent>]
            abstract SettingChanged : IEvent<int>

        type IDerived =
            abstract X : int
            inherit IBase

        type Foo(z : IDerived) =
            do
                printfn "%d" z.X // ok
                printfn "%s" z.BaseProp   // ok
                z.SettingChanged |> Event.add(fun x -> printfn "%d" x)  // compiler went boom

    module GenericCase = 
        type IBase<'T> =
            abstract BaseProp : string
            [<CLIEvent>]
            abstract SettingChanged : IEvent<'T>

        type IDerived<'T> =
            abstract X : int
            inherit IBase<list<'T>>

        type Foo(z : IDerived<string>) =
            do
                printfn "%d" z.X // ok
                printfn "%s" z.BaseProp   // ok
                z.SettingChanged |> Event.add(fun x -> printfn "%A" x)  // compiler went boom

module EventWithNonPublicDelegateTypes_DevDiv271288 =
    // The bug was about throwing a runtime exception.
    // The fix was in FSharp.Core
    module ActualRepro =
        type internal RequestFinishedDelegate = delegate of obj * unit -> unit
        let private requestFinishedEvent = Event<RequestFinishedDelegate, unit>()
        let _ =
            let p = requestFinishedEvent.Publish
            use s = p.Subscribe(fun () -> printfn "called EventWithNonPublicDelegateTypes_DevDiv271288.ActualRepro: OK")
            requestFinishedEvent.Trigger( null, () )

    // Same as above, "public" instead of "internal"
    module WithPublic =
        type public RequestFinishedDelegate = delegate of obj * unit -> unit 
        let private requestFinishedEvent = Event<RequestFinishedDelegate, unit>()
        let _ =
            let p = requestFinishedEvent.Publish
            use s = p.Subscribe(fun () -> printfn "called EventWithNonPublicDelegateTypes_DevDiv271288.WithPublic: OK")
            requestFinishedEvent.Trigger( null, () )

    // Similar, but involving a different signature
    module AnotherVariation =
        type internal RequestFinishedDelegate = delegate of obj * unit * unit -> unit // note the extra unit
        let private requestFinishedEvent = Event<RequestFinishedDelegate, unit * unit>()
        let _ =
            let p = requestFinishedEvent.Publish
            use s = p.Subscribe(fun (_,_) -> printfn "called EventWithNonPublicDelegateTypes_DevDiv271288.AnotherVariation: OK")
            requestFinishedEvent.Trigger( null, ((), ()) )

let _ = 
  if failures.Length > 0 then (printfn "Tests Failed: %A" failures; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        printf "TEST PASSED OK"; 
        exit 0)
