#!/bin/bash
# End-to-end smoke: scaffold a fresh project from `dotnet new`, scramble its
# file order, enable FSharpAutoFileOrder, and verify it builds + runs.
# Use the locally-built fsc by overriding DotnetFscCompilerPath.

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
FSC="$REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE="$REPO_ROOT/artifacts/bin/FSharp.Core/Release/netstandard2.0/FSharp.Core.dll"

if [ ! -f "$FSC" ]; then
    echo "ERROR: built fsc not found at $FSC" >&2
    exit 1
fi

export DOTNET_ROOT="$REPO_ROOT/.dotnet"
export PATH="$REPO_ROOT/.dotnet:$PATH"
export DOTNET_GCHeapHardLimit=0x100000000

work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT
cd "$work"

echo "--- Step 1: scaffold a fresh F# console app ---"
dotnet new console -lang F# -n EndToEndAuto -o EndToEndAuto >/dev/null
cd EndToEndAuto

echo "--- Step 2: add a few interdependent files in deliberately wrong order ---"
cat > Geometry.fs <<'EOF'
module EndToEndAuto.Geometry

let area (radius: float) = MathHelpers.pi * radius * radius
EOF

cat > MathHelpers.fs <<'EOF'
module EndToEndAuto.MathHelpers

let pi = 3.141592653589793
let square (x: float) = x * x
EOF

# Replace Program.fs with one that references both
cat > Program.fs <<'EOF'
module EndToEndAuto.Program

[<EntryPoint>]
let main _ =
    let r = 2.5
    let a = EndToEndAuto.Geometry.area r
    let s = EndToEndAuto.MathHelpers.square r
    printfn "radius=%f area=%f square=%f" r a s
    0
EOF

# Wrong order: Program references both, then Geometry which depends on MathHelpers.
# NOTE: <FSharpAutoFileOrder>true</FSharpAutoFileOrder> in PropertyGroup is the
# documented user-facing knob, but `dotnet build` uses the SDK-installed
# FSharp.Build task which doesn't yet know to translate it. Until our
# FSharp.Build ships in an SDK, the flag is passed via OtherFlags below.
cat > EndToEndAuto.fsproj <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <FSharpAutoFileOrder>true</FSharpAutoFileOrder>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Program.fs" />
    <Compile Include="Geometry.fs" />
    <Compile Include="MathHelpers.fs" />
  </ItemGroup>
</Project>
EOF

echo "--- Step 3: build with --file-order-auto+ + locally-built fsc ---"
build_log=$(mktemp)
trap 'rm -rf "$work" "$build_log"' EXIT
if ! dotnet build EndToEndAuto.fsproj -c Release \
        -p:DotnetFscCompilerPath="$FSC" \
        -p:OtherFlags="--file-order-auto+" \
        >"$build_log" 2>&1; then
    echo "  FAIL: build failed"
    tail -20 "$build_log"
    exit 1
fi
echo "  PASS: build succeeded"

echo "--- Step 4: run the built exe and verify output ---"
out=$(dotnet bin/Release/net10.0/EndToEndAuto.dll 2>&1)
echo "  stdout: $out"
expected="radius=2.500000 area=19.634954 square=6.250000"
if [ "$out" = "$expected" ]; then
    echo "  PASS: output matches expected"
else
    echo "  FAIL: output mismatch"
    echo "  expected: $expected"
    exit 1
fi

echo ""
echo "=== End-to-end smoke: PASS ==="
