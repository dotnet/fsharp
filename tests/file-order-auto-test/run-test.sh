#!/bin/bash
# End-to-end test for --file-order-auto flag
# Must be run from the repo root inside the Docker container after a successful build.

set -e

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
TEST_DIR="$REPO_ROOT/tests/file-order-auto-test"
FSC="$REPO_ROOT/artifacts/bin/fsc/Debug/net10.0/fsc.dll"

echo "=== Test: File Order Auto ==="
echo "Repo root: $REPO_ROOT"
echo "Test dir:  $TEST_DIR"
echo "Compiler:  $FSC"
echo ""

if [ ! -f "$FSC" ]; then
    echo "ERROR: Compiler not found at $FSC"
    echo "Available artifacts:"
    ls "$REPO_ROOT/artifacts/bin/fsc/" 2>/dev/null || echo "  (none)"
    exit 1
fi

cd "$TEST_DIR"

# Collect source files in the WRONG order (as listed in fsproj)
FILES="FileC.fs FileB.fs FileA.fs Program.fs"

echo "--- Test 1: Normal compilation (wrong file order) — should FAIL ---"
if dotnet "$FSC" $FILES -o:test_normal.dll --targetprofile:netcore --noframework -r:"$(dotnet --info | grep 'Base Path' | awk '{print $3}')/../../shared/Microsoft.NETCore.App/10.0.*/System.Runtime.dll" 2>&1; then
    echo "UNEXPECTED: Normal compilation succeeded with wrong file order!"
    echo "TEST 1: FAIL (expected failure, got success)"
    RESULT1="UNEXPECTED_SUCCESS"
else
    echo "Expected failure — normal compiler rejects wrong file order."
    echo "TEST 1: PASS"
    RESULT1="PASS"
fi

echo ""
echo "--- Test 2: Compilation with --file-order-auto+ (wrong file order) — should SUCCEED ---"
if dotnet "$FSC" --file-order-auto+ $FILES -o:test_auto.dll --targetprofile:netcore --noframework -r:"$(dotnet --info | grep 'Base Path' | awk '{print $3}')/../../shared/Microsoft.NETCore.App/10.0.*/System.Runtime.dll" 2>&1; then
    echo "TEST 2: PASS — file-order-auto correctly resolved dependencies!"
else
    echo "TEST 2: FAIL — file-order-auto did not resolve dependencies."
    RESULT2="FAIL"
fi

echo ""
echo "=== Results ==="
echo "Test 1 (normal, wrong order → expect fail): ${RESULT1:-PASS}"
echo "Test 2 (auto order, wrong order → expect pass): ${RESULT2:-PASS}"

if [ "$RESULT1" = "PASS" ] && [ "${RESULT2:-PASS}" = "PASS" ]; then
    echo ""
    echo "ALL TESTS PASSED"
    exit 0
else
    echo ""
    echo "SOME TESTS FAILED"
    exit 1
fi
