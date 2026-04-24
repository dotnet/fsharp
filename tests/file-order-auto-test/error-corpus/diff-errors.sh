#!/bin/bash
# Compare error messages between manual and --file-order-auto+ modes
# for each project in the error corpus.

set -u

REPO_ROOT="$(cd "$(dirname "$0")/../../.." && pwd)"
CUSTOM_FSC="$REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"
CORPUS_DIR="$(dirname "$0")"

if [ ! -f "$CUSTOM_FSC" ]; then
    echo "ERROR: Custom compiler not found at $CUSTOM_FSC"
    exit 1
fi

echo "=== Error Message Comparison: Manual vs Auto File Order ==="
echo ""

DIFFS=0
SAME=0

for proj_dir in "$CORPUS_DIR"/*/; do
    [ -d "$proj_dir" ] || continue
    proj_file=$(ls "$proj_dir"*.fsproj 2>/dev/null | head -1)
    [ -n "$proj_file" ] || continue

    test_name=$(basename "$proj_dir")
    echo "--- $test_name ---"

    # Clean
    rm -rf "$proj_dir"bin "$proj_dir"obj

    # Mode 1: manual (no flag)
    manual_out=$(mktemp)
    dotnet build "$proj_file" \
        -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
        -v:quiet 2>&1 | grep "error FS" | sed 's|.*error FS|error FS|' | sed 's|\[.*||' | sort > "$manual_out"

    rm -rf "$proj_dir"bin "$proj_dir"obj

    # Mode 2: auto (FSharpAutoFileOrder)
    auto_out=$(mktemp)
    dotnet build "$proj_file" \
        -p:DotnetFscCompilerPath="$CUSTOM_FSC" \
        -p:FSharpAutoFileOrder=true \
        -v:quiet 2>&1 | grep "error FS" | sed 's|.*error FS|error FS|' | sed 's|\[.*||' | sort > "$auto_out"

    # Compare
    if diff -q "$manual_out" "$auto_out" > /dev/null 2>&1; then
        echo "  IDENTICAL"
        SAME=$((SAME + 1))
    else
        echo "  DIFFERS:"
        echo "  --- Manual mode ---"
        cat "$manual_out" | head -3 | sed 's/^/    /'
        echo "  --- Auto mode ---"
        cat "$auto_out" | head -3 | sed 's/^/    /'
        DIFFS=$((DIFFS + 1))
    fi
    echo ""

    rm -f "$manual_out" "$auto_out"
done

echo "=== Summary ==="
echo "Identical: $SAME"
echo "Different: $DIFFS"

if [ $DIFFS -eq 0 ]; then
    echo "All error messages match between modes."
    exit 0
else
    echo "Some error messages differ. Review above."
    exit 1
fi
