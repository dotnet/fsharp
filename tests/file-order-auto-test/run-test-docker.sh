#!/bin/bash
# End-to-end tests for order-independent compilation (Track 01 + Track 02)
# Run this inside the Docker container after a successful build.
#
# Tests:
#   1. Standard compiler rejects wrong file order (baseline)
#   2. Custom compiler + --file-order-auto+ with WRONG file order → should SUCCEED (Track 02)
#   3. Correct file order + --file-order-auto+ → no regression
#   4. Correct file order, no flag → default behavior preserved

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
TEST_DIR="$REPO_ROOT/tests/file-order-auto-test"
CUSTOM_FSC="$REPO_ROOT/artifacts/bin/fsc/Debug/net10.0/fsc.dll"

echo "=== Order-Independent Compilation Tests ==="
echo ""

if [ ! -f "$CUSTOM_FSC" ]; then
    echo "ERROR: Custom compiler not found at $CUSTOM_FSC"
    exit 1
fi

cd "$TEST_DIR"

PASS=0
FAIL=0

# --- Test 1: Wrong file order with standard compiler should FAIL ---
echo "--- Test 1: Standard compiler, wrong file order → expect FAIL ---"
dotnet build FileOrderAutoTest.fsproj -v:quiet 2>&1 | tail -3
if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo "  PASS: Standard compiler correctly rejects wrong file order."
    PASS=$((PASS + 1))
else
    echo "  UNEXPECTED: Standard compiler accepted wrong file order."
    FAIL=$((FAIL + 1))
fi
echo ""

# --- Test 2: THE BIG ONE — Wrong file order + custom compiler + flag should SUCCEED ---
echo "--- Test 2: WRONG file order + custom compiler + --file-order-auto+ → expect PASS ---"
dotnet build FileOrderAutoTest.fsproj -v:quiet \
    -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
    -p:OtherFlags="--file-order-auto+" \
    2>&1 | tail -5
if [ ${PIPESTATUS[0]} -eq 0 ]; then
    echo "  PASS: Auto file ordering resolved wrong file order!"
    PASS=$((PASS + 1))
else
    echo "  FAIL: Auto file ordering did not resolve wrong file order."
    FAIL=$((FAIL + 1))
fi
echo ""

# --- Test 3: Correct file order with custom compiler + flag should SUCCEED ---
echo "--- Test 3: Correct file order + custom compiler + --file-order-auto+ → expect PASS ---"
cat > FileOrderAutoTest_CorrectOrder.fsproj <<'PROJ'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="FileA.fs" />
    <Compile Include="FileB.fs" />
    <Compile Include="FileC.fs" />
    <Compile Include="Program.fs" />
  </ItemGroup>
</Project>
PROJ

dotnet build FileOrderAutoTest_CorrectOrder.fsproj -v:quiet \
    -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
    -p:OtherFlags="--file-order-auto+" \
    2>&1 | tail -3
if [ ${PIPESTATUS[0]} -eq 0 ]; then
    echo "  PASS: Custom compiler + flag works with correct file order (no regression)."
    PASS=$((PASS + 1))
else
    echo "  FAIL: Custom compiler + flag broke correct file order."
    FAIL=$((FAIL + 1))
fi
echo ""

# --- Test 4: Correct file order WITHOUT flag should also SUCCEED ---
echo "--- Test 4: Correct file order + custom compiler, NO flag → expect PASS ---"
dotnet build FileOrderAutoTest_CorrectOrder.fsproj -v:quiet \
    -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
    2>&1 | tail -3
if [ ${PIPESTATUS[0]} -eq 0 ]; then
    echo "  PASS: Custom compiler works without flag (default mode preserved)."
    PASS=$((PASS + 1))
else
    echo "  FAIL: Custom compiler failed without flag."
    FAIL=$((FAIL + 1))
fi
echo ""

# Cleanup
rm -f FileOrderAutoTest_CorrectOrder.fsproj

echo "=== Results: $PASS passed, $FAIL failed ==="
if [ $FAIL -eq 0 ]; then
    echo "ALL TESTS PASSED"
    exit 0
else
    echo "SOME TESTS FAILED"
    exit 1
fi
