module FSharp.Editor.Tests.Refactors.ConvertAnonymousRecordTests

open Microsoft.VisualStudio.FSharp.Editor

open Xunit

open FSharp.Editor.Tests.Refactors.RefactorTestFramework

let private refactored (body: string) (marker: string) =
    refactoredBy (new FSharpConvertAnonymousRecordRefactoring()) body marker

let private actionsAt (body: string) (marker: string) =
    actionsBy (new FSharpConvertAnonymousRecordRefactoring()) body marker

let private convertsBothWays (markers: string seq) (reference: string) (structs: string) =
    convertsBothWaysBy (new FSharpConvertAnonymousRecordRefactoring()) markers reference structs

[<Theory>]
[<InlineData("""
let show () = printfn "%A" {| A = 1 |}
""",
             "A = 1",
             """
let show () = printfn "%A" struct {| A = 1 |}
""")>]
[<InlineData("""
let items: list<{| A: int |}> = []
""",
             "A: int",
             """
let items: list<struct {| A: int |}> = []
""")>]
[<InlineData("""
let config =
    {|
        Name = "x"
        Size = 1
    |}
""",
             "Size",
             """
let config =
    struct {|
        Name = "x"
        Size = 1
    |}
""")>]
let ``Anonymous record that flows nowhere else converts on its own`` (reference: string, marker: string, structs: string) =
    convertsBothWays [ marker ] reference structs

[<Fact>]
let ``Value and the annotations it flows into convert together`` () =
    let reference =
        """
let person = {| Name = "Ada"; Age = 36 |}
let copy: {| Name: string; Age: int |} = person
let age = person.Age
"""

    let structs =
        """
let person = struct {| Name = "Ada"; Age = 36 |}
let copy: struct {| Name: string; Age: int |} = person
let age = person.Age
"""

    convertsBothWays [ "Ada" ] reference structs

[<Fact>]
let ``Parameter annotation converts its arguments`` () =
    let reference =
        """
let greet (p: {| Name: string |}) = "Hi " + p.Name

let ada = {| Name = "Ada" |}
let greetings = [ greet ada; greet {| Name = "Bob" |} ]
"""

    let structs =
        """
let greet (p: struct {| Name: string |}) = "Hi " + p.Name

let ada = struct {| Name = "Ada" |}
let greetings = [ greet ada; greet struct {| Name = "Bob" |} ]
"""

    convertsBothWays [ "Name: string" ] reference structs

[<Fact>]
let ``Return type converts the result and the values it is bound to`` () =
    let reference =
        """
let origin () : {| X: int; Y: int |} = {| X = 0; Y = 0 |}
let start: {| X: int; Y: int |} = origin ()
"""

    let structs =
        """
let origin () : struct {| X: int; Y: int |} = struct {| X = 0; Y = 0 |}
let start: struct {| X: int; Y: int |} = origin ()
"""

    convertsBothWays [ "X: int" ] reference structs

[<Fact>]
let ``Record field converts its values and the annotations reading it`` () =
    let reference =
        """
type Person = { Info: {| Age: int |}; Tags: {| Count: int |} }

let person = { Info = {| Age = 30 |}; Tags = {| Count = 0 |} }
let info: {| Age: int |} = person.Info
"""

    let structs =
        """
type Person = { Info: struct {| Age: int |}; Tags: {| Count: int |} }

let person = { Info = struct {| Age = 30 |}; Tags = {| Count = 0 |} }
let info: struct {| Age: int |} = person.Info
"""

    convertsBothWays [ "Age: int" ] reference structs

[<Theory>]
[<InlineData("""
let point = {| X = 1 |}
let moved = {| point with Y = 2 |}
""",
             "X = 1",
             """
let point = struct {| X = 1 |}
let moved = {| point with Y = 2 |}
""")>]
[<InlineData("""
let point = {| X = 1 |}
let moved = {| point with Y = 2 |}
""",
             "with",
             """
let point = {| X = 1 |}
let moved = struct {| point with Y = 2 |}
""")>]
let ``Copy-and-update converts independently of its source`` (reference: string, marker: string, structs: string) =
    convertsBothWays [ marker ] reference structs

[<Fact>]
let ``Value declared in another file converts there`` () =
    let definition =
        """
module A

let person = {| Name = "Ada" |}
"""

    let code =
        """
module B

let name (p: {| Name: string |}) = p.Name
let text = name A.person
"""

    let struct (codeAfter, definitionAfter) =
        refactoredAcrossFiles (new FSharpConvertAnonymousRecordRefactoring()) definition code "Name: string"

    Assert.Equal(
        """
module B

let name (p: struct {| Name: string |}) = p.Name
let text = name A.person
""",
        codeAfter
    )

    Assert.Equal(
        """
module A

let person = struct {| Name = "Ada" |}
""",
        definitionAfter
    )

[<Fact>]
let ``Title names the target kind`` () =
    let reference =
        """
let point = {| X = 1 |}
"""

    let structs =
        """
let point = struct {| X = 1 |}
"""

    Assert.Equal("Convert to struct anonymous record", (actionsAt reference "X = 1" |> Seq.exactlyOne).Title)
    Assert.Equal("Convert to reference anonymous record", (actionsAt structs "X = 1" |> Seq.exactlyOne).Title)

[<Theory>]
[<InlineData("""
let make () =
    try {| A = 1 |}
    finally ()

let copy: {| A: int |} = make ()
""",
             """
let make () =
    try struct {| A = 1 |}
    finally ()

let copy: struct {| A: int |} = make ()
""")>]
[<InlineData("""
let make () =
    try {| A = 1 |}
    with _ -> {| A = 0 |}

let copy: {| A: int |} = make ()
""",
             """
let make () =
    try struct {| A = 1 |}
    with _ -> struct {| A = 0 |}

let copy: struct {| A: int |} = make ()
""")>]
let ``Result flowing out of try converts every branch of it`` (reference: string, structs: string) =
    convertsBothWays [ "A: int" ] reference structs

[<Theory>]
[<InlineData("""
let person = {| A = 1 |}
let copy = match person with | p -> p
let typed: {| A: int |} = copy
""",
             """
let person = struct {| A = 1 |}
let copy = match person with | p -> p
let typed: struct {| A: int |} = copy
""")>]
[<InlineData("""
let person = {| A = 1 |}
let alias = person
let typed: {| A: int |} = alias
""",
             """
let person = struct {| A = 1 |}
let alias = person
let typed: struct {| A: int |} = alias
""")>]
[<InlineData("""
let (person: {| A: int |}) = {| A = 1 |}
let typed: {| A: int |} = person
""",
             """
let (person: struct {| A: int |}) = struct {| A = 1 |}
let typed: struct {| A: int |} = person
""")>]
let ``Value converts through the names it is bound to again`` (reference: string, structs: string) =
    convertsBothWays [ "A = 1"; "A: int" ] reference structs

[<Theory>]
[<InlineData("""
let size (p: {| Size: int |}) = p.Size
let total = (size) {| Size = 1 |}
""",
             """
let size (p: struct {| Size: int |}) = p.Size
let total = (size) struct {| Size = 1 |}
""")>]
[<InlineData("""
let make () = {| Size = 1 |}
let copy: {| Size: int |} = (make) ()
""",
             """
let make () = struct {| Size = 1 |}
let copy: struct {| Size: int |} = (make) ()
""")>]
[<InlineData("""
let size = fun (p: {| Size: int |}) -> p.Size
let total = size {| Size = 1 |}
""",
             """
let size = fun (p: struct {| Size: int |}) -> p.Size
let total = size struct {| Size = 1 |}
""")>]
[<InlineData("""
let make = fun () -> {| Size = 1 |}
let copy: {| Size: int |} = make ()
""",
             """
let make = fun () -> struct {| Size = 1 |}
let copy: struct {| Size: int |} = make ()
""")>]
[<InlineData("""
let make () = {| Size = 1 |}
let alias = make
let copy: {| Size: int |} = alias ()
""",
             """
let make () = struct {| Size = 1 |}
let alias = make
let copy: struct {| Size: int |} = alias ()
""")>]
[<InlineData("""
let grow (by: int) (p: {| Size: int |}) = p.Size + by
let growOne = grow 1
let total = growOne {| Size = 1 |}
""",
             """
let grow (by: int) (p: struct {| Size: int |}) = p.Size + by
let growOne = grow 1
let total = growOne struct {| Size = 1 |}
""")>]
let ``Function in parentheses, bound as a lambda, aliased or partly applied converts with its calls`` (reference: string, structs: string) =
    convertsBothWays [ "Size = 1"; "Size: int" ] reference structs

[<Theory>]
[<InlineData("""
type C() =
    member _.Size(p: {| Size: int |}) = p.Size

let total = C().Size({| Size = 1 |})
""",
             """
type C() =
    member _.Size(p: struct {| Size: int |}) = p.Size

let total = C().Size(struct {| Size = 1 |})
""")>]
[<InlineData("""
type C() =
    member _.Grow(p: {| Size: int |}, by: int) = p.Size + by

let small = {| Size = 1 |}
let total = C().Grow(p = small, by = 1)
""",
             """
type C() =
    member _.Grow(p: struct {| Size: int |}, by: int) = p.Size + by

let small = struct {| Size = 1 |}
let total = C().Grow(p = small, by = 1)
""")>]
[<InlineData("""
type C() =
    member _.Grow(by: int, p: {| Size: int |}) = p.Size + by

let total = C().Grow(p = {| Size = 1 |}, by = 1)
""",
             """
type C() =
    member _.Grow(by: int, p: struct {| Size: int |}) = p.Size + by

let total = C().Grow(p = struct {| Size = 1 |}, by = 1)
""")>]
let ``Member parameter converts with the arguments given for it by position or by name`` (reference: string, structs: string) =
    convertsBothWays [ "Size = 1"; "Size: int" ] reference structs

[<Fact>]
let ``Generic parameter stops the conversion`` () =
    let reference =
        """
let identity<'T> (x: 'T) = x
let a = identity {| A = 1 |}
let b = identity<{| B: int |}> {| B = 2 |}
"""

    let structs =
        """
let identity<'T> (x: 'T) = x
let a = identity struct {| A = 1 |}
let b = identity<{| B: int |}> {| B = 2 |}
"""

    convertsBothWays [ "A = 1" ] reference structs

[<Fact>]
let ``Generic field does not stop the conversion`` () =
    let reference =
        """
let make value = {| Value = value |}
let a: {| Value: int |} = make 1
let b: {| Value: string |} = make "s"
"""

    let structs =
        """
let make value = struct {| Value = value |}
let a: struct {| Value: int |} = make 1
let b: struct {| Value: string |} = make "s"
"""

    convertsBothWays [ "Value = value"; "Value: int" ] reference structs

[<Theory>]
[<InlineData("\nlet x = id<_>{| A = 1 |}\n", "\nlet x = id<_> struct {| A = 1 |}\n")>]
[<InlineData("\nlet x = id{| A = 1 |}\n", "\nlet x = id struct {| A = 1 |}\n")>]
[<InlineData("\nlet f́ x = x\nlet y = f́{| A = 1 |}\n", "\nlet f́ x = x\nlet y = f́ struct {| A = 1 |}\n")>]
let ``Struct keyword is separated from a name or type arguments the record follows`` (before: string, after: string) =
    Assert.Equal(after, refactored before "A = 1")

[<Theory>]
[<InlineData("""
let x = 1
""",
             "1")>]
[<InlineData("""
type Point = { X: int }

let point = { X = 1 }
""",
             "X = 1")>]
[<InlineData("""
let quoted = <@ {| A = 1 |} @>
""",
             "A = 1")>]
let ``No action`` (code: string, marker: string) = Assert.Empty(actionsAt code marker)
