#!/bin/bash
# Signature generation roundtrip sweep.
# For each .fs file: generate sig via --sig, then compile sig+impl together.
# Uses the locally-built fsc with full BCL references.
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
FSC="$REPO_ROOT/artifacts/bin/fsc/Release/net10.0/fsc.dll"
REFS_FILE="$1"        # file containing -r:path lines
CORPUS_FILE="$2"      # file containing .fs paths (one per line)
RESULTS_FILE="$3"     # output CSV

REFS=$(cat "$REFS_FILE" | sed 's/^/-r:/' | tr '\n' ' ')
FSC_COMMON="dotnet $FSC $REFS --nologo --noframework --target:library --nowarn:20,988,3391,58,64,1182,3370"

PASS=0; FAIL=0; SKIP=0; ERROR=0
echo "file,status,detail" > "$RESULTS_FILE"

while IFS= read -r srcfile; do
    [ -z "$srcfile" ] && continue
    [ ! -f "$REPO_ROOT/$srcfile" ] && continue
    
    TMPD=$(mktemp -d)
    cp "$REPO_ROOT/$srcfile" "$TMPD/source.fs"
    
    # Step 1: compile source and generate sig
    SIG_OUT=$($FSC_COMMON --sig:"$TMPD/source.fsi" "$TMPD/source.fs" -o:"$TMPD/source.dll" 2>&1) || true
    
    if [ ! -f "$TMPD/source.fsi" ]; then
        # Source didn't compile or sig not generated
        echo "$srcfile,SKIP,\"source compile or sig gen failed\"" >> "$RESULTS_FILE"
        SKIP=$((SKIP + 1))
        rm -rf "$TMPD"
        continue
    fi
    
    # Step 2: roundtrip compile sig + impl
    RT_OUT=$($FSC_COMMON "$TMPD/source.fsi" "$TMPD/source.fs" -o:"$TMPD/roundtrip.dll" 2>&1)
    RT_RC=$?
    
    if [ $RT_RC -eq 0 ]; then
        echo "$srcfile,PASS," >> "$RESULTS_FILE"
        PASS=$((PASS + 1))
    else
        FIRST_ERR=$(echo "$RT_OUT" | grep "error FS" | head -1 | sed 's/.*error //' | tr '"' "'")
        echo "$srcfile,FAIL,\"$FIRST_ERR\"" >> "$RESULTS_FILE"
        FAIL=$((FAIL + 1))
        echo "FAIL: $srcfile -- $FIRST_ERR" >&2
    fi
    
    rm -rf "$TMPD"
done < "$CORPUS_FILE"

echo "Done: $PASS pass, $FAIL FAIL, $SKIP skip, $ERROR error (total $((PASS+FAIL+SKIP+ERROR)))" >&2
