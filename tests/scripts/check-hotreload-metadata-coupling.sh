#!/usr/bin/env bash
set -euo pipefail

BASE_REF="${1:-origin/main}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

if [[ "${FSHARP_SKIP_HOTRELOAD_METADATA_COUPLING_CHECK:-0}" == "1" ]]; then
  echo "metadata-coupling-check: skipped via FSHARP_SKIP_HOTRELOAD_METADATA_COUPLING_CHECK=1"
  exit 0
fi

if ! git -C "${REPO_ROOT}" rev-parse --verify "${BASE_REF}" >/dev/null 2>&1; then
  echo "error: baseline ref '${BASE_REF}' not found" >&2
  exit 2
fi

core_metadata_files=(
  "src/Compiler/AbstractIL/ilread.fs"
  "src/Compiler/AbstractIL/ilread.fsi"
  "src/Compiler/AbstractIL/ilwrite.fs"
  "src/Compiler/AbstractIL/ilwrite.fsi"
)

hotreload_coupled_files=(
  "src/Compiler/CodeGen/DeltaIndexSizing.fs"
  "src/Compiler/CodeGen/DeltaMetadataSerializer.fs"
  "src/Compiler/CodeGen/DeltaMetadataTables.fs"
  "src/Compiler/AbstractIL/ILBaselineReader.fs"
  "tests/FSharp.Compiler.Service.Tests/HotReload/CodedIndexTests.fs"
  "tests/FSharp.Compiler.Service.Tests/HotReload/FSharpDeltaMetadataWriterTests.fs"
)

mapfile -t changed_core < <(
  git -C "${REPO_ROOT}" diff --name-only "${BASE_REF}...HEAD" -- "${core_metadata_files[@]}" |
    LC_ALL=C sort -u
)

if [[ ${#changed_core[@]} -eq 0 ]]; then
  echo "metadata-coupling-check: no ilread/ilwrite drift relative to ${BASE_REF}."
  exit 0
fi

mapfile -t changed_coupled < <(
  git -C "${REPO_ROOT}" diff --name-only "${BASE_REF}...HEAD" -- "${hotreload_coupled_files[@]}" |
    LC_ALL=C sort -u
)

echo "baseline: ${BASE_REF}"
echo "core metadata drift detected:"
printf '  %s\n' "${changed_core[@]}"

if [[ ${#changed_coupled[@]} -eq 0 ]]; then
  echo "error: core metadata writer/reader files changed without corresponding hot reload serializer coverage updates." >&2
  echo "expected at least one change in:" >&2
  printf '  %s\n' "${hotreload_coupled_files[@]}" >&2
  echo "set FSHARP_SKIP_HOTRELOAD_METADATA_COUPLING_CHECK=1 only for temporary local bypasses." >&2
  exit 1
fi

echo "hot reload metadata coupling updates present:"
echo "metadata-coupling-check: coupled updates present."
printf '  %s\n' "${changed_coupled[@]}"
