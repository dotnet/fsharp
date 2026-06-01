#!/usr/bin/env bash
set -euo pipefail
REPO=/Users/tomasgrosup/code/fsharps/6
DOTNET="$REPO/.dotnet/dotnet"
FSC_DLL="$REPO/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE_DIR="$REPO/artifacts/bin/FSharp.Core/Release/netstandard2.0"
REFPACK="$REPO/.dotnet/packs/Microsoft.NETCore.App.Ref/10.0.8/ref/net10.0"

LABEL=$1
MODE=$2
OUT=$3
mkdir -p "$OUT"

# Build ref list
REFS=()
for ref in "$REFPACK"/*.dll; do
  REFS+=( -r:"$ref" )
done

ARGS=(
  --out:"$OUT/Test.dll"
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

case "$MODE" in
  seq) ARGS+=( --parallelcompilation- --test:ParallelOff ) ;;
  par) ARGS+=( --parallelcompilation+ ) ;;
esac

echo "[$LABEL] mode=$MODE → $OUT"
$DOTNET $FSC_DLL "${ARGS[@]}" 2>&1 | grep -vE "^$|^FSC" | tail -5
md5 -q "$OUT/Test.dll" 2>/dev/null || echo "FAILED"
