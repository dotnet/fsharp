# Show Call Stack on Code Map — manual repro

Every call shape an F# stack can take, in one solution, for exercising
**Debug ▸ Show Call Stack on Code Map** by hand. Not part of any solution the build
produces, and no CI job restores it.

The unit tests for the same shapes live in `vsintegration/tests/FSharp.Editor.Tests/CodeMap`.
They run against `CallStackSample.fs`, which is compiled into the test assembly and captures
its own frames at runtime, so the frame corpus there is observed rather than written down.
This solution is what those shapes look like to a debugger.

## Layout

| Project | Why |
|---|---|
| `ClassLibrary` (F#) | `Demo.fs` — the scenarios. Every one funnels into `Demo.sink`. |
| `ConsoleApp` (C#) | Calls them through `Driver.Run` → `Driver.Dispatch`, so every stack has C# frames above the F# ones. |

## Running it

1. Build the `VisualFSharp` solution and start the experimental VS instance (`devenv /rootSuffix RoslynDev`).
2. Open this solution in it, put a breakpoint on `printfn` inside `Demo.sink`, and start debugging.
3. At each stop: **Debug ▸ Show Call Stack on Code Map**, then continue.

Two things about the map are worth knowing before reading it:

- A map document **accumulates** across stops and across debug sessions — the pipeline removes
  neither nodes nor links. Close it and open a new one when you want to read one stack.
- Resolution is cached per debug session (`CallStackCache`, cleared only when debugging starts
  or stops), so the first answer for a given frame name is the one you keep. This matters for
  `<StartupCode$…>` frames: a file's module-level bindings and the `static let` of a type
  declared in it all run in **one** compiler-generated method, so they share a node.
