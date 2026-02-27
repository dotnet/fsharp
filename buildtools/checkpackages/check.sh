#!/usr/bin/env bash
set -uo pipefail

if [ $# -ne 1 ]; then
    echo "Usage: $0 <project>" >&2
    exit 1
fi

project="$1"

dotnet restore "$project" 2>/dev/null

if [ $? -eq 0 ]; then
    package=$(cat Version.txt)
    echo "Package restore succeeded for '${package}', expected to fail." >&2
    echo "This usually means that the package has been already published." >&2
    echo "Please, bump the version to fix this failure." >&2
    exit 1
fi
