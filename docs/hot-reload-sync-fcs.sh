#!/usr/bin/env bash
#
# Copies the locally built hot-reload FSharp.Compiler.Service (and the matching FSharp.Core)
# into a locally built .NET SDK redist, so `dotnet watch` resolves the hot-reload compiler
# instead of the stock one the SDK ships. Safe to re-run.
#
# Usage: hot-reload-sync-fcs.sh [<fsharp-repo-root> <sdk-repo-root>]
#   Defaults: this script's repo for the compiler, and a sibling "sdk-hotreload" for the SDK.
#
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
fsharp_root="${1:-$(cd "$script_dir/.." && pwd)}"
sdk_root="${2:-$(cd "$script_dir/../.." && pwd)/sdk-hotreload}"

fcs_bin="$fsharp_root/artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
redist="$sdk_root/artifacts/bin/redist/Debug/dotnet"

if [[ ! -f "$fcs_bin/FSharp.Compiler.Service.dll" ]]; then
  echo "error: built FCS not found at $fcs_bin" >&2
  echo "       build the F# repo first: (cd '$fsharp_root' && ./build.sh -c Debug)" >&2
  exit 1
fi

if [[ ! -x "$redist/dotnet" ]]; then
  echo "error: SDK redist not found at $redist" >&2
  echo "       build the SDK repo first (see hot-reload-quickstart.md, step 2)" >&2
  exit 1
fi

shopt -s nullglob
synced=0
for fsharp_dir in "$redist"/sdk/*/FSharp; do
  cp -f "$fcs_bin/FSharp.Compiler.Service.dll" "$fsharp_dir/"
  cp -f "$fcs_bin/FSharp.Core.dll" "$fsharp_dir/"
  echo "synced hot-reload FCS -> $fsharp_dir"
  synced=1
done

if [[ "$synced" != 1 ]]; then
  echo "error: no FSharp directory found under $redist/sdk/*/FSharp" >&2
  exit 1
fi

echo "done. now: source '$sdk_root/eng/dogfood.sh' and run 'dotnet watch run' in an F# project."
