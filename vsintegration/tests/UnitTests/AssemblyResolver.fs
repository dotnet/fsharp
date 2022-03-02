namespace Microsoft.VisualStudio.FSharp

open NUnit.Framework
open System
open System.IO
open System.Reflection

module AssemblyResolver =
    open System.Globalization

    let vsInstallDir =
        // use the environment variable to find the VS installdir
        let vsvar =
            let var = Environment.GetEnvironmentVariable("VS170COMNTOOLS")
            if String.IsNullOrEmpty var then
                Environment.GetEnvironmentVariable("VSAPPIDDIR")
            else
                var
        if String.IsNullOrEmpty vsvar then failwith "VS170COMNTOOLS and VSAPPIDDIR environment variables not found."
        Path.Combine(vsvar, "..")

    let probingPaths = [|
        Path.Combine(vsInstallDir, @"IDE\CommonExtensions\Microsoft\Editor")
        Path.Combine(vsInstallDir, @"IDE\PublicAssemblies")
        Path.Combine(vsInstallDir, @"IDE\PrivateAssemblies")
        Path.Combine(vsInstallDir, @"IDE\CommonExtensions\Microsoft\ManagedLanguages\VBCSharp\LanguageServices")
        Path.Combine(vsInstallDir, @"IDE\Extensions\Microsoft\CodeSense\Framework")
        Path.Combine(vsInstallDir, @"IDE")
    |]

    let addResolver () =
        AppDomain.CurrentDomain.add_AssemblyResolve(fun h args ->
            let found () =
                (probingPaths ) |> Seq.tryPick(fun p ->
                    try
                        let name = AssemblyName(args.Name)
                        let codebase = Path.GetFullPath(Path.Combine(p, name.Name) + ".dll")
                        if File.Exists(codebase) then
                            name.CodeBase <- codebase
                            name.CultureInfo <- Unchecked.defaultof<CultureInfo>
                            name.Version <- Unchecked.defaultof<Version>
                            Some (name)
                        else None
                    with | _ -> None
                    )
            match found() with
            | None -> Unchecked.defaultof<Assembly>
            | Some name -> Assembly.Load(name) )

[<SetUpFixture>]
type public AssemblyResolverTestFixture () =

    [<OneTimeSetUp>]
    member public _.Init () = AssemblyResolver.addResolver ()
