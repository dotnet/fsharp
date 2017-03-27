#!/bin/sh
# Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

# This script generates a patch by comparing the contents of the fsharp/fsharp
# and Microsoft/visualfsharp Github repositories. The resulting patch can be applied,
# if desired, to one of the repositories to synchronize changes in one direction;
# CAUTION MUST BE TAKEN though, since this script generates the patch in a way that
# ignores any ordering of commits in the repos -- so it could also end up reverting
# changes that were actually needed.
# The produced patch is primarily useful for examining differences between two repos
# and making it easier to submit targeted PRs to manually synchronize changes.

# Helper function to clean up after we're done (or if there's an error).
cleanup () {
    # Leave the working directory, then delete it.
    { popd && rm -rf "$tmpdir"; } || printf "Unable to clean up temp directory '%s'\n" "$tmpdir"
}

# Helper function to print an error message and exit with a non-zero error code.
failwith () {
    printf "Error: %s\n" "$1" >&2
    cleanup
    exit 1
}

# Delete the .git folder from a folder containing an extracted/cloned repository.
remove_git () {
    { pushd "$1" && rm -rf .git && popd; } || \
    { printf "Error: Couldn't remove .git from '%s'\n" "$1"; cleanup_error; }
}

# Delete "don't care" files from a folder containing an extracted/cloned repository.
# These are files with known differences between the two repos, but where we don't care
# about those differences for the purposes of generating a patch.
remove_irrelevant_files () {
    if pushd "$1"; then
        # TODO: Use 'for in _ do' syntax here instead? Maybe read the list from a separate text file?
        # If any of the files in here can't be removed, ignore the failure and keep going.
        rm .travis.yml
        rm CHANGELOG.md
        rm CONTRIBUTING.md
        rm README.md

        popd
    fi
}

# Options for controlling the behavior of this script.
tmpdir="diff_tmp"           # name of temp folder to create and work in
fetch_source_by_cloning=1   # git diff doesn't seem to like curl+tar, so clone with git for now.
github_org1="fsharp"
github_repo1="fsharp"
github_org2="Microsoft"
github_repo2="visualfsharp"
repo_dir1="${github_org1}_${github_repo1}"
repo_dir2="${github_org2}_${github_repo2}"
include_all_changes=0       # If set to 1, file additions/deletions will also be included in the patch.
patch_path="../${github_repo1}-${github_repo2}.patch"

# Create a temporary directory to work in.
if ! mkdir "$tmpdir"; then
    printf "Unable to create temp directory '%s' (does it already exist?)\n" "$tmpdir"
    exit 1
fi

# Enter the temporary directory.
if ! pushd "$tmpdir"; then
    printf "Can't enter temp directory '%s'\n" "$tmpdir"
    exit 1
fi

# We can either fetch the source from the target repos by cloning them,
# or simply by downloading an archive/tarball of the current 'master' branches.
if [ "$fetch_source_by_cloning" = '1' ]; then
    # Clone folders with git.
    # The code above using curl and tar should work, but git diff doesn't seem to like it.
    { git clone "https://github.com/${github_org1}/${github_repo1}.git" "$repo_dir1" && \
      git clone "https://github.com/${github_org2}/${github_repo2}.git" "$repo_dir2"; } ||
    { printf "Unable to clone one or more of the target repositories.\n" && cleanup_error; }

    # Enter each cloned repo and delete the .git folder to eliminate a bunch of garbage in the patch.
    remove_git "$repo_dir1"
    remove_git "$repo_dir2"
else
    # Download source tarballs for both repos.
    repo_archive1="$repo_dir1.zip"
    repo_archive2="$repo_dir2.zip"
    { curl -L -o "$repo_archive1" "https://github.com/${github_org1}/${github_repo1}/archive/master.zip" && \
      curl -L -o "$repo_archive2" "https://github.com/${github_org2}/${github_repo2}/archive/master.zip"; } || \
    { printf "Couldn't download one or more of the source tarballs.\n" && cleanup_error; }

    # Extract the contents of the source tarballs.
    { mkdir "$repo_dir1" && tar -xzf "$repo_archive1" -C "$repo_dir1" && \
      mkdir "$repo_dir2" && tar -xzf "$repo_archive2" -C "$repo_dir2"; } || \
    { printf "Couldn't extract one or more of the source tarballs.\n" && cleanup_error; }
fi

# Remove unwanted/"don't care" files from both repos.
remove_irrelevant_files "$repo_dir1"
remove_irrelevant_files "$repo_dir2"

# Use git-diff to compare the two source trees and create a patch.
# Note, the 'git diff' command reeturns with non-zero exit code if there are any warnings emitted,
# which there will be -- so we can't really check the exit code to see if it succeeded'
if [ "$include_all_changes" != '1' ]; then
    diff_filter_arg="--diff-filter=ad"
fi

git diff --diff-algorithm=minimal "$diff_filter_arg" --no-index -- "$repo_dir1" "$repo_dir2" > "$patch_path"

# Cleanup the temp folder.
cleanup
