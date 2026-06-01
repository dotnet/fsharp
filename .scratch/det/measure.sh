#!/usr/bin/env bash
# Usage: ./measure.sh <combo-label> <par-iters>
# Runs par x N + seq x 1 against the prebuilt fsc.dll, prints hashes and counts.
set -euo pipefail
LABEL=${1:-?}
N=${2:-20}
REPO=/Users/tomasgrosup/code/fsharps/6
DOTNET="$REPO/.dotnet/dotnet"
FSC_DLL="$REPO/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE_DIR="$REPO/artifacts/bin/FSharp.Core/Release/netstandard2.0"
REFPACK="$REPO/.dotnet/packs/Microsoft.NETCore.App.Ref/10.0.8/ref/net10.0"

REFS=()
for ref in "$REFPACK"/*.dll; do REFS+=( -r:"$ref" ); done

WORK=$(pwd)/work
rm -rf "$WORK"; mkdir -p "$WORK"

compile() {
  local mode=$1
  local out=$2
  ARGS=(
    --out:"$out"
    --target:library
    --deterministic+
    --debug:portable
    --optimize+
    --noframework
    --targetprofile:netcore
    --nowarn:75
    -r:"$FSCORE_DIR/FSharp.Core.dll"
    "${REFS[@]}"
    @files.rsp
  )
  case "$mode" in
    seq) ARGS+=( --parallelcompilation- --test:ParallelOff ) ;;
    par) ARGS+=( --parallelcompilation+ ) ;;
  esac
  $DOTNET "$FSC_DLL" "${ARGS[@]}" > "$out.log" 2>&1 || { echo "FAILED $mode $out"; tail -10 "$out.log"; exit 1; }
}

# 1 seq + N par
compile seq "$WORK/seq.dll"
seq_hash=$(md5 -q "$WORK/seq.dll")
echo "[$LABEL] seq      $seq_hash"

pfirst=""
divergent=0
HASHES=""
for i in $(seq 1 $N); do
  compile par "$WORK/par_$i.dll"
  h=$(md5 -q "$WORK/par_$i.dll")
  if [ -z "$pfirst" ]; then pfirst="$h"; fi
  HASHES="$HASHES $h"
  if [ "$h" != "$pfirst" ]; then divergent=$((divergent+1)); fi
  printf "[$LABEL] par#%02d  %s\n" "$i" "$h"
done

echo "[$LABEL] ---- summary ----"
echo "[$LABEL] unique par hashes:"
echo "$HASHES" | tr ' ' '\n' | sort | uniq -c | awk '{print "[" "'$LABEL'" "]   "$2"  x"$1}'
UNIQ=$(echo "$HASHES" | tr ' ' '\n' | grep -v '^$' | sort -u | wc -l | tr -d ' ')
echo "[$LABEL] unique-count: $UNIQ / $N"
echo "[$LABEL] par-vs-par divergent-from-first: $divergent / $N"
if [ "$seq_hash" = "$pfirst" ]; then
  echo "[$LABEL] seq-vs-par(first): MATCH"
else
  echo "[$LABEL] seq-vs-par(first): DIFFER ($seq_hash vs $pfirst)"
fi
