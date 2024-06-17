// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Test.SurfaceArea
    open System
    open System.IO
    open System.Reflection
    open System.Text.RegularExpressions 
        
    // Gets string form of public surface area for the currently-loaded assembly
    let private getSurfaceAreaForAssembly (assembly: Assembly) =

        // get current FSharp.Core
        let fsCoreFullName = assembly.FullName

        // public types only
        let types = assembly.ExportedTypes |> Seq.filter (fun ty -> let ti = ty.GetTypeInfo() in ti.IsPublic || ti.IsNestedPublic) |> Array.ofSeq
        let references = assembly.GetReferencedAssemblies()

        // extract canonical string form for every public member of every type
        let getTypeMemberStrings (t : Type) =
            // for System.Runtime-based profiles, need to do lots of manual work
            let getMembers (t : Type) =
                let ti = t.GetTypeInfo()
                let cast (info: #MemberInfo) = (t, info :> MemberInfo)
                let isDeclaredInFSharpCore (m:MemberInfo) = m.DeclaringType.Assembly.FullName = fsCoreFullName
                seq {
                    yield! ti.DeclaredEvents     |> Seq.filter (fun m -> m.AddMethod.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! ti.DeclaredProperties |> Seq.filter (fun m -> m.GetMethod.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! ti.DeclaredMethods    |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! ti.DeclaredFields     |> Seq.filter (fun m -> m.IsPublic && m |> isDeclaredInFSharpCore) |> Seq.map cast
                    yield! ti.DeclaredConstructors  |> Seq.filter (fun m -> m.IsPublic) |> Seq.map cast
                    yield! ti.DeclaredNestedTypes   |> Seq.filter (fun ty -> ty.IsNestedPublic) |> Seq.map cast
                } |> Array.ofSeq

            getMembers t
            |> Array.map (fun (ty, m) -> sprintf "%s: %s" (ty.ToString()) (m.ToString()))

        let actual = [|
                yield! references |> Array.map(fun name -> $"! AssemblyReference: {name.Name}")
                yield! types |> Array.collect getTypeMemberStrings
            |]
        assembly, actual

    let private appendNewLine str = str + System.Environment.NewLine

    // verify public surface area matches expected, handles baseline update when TEST_UPDATE_BSL is set
    let verify assembly baselinePath outFileName : unit =

        let expected =
            File.ReadAllLines(baselinePath)
            |> String.concat System.Environment.NewLine
            |> appendNewLine

        let normalize (s:string) = Regex.Replace(s, "(\\r\\n|\\n|\\r)+", Environment.NewLine).Trim()
        let asm, actualNotNormalized = getSurfaceAreaForAssembly (assembly)
        let actual = 
            actualNotNormalized 
            |> Seq.map normalize 
            |> Seq.filter (String.IsNullOrWhiteSpace >> not)
            |> Seq.sort
            |> String.concat Environment.NewLine

        let expected = normalize expected

        let logFile =
            Path.Combine(Path.GetDirectoryName(assembly.Location), outFileName)

        File.WriteAllText(logFile, actual)

        match Assert.shouldBeSameMultilineStringSets expected actual with
        | None -> ()
        | Some diff ->
            // Update baselines here
            match Environment.GetEnvironmentVariable("TEST_UPDATE_BSL") with
            | null -> ()
            | _ -> File.Copy(logFile, baselinePath, true)
                
            let msg = $"""Assembly: %A{asm}

              Expected and actual surface area don't match. To see the delta, run:
                  git diff --no-index --word-diff {baselinePath} {logFile}

              {diff}"""

            failwith msg
