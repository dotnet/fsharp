#!/bin/bash

# =============================================================================
# kill-dotnet.sh
# Continuously kills dotnet processes.
#
# SYNOPSIS
#     ./kill-dotnet.sh [OPTIONS]
#
# DESCRIPTION
#     Runs in an infinite loop, checking every second for dotnet processes
#     and killing them. Optionally filters processes by their command line
#     arguments. Supports both macOS and Linux.
#
# OPTIONS
#     -f, --filter PATTERN
#         Optional filter to match against the command line arguments of
#         dotnet processes. Only processes whose command line contains this
#         string will be killed.
#         Example: -f "MyFoo.dll" will kill processes running "dotnet MyFoo.dll"
#
#     -t, --max-time SECONDS
#         Maximum time in seconds to run the loop. Default is 30 seconds.
#         Use -1 to run indefinitely.
#
#     -h, --help
#         Display this help message and exit.
#
# EXAMPLES
#     ./kill-dotnet.sh
#         Kills all dotnet processes for 30 seconds.
#
#     ./kill-dotnet.sh -f "MyFoo.dll"
#         Kills only dotnet processes that have "MyFoo.dll" in their command line.
#
#     ./kill-dotnet.sh -t -1
#         Kills all dotnet processes indefinitely.
#
#     ./kill-dotnet.sh -t 60 -f "MyFoo.dll"
#         Kills matching processes for 60 seconds.
#
# NOTES
#     Press Ctrl+C to stop the script.
# =============================================================================

show_help() {
    sed -n '3,44p' "$0" | sed 's/^# //' | sed 's/^#//'
}

ARGUMENT_FILTER=""
MAX_TIME=30

while [[ $# -gt 0 ]]; do
    case $1 in
        -f|--filter)
            ARGUMENT_FILTER="$2"
            shift 2
            ;;
        -t|--max-time)
            MAX_TIME="$2"
            shift 2
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use -h or --help for usage information."
            exit 1
            ;;
    esac
done

START_TIME=$(date +%s)

while true; do
    if [[ $MAX_TIME -ne -1 ]]; then
        CURRENT_TIME=$(date +%s)
        ELAPSED=$((CURRENT_TIME - START_TIME))
        if [[ $ELAPSED -ge $MAX_TIME ]]; then
            break
        fi
    fi
    if [[ -n "$ARGUMENT_FILTER" ]]; then
        # Filter by argument pattern
        pids=$(ps aux | grep -E '[d]otnet' | grep "$ARGUMENT_FILTER" | awk '{print $2}')
    else
        # All dotnet processes
        pids=$(pgrep -x dotnet 2>/dev/null || pgrep dotnet 2>/dev/null)
    fi

    if [[ -n "$pids" ]]; then
        count=$(echo "$pids" | wc -l | tr -d ' ')
        echo "$pids" | xargs kill -9 2>/dev/null
        echo "Killed $count dotnet process(es)"
    fi

    sleep 1
done
