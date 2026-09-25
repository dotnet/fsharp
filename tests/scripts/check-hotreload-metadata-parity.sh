#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
DOTNET="${ROOT}/.dotnet/dotnet"

if [[ ! -x "${DOTNET}" ]]; then
  echo "error: dotnet executable not found at ${DOTNET}" >&2
  exit 1
fi

cd "${ROOT}"

# Tests run on Microsoft.Testing.Platform (xunit.v3); use MTP filter syntax.
DOTNET_MODIFIABLE_ASSEMBLIES=debug COMPlus_ForceEnc=1 \
  "${DOTNET}" test --project tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Debug --no-build -- --filter-class "*MdvValidationTests*"

echo "hotreload-metadata-parity-check: mdv parity slice passed."
