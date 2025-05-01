// #Regression #Conformance #TypeInference 
// Regression test for FSharp1.0:3187
// Title: better inference for mutually recursive generic classes
// Descr: Verify types are inferred correctly for generic classes

type Vec<'a>(x:'a,y:'a) =
  member v.x : 'a = x
  member v.y  = y
  member v.Mul(w:Vec<'b>) : Vec<'a*'b> = new Vec<'a*'b>((v.x,w.x),(v.y,w.y))
  
let vecA = Vec<int>(1, 1)
let vecB = Vec<float>(1.0, 1.0)
let c = vecA.Mul(vecB)

type Lazy<'T> = 
    { /// This field holds the result of a successful computation. It's initial value is Unchecked.defaultof
      mutable value : 'T
      /// This field holds either the function to run or a LazyFailure object recording the exception raised 
      /// from running the function. It is null if the thunk has been evaluated successfully.
      mutable funcOrException: obj }
    member x.Force() : 'T =  x.UnsynchronizedForce()
    member x.UnsynchronizedForce() =  x.value 
    
let lazyVal = { value = 1; funcOrException = null }
if lazyVal.Force() <> 1 then exit 1
