#!/bin/bash
# Performance analysis orchestration script (Unix)
# Usage: ./RunPerfAnalysis.sh --total 1500

set -e

TOTAL=1500
METHODS=10
OUTPUT="./results"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

while [[ $# -gt 0 ]]; do
    case $1 in
        --total) TOTAL="$2"; shift 2 ;;
        --methods) METHODS="$2"; shift 2 ;;
        --output) OUTPUT="$2"; shift 2 ;;
        --help) echo "Usage: $0 [--total N] [--methods N] [--output DIR]"; exit 0 ;;
        *) echo "Unknown: $1"; exit 1 ;;
    esac
done

echo "=== F# Performance Analysis ==="
echo "Total: $TOTAL, Methods: $METHODS"

if ! command -v dotnet &> /dev/null; then
    echo "Error: dotnet not found"
    exit 1
fi

echo -e "\nRunning profiler..."
dotnet fsi "$SCRIPT_DIR/PerfProfiler.fsx" --total "$TOTAL" --methods "$METHODS" --output "$OUTPUT"

echo -e "\nDone! Results in: $OUTPUT"
