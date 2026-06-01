#!/usr/bin/env bash
set -euo pipefail
REPO=/Users/tomasgrosup/code/fsharps/6
DOTNET="$REPO/.dotnet/dotnet"
FSC_DLL="$REPO/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE_DIR="$REPO/artifacts/bin/FSharp.Core/Release/netstandard2.0"
REFPACK="$REPO/.dotnet/packs/Microsoft.NETCore.App.Ref/10.0.8/ref/net10.0"

REFS=()
for ref in "$REFPACK"/*.dll; do REFS+=( -r:"$ref" ); done

compile() {
  local mode=$1
  local result_path=$2
  rm -rf ./out
  mkdir -p ./out
  ARGS=(
    --out:./out/Test.dll
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
  $DOTNET $FSC_DLL "${ARGS[@]}" 2>&1 | grep -E "error|FS[0-9]" | head -3 || true
  cp ./out/Test.dll "$result_path"
}

compile seq seq1.dll
compile seq seq2.dll
compile seq seq3.dll
compile par par1.dll
compile par par2.dll
compile par par3.dll
compile par par4.dll
compile par par5.dll

echo "--- hashes ---"
for f in seq1.dll seq2.dll seq3.dll par1.dll par2.dll par3.dll par4.dll par5.dll; do
  echo "$(md5 -q $f)  $f"
done
