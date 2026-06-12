# Trying F# Hot Reload

Edit a **running** F# app and watch the change apply in place — no restart, state preserved.
This guide takes you from two `git clone`s to a live hot-reload session. Budget ~30–45
minutes, most of it waiting on the two builds.

You need: git, bash (macOS/Linux; Windows works with the `.cmd` equivalents), and ~10 GB of
disk for the two builds. You do **not** need any .NET SDK preinstalled — both repos bootstrap
their own.

## 1. Build the compiler (this repo, branch `hot-reload-v2`)

```bash
git clone --branch hot-reload-v2 --single-branch https://github.com/NatElkins/fsharp fsharp-hotreload
cd fsharp-hotreload
./build.sh -c Debug          # bootstraps a repo-local SDK, then builds; ~15-25 min first time
cd ..
```

## 2. Build the SDK with the F#-aware dotnet-watch (branch `fsharp-hotreload-watch-v2`)

```bash
git clone --branch fsharp-hotreload-watch-v2 --single-branch https://github.com/NatElkins/sdk sdk-hotreload
cd sdk-hotreload
./build.sh                   # ~10-20 min first time
cd ..
```

This produces a complete, runnable .NET CLI at `sdk-hotreload/artifacts/bin/redist/Debug/dotnet`.

## 3. Point the SDK at the hot-reload compiler

The SDK ships a stock F# compiler; replace its `FSharp.Compiler.Service` (and the matching
`FSharp.Core`) with the ones you just built:

```bash
REDIST="$PWD/sdk-hotreload/artifacts/bin/redist/Debug/dotnet"
FCS="$PWD/fsharp-hotreload/artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"

cp "$FCS/FSharp.Compiler.Service.dll" "$REDIST"/sdk/*/FSharp/
cp "$FCS/FSharp.Core.dll"             "$REDIST"/sdk/*/FSharp/
```

## 4. Create a demo app and start watching

```bash
mkdir HotReloadDemo && cd HotReloadDemo

# Environment for the locally built CLI (paste as-is; $REDIST from step 3)
export DOTNET_ROOT="$REDIST"
export DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR="$REDIST"
export DOTNET_MULTILEVEL_LOOKUP=0
export PATH="$REDIST:$PATH"

dotnet new console -lang F#
```

Open the `.fsproj` and add one line inside the `<PropertyGroup>` — this is the hot reload
opt-in flag:

```xml
<OtherFlags>$(OtherFlags) --enable:hotreloaddeltas</OtherFlags>
```

Replace `Program.fs` with something that loops, so you can see updates land:

```fsharp
type Greeter() =
    let mutable count = 0

    member _.Message() =
        count <- count + 1
        sprintf "hello (count: %d)" count

let greeter = Greeter()

while true do
    printfn "%s" (greeter.Message())
    System.Threading.Thread.Sleep(1000)
```

Run it:

```bash
dotnet watch run --non-interactive
```

You should see `⌚ F# hot reload session prestarted`, then the counter ticking once a second.

## 5. Edit and save — the counter is your proof

The `count` keeps climbing through every successful in-place apply. If it resets to 1, the
process restarted. Try these, saving after each:

1. **Change the string** — `"hello"` → anything. Applies in ~2–3 s.
2. **Add a lambda** (the thing F# hot reload could never do before):
   ```fsharp
   member _.Message() =
       count <- count + 1
       let parts = [ "hot"; "reload" ] |> List.map (fun s -> s.ToUpper())
       sprintf "%s (count: %d)" (String.concat " " parts) count
   ```
   This synthesizes a brand-new closure class and patches it into the running process.
3. **Edit the lambda's body** — `ToUpper` → `ToLower`. The added closure is updated in place.
4. **Add a member, a `let mutable` field, a property, a whole new type or module** and use it
   from `Message()` — all apply in place.
5. **Save with only a comment/whitespace change** — detected as a no-op, nothing happens.
6. **Break it on purpose** — change `Message()` to take a parameter. That's a *rude edit*
   (same as C#): the console names the reason and the app restarts cleanly (counter resets).

## What's supported

Method-body edits (including closures, `async`, resume-point-stable `task`, generics);
adding methods, functions, module values, fields, properties, events, and new types
(classes/records/unions/structs/modules/enums/interfaces/delegates); attribute edits;
parameter renames; multiple projects in one watch session. The design docs in this folder
(`hot-reload-architecture.md` is the entry point) record exactly what stays a rude edit
and why — generally the same set as C#.

## Troubleshooting

- **Everything restarts instead of applying**: confirm the `OtherFlags` line is in the
  `.fsproj` (the on-disk build and the in-memory compiles must both carry the flag), and that
  step 3's copy actually overwrote the SDK's `FSharp/FSharp.Compiler.Service.dll`.
- **Want to see why a specific edit restarted**: `export DOTNET_WATCH_TRACE_FSHARP_HOTRELOAD=1`
  before `dotnet watch run` — the F# service logs its classification (e.g. which rude edit or
  missing runtime capability) for every emit.
- **First edit is slow**: the first delta includes compiler-service warm-up; subsequent edits
  are ~2–3 s. (Reducing this is roadmapped — the bulk is an FCS recheck that composes with
  the in-flight compiler caching work in
  [dotnet/fsharp#19267](https://github.com/dotnet/fsharp/pull/19267).)
