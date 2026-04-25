#!/bin/bash
# Validates the FS3885 ('and' keyword) deprecation warning behaves correctly:
#   - manual mode: silent (warning gated on cenv.fileOrderAuto)
#   - auto mode: warning fires once per `and`-joined declaration tail
#   - auto mode + --nowarn:3885: silent

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
FSC="$REPO_ROOT/.dotnet/dotnet $REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE="$REPO_ROOT/artifacts/bin/FSharp.Core/Release/netstandard2.0/FSharp.Core.dll"
COMMON_FLAGS="--targetprofile:netcore -r:$FSCORE --nologo --target:library"
SRC="$(dirname "$0")/AndUsage.fs"

export DOTNET_ROOT="$REPO_ROOT/.dotnet"
export PATH="$REPO_ROOT/.dotnet:$PATH"
export DOTNET_GCHeapHardLimit=0x100000000

pass=0
fail=0
tmpout=$(mktemp)
trap 'rm -f "$tmpout" out_dep_*.dll' EXIT

count_3885 () {
    grep -c "FS3885" "$1" || true
}

assert () {
    local label="$1"
    local expected="$2"
    local got="$3"
    if [ "$expected" = "$got" ]; then
        echo "  PASS: $label (FS3885 count=$got)"
        pass=$((pass + 1))
    else
        echo "  FAIL: $label (expected $expected, got $got)"
        fail=$((fail + 1))
    fi
}

echo "--- manual mode (no flag) ---"
$FSC $COMMON_FLAGS -o:out_dep_manual.dll "$SRC" 2>&1 | tee "$tmpout" >/dev/null
assert "manual mode emits no FS3885" 0 "$(count_3885 "$tmpout")"

echo "--- auto mode (--file-order-auto+) ---"
$FSC $COMMON_FLAGS --file-order-auto+ -o:out_dep_auto.dll "$SRC" 2>&1 | tee "$tmpout" >/dev/null
# AndUsage.fs has two `and`-joined groups, each contributes one warning
# (only the tail entries trigger; first head doesn't).
assert "auto mode emits FS3885 for each and-tail (expect 2)" 2 "$(count_3885 "$tmpout")"

echo "--- auto mode + --nowarn:3885 ---"
$FSC $COMMON_FLAGS --file-order-auto+ --nowarn:3885 -o:out_dep_suppress.dll "$SRC" 2>&1 | tee "$tmpout" >/dev/null
assert "--nowarn:3885 suppresses FS3885" 0 "$(count_3885 "$tmpout")"

echo ""
echo "=== Results: $pass passed, $fail failed ==="
exit $fail
