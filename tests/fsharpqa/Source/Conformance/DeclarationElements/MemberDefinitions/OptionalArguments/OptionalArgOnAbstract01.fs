// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments  
// Regression for 6385

type Optional() =
  abstract Foo:int * ?b : int -> int
  default x.Foo(a, ?b:int) =
    match b with
      | Some _ -> 1
      | _ -> 0
  member x.Bar (a, ?b:int) =
    match b with
      | Some _ -> ()
      | _ -> ()

let x = Optional()
let r = x.Foo(1)
let r2 = x.Foo(1,2)

exit <| if r = 0 && r2 = 1 then 0 else 1