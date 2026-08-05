#r "SpreadInlineLib.dll"

open System
let errors = ResizeArray<string>()
let check label cond = if not cond then errors.Add label

type Pt = { X : int; Y : int }
type Lbl = { A : int; B : int }

module ``Units of measure preserved through overriding spread`` =
    [<Measure>] type m
    type Tagged = { D : int<m>; Note : string }
    check "D measure stripped" ({ ...{ D = 5<m>; Note = "a" }; D = 9<m> }.D = 9<m>)

module ``Type alias as spread source`` =
    type PtAlias = Pt
    type FromAlias = { ...PtAlias; Z : int }
    let v : FromAlias = { ...{ X = 10; Y = 20 }; Z = 30 }
    check "alias source dropped fields" (v.X = 10 && v.Z = 30)

module ``Elaborated tree shape inside FSharp Quotations`` =
    open Microsoft.FSharp.Quotations.Patterns
    let rec args expr =
        match expr with
        | Let (_, _, body) -> args body
        | NewRecord (_, a) -> Some a.Length
        | _ -> None
    let p = { X = 1; Y = 2 }
    check "quotation record/anon shape" (args <@ { ...p; Y = 3 } @> = Some 2 && args <@ {| ...p; W = 5 |} @> = Some 3)

module ``Spread inside seq, async and task state machines`` =
    let b = { A = 1; B = 2 }
    let fromSeq = seq { for i in 1..2 -> { ...b; A = i } } |> Seq.toList
    check "seq spread wrong" (fromSeq.[1].A = 2)
    check "async return wrong" ((async { return { ...b; A = 9 } } |> Async.RunSynchronously).A = 9)
    check "task return wrong" ((task { return { ...b; A = 7 } }).Result.A = 7)

module ``CLIMutable target emits settable IL properties for spread-carried fields`` =
    type Src = { A : int; B : int }
    [<CLIMutable>] type Dst = { ...Src; C : int }
    let hasCli (t: Type) = t.GetCustomAttributes(typeof<CLIMutableAttribute>, false).Length > 0
    let settable n = typeof<Dst>.GetProperty(n: string).CanWrite
    check "CLIMutable attr leaked to Src" (not (hasCli typeof<Src>))
    check "Dst missing CLIMutable" (hasCli typeof<Dst>)
    check "settable A/B/C" (settable "A" && settable "B" && settable "C")
    check "Dst C wrong" (({ ...{ A = 1; B = 2 }; C = 3 } : Dst).C = 3)

module ``Type-level attributes do not propagate from spread source`` =
    [<CLIMutable; NoComparison>] type Src = { A : int; B : int }
    type Plain = { ...Src; C : int }
    let has<'a when 'a :> Attribute> (t: Type) = t.GetCustomAttributes(typeof<'a>, false).Length > 0
    check "CLIMutable propagated to Plain" (not (has<CLIMutableAttribute> typeof<Plain>))
    check "NoComparison propagated to Plain" (not (has<NoComparisonAttribute> typeof<Plain>))
    check "Src lost CLIMutable" (has<CLIMutableAttribute> typeof<Src>)

module ``Mutable field carried via spread, then overridden`` =
    type R = { mutable M : int; Name : string }
    check "mutable override wrong" ({ ...{ M = 1; Name = "a" }; M = 10 }.M = 10)

module ``SRTP resolves member carried by the spread source`` =
    let inline getB< ^T when ^T : (member B : int)> (x: ^T) = (^T : (member B : int) x)
    check "SRTP getB <> 6" (getB {| ...{| A = 5; B = 6 |}; A = 7 |} = 6)

module ``Inline spread elaboration across an assembly boundary`` =
    let r = SpreadInlineLib.bump { SpreadInlineLib.Lbl.A = 0; B = 7 }
    check "cross-assembly bump A/B" (r.A = 99 && r.B = 7)

module ``Property-get expression as spread source`` =
    type Holder() = member _.P = { A = 1; B = 2 }
    let r = { ...(Holder()).P; B = 9 }
    check "property-get source dropped fields" (r.A = 1 && r.B = 9)

module ``Field-level attribute carries from spread source to target`` =
    type Src = { [<Obsolete("x")>] A : int; B : int }
    type Dst = { ...Src; C : int }
    let obsolete (t: Type) = t.GetProperty("A").GetCustomAttributes(typeof<ObsoleteAttribute>, false).Length
    check "field attr not carried Src/Dst" (obsolete typeof<Src> = 1 && obsolete typeof<Dst> = 1)

module ``Linear non-mutual transitive spread chain`` =
    type A = { Z : int }
    type B = { ...A; Y : int }
    type C = { ...B; X : int }
    let c : C = { Z = 1; Y = 2; X = 3 }
    check "transitive chain dropped fields" (c.Z = 1 && c.X = 3)
if errors.Count > 0 then
    failwithf "%d failures:\n%s" errors.Count (String.concat "\n" errors)
