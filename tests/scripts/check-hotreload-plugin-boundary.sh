#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

allowed_direct_consumers=(
  "src/Compiler/Driver/CompilerEmitHookBootstrap.fs"
  "src/Compiler/Driver/CompilerEmitHookState.fs"
  "src/Compiler/Driver/HotReloadEmitHook.fs"
  "src/Compiler/Service/service.fs"
)

declare -A allowset=()
for file in "${allowed_direct_consumers[@]}"; do
  allowset["${file}"]=1
done

mapfile -t candidate_files < <(
  git -C "${REPO_ROOT}" ls-files \
    "src/Compiler/Driver/*.fs" \
    "src/Compiler/Driver/*.fsi" \
    "src/Compiler/TypedTree/*.fs" \
    "src/Compiler/TypedTree/*.fsi" \
    "src/Compiler/Generated/*.fs" \
    "src/Compiler/Generated/*.fsi" \
    "src/Compiler/CodeGen/IlxGen.fs" \
    "src/Compiler/CodeGen/IlxGen.fsi" |
    LC_ALL=C sort -u
)

violations=()

for file in "${candidate_files[@]}"; do
  if [[ -n "${allowset["${file}"]+x}" ]]; then
    continue
  fi

  full_path="${REPO_ROOT}/${file}"
  [[ -f "${full_path}" ]] || continue

  if rg -n \
    -e 'open FSharp\.Compiler\.HotReload$' \
    -e 'open FSharp\.Compiler\.HotReloadBaseline$' \
    -e 'open FSharp\.Compiler\.HotReloadPdb$' \
    -e 'open FSharp\.Compiler\.HotReloadEmitHook$' \
    -e 'open FSharp\.Compiler\.HotReloadState$' \
    -e 'FSharp\.Compiler\.HotReload\.' \
    -e 'FSharp\.Compiler\.HotReloadState\.' \
    -e 'FSharpEditAndContinueLanguageService\.Instance' \
    "${full_path}" >/dev/null; then
    violations+=("${file}")
  fi
done

if [[ ${#violations[@]} -gt 0 ]]; then
  echo "error: plugin-boundary violation(s) detected outside allowlist." >&2
  echo "allowed direct consumers:" >&2
  printf '  %s\n' "${allowed_direct_consumers[@]}" >&2
  echo "violating files:" >&2
  printf '  %s\n' "${violations[@]}" >&2
  exit 1
fi

echo "hotreload-plugin-boundary-check: direct hot reload implementation references are fenced."
