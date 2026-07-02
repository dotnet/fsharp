# G1 report: stale-string hot reload delta decode

## Summary

Verdict on H1: refuted for the minimal session-level shape that emits a delta.

The Giraffe-like handler edit that successfully emitted a session delta did not point its `ldstr`
operand back into the baseline `#US` heap. The baseline `#US` heap size was 92 bytes, and the
updated method body used token `0x7000005D`, offset 93. Offset 93 is the first appended delta
user-string entry after the baseline heap, and it decodes to `"Hello World EDITED"`.

The simple working contrast shows the same fingerprint: baseline `#US` size 36, delta `ldstr`
token `0x70000025`, offset 37, value `"lib generation 1"`.

## Repro test

Added test:

`FSharp.Compiler.Service.Tests.HotReload.HotReloadSessionTests.G1 same-line Giraffe-shaped handler string edit decodes delta user string operand`

Source shape that emits:

```fsharp
module Sample.G1

type HttpFunc = string -> string

let handler1 (_: HttpFunc) (ctx: string) = "Hello World"

let endpointA: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint A"
let endpointB: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint B"
let endpointC: (HttpFunc -> string -> string) = fun (_: HttpFunc) (ctx: string) -> "Endpoint C"
```

Edit: `"Hello World"` to `"Hello World EDITED"` on the same line.

This emitted `Ok` with exactly one updated method, `0x06000001`, decoded as
`Sample.G1::handler1`. It does not reproduce the stale-string bug.

I also tried closer shapes with an endpoint container and the requested probe, including the exact
value-bound handler form from the task (`let handler1: (HttpFunc -> string -> string) = fun ...`).
Those did not produce a delta, so there was no IL to decode:

- Inline endpoint-list closures after `handler1`: `UnsupportedEdit` / `FSHRDL099`, synthesized type
  `Sample.G1.endpoints@8` had no baseline counterpart.
- Named module-level endpoint lambdas plus a list or array container: same `FSHRDL099`, synthesized
  type `Sample.G1.endpoints@12`.
- No endpoint container, but `probe () = handler1 id "x"` or a named equivalent argument:
  `FSHRDL099`, synthesized type `Sample.G1.probe@11` or `probe@13`.

## Giraffe-like decode

Baseline:

- Baseline `#US` heap size: 92
- Updated method: `Sample.G1::handler1`
- Baseline method `ldstr`: `il+0x0000 token=0x70000001 offset=1`
- Baseline offset 1 decodes to: `"Hello World"`

Delta metadata:

- Updated methods: `0x06000001`
- EncLog: `(0x00000001, op=0), (0x06000001, op=0), (0x08000001, op=0), (0x08000002, op=0)`
- EncMap: `0x00000001, 0x06000001, 0x08000001, 0x08000002`
- Delta MethodDef row: `0x06000001`

Delta `#US`:

- Relative offset: 1
- Absolute offset: 93
- Value: `"Hello World EDITED"`
- Encoded bytes:
  `25 48 00 65 00 6C 00 6C 00 6F 00 20 00 57 00 6F 00 72 00 6C 00 64 00 20 00 45 00 44 00 49 00 54 00 45 00 44 00 00`

Delta IL:

- `ldstr`: `il+0x0000 token=0x7000005D offset=93`
- `UserStringUpdates`: original `0x70000001`, new `0x7000005D`, value `"Hello World EDITED"`

Interpretation:

- Offset 93 is greater than the baseline heap size 92.
- The token points into the appended delta `#US` suffix, not into the baseline heap.
- The appended entry at absolute offset 93 is the edited string.

## Working contrast decode

Test:

`FSharp.Compiler.Service.Tests.HotReload.HotReloadSessionTests.G1 simple module string edit decodes working contrast delta user string operand`

Baseline:

- Source: `let libValue () = "lib generation 0"`
- Baseline `#US` heap size: 36
- Updated method: `SessionLib::libValue`
- Baseline method `ldstr`: `il+0x0000 token=0x70000001 offset=1`
- Baseline offset 1 decodes to: `"lib generation 0"`

Delta metadata:

- Updated methods: `0x06000001`
- EncLog: `(0x00000001, op=0), (0x06000001, op=0)`
- EncMap: `0x00000001, 0x06000001`
- Delta MethodDef row: `0x06000001`

Delta `#US`:

- Relative offset: 1
- Absolute offset: 37
- Value: `"lib generation 1"`
- Encoded bytes:
  `21 6C 00 69 00 62 00 20 00 67 00 65 00 6E 00 65 00 72 00 61 00 74 00 69 00 6F 00 6E 00 20 00 31 00 00`

Delta IL:

- `ldstr`: `il+0x0000 token=0x70000025 offset=37`
- `UserStringUpdates`: original `0x70000001`, new `0x70000025`, value `"lib generation 1"`

This is the same appended-offset pattern as the Giraffe-like handler case.

## Analysis

The observed delta does not support the heap-offset hypothesis. In both cases, the original fresh
compile token was `0x70000001`, but the emitter remapped it to `baselineUserStringHeapSize + 1` via
the delta user-string allocator:

- Giraffe-like handler: `92 + 1 = 93`, token `0x7000005D`
- Simple contrast: `36 + 1 = 37`, token `0x70000025`

That is the expected behavior of `UserStringTokenCalculator` in
`src/Compiler/CodeGen/IlxDeltaStreams.fs`. The observed `UserStringUpdates` also match the remap
path in `src/Compiler/CodeGen/IlxDeltaEmitter.fs`: the fresh method body's old-looking token
`0x70000001` is resolved in the fresh assembly to the new text, then rewritten to an absolute
delta token.

So if the real Giraffe endpoint still serves the old string after the runtime accepts an update,
this session-level decode points away from a bad `#US` heap offset in the emitted delta. The next
place I would look is runtime application or method identity/body selection in the real app
topology, especially because the endpoint-container and probe variants hit synthesized-type
alignment failures in this harness before a delta can be emitted.

## Verification

Focused G1 tests:

```text
./.dotnet/dotnet build tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --framework net10.0 --no-restore
./.dotnet/dotnet artifacts/bin/FSharp.Compiler.Service.Tests/Debug/net10.0/FSharp.Compiler.Service.Tests.dll --filter-method "*G1*" --show-live-output on --output Detailed
```

Result: 2 passed, 0 failed.

HotReload service suite:

```text
./.dotnet/dotnet build tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --framework net10.0 --no-restore
./.dotnet/dotnet artifacts/bin/FSharp.Compiler.Service.Tests/Debug/net10.0/FSharp.Compiler.Service.Tests.dll --filter-namespace "FSharp.Compiler.Service.Tests.HotReload" --output Normal
```

Result: 419 passed, 0 failed.
