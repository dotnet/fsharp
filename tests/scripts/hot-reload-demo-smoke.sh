#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"

APP_DIR="${ROOT}/tests/projects/HotReloadDemo/HotReloadDemoApp"

if [[ ! -d "${APP_DIR}" ]]; then
  echo "error: HotReloadDemoApp directory not found at ${APP_DIR}" >&2
  exit 1
fi

export DOTNET_MODIFIABLE_ASSEMBLIES=debug
export FSHARP_HOTRELOAD_DUMP_DELTA=1

runtime_apply_args=()
if [[ "${HOTRELOAD_SMOKE_RUNTIME_APPLY:-}" == "1" ]]; then
  export FSHARP_HOTRELOAD_ENABLE_RUNTIME_APPLY=1
  export COMPlus_ForceEnc=1
  runtime_apply_args+=(--runtime-apply)
  echo "HOTRELOAD_SMOKE_RUNTIME_APPLY=1 (MetadataUpdater.ApplyUpdate will be exercised)" >&2
else
  unset FSHARP_HOTRELOAD_ENABLE_RUNTIME_APPLY
  unset COMPlus_ForceEnc
  echo "hint: set HOTRELOAD_SMOKE_RUNTIME_APPLY=1 to enable MetadataUpdater.ApplyUpdate during the smoke test." >&2
fi

if [[ "${FSHARP_HOTRELOAD_TRACE_STRINGS:-}" != "1" ]]; then
  echo "hint: set FSHARP_HOTRELOAD_TRACE_STRINGS=1 to log user-string updates during the demo" >&2
else
  echo "FSHARP_HOTRELOAD_TRACE_STRINGS is enabled; user-string updates will be logged." >&2
fi

if [[ "${HOTRELOAD_SMOKE_KEEP_WORKDIR:-}" == "1" ]]; then
  export FSHARP_HOTRELOAD_KEEP_WORKDIR=1
  echo "FSHARP_HOTRELOAD_KEEP_WORKDIR=1 (temporary demo directory will be preserved)" >&2
else
  unset FSHARP_HOTRELOAD_KEEP_WORKDIR
  echo "hint: set HOTRELOAD_SMOKE_KEEP_WORKDIR=1 to keep the demo working directory (for inspecting dumped deltas)." >&2
fi

mdv_available=1
MDV_PATH="${FSHARP_HOTRELOAD_MDV_PATH:-}"
if [[ -z "${MDV_PATH}" ]]; then
  if command -v mdv >/dev/null 2>&1; then
    MDV_PATH="$(command -v mdv)"
  else
    mdv_available=0
  fi
fi

if [[ ${mdv_available} -eq 1 ]]; then
  if [[ ! -x "${MDV_PATH}" ]]; then
    echo "error: mdv executable at ${MDV_PATH} is not runnable" >&2
    exit 3
  fi

  export FSHARP_HOTRELOAD_MDV_PATH="${MDV_PATH}"
  export FSHARP_HOTRELOAD_RUN_MDV=1
else
  echo "warning: mdv executable not found; skipping automatic mdv validation" >&2
  unset FSHARP_HOTRELOAD_MDV_PATH
  export FSHARP_HOTRELOAD_RUN_MDV=0
fi

pushd "${APP_DIR}" >/dev/null

echo "Running HotReloadDemoApp in scripted mode..." >&2

set +e
if [[ ${#runtime_apply_args[@]} -gt 0 ]]; then
  output="$(../../../../.dotnet/dotnet run -- --scripted --multi-delta "${runtime_apply_args[@]}")"
else
  output="$(../../../../.dotnet/dotnet run -- --scripted --multi-delta)"
fi
exit_code=$?
set -e

popd >/dev/null

echo "${output}"

if [[ ${exit_code} -ne 0 ]]; then
  echo "error: HotReloadDemoApp scripted run failed" >&2
  exit ${exit_code}
fi

if ! grep -q "Scripted run succeeded: emitted" <<<"${output}"; then
  echo "error: scripted run did not report success" >&2
  exit 10
fi

if [[ ${mdv_available} -eq 1 ]]; then
  if ! grep -Fq "[hotreload-delta] mdv" <<<"${output}"; then
    echo "error: mdv command was not recorded in the demo output" >&2
    exit 11
  fi
fi

echo "Hot reload demo smoke test completed successfully." >&2
