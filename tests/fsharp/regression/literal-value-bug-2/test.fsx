open System
open System.Linq
open System.Reflection

type thisAssembly (_dummy:System.Object) = class end

module public MyLiteralFields =
    [<Literal>]
    let public literalFieldX = 7

printfn "MyFields.literalFieldX = %d" MyLiteralFields.literalFieldX

// Use dotnet reflection to verify that x is a Literal Constant
// This works on full desktop / coreclr and also fsi
let asm =   thisAssembly(null).GetType().GetTypeInfo().Assembly
let typ =  asm.GetTypes() |> Array.filter(fun t -> t.FullName.EndsWith("+MyLiteralFields")) |> Array.tryLast
let result =
    match typ with
    | Some typ ->
        // Gets literalFieldX checks to see if it is marked "Literal"
        let fieldInfo = typ.GetTypeInfo().DeclaredFields |> Seq.tryFind(fun fi -> fi.Name.StartsWith("literalFieldX"))
        match fieldInfo with
        | Some fieldInfo -> 
            if fieldInfo.IsLiteral = true then 0 else 2
        | None -> 
            printfn "Failed to find fieldliteralFieldX ="
            3
    | None ->
        printfn "Failed to find module public MyLiteralFields ="
        1

if result = 0 then 
    printf "TEST PASSED OK"; 
    printfn "Succeeded" 
else
    printfn "Failed: %d" result
exit result
