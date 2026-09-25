#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
DOTNET="${ROOT}/.dotnet/dotnet"
SMOKE_SCRIPT="tests/scripts/hot-reload-demo-smoke.sh"
DEMO_PROJECT="tests/projects/HotReloadDemo/HotReloadDemoApp/HotReloadDemoApp.fsproj"

TIMESTAMP="$(date +"%Y%m%d-%H%M%S")"
TMP_ROOT="${TMPDIR:-/tmp}"
LOG_DIR="${HOTRELOAD_VERIFY_LOG_DIR:-${TMP_ROOT%/}/fsharp-hotreload-verify-${TIMESTAMP}}"

if [[ ! -x "${DOTNET}" ]]; then
  echo "error: dotnet executable not found at ${DOTNET}" >&2
  exit 1
fi

if [[ ! -x "${ROOT}/${SMOKE_SCRIPT}" ]]; then
  echo "error: smoke script not found at ${ROOT}/${SMOKE_SCRIPT}" >&2
  exit 1
fi

if [[ ! -f "${ROOT}/${DEMO_PROJECT}" ]]; then
  echo "error: demo project not found at ${ROOT}/${DEMO_PROJECT}" >&2
  exit 1
fi

mkdir -p "${LOG_DIR}"

run_step() {
  local name="$1"
  shift
  local logfile="${LOG_DIR}/${name}.log"

  echo ""
  echo "=== ${name} ==="
  echo "command: $*"

  set +e
  (
    cd "${ROOT}"
    "$@"
  ) >"${logfile}" 2>&1
  local exit_code=$?
  set -e

  if [[ ${exit_code} -ne 0 ]]; then
    echo "error: step '${name}' failed (exit ${exit_code}). log: ${logfile}" >&2
    tail -n 120 "${logfile}" >&2 || true
    exit "${exit_code}"
  fi

  echo "ok: ${name} (log: ${logfile})"
}

assert_contains() {
  local step="$1"
  local marker="$2"
  local logfile="${LOG_DIR}/${step}.log"

  if ! grep -Fq "${marker}" "${logfile}"; then
    echo "error: missing marker in ${step}: ${marker}" >&2
    echo "log: ${logfile}" >&2
    tail -n 120 "${logfile}" >&2 || true
    exit 90
  fi
}

echo "Hot Reload verification started."
echo "root: ${ROOT}"
echo "logs: ${LOG_DIR}"

run_step "build" "${DOTNET}" build FSharp.slnx -c Debug -v minimal
assert_contains "build" "Build succeeded"

run_step "main-fsi-drift" bash tests/scripts/check-main-fsi-drift.sh origin/main
assert_contains "main-fsi-drift" "allowlist:"

run_step "metadata-coupling" bash tests/scripts/check-hotreload-metadata-coupling.sh origin/main
assert_contains "metadata-coupling" "metadata-coupling-check:"

run_step "ilxgen-name-path" bash tests/scripts/check-ilxgen-name-path.sh
assert_contains "ilxgen-name-path" "ilxgen-name-path-check:"

run_step "plugin-boundary" bash tests/scripts/check-hotreload-plugin-boundary.sh
assert_contains "plugin-boundary" "hotreload-plugin-boundary-check:"

run_step "metadata-parity" bash tests/scripts/check-hotreload-metadata-parity.sh
assert_contains "metadata-parity" "hotreload-metadata-parity-check:"

# Tests run on Microsoft.Testing.Platform (xunit.v3): VSTest --filter syntax and
# RunSettingsFilePath are ignored, so pass the MTP filter and the EnC env vars explicitly.
run_step "service-tests" \
  env DOTNET_MODIFIABLE_ASSEMBLIES=debug COMPlus_ForceEnc=1 \
  "${DOTNET}" test --project tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj \
  -c Debug --no-build -- --filter-class "*HotReload*"
assert_contains "service-tests" "Test run summary: Passed!"
assert_contains "service-tests" "failed: 0"

run_step "component-tests" \
  env DOTNET_MODIFIABLE_ASSEMBLIES=debug COMPlus_ForceEnc=1 \
  "${DOTNET}" test --project tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Debug --no-build -- --filter-class "*HotReload*"
assert_contains "component-tests" "Test run summary: Passed!"
assert_contains "component-tests" "failed: 0"

run_step "smoke-default" "${SMOKE_SCRIPT}"
assert_contains "smoke-default" "Scripted run succeeded: emitted"
assert_contains "smoke-default" "Hot reload demo smoke test completed successfully."

run_step "smoke-runtime-apply" env HOTRELOAD_SMOKE_RUNTIME_APPLY=1 "${SMOKE_SCRIPT}"
assert_contains "smoke-runtime-apply" "Hot reload applied (delta #1)"
assert_contains "smoke-runtime-apply" "Hot reload applied (delta #2)"
assert_contains "smoke-runtime-apply" "Scripted run succeeded: emitted 2 delta(s) (runtime apply enabled)."

run_step "demo-direct-runtime-apply" \
  env DOTNET_MODIFIABLE_ASSEMBLIES=debug \
    FSHARP_HOTRELOAD_ENABLE_RUNTIME_APPLY=1 \
    FSHARP_HOTRELOAD_TRACE_RUNTIME_APPLY=1 \
    COMPlus_ForceEnc=1 \
    "${DOTNET}" run --project "${DEMO_PROJECT}" -- --scripted --multi-delta --runtime-apply
assert_contains "demo-direct-runtime-apply" "[hotreload-runtime] applying delta gen=0"
assert_contains "demo-direct-runtime-apply" "[hotreload-runtime] applying delta gen=1"
assert_contains "demo-direct-runtime-apply" "Hot reload applied (delta #1): Hello from generation 1"
assert_contains "demo-direct-runtime-apply" "Hot reload applied (delta #2): Hello from generation 2"
assert_contains "demo-direct-runtime-apply" "Scripted run succeeded: emitted 2 delta(s) (runtime apply enabled)."

echo ""
echo "Hot Reload verification succeeded."
echo "logs: ${LOG_DIR}"
