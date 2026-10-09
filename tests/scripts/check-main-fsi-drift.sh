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

if [[ ! -f "${LOCKED_HASH_FILE}" ]]; then
  echo "error: lock file not found: ${LOCKED_HASH_FILE}" >&2
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
  echo
  echo "Unexpected .fsi drift relative to ${BASE_REF}:" >&2
  echo "${unexpected}" >&2
  exit 1
fi

declare -A locked
mapfile -t locked_entries < <(
  rg -v '^\s*(#|$)' "${LOCKED_HASH_FILE}" || true
)

for entry in "${locked_entries[@]}"; do
  locked_path="${entry%% *}"
  expected_hash="${entry#* }"

  if [[ "${locked_path}" == "${expected_hash}" ]]; then
    echo "error: invalid locked fingerprint entry '${entry}'" >&2
    exit 2
  fi

  if [[ ! -f "${REPO_ROOT}/${locked_path}" ]]; then
    echo "error: locked fingerprint path not found: ${locked_path}" >&2
    exit 2
  fi

  locked["${locked_path}"]="${expected_hash}"
done

missing_locks=()
for changed_path in "${changed[@]}"; do
  if [[ -z "${locked["${changed_path}"]+x}" ]]; then
    missing_locks+=("${changed_path}")
  fi
done

if [[ ${#missing_locks[@]} -gt 0 ]]; then
  echo
  echo "error: allowlisted .fsi drift must be hash-locked in ${LOCKED_HASH_FILE}." >&2
  printf '  %s\n' "${missing_locks[@]}" >&2
  echo "run tests/scripts/refresh-main-fsi-drift-hashes.sh ${BASE_REF} after intentional updates." >&2
  exit 1
fi

for locked_path in "${!locked[@]}"; do
  if git -C "${REPO_ROOT}" diff --quiet "${BASE_REF}...HEAD" -- "${locked_path}"; then
    echo "error: locked fingerprint path '${locked_path}' no longer differs from ${BASE_REF}; remove it from ${LOCKED_HASH_FILE}" >&2
    exit 1
  fi

  actual_hash="$(git -C "${REPO_ROOT}" diff --no-color "${BASE_REF}...HEAD" -- "${locked_path}" | shasum -a 256 | awk '{print $1}')"
  expected_hash="${locked["${locked_path}"]}"

  if [[ "${actual_hash}" != "${expected_hash}" ]]; then
    echo "error: locked fingerprint mismatch for '${locked_path}'" >&2
    echo "  expected: ${expected_hash}" >&2
    echo "  actual:   ${actual_hash}" >&2
    echo "  update ${LOCKED_HASH_FILE} only when intentionally changing mainline .fsi drift." >&2
    exit 1
  fi
done

echo "baseline: ${BASE_REF}"
echo "allowlist: ${ALLOWLIST_FILE}"
echo "locked-fingerprints: ${LOCKED_HASH_FILE}"
echo

if [[ ${#changed[@]} -eq 0 ]]; then
  echo "No src/Compiler .fsi drift detected."
else
  echo "Allowed + hash-locked src/Compiler .fsi drift (${#changed[@]} files):"
  printf '  %s\n' "${changed[@]}"
fi
