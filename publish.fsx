#r "nuget: Fun.Build, 0.3.6"

open Fun.Build
open System.IO
open System

let nupkg = "FSharp.Compiler.Service.Bolero.nupkg"

let nugetPushCommand (apiKey: string) : System.FormattableString =
    $"dotnet nuget push {nupkg} -s https://api.nuget.org/v3/index.json --skip-duplicate -k {apiKey}"

pipeline "Publish" {
    description "Publish to NuGet"

    whenAll {
        branch "main"

        whenAny {
            envVar "NUGET_API_KEY"
            cmdArg "NUGET_API_KEY"
        }
    }

    stage "Pack" { run "dotnet pack -c Release ./src/Compiler -o ." }

    stage "Publish" {
        run (fun ctx ->
            let key = ctx.GetCmdArgOrEnvVar "NUGET_API_KEY"
            runSensitive (nugetPushCommand key))
    }

    post
        [ stage "Post publish" {
              whenNot { envVar "GITHUB_ACTION" }

              run (fun _ ->
                  File.Delete nupkg)
          } ]
}