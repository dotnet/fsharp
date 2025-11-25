#!/bin/bash
# collect-diagnostics.sh
# Main diagnostic collection script for FSharpPlus build hang investigation
# This script reproduces issue #19116 and collects trace/dump data

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="${SCRIPT_DIR}/output"
TIMEOUT_SECONDS=120
REPO_URL="https://github.com/fsprojects/FSharpPlus.git"
BRANCH="gus/fsharp9"
CLONE_DIR="${OUTPUT_DIR}/FSharpPlus-repro"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

echo_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

echo_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Create output directory
mkdir -p "${OUTPUT_DIR}"
cd "${OUTPUT_DIR}"

# Record start time
START_TIME=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
echo_info "Diagnostic collection started at ${START_TIME}"

# Verify SDK version
SDK_VERSION=$(dotnet --version)
echo_info "Using .NET SDK version: ${SDK_VERSION}"

# Install diagnostic tools
echo_info "Installing dotnet-trace and dotnet-dump..."
dotnet tool install --global dotnet-trace 2>/dev/null || dotnet tool update --global dotnet-trace || true
dotnet tool install --global dotnet-dump 2>/dev/null || dotnet tool update --global dotnet-dump || true
export PATH="$PATH:$HOME/.dotnet/tools"

# Verify tools are available
if ! command -v dotnet-trace &> /dev/null; then
    echo_error "dotnet-trace not found. Please install manually: dotnet tool install --global dotnet-trace"
    exit 1
fi

if ! command -v dotnet-dump &> /dev/null; then
    echo_error "dotnet-dump not found. Please install manually: dotnet tool install --global dotnet-dump"
    exit 1
fi

echo_info "dotnet-trace version: $(dotnet-trace --version)"
echo_info "dotnet-dump version: $(dotnet-dump --version)"

# Clone FSharpPlus repository
echo_info "Cloning FSharpPlus repository (branch: ${BRANCH})..."
if [ -d "${CLONE_DIR}" ]; then
    echo_info "Removing existing clone directory..."
    rm -rf "${CLONE_DIR}"
fi

git clone --depth 1 --branch "${BRANCH}" "${REPO_URL}" "${CLONE_DIR}"

# Record git commit hash
cd "${CLONE_DIR}"
GIT_COMMIT=$(git rev-parse HEAD)
echo_info "FSharpPlus commit: ${GIT_COMMIT}"

# Restore dependencies first (without timeout)
echo_info "Restoring dependencies..."
dotnet restore build.proj 2>&1 | tee "${OUTPUT_DIR}/restore-output.txt" || true

# Run the command with trace collection and timeout
echo_info "Running 'dotnet test build.proj -v n' with ${TIMEOUT_SECONDS}s timeout and trace collection..."
echo_info "Command: timeout --kill-after=10s ${TIMEOUT_SECONDS}s dotnet-trace collect ... -- dotnet test build.proj -v n"

# Start the command with trace collection
TRACE_FILE="${OUTPUT_DIR}/hang-trace.nettrace"

# Run with timeout
set +e
timeout --kill-after=10s ${TIMEOUT_SECONDS}s dotnet-trace collect \
    --providers "Microsoft-Windows-DotNETRuntime:0xFFFFFFFFFFFFFFFF:5,Microsoft-Diagnostics-DiagnosticSource,Microsoft-Windows-DotNETRuntimeRundown,System.Threading.Tasks.TplEventSource" \
    --format speedscope \
    --output "${TRACE_FILE}" \
    -- dotnet test build.proj -v n 2>&1 | tee "${OUTPUT_DIR}/console-output.txt"

EXIT_CODE=$?
set -e

# Record end time
END_TIME=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# Interpret exit code
case $EXIT_CODE in
    124)
        RESULT="HANG_CONFIRMED"
        echo_warn "Process timed out after ${TIMEOUT_SECONDS} seconds - HANG CONFIRMED (this is the bug)"
        ;;
    0)
        RESULT="NO_HANG"
        echo_info "Process completed successfully - NO HANG (unexpected, may be fixed or intermittent)"
        ;;
    137)
        RESULT="KILLED"
        echo_warn "Process was killed (SIGKILL) - likely due to timeout"
        ;;
    *)
        RESULT="TEST_FAILURE"
        echo_error "Process failed with exit code ${EXIT_CODE} - TEST FAILURE (different issue)"
        ;;
esac

# Try to capture dumps of any remaining dotnet processes
echo_info "Attempting to capture memory dumps of hanging processes..."
DOTNET_PIDS=$(pgrep -x dotnet 2>/dev/null || true)
if [ ! -z "$DOTNET_PIDS" ]; then
    echo_info "Found dotnet processes: ${DOTNET_PIDS}"
    for PID in $DOTNET_PIDS; do
        echo_info "Capturing dump for process ${PID}..."
        dotnet-dump collect -p $PID --output "${OUTPUT_DIR}/hang-dump-${PID}.dmp" 2>/dev/null || echo_warn "Failed to capture dump for PID ${PID}"
    done
else
    echo_info "No dotnet processes found (may have been terminated by timeout)"
fi

# Kill any remaining dotnet processes from our test
pkill -f "dotnet test build.proj" 2>/dev/null || true
pkill -f "dotnet-trace" 2>/dev/null || true

# List generated files
echo_info "Generated files:"
ls -la "${OUTPUT_DIR}"/*.nettrace "${OUTPUT_DIR}"/*.dmp "${OUTPUT_DIR}"/*.txt 2>/dev/null || true

# Save metadata for later analysis
cat > "${OUTPUT_DIR}/run-metadata.json" << EOF
{
    "start_time": "${START_TIME}",
    "end_time": "${END_TIME}",
    "sdk_version": "${SDK_VERSION}",
    "git_commit": "${GIT_COMMIT}",
    "branch": "${BRANCH}",
    "repository_url": "${REPO_URL}",
    "command": "dotnet test build.proj -v n",
    "timeout_seconds": ${TIMEOUT_SECONDS},
    "exit_code": ${EXIT_CODE},
    "result": "${RESULT}",
    "trace_file": "hang-trace.nettrace",
    "output_directory": "${OUTPUT_DIR}"
}
EOF

echo_info "Metadata saved to run-metadata.json"
echo_info "Diagnostic collection completed with result: ${RESULT}"
echo_info "Output directory: ${OUTPUT_DIR}"

exit 0
