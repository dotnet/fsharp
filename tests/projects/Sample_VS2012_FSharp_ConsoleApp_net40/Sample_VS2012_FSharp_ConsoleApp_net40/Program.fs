// This is a sample F# app created in Visual Studio 2012, targeting .NET 4.0

// On Windows, the build should reference
//    -r:"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\mscorlib.dll"
//    -r:"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.dll"
//    -r:"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Core.dll"
//    -r:"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Numerics.dll"
//  and should reference one of these depending on the language version of F# being used
//    -r:"C:Program Files\Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v4.0\FSharp.Core.dll"
//    -r:"C:Program Files\Reference Assemblies\Microsoft\FSharp\3.0\Runtime\v4.0\FSharp.Core.dll"
//
// On Mac, you'll get something like this:
//    -r:"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.0/FSharp.Core.dll"
//    -r:"/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/mscorlib.dll"
//    -r:"/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/System.dll"
//    -r:"/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/System.Core.dll"
//    -r:"/Library/Frameworks/Mono.framework/Versions/2.10.9/lib/mono/4.0/System.Numerics.dll"

module M 

type C() = 
   member val x = 1
   
System.Console.WriteLine "Helo World"

   
[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // return an integer exit code

