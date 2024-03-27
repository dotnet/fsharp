// <testmetadata>
// { "optimization": { "reported_in": "#12416", "reported_by": "@mrange", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>
type 'T PushStream = ('T -> bool) -> bool

let inline ofArray (vs : _ array) : _ PushStream = fun ([<InlineIfLambda>] r) ->
  let mutable i = 0
  while i < vs.Length && r vs.[i] do
    i <- i + 1
  i = vs.Length

let inline fold ([<InlineIfLambda>] f) z ([<InlineIfLambda>] ps : _ PushStream) =
  let mutable s = z
  let _ = ps (fun v -> s <- f s v; true)
  s

let values = [|0..10000|]

let thisIsInlined1 () = fold (+) 0 (ofArray values)
let thisIsInlined2 () = 
  let vs = [|0..10000|]
  fold (+) 0 (ofArray vs)

let thisIsNotInlined1 () = fold (+) 0 (ofArray [|0..10000|])

type Test() =
  let _values = [|0..10000|]
  static let _svalues = [|0..10000|]

  let array vs =  fold (+) 0 (ofArray vs)

  member x.thisIsInlined2 () = fold (+) 0 (ofArray values)
  member x.thisIsInlined3 () = array _values

  member x.thisIsNotInlined2 () = fold (+) 0 (ofArray _values)
  member x.thisIsNotInlined3 () = fold (+) 0 (ofArray _svalues)
  
