#!/bin/bash
# Inference sensitivity test suite for --file-order-auto
# Tests that auto-ordering produces correct compilation for each inference pattern.
# Run inside Docker container after a successful build.

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
CUSTOM_FSC="$REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"

echo "=== Inference Sensitivity Test Suite ==="
echo ""

if [ ! -f "$CUSTOM_FSC" ]; then
    echo "ERROR: Custom compiler not found at $CUSTOM_FSC"
    exit 1
fi

PASS=0
FAIL=0
TESTS=""

for test_dir in "$(dirname "$0")"/*/; do
    if [ ! -f "$test_dir"/*.fsproj 2>/dev/null ]; then
        continue
    fi

    proj=$(ls "$test_dir"*.fsproj 2>/dev/null | head -1)
    test_name=$(basename "$test_dir")

    echo "--- $test_name ---"

    # Test: wrong order + --file-order-auto+ should compile
    output=$(dotnet build "$proj" -v:quiet \
        -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
        -p:OtherFlags="--file-order-auto+" \
        2>&1)
    exit_code=$?

    error_count=$(echo "$output" | grep -c "Error(s)")
    errors=$(echo "$output" | grep "error FS" | head -3)

    if [ $exit_code -eq 0 ]; then
        echo "  PASS"
        PASS=$((PASS + 1))
    else
        echo "  FAIL"
        echo "$errors" | sed 's/^/    /'
        FAIL=$((FAIL + 1))
    fi
    TESTS="$TESTS\n  $test_name: $([ $exit_code -eq 0 ] && echo PASS || echo FAIL)"
done

echo ""
echo "=== Results: $PASS passed, $FAIL failed ==="
echo -e "$TESTS"

if [ $FAIL -eq 0 ]; then
    echo ""
    echo "ALL INFERENCE TESTS PASSED"
    exit 0
else
    echo ""
    echo "SOME INFERENCE TESTS FAILED"
    exit 1
fi
