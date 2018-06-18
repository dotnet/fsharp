open System
open System.IO
open System.Reflection
open System.Reflection.Emit
try
    let name = "TestLibrary"
    let filename = name + ".dll"

    let asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName(name), AssemblyBuilderAccess.Save, __SOURCE_DIRECTORY__)
    let modBuilder = asmBuilder.DefineDynamicModule(name, filename)
    let tyBuilder = modBuilder.DefineType("TestType", TypeAttributes.Class ||| TypeAttributes.Public)
    let methBuilder = 
        tyBuilder.DefineMethod(
            name = "Get", 
            attributes = (MethodAttributes.Public ||| MethodAttributes.Static), 
            callingConvention = CallingConventions.Standard,
            returnType = typeof<int>,
            returnTypeRequiredCustomModifiers = null,
            returnTypeOptionalCustomModifiers = null,
            parameterTypes = [| typeof<int> |],
            parameterTypeRequiredCustomModifiers = null,
            parameterTypeOptionalCustomModifiers = [| [| typeof<System.Runtime.CompilerServices.IsConst> |] |]
            )
    do
        let ilG = methBuilder.GetILGenerator()
        ilG.Emit(OpCodes.Ldarg_0)
        ilG.Emit(OpCodes.Ret)

    tyBuilder.CreateType() 
    |> ignore

    asmBuilder.Save(filename)
    exit 0
with err -> 
    printfn "Error: %A" err
    exit 1