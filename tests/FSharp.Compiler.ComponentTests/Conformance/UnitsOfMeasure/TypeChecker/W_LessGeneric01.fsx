// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints #ReqNOMT 

// Regression test for FSharp1.0:3579 - Problems in Units of Measure

//<Expects status="success">  let fn \(x:float<'u>\) =</Expects>
//<Expects status="success">  ----------------\^\^</Expects>
//<Expects status="warning" span="(18,17)" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The unit-of-measure variable 'u has been constrained to be measure '1'\.$</Expects>
//<Expects status="success">val loop: f: \('a -> 'a\) -> init: 'a -> comp: \('a -> 'a -> bool\) -> 'a</Expects>
//<Expects status="success">val fn: x: float -> float</Expects>

let rec loop f init comp =
  let next = f init
  if (comp next init) then
    next
  else
    loop f next comp

let fn (x:float<'u>) =
  let guess = 1.<_>
  let iter guess =
    let other = x/guess
    let avg = (guess + other) / 2.
    avg
  loop iter guess (fun x y -> abs(x-y) < 0.00001<_>)
  
;;

#q;;


