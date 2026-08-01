# GitHub Copilot Instructions for csharp_asyncruntime.csproj

This project uses the new async/await patterns as specified in the .NET runtime design document: 

https://github.com/dotnet/runtime/blob/main/docs/design/specs/runtime-async.md

To use the feature, a number of flags need to be enabled:

- The project must target net10.0 (note: running on net10.0 is not sufficient, it must compile against net10.0 to see the value)
- <EnablePreviewFeatures>true</EnablePreviewFeatures> must be set in the project file
- <Features>$(Features);runtime-async=on</Features> must be set in the project file
- When running, the environment variable DOTNET_RuntimeAsync=1 must be set

This project has a special target called "Decompile" that will decompile the emitted IL back to C# code so you can see the emitted IL patterns. 

The output will end up in `artifacts\bin\csharp_asyncruntime\Debug\net10.0\csharp_asyncruntime.il`. 

Notably, you will see the new async IL patterns as specified in the design document.

- `AsyncMethod() cil managed async` indicates that the method is async.
- Async methods also do not have matching return type conventions as sync methods. For sync methods, the stack should contain a value convertible to the stated return type before the ret instruction. For async methods, the stack should be empty in the case of Task or ValueTask, or the type argument in the case of Task<T> or ValueTask<T>.

Use this project as a reference for how the IL should look when emitting async methods from C# code.

### Relevant commands

`dotnet build` - builds the project
`dotnet run` - runs the project
