// #Regression #Conformance #DeclarationElements #Events  
// Regression for 5180, this used to not compile in VS2008

type 'a Wrapper() =
  let event = Event<_,_>()
  [<CLIEvent>]
  member x.SomethingHappened : IEvent<System.EventHandler,System.EventArgs> = event.Publish
  member x.Packed = 
    { new IExistsWrapper with member i.Apply u = u.Use () x }
    
and IExistsWrapper =
  abstract Apply<'t> : IWrapperUser<'t> -> 't
and IWrapperUser<'t> =
  abstract Use<'b> : unit -> ('b Wrapper -> 't)
 
and Registry() =
  static member Register (wrap:IExistsWrapper) =
    wrap.Apply 
      { new IWrapperUser<_> with 
        member x.Use() = 
          fun w -> 
            w.SomethingHappened.Add(fun _ -> ()) }
