#!/usr/bin/env bash
set -euo pipefail
LABEL=${1:-?}; N=${2:-5}
REPO=/Users/tomasgrosup/code/fsharps/6
DOTNET="$REPO/.dotnet/dotnet"
FSC_DLL="$REPO/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE_DIR="$REPO/artifacts/bin/FSharp.Core/Release/netstandard2.0"
REFPACK="$REPO/.dotnet/packs/Microsoft.NETCore.App.Ref/10.0.8/ref/net10.0"
REFS=(); for ref in "$REFPACK"/*.dll; do REFS+=( -r:"$ref" ); done
WORK=$(pwd)/work2; rm -rf "$WORK"; mkdir -p "$WORK"
compile() {
  local mode=$1 out=$2
  ARGS=( --out:"$out" --target:library --deterministic+ --optimize+ --noframework --targetprofile:netcore --nowarn:75 -r:"$FSCORE_DIR/FSharp.Core.dll" "${REFS[@]}" @files.rsp )
  case "$mode" in seq) ARGS+=( --parallelcompilation- --test:ParallelOff );; par) ARGS+=( --parallelcompilation+ );; esac
  $DOTNET "$FSC_DLL" "${ARGS[@]}" > "$out.log" 2>&1 || { echo FAILED; tail "$out.log"; exit 1; }
}
compile seq "$WORK/seq.dll"
echo "[$LABEL] seq $(md5 -q $WORK/seq.dll)"
for i in $(seq 1 $N); do compile par "$WORK/par_$i.dll"; printf "[$LABEL] par#%02d  %s\n" "$i" "$(md5 -q $WORK/par_$i.dll)"; done
