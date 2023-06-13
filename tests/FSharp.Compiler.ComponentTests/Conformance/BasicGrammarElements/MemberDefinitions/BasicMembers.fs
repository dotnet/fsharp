#light

// Test basic class member syntax

type Foo(x : int) =
    inherit System.Object()         // as base

    member this.Value = x
    static member StaticMethod x = x * x
    override x.ToString() = sprintf "Value = %d, Base.ToString() = %s" x.Value (base.ToString())

let t = new Foo(5)
if t.Value <> 5 then failwith "Failed: 1"
if Foo.StaticMethod 10 <> 100 then failwith "Failed: 2"
if not (t.ToString().StartsWith("Value = 5, Base.ToString() =")) then failwith "Failed: 3"
