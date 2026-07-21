# Vendored: FSharp.Data.TypeProviders.dll

`FSharp.Data.TypeProviders.dll` (assembly version `4.3.0.0`, file version `3.0.23401.0`,
PublicKeyToken `b03f5f7f11d50a3a`) is the legacy F# type providers redistributable that the
VS 2017 (15.9) F# insertion ships as part of the compiler component.

It originally came from the `Microsoft.VisualFSharp.Type.Providers.Redist` NuGet package
(`content/4.3.0.0/FSharp.Data.TypeProviders.dll`), which is published **only on nuget.org** and is
not mirrored to any of this repo's approved (network-isolated) feeds. The already-shipped, immutable,
Microsoft-Authenticode-signed binary is therefore vendored here — matching the existing vendored-binary
convention in this repo (see `fcs/dependencies/`) — so the servicing build can produce the full insertion
without reaching nuget.org.

Consumed by:
- `src/fsharp/FSharp.Build/FSharp.Build.fsproj` (compile-time reference).
- `setup/Swix/Microsoft.FSharp.Compiler/Files.swr` (staged into `net40/bin` and packaged into the drop).

Do not modify; this is a redistributed, signed Microsoft binary.
