// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:3404
// "System.Reflection doesn't like '.' and '`' in type names"
// You must compile with --optimize in order to repro.
//
//<Expects status="success">                 \| :\? Planet as obj -></Expects>
//<Expects status="success">  -----------------\^\^\^\^\^\^\^\^\^</Expects>
//<Expects status="warning" id="FS0067">This type test or downcast will always hold</Expects>
//<Expects status="success">                 match paintObject with</Expects>
//<Expects status="success">  ---------------------\^\^\^\^\^\^\^\^\^\^\^</Expects>
//<Expects status="warning" id="FS0025">Incomplete pattern matches on this expression\. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern\(s\)</Expects>
//<Expects status="success">          match lastTimeOption with</Expects>
//<Expects status="success">  --------------\^\^\^\^\^\^\^\^\^\^\^\^\^\^</Expects>
//<Expects status="warning" id="FS0025">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)</Expects>
//<Expects status="success">type Planet =</Expects>
//<Expects status="success">  new: ipx: float \* ivx: float -> Planet</Expects>
//<Expects status="success">  member VX: float</Expects>
//<Expects status="success">  member X: float</Expects>
//<Expects status="success">val paintObjects: Planet list</Expects>
//<Expects status="success">type Simulator =</Expects>
//<Expects status="success">  new: unit -> Simulator</Expects>


type Planet(ipx:float,ivx:float) =
    let mutable px = ipx
    let mutable vx = ivx

    member p.X with get() = px and set(v) = (px <- v)
    member p.VX with get() = vx and set(v) = (vx <- v)

let paintObjects : Planet list = []

type Simulator() =
    let lastTimeOption = None

    let step =
        match lastTimeOption with
        | Some(lastTime) ->
           for paintObject in paintObjects do
               match paintObject with
               | :? Planet as obj ->
                   let objects : Planet list =  [ for paintObject in paintObjects do yield paintObject ]

                   for obj2 in objects do
                       let dx = (obj2.X-obj.X)
                       let dx = (obj2.X-obj.X)
                       let d2 = (dx*dx) + (dx*dx)
                       obj.VX <- obj.VX + 0.0
                       obj.VX <- obj.VX + 0.0     // same as above!

;;
exit 0
;;
