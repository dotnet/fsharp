(* SHOULD GIVE UnionCase1 NICE ERROR INCLUDING THE NAME "Ascii": *)
module Test

let test() =
    try
        failwith "message"
    with UndeclaredException ->
        System.Console.WriteLine "UndeclaredException"
        
do test()



module TestRequiredQualifiedAccess = begin
    [<RequireQualifiedAccess>]
    type X = 
       | UnionCase1 
       | UnionCase2
       with 
          member x.M() = match x with UnionCase1 -> 1 | UnionCase2 -> 2 // expect ok, since inside the member of the type
          member x.N() = (UnionCase1, UnionCase2)                       // expect ok, since inside the member of the type
       end

    let _ = UnionCase1 // expect not ok
    let f x = 
        match x with 
        | UnionCase1 -> 1 // expect not ok with warnings about uppercase variable names
        | UnionCase2 -> 2 // expect not ok with warnings about uppercase variable names

    let x = X.UnionCase1 // expect ok
    let x2 : X = UnionCase1 // expect not ok

    let g (x:X) = 
        match x with
        | UnionCase1 -> ()  // expect not ok with warnings about uppercase variable names
        | UnionCase2 -> ()  // expect not ok with warnings about uppercase variable names

    [<RequireQualifiedAccess>]
    type R = 
        { RecordLabel1 : int; RecordLabel2 : int }
        with 
           member x.M() = { RecordLabel1 = 1; RecordLabel2 = 2 } // expect ok, since inside the member of the type
           member x.N() = (fun y -> y.RecordLabel1)              // expect ok, since inside the member of the type
        end

    let _ = { RecordLabel1 = 1; RecordLabel2 = 2 }  // expect not ok
    let _ = fun { RecordLabel1 = a; RecordLabel2 = b } -> a + b // expect not ok

    let _ = { R.RecordLabel1 = 1; R.RecordLabel2 = 2 } // expect ok
    let xR: R = { RecordLabel1 = 1; RecordLabel2 = 2 } // expect ok since resolution is inferred by type
end


module TestUsNullAttribute = begin
    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyUnion = 
        | A1
        | A
        | B of string

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyUnion2 =
        | A1
        | A 

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyUnion3 = 
        | A1

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyRecord3 = { x : int }

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyClass4() = 
       class
          member x.P = 1
       end

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyInterface5 = 
       interface 
           abstract P : int
       end

    // expect error
    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    type MyStruct6(x : int) = 
       struct
           member __.X = x
       end

end
