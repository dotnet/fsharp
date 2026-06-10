#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
TARGET="${ROOT}/src/Compiler/CodeGen/IlxGen.fs"

if [[ ! -f "${TARGET}" ]]; then
  echo "error: target file not found: ${TARGET}" >&2
  exit 1
fi

if ! rg -q "let private freshIlxName" "${TARGET}"; then
  echo "error: expected helper freshIlxName in IlxGen.fs" >&2
  exit 1
fi

if ! rg -q "let private freshCoreName" "${TARGET}"; then
  echo "error: expected helper freshCoreName in IlxGen.fs" >&2
  exit 1
fi

if ! rg -q "let private nextIlxOrdinal" "${TARGET}"; then
  echo "error: expected helper nextIlxOrdinal in IlxGen.fs" >&2
  exit 1
fi

matches="$(rg -n "CompilerGlobalState\\.Value\\.(IlxGenNiceNameGenerator|NiceNameGenerator)\\.(FreshCompilerGeneratedName|IncrementOnly)" "${TARGET}" || true)"

match_count=0
if [[ -n "${matches}" ]]; then
  match_count="$(printf '%s\n' "${matches}" | wc -l | tr -d '[:space:]')"
fi

if [[ "${match_count}" -ne 3 ]]; then
  echo "error: IlxGen.fs must centralize compiler-generated-name map access through helper wrappers." >&2
  echo "expected 3 direct generator calls (helper bodies), found ${match_count}." >&2
  if [[ -n "${matches}" ]]; then
    echo "matched lines:" >&2
    printf '%s\n' "${matches}" >&2
  fi
  exit 1
fi

echo "ilxgen-name-path-check: centralized naming helpers intact."
