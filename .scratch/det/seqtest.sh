#!/usr/bin/env bash
set -euo pipefail
REPO=/Users/tomasgrosup/code/fsharps/6
DOTNET="$REPO/.dotnet/dotnet"
FSC_DLL="$REPO/artifacts/bin/fsc/Release/net10.0/fsc.dll"
FSCORE_DIR="$REPO/artifacts/bin/FSharp.Core/Release/netstandard2.0"
REFPACK="$REPO/.dotnet/packs/Microsoft.NETCore.App.Ref/10.0.8/ref/net10.0"
REFS=(); for ref in "$REFPACK"/*.dll; do REFS+=( -r:"$ref" ); done
mkdir -p workseq
for i in $(seq 1 5); do
  $DOTNET "$FSC_DLL" --out:workseq/s$i.dll --target:library --deterministic+ --debug:portable --optimize+ --noframework --targetprofile:netcore --nowarn:75 -r:"$FSCORE_DIR/FSharp.Core.dll" "${REFS[@]}" @files.rsp --parallelcompilation- --test:ParallelOff >/dev/null 2>&1
  echo "seq#$i $(md5 -q workseq/s$i.dll)"
done
