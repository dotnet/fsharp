#!/bin/bash
# Test --file-order-auto+ on a real F# project by shuffling file order.
# Usage: ./test-real-project.sh <path-to-fsproj> [custom-fsc-dll]
#
# The script:
# 1. Copies the fsproj to a .shuffled.fsproj
# 2. Randomizes the order of <Compile Include="..."> lines
# 3. Builds the shuffled version with the custom compiler + --file-order-auto+
# 4. Reports pass/fail

set -u

PROJ="${1:?Usage: $0 <path-to-fsproj> [custom-fsc-dll]}"
REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CUSTOM_FSC="${2:-$REPO_ROOT/artifacts/bin/fsc/Debug/net10.0/fsc.dll}"

if [ ! -f "$PROJ" ]; then
    echo "ERROR: Project file not found: $PROJ"
    exit 1
fi

if [ ! -f "$CUSTOM_FSC" ]; then
    echo "ERROR: Custom compiler not found: $CUSTOM_FSC"
    exit 1
fi

PROJ_DIR="$(dirname "$PROJ")"
PROJ_NAME="$(basename "$PROJ" .fsproj)"
SHUFFLED="$PROJ_DIR/${PROJ_NAME}.shuffled.fsproj"

echo "=== Testing: $PROJ ==="
echo "Compiler: $CUSTOM_FSC"

# Step 1: Verify normal build works
echo ""
echo "--- Step 1: Normal build (baseline) ---"
dotnet build "$PROJ" -v:quiet 2>&1 | tail -3
if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo "  SKIP: Normal build failed — project may need setup. Skipping."
    rm -f "$SHUFFLED"
    exit 2
fi
echo "  Baseline: OK"

# Step 2: Create shuffled fsproj
echo ""
echo "--- Step 2: Shuffling file order ---"

# Extract Compile lines, shuffle them, rebuild the fsproj
python3 -c "
import re, random, sys

with open('$PROJ', 'r') as f:
    content = f.read()

# Find all Compile Include lines
pattern = r'(\s*<Compile Include=\"[^\"]+\"\s*/?>)'
matches = re.findall(pattern, content)

if len(matches) < 2:
    print(f'  Only {len(matches)} Compile items — nothing to shuffle.')
    sys.exit(0)

# Shuffle
random.seed(42)  # deterministic for reproducibility
shuffled = matches[:]
random.shuffle(shuffled)

# Replace in order
result = content
for orig, shuf in zip(matches, shuffled):
    result = result.replace(orig, '###PLACEHOLDER###', 1)
for shuf in shuffled:
    result = result.replace('###PLACEHOLDER###', shuf, 1)

with open('$SHUFFLED', 'w') as f:
    f.write(result)

print(f'  Shuffled {len(matches)} Compile items.')
for m in shuffled[:5]:
    print(f'    {m.strip()}')
if len(shuffled) > 5:
    print(f'    ... and {len(shuffled)-5} more')
"

if [ ! -f "$SHUFFLED" ]; then
    echo "  Nothing to shuffle."
    exit 0
fi

# Step 3: Build shuffled version with custom compiler + flag
echo ""
echo "--- Step 3: Build shuffled project with --file-order-auto+ ---"
dotnet build "$SHUFFLED" -v:quiet \
    -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
    -p:OtherFlags="--file-order-auto+" \
    2>&1 | tail -5
BUILD_EXIT=${PIPESTATUS[0]}

# Cleanup
rm -f "$SHUFFLED"

echo ""
if [ $BUILD_EXIT -eq 0 ]; then
    echo "=== PASS: $PROJ_NAME compiled with shuffled file order ==="
    exit 0
else
    echo "=== FAIL: $PROJ_NAME did not compile with shuffled file order ==="
    exit 1
fi
