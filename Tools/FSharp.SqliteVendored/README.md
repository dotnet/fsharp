## SourceGear.Sqlite3 vendor (build-time DLL acquisition)

This directory vendors the SourceGear.Sqlite3 native `e_sqlite3.dll` into the F# 15.9 VSIX payload at build time.

## Design

The DLLs are NOT committed to git (Microsoft FCIB policy prohibits .dll commits). Instead:

1. `SourceGearSqlite.csproj` has a `<PackageReference Include="SourceGear.Sqlite3" Version="3.50.4.5" />`
2. NuGet restore pulls the nupkg into the local cache (`~/.nuget/packages/sourcegear.sqlite3/3.50.4.5/`)
3. The `AcquireSourceGearDll` MSBuild target (in this csproj) runs `BeforeTargets="Build"` and copies the DLLs from the cache into `runtimes/win-x86/native/e_sqlite3.dll` and `runtimes/win-x64/native/e_sqlite3.dll`
4. From there they get picked up by:
   - `vsintegration/Vsix/VisualFSharpFull/VisualFSharpFull.csproj` `<Content Include>` rows (commit 6.0)
   - `setup/Swix/Microsoft.FSharp.SDK/SourceGearSqlite.swr` payload entries (commit 6.0)
   - `setup/Swix/Microsoft.FSharp.SDK/SourceGearSqlite.signproj` `FilesToSign` (commit 7.0)
5. The signed VSIX ships the DLLs to `Common7\IDE\CommonExtensions\Microsoft\FSharp\runtimes\<rid>\native\e_sqlite3.dll`

## SHA pins (§3.3 [L.2]/[L.3])

| Path | SHA-256 |
|---|---|
| `runtimes/win-x86/native/e_sqlite3.dll` | `61544338560CCF41A665BAC6FABA5A2270CD0088DCDF63F6906AB7CCF6B34E5E` |
| `runtimes/win-x64/native/e_sqlite3.dll` | `AABF85D7A8B416FB15203CB754FCFC9858C8F1DD3BBC7EAB82335F6C362BA0D6` |

These are verified by `AcquireSourceGearDll` target — mismatch throws.

## Why this design

The original §10 plan committed the DLLs directly. Microsoft's FCIB (Foreign Checked-In Binaries) policy on AzDO internal prohibits .dll commits — they cite EO 14028, CRA Article 13, SSDF PO.3.1/PS.2. The build-time acquisition pattern keeps the binary OUT of git but still lands it in the VSIX payload, satisfying both:
- FCIB: no binary in git
- Ship goal: DLL still ends up in `Common7\IDE\CommonExtensions\Microsoft\FSharp\`
