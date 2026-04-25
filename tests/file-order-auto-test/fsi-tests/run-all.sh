#!/bin/bash
# Drives the .fsi pairing test cases against the locally-built fsc.
# Compares: standard mode (wrong order → expected FAIL) vs --file-order-auto+
# (expected PASS for both partial-fsi and fsi-ordering scenarios).

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
FSC="$REPO_ROOT/.dotnet/dotnet $REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE="$REPO_ROOT/artifacts/bin/FSharp.Core/Release/netstandard2.0/FSharp.Core.dll"
COMMON_FLAGS="--targetprofile:netcore -r:$FSCORE --nologo"

export DOTNET_ROOT="$REPO_ROOT/.dotnet"
export PATH="$REPO_ROOT/.dotnet:$PATH"
export DOTNET_GCHeapHardLimit=0x100000000

pass=0
fail=0

run_case () {
    local name="$1"
    local dir="$2"
    shift 2
    local files=("$@")

    pushd "$dir" >/dev/null

    echo "--- $name (no flag, expect FAIL) ---"
    local out
    out=$($FSC $COMMON_FLAGS --target:library -o:out_baseline.dll "${files[@]}" 2>&1)
    local rc=$?
    rm -f out_baseline.dll
    if [ $rc -ne 0 ]; then
        echo "  baseline correctly failed"
    else
        echo "  UNEXPECTED: baseline succeeded — order may not actually be wrong"
        echo "$out" | tail -5
    fi

    echo "--- $name (--file-order-auto+, expect PASS) ---"
    out=$($FSC $COMMON_FLAGS --file-order-auto+ --target:library -o:out_auto.dll "${files[@]}" 2>&1)
    rc=$?
    rm -f out_auto.dll
    if [ $rc -eq 0 ]; then
        echo "  PASS"
        pass=$((pass + 1))
    else
        echo "  FAIL"
        echo "$out" | tail -10
        fail=$((fail + 1))
    fi

    popd >/dev/null
    echo ""
}

cd "$(dirname "$0")"

# partial-fsi: Main.fs uses Lib (sig+impl pair) and Util (no sig). Wrong order.
run_case "partial-fsi" "partial-fsi" Main.fs Lib.fsi Lib.fs Util.fs

# fsi-ordering: Consumer + Main use Types defined via .fsi/.fs pair. Wrong order.
run_case "fsi-ordering" "fsi-ordering" Main.fs Consumer.fs Types.fsi Types.fs

echo "=== Results: $pass passed, $fail failed ==="
exit $fail
