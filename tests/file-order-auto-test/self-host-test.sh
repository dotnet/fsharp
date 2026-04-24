#!/bin/bash
# Self-host test: compile the F# compiler itself with shuffled file order.
# Must be run inside Docker after a successful build.

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
CUSTOM_FSC="$REPO_ROOT/artifacts/bin/fsc/Debug/net10.0/fsc.dll"
PROJ="$REPO_ROOT/src/Compiler/FSharp.Compiler.Service.fsproj"
SHUFFLED="$REPO_ROOT/src/Compiler/FSharp.Compiler.Service.shuffled.fsproj"

echo "=== Self-Host Test: Compile F# Compiler with Shuffled File Order ==="
echo ""

if [ ! -f "$CUSTOM_FSC" ]; then
    echo "ERROR: Custom compiler not found at $CUSTOM_FSC"
    exit 1
fi

COMPILE_COUNT=$(grep -c '<Compile Include=' "$PROJ")
echo "Project: $PROJ"
echo "Compile items: $COMPILE_COUNT"
echo ""

# Step 1: Shuffle
# Strategy: number each line, tag Compile lines, shuffle only those, reassemble.
echo "--- Step 1: Shuffling file order ---"

# For simplicity, only shuffle single-line Compile entries (the vast majority).
# Multi-line entries (<Compile Include="..."> ... </Compile>) are left in place.

# Extract single-line Compile entries with their line numbers
grep -n '<Compile Include=.*\/>' "$PROJ" > /tmp/compiles.txt
SINGLE_COUNT=$(wc -l < /tmp/compiles.txt | tr -d ' ')

# Shuffle them (deterministic with sort -R and fixed seed via awk)
awk 'BEGIN{srand(42)} {print rand() "\t" $0}' /tmp/compiles.txt | sort -n | cut -f2- > /tmp/compiles_shuffled.txt

echo "  Shuffling $SINGLE_COUNT single-line Compile items"

# Build the shuffled fsproj by replacing each Compile line with the next shuffled one
cp "$PROJ" "$SHUFFLED"
line_idx=0
while IFS=: read -r orig_linenum orig_content; do
    line_idx=$((line_idx + 1))
    # Read the corresponding shuffled line
    shuffled_line=$(sed -n "${line_idx}p" /tmp/compiles_shuffled.txt)
    shuffled_linenum=$(echo "$shuffled_line" | cut -d: -f1)
    shuffled_content=$(echo "$shuffled_line" | cut -d: -f2-)
    # Replace the original line with the shuffled content
    # Use awk for exact line replacement
done < /tmp/compiles.txt

# Simpler: just use awk to do the replacement in one pass
awk '
BEGIN {
    srand(42)
    # Read all single-line Compile entries
    n = 0
}
FILENAME == ARGV[1] {
    compiles[n++] = $0
    next
}
FILENAME == ARGV[2] {
    if ($0 ~ /<Compile Include=.*\/>/) {
        if (!shuffled) {
            # Shuffle on first encounter
            for (i = n - 1; i > 0; i--) {
                j = int(rand() * (i + 1))
                tmp = compiles[i]; compiles[i] = compiles[j]; compiles[j] = tmp
            }
            shuffled = 1
            ci = 0
        }
        if (ci < n) {
            print compiles[ci++]
        } else {
            print $0
        }
    } else {
        print $0
    }
}
' <(grep '<Compile Include=.*\/>' "$PROJ") "$PROJ" > "$SHUFFLED"

rm -f /tmp/compiles.txt /tmp/compiles_shuffled.txt

SHUFFLED_COUNT=$(grep -c '<Compile Include=' "$SHUFFLED")
echo "  Result: $SHUFFLED_COUNT Compile items"
echo "  First 3:"
grep '<Compile Include=' "$SHUFFLED" | head -3 | sed 's/^/    /'
echo "  ..."

# Step 2: Build
echo ""
echo "--- Step 2: Building shuffled project with --file-order-auto+ ---"
echo "  (Compiling $COMPILE_COUNT F# files with auto dependency ordering)"

dotnet build "$SHUFFLED" \
    -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
    -p:OtherFlags="--file-order-auto+" \
    -v:quiet \
    2>&1 | tail -10

BUILD_EXIT=${PIPESTATUS[0]}

rm -f "$SHUFFLED"

echo ""
if [ $BUILD_EXIT -eq 0 ]; then
    echo "=== PASS: F# compiler compiled itself with shuffled file order! ==="
    exit 0
else
    echo "=== FAIL: F# compiler did not compile with shuffled file order ==="
    exit 1
fi
