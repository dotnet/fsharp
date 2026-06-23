#!/usr/bin/env bash
#
# One-shot setup for trying F# hot reload. From a clone of this repo, this:
#   1. builds the F# compiler (this repo),
#   2. clones + builds the F#-aware .NET SDK (dotnet-watch),
#   3. syncs the hot-reload compiler into the SDK redist.
#
# Then start a session with:
#   source <sdk>/eng/dogfood.sh   # puts the built `dotnet` on PATH
#   dotnet watch run              # in an F# project that opts in (see hot-reload-quickstart.md)
#
# Budget ~30-45 minutes, almost all of it the two first-time builds. Re-runs are incremental.
#
# Overridable via env: SDK_REMOTE, SDK_BRANCH, SDK_ROOT.
#
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
fsharp_root="$(cd "$script_dir/.." && pwd)"
workdir="$(cd "$fsharp_root/.." && pwd)"

sdk_remote="${SDK_REMOTE:-https://github.com/NatElkins/sdk}"
sdk_branch="${SDK_BRANCH:-fsharp-hotreload-watch-v2}"
sdk_root="${SDK_ROOT:-$workdir/sdk-hotreload}"

echo "==> [1/3] Building the F# compiler"
echo "        $fsharp_root"
( cd "$fsharp_root" && ./build.sh -c Debug )

echo "==> [2/3] Cloning + building the F#-aware SDK"
echo "        $sdk_root ($sdk_branch)"
if [[ ! -d "$sdk_root/.git" ]]; then
  git clone --branch "$sdk_branch" --single-branch "$sdk_remote" "$sdk_root"
fi
# Build only the redist (the runnable SDK layout). This pulls in dotnet-watch and the rest of
# the product without compiling the repo's large test projects, so it is faster and avoids
# unrelated test-only build breaks.
( cd "$sdk_root" && ./build.sh --projects "$sdk_root/src/Layout/redist/redist.csproj" -c Debug )

echo "==> [3/3] Syncing the hot-reload compiler into the SDK redist"
"$script_dir/hot-reload-sync-fcs.sh" "$fsharp_root" "$sdk_root"

cat <<EOF

==> Setup complete.

Start a hot-reload session:

  source "$sdk_root/eng/dogfood.sh"
  mkdir -p ~/hot-reload-demo && cd ~/hot-reload-demo
  dotnet new console -lang F#
  # add this line inside the .fsproj <PropertyGroup>:
  #   <OtherFlags>\$(OtherFlags) --test:HotReloadDeltas</OtherFlags>
  dotnet watch run --non-interactive

Then edit Program.fs and watch changes apply in place. See docs/hot-reload-quickstart.md
(step 4 onward) for a demo program and exactly what to try editing.
EOF
