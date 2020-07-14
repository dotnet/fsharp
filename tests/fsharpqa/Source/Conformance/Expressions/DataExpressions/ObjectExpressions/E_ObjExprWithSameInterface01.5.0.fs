// #Regression #Conformance #DataExpressions #ObjectConstructors 
// This was Dev10:854519 and Dev11:5525. The fix was to make this a compile error to avoid a runtime exception.
//<Expects status="error" span="(13,5-16,31)" id="FS3361">You cannot implement the interface 'IQueue<_>' with the two instantiations 'IQueue<'T>' and 'IQueue<obj>' because they may unify.</Expects>

type IQueue<'a> =
    abstract Addd: 'a -> IQueue<'a>
    
type IQueueEx<'a> =
    inherit IQueue<'a>
    abstract Add: 'a -> IQueueEx<'a>
  
let makeQueueEx2() = 
    {new IQueueEx<'T> with
        member q.Add(x) = q
     interface IQueue<obj> with
        member q.Addd(x) = q }
 
makeQueueEx2() |> ignore

exit 1