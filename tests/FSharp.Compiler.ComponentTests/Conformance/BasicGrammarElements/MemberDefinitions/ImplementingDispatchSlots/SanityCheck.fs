// #Conformance #DeclarationElements #MemberDefinitions 
#light

type Shape =
    | Shape
    | Oval
    | Circle

type CShape() =
    abstract GetKind : unit -> Shape
    default this.GetKind() = Shape.Shape
    member this.Name() = "shape"
    
type COval() =
    inherit CShape()
    override this.GetKind() = Shape.Oval
    member this.Name() = "shape.oval"
    
type CCircle() =
    inherit COval()
    override this.GetKind() = Shape.Circle
    member this.Name() = "shape.oval.circle"
    
let shape = new CShape()
let oval = new COval()
let circle = new CCircle()

// Non virtual methods
if shape.Name() <> "shape" then failwith "Failed: 1"
if oval.Name() <> "shape.oval" then failwith "Failed: 2"
if circle.Name() <> "shape.oval.circle" then failwith "Failed: 3"

// Virtual methods
if shape.GetKind() <> Shape then failwith "Failed: 4"
if oval.GetKind() <> Oval then failwith "Failed: 5"
if circle.GetKind() <> Circle then failwith "Failed: 6"

// Dispatch once cast tests
let circleAsShape = circle :> CShape
// Name isn't declared virtual, so we get base types behavior
if circleAsShape.Name() <> "shape" then failwith "Failed: 7"
// GetKind is virtual, so we should get correct method
if circleAsShape.GetKind() <> Circle then failwith "Failed: 8"
