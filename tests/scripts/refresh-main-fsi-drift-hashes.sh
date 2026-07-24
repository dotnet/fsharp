#!/usr/bin/env bash
set -euo pipefail

BASE_REF="${1:-origin/main}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
ALLOWLIST_FILE="${SCRIPT_DIR}/main-fsi-allowlist.txt"
LOCKED_HASH_FILE="${SCRIPT_DIR}/main-fsi-drift-hashes.txt"

if ! git -C "${REPO_ROOT}" rev-parse --verify "${BASE_REF}" >/dev/null 2>&1; then
  echo "error: baseline ref '${BASE_REF}' not found" >&2
  exit 2
fi

if [[ ! -f "${ALLOWLIST_FILE}" ]]; then
  echo "error: allowlist file not found: ${ALLOWLIST_FILE}" >&2
  exit 2
fi

mapfile -t changed < <(
  git -C "${REPO_ROOT}" diff --name-only "${BASE_REF}...HEAD" |
    rg '^src/Compiler/.*\.fsi$' |
    LC_ALL=C sort
)

mapfile -t allowed < <(
  rg -v '^\s*(#|$)' "${ALLOWLIST_FILE}" |
    LC_ALL=C sort
)

unexpected="$(comm -23 <(printf '%s\n' "${changed[@]}") <(printf '%s\n' "${allowed[@]}"))"
if [[ -n "${unexpected}" ]]; then
  echo "error: cannot refresh hashes because there is unexpected .fsi drift:" >&2
  echo "${unexpected}" >&2
  exit 1
fi

{
  echo "# SHA256 fingerprints for allowed .fsi drift relative to ${BASE_REF}."
  echo "# Format: <path> <sha256(git diff --no-color BASE_REF...HEAD -- <path>)>"
  echo

  for path in "${changed[@]}"; do
    hash="$(git -C "${REPO_ROOT}" diff --no-color "${BASE_REF}...HEAD" -- "${path}" | shasum -a 256 | awk '{print $1}')"
    echo "${path} ${hash}"
  done
} > "${LOCKED_HASH_FILE}"

echo "updated: ${LOCKED_HASH_FILE}"
echo "entries: ${#changed[@]}"
