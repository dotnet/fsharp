// #CodeGen #EmittedIL #Sequences   
type Foo = Foo | Bar
let getTuple foo = match foo with | Foo -> 1,2 | Bar -> 3,4
let seq1 () =
  seq {
     let a,b = getTuple Foo
     yield! [a;b]
  }
let seq2 () =
  seq {
     let (a,b) as c = getTuple Foo
     yield! [a;b]
  }
  
let seq3 foo =
  seq {
    match foo with
    | Foo ->
      let a,b = getTuple foo
      yield! [a;b]
    | Bar ->
      let c,d = getTuple foo
      yield! [c;d] 
  }