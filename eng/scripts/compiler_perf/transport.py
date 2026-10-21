"""Guarded fixed-branch transport; credentials never enter compiler environments.

The explicit Python-only local_remote fixture seam cannot acknowledge GitHub.
GitJournal receipts remain in local; transport acknowledgment is issued only
after an exact remote read-back. Failed writes retain a candidate ref for recovery.
"""

import base64
from contextlib import contextmanager
import os
from pathlib import Path
import re
import stat
import subprocess
import uuid

import memory as mem

REMOTE = "https://github.com/dotnet/fsharp.git"
REF = "refs/heads/" + mem.STATE_BRANCH


def _local_path(value):
    name = os.fspath(value)
    mem.require(isinstance(name, str) and name and not name.startswith(("\\\\", "//"))
                and "\0" not in name and not re.search(r":", name[2:] if re.match(r"^[A-Za-z]:[\\/]", name) else name),
                "Expected a local filesystem path, not a URL or UNC path")
    path = Path(os.path.abspath(name))
    for component in (*reversed(path.parents), path):
        info = component.lstat()
        mem.require(not stat.S_ISLNK(info.st_mode)
                    and not getattr(info, "st_file_attributes", 0) & stat.FILE_ATTRIBUTE_REPARSE_POINT,
                    "Local repository cannot traverse a symlink or reparse point")
    return path


class _Journal(mem.GitJournal):
    def __init__(self, repository, owner):
        super().__init__(repository)
        self.owner = owner

    def git(self, *args, data=None, check=True):
        return self.owner._git(*args, data=data, check=check)


class JournalTransport:
    def __init__(self, local_repository, *, local_remote=None):
        repository = _local_path(local_repository)
        gitdir = repository / ".git" if (repository / ".git").exists() else repository
        self.gitdir = _local_path(gitdir)
        mem.require(self.gitdir.is_dir(), "Journal requires a bare repository or a normal checkout")
        self.remote = str(_local_path(local_remote)) if local_remote is not None else REMOTE
        self.scope = "local_fixture" if local_remote is not None else "github"
        self.lock_path = self.gitdir / "compiler-perf-transport.lock"
        self.busy = False
        self.journal = _Journal(repository, self)
        self._check_repositories()

    def _environment(self, *, network=False):
        env = mem.c.tool_environment()
        settings = {
            "core.hooksPath": os.devnull, "core.fsmonitor": "false", "core.pager": "",
            "protocol.allow": "never", "protocol.file.allow": "always" if self.scope == "local_fixture" else "never",
            "protocol.https.allow": "always" if self.scope == "github" else "never",
            "credential.helper": "", "credential.interactive": "false",
            "http.followRedirects": "false", "http.sslVerify": "true", "http.proxy": "",
            "gc.auto": "0", "maintenance.auto": "false", "fetch.fsckObjects": "true",
            "transfer.fsckObjects": "true", "fetch.recurseSubmodules": "false",
            "push.recurseSubmodules": "no", "push.followTags": "false", "push.gpgSign": "false",
            "user.name": "Compiler performance journal", "user.email": "compiler-perf@users.noreply.github.com",
        }
        if network and self.scope == "github":
            token = os.environ.get("GH_TOKEN") or os.environ.get("GITHUB_TOKEN")
            mem.require(token and not any(c in token for c in "\r\n\0"), "Trusted GitHub transport requires a token")
            credential = base64.b64encode(("x-access-token:" + token).encode()).decode()
            settings["http.https://github.com/.extraHeader"] = "AUTHORIZATION: basic " + credential
        env.update(GIT_CONFIG_NOSYSTEM="1", GIT_CONFIG_SYSTEM=os.devnull, GIT_CONFIG_GLOBAL=os.devnull,
                   GIT_TERMINAL_PROMPT="0", GIT_NO_REPLACE_OBJECTS="1", GIT_OPTIONAL_LOCKS="0",
                   GIT_CONFIG_COUNT=str(len(settings)))
        for index, (key, value) in enumerate(settings.items()):
            env[f"GIT_CONFIG_KEY_{index}"] = key
            env[f"GIT_CONFIG_VALUE_{index}"] = value
        return env

    def _git(self, *args, data=None, check=True, network=False):
        result = subprocess.run(["git", *self.journal.location, *args], input=data, capture_output=True,
                                check=False, timeout=30, env=self._environment(network=network))
        if check and result.returncode:
            raise ValueError("Journal Git operation failed: " + mem.c.sanitize(result.stderr.decode(errors="replace")))
        return result

    def _check_repositories(self):
        paths = [self.gitdir] + ([Path(self.remote)] if self.scope == "local_fixture" else [])
        for path in paths:
            _local_path(path)
            config = _local_path(path / "config")
            mem.require(config.is_file() and (path / "objects").is_dir() and (path / "refs").is_dir(),
                        "Expected a local Git repository")
            for relative in ("commondir", "objects/info/alternates"):
                mem.require(not (path / relative).exists(), "Shared or alternate object stores are not supported")
            keys = self._git("config", "--no-includes", "--file", str(config), "--null", "--name-only",
                             "--list").stdout.decode().split("\0")
            for key in filter(None, keys):
                mem.require(re.fullmatch(
                    r"(core\.(repositoryformatversion|filemode|bare|logallrefupdates|symlinks|ignorecase|"
                    r"precomposeunicode|autocrlf|hookspath)|user\.(name|email)|remote\.origin\.(url|fetch|mirror)|"
                    r"branch\.[^.]+\.(remote|merge))", key.lower()) is not None,
                    "Untrusted repository Git configuration: " + key)
            if path != self.gitdir or self.scope == "local_fixture" and str(path) == self.remote:
                bare = self._git("config", "--no-includes", "--file", str(config), "--bool", "core.bare")
                mem.require(bare.stdout.strip() == b"true", "Fixture remote must be a bare repository")

    @contextmanager
    def _locked(self):
        if self.busy:
            raise mem.Conflict("Journal operation already active")
        try:
            self.busy = True
            self._check_repositories()
            nonce = uuid.uuid4().hex.encode()
            try:
                descriptor = os.open(self.lock_path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
            except FileExistsError as error:
                raise mem.Conflict("Local journal lock exists; ownership cannot be proven") from error
            try:
                with os.fdopen(descriptor, "wb") as stream:
                    stream.write(nonce)
                    stream.flush()
                    os.fsync(stream.fileno())
                yield
            finally:
                if self.lock_path.is_file() and not self.lock_path.is_symlink() and self.lock_path.read_bytes() == nonce:
                    self.lock_path.unlink()
        finally:
            self.busy = False

    def _list_head(self):
        output = self._git("ls-remote", "--refs", self.remote, REF, network=True).stdout.decode().splitlines()
        mem.require(len(output) <= 1, "Unexpected remote branch listing")
        if not output:
            return None
        fields = output[0].split()
        mem.require(len(fields) == 2 and fields[1] == REF and mem.digest(fields[0], 40),
                    "Invalid remote branch identity")
        return fields[0]

    def _read(self):
        for _ in range(5):
            head = self._list_head()
            if head is None:
                old = self.journal.head()
                if old is not None:
                    self._git("update-ref", "-d", REF, old)
                return mem.empty_state(), None
            self._git("fetch", "--quiet", "--no-tags", "--no-recurse-submodules", "--no-write-fetch-head",
                      self.remote, REF, network=True)
            # Listing/fetch can race, so validate the advertised object and recheck
            # the exact ref before adopting it. No remote wildcard is fetched.
            if self._list_head() != head:
                continue
            files = self.journal.files(head)
            mem.require(set(files) == {"state.json"}, "Canonical branch must contain only valid state.json")
            state = mem.decoded(files["state.json"])
            mem.validate_state(state)
            old = self.journal.head()
            self._git("update-ref", REF, head, old or "0" * 40)
            return state, head
        raise mem.Conflict("Remote read retry budget exhausted")

    def read(self):
        with self._locked():
            return self._read()

    def _push(self, expected, commit):
        if expected is not None:
            ancestry = self._git("merge-base", "--is-ancestor", expected, commit, check=False)
            mem.require(ancestry.returncode == 0, "Journal push is not a verified fast-forward")
        return self._git("push", "--porcelain", "--no-verify",
                         f"--force-with-lease={REF}:{expected or ''}",
                         *([f"--receive-pack=git -c core.hooksPath={os.devnull} -c gc.auto=0 "
                            "-c maintenance.auto=false receive-pack"] if self.scope == "local_fixture" else []),
                         self.remote, f"{commit}:{REF}", check=False, network=True)

    def write_state(self, base, expected_head, candidate):
        with self._locked():
            self._read()
            retained = "refs/heads/memory/compiler-self-profile-candidates/" + uuid.uuid4().hex
            retained_commit = None
            try:
                for _ in range(5):
                    remote_head = self.journal.head()
                    receipt = self.journal.write_state(base, expected_head, candidate)
                    commit = receipt["commit"]
                    mem.require(mem.digest(commit, 40), "Cannot acknowledge an absent journal commit")
                    self._git("update-ref", retained, commit, retained_commit or "0" * 40)
                    retained_commit = commit
                    result = self._push(remote_head, commit)
                    if result.returncode:
                        _, current_head = self._read()
                        mem.require(current_head != remote_head, "Journal push denied: " +
                                    mem.c.sanitize(result.stderr.decode(errors="replace")))
                        continue
                    state, verified = self._read()
                    mem.require(verified == commit, "Remote acknowledgment differs from the written commit")
                    mem.require(self.journal.files(commit) == {"state.json": mem.encoded(state)},
                                "Remote acknowledgment state differs")
                    self._git("update-ref", "-d", retained, retained_commit)
                    return {
                        "scope": self.scope, "acknowledged": True, "commit": commit,
                        "branch": mem.STATE_BRANCH, "remote": self.remote, "local": receipt,
                    }
                raise mem.Conflict("Remote CAS retry budget exhausted")
            except (ValueError, OSError, subprocess.SubprocessError) as error:
                raise mem.Conflict(str(error), getattr(error, "candidate_ref", None) or
                                   (retained if retained_commit else None)) from error
