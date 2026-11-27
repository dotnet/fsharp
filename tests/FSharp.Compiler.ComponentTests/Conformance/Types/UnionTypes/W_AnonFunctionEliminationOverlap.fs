// Anonymous Union Types
// One type is fully included in another w.r.t. runtime type tests (FsharpFunc elimination)

type y = ((int -> int) | FSharpFunc<int,int>)