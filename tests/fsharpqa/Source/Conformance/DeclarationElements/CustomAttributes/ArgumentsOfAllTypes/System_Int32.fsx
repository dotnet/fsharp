
module T_System_Int32 =
    [<CSAttributes.System_Int32(2147483647, N1 = 2147483647)>]
    type T1() = class end
    let t1 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
    printfn "%A" t1

    [<CSAttributes.OuterClassWithNestedAttributes.System_Int32(2147483647, N1 = 2147483647)>]
    type T2() = class end
    let t2 = (typeof<T1>.GetCustomAttributes(false) |> Array.map (fun x -> x :?> System.Attribute)).[0]
    printfn "%A" t2

    // Create a new attribute that inherits from the C# one...
    type A1_System_Int32(typeofResult : System.Type, typedefofResult : System.Type) =
        inherit CSAttributes.System_Int32(2147483647)
        member this.TypeofResult    = typeofResult
        member this.TypedefofResult = typedefofResult

#if WITHQUERY
    let q1 = query { for x in CSAttributes.ClassWithAttrs.MethodWithAttrs().AsQueryable() do
                     where (x.HasValue && CSAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType() <> typeof<CSAttributes.ClassWithAttrs>)
                     select (x,CSAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType()) }
    q1.Distinct() |> Seq.iter (fun x -> printfn "%A" x)

    let q2 = query { for x in CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrs().AsQueryable() do
                     where (x.HasValue && CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType() <> typeof<CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs>)
                     select (x,CSAttributes.OuterClassWithNestedAttributes.ClassWithAttrs.MethodWithAttrsThatReturnsAType()) }
    q2.Distinct() |> Seq.iter (fun x -> printfn "%A" x)

#endif
    (* * * * *)

