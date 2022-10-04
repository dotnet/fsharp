// #Regression #NoMT #Printing 
#light

// Test for FSharp1.0:2581 - FSI should display bound values, not just evaluated expressions (was: FSI should print the value of the last declared value is there is no last expression)

//<Expects status="success">type RecT =</Expects>
//<Expects status="success">  { Name: string }</Expects>
//<Expects status="success">type Bldg =</Expects>
//<Expects status="success">  \| House</Expects>
//<Expects status="success">  \| Museum</Expects>
//<Expects status="success">  \| Office</Expects>
//<Expects status="success">val a: int = 1</Expects>
//<Expects status="success">val B: string = "Hello"</Expects>
//<Expects status="success">val c': RecT = { Name = "F#" }</Expects>
//<Expects status="success">val _d: Bldg = Office</Expects>
//<Expects status="success">val e: seq<int></Expects>
//<Expects status="success">val F'F: int list = \[3; 2; 1]</Expects>
//<Expects status="success">val g: Set<int></Expects>
//<Expects status="success">val g': Set<'a></Expects>
//<Expects status="success">val getPointF: x: float32 \* y: float32 -> System\.Drawing\.PointF</Expects>
//<Expects status="success">val h: System\.Drawing\.PointF = {X=.+, Y=.+}</Expects>
//<Expects status="success">val i: int \* RecT \* Bldg = \(1, { Name = "F#" }, Office\)</Expects>
//<Expects status="success">val J_: int\[\] = \[\|1; 2; 3\|]</Expects>
//<Expects status="success">val j_': float\[\] = \[\|1\.0; 1\.0\|]</Expects>
//<Expects status="success">val j_'_: RecT\[\] = \[\|\|]</Expects>
//<Expects status="success">val j_'': string\[\] = \[\|"0"; "1"; "2"; "3"; "4"\|]</Expects>


type RecT = { Name : string }
type Bldg = House | Museum | Office

let a = 1                                       // int              - val a: int = 1
let B = "Hello"                                 // reference type   - val B: string = "Hello"

let c' = { Name = "F#" }                        // record           - val c': RecT = { Name = "F#" }

let _d = Office                                 // disc unioin      - val _d: Bldg = Office

let e = {1..2..100}                             // sequence         - val e: seq<int> = <seq>
let F'F = [3..-1..1]                            // list             - val F'F: int list = [3; 2; 1]

let g = (Set.ofSeq e)
        |> Set.intersect (Set.ofList F'F)      // set              - val g: Set<int> = <seq>

let g' = Set.empty                              // another set      - val g': Set<'a>

let getPointF (x, y) = System.Drawing.PointF(x, y)
                                                // function value   - val getPointF: float32 * float32 -> System.Drawing.PointF

let h    = getPointF (1.5f, -1.5f)              // PointF structure - val h: System.Drawing.PointF = {X=1.5, Y=-1.5}

let i    = (1, c', _d)                          // tuple            - val i: int * RecT * Bldg = (1, { Name = "F#" }, Office)

let J_   = [| 1; 2; 3; |]                       // array            - val J_: int array = [|1; 2; 3|]
let j_'  = Array.create 2 1.0                   // another array    - val j_': float array = [|1.0; 1.0|]
let j_'_ = Array.empty<RecT>                    // empty RecT array - val j_'_: RecT array = [||]
let j_'' = Array.init 5 (fun i -> i.ToString()) // string array     - val j_'': string array = [|"0"; "1"; "2"; "3"; "4"|]

;;

#q;;
