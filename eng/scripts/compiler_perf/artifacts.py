"""Deterministic, allowlisted public summaries and private evidence identities.

Only controller-constructed workflow-summary JSON is publishable. Raw EventPipe
traces can contain private paths, inputs and runtime strings: retain their hash
and conclusion, not an unsanitizable binary. This module never uploads files.
"""

from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
import hashlib
import os
from pathlib import Path
import re
import stat
import weakref

import memory as mem

c = mem.c
require = mem.require
PUBLIC_BYTES = 512 * 1024**2
COMPACT_BYTES = 20 * 1024**2
RETENTION_DAYS = 14
CONCLUSION_BYTES = 2048
MAX_ENTRIES = 64
SUMMARY_FIELDS = {"schema_version", "kind", "status", "conclusion", "attempt_id",
                  "outcome", "profile_status", "measurement_status", "reason"}


def _path(value, *, exists=True):
    path = Path(value)
    require(path.is_absolute() and ".." not in path.parts
            and not any(":" in p or p.lower() == ".git" or p.rstrip(" .") != p for p in path.parts[1:])
            and not str(path).startswith(("\\\\?\\", "\\\\.\\")),
            "An absolute, non-traversing artifact path is required")
    for ancestor in (*reversed(path.parents), path):
        try:
            info = ancestor.lstat()
        except FileNotFoundError:
            require(not exists and ancestor == path, "Artifact ancestor is missing")
            continue
        require(not stat.S_ISLNK(info.st_mode)
                and not getattr(info, "st_file_attributes", 0) & stat.FILE_ATTRIBUTE_REPARSE_POINT,
                "Symlink or junction artifact paths are forbidden")
    return path


def _identity(info):
    return info.st_dev, info.st_ino


def _read_file(path, *, limit=None):
    """Check the open object as well as every ancestor; never hash a recorded path alone."""
    path = _path(path)
    before = path.stat()
    require(stat.S_ISREG(before.st_mode) and before.st_nlink == 1,
            "Artifacts must be regular files without hardlinks")
    require(before.st_size <= (c.LIMITS["artifact_bytes"] if limit is None else limit),
            "Artifact size budget exceeded")
    digest, data = hashlib.sha256(), []
    descriptor = os.open(path, os.O_RDONLY | getattr(os, "O_BINARY", 0) | getattr(os, "O_NOFOLLOW", 0))
    with os.fdopen(descriptor, "rb") as stream:
        opened = os.fstat(stream.fileno())
        require(_identity(opened) == _identity(before), "Artifact identity changed")
        size = 0
        while block := stream.read(1024 * 1024):
            size += len(block)
            require(size <= before.st_size, "Artifact grew while reading")
            digest.update(block)
            if limit is not None:
                data.append(block)
        after = os.fstat(stream.fileno())
    current = _path(path).stat()
    require(size == before.st_size and _identity(before) == _identity(after) == _identity(current)
            and before.st_mtime_ns == after.st_mtime_ns == current.st_mtime_ns
            and before.st_ctime_ns == current.st_ctime_ns and opened.st_ctime_ns == after.st_ctime_ns
            and current.st_size == size and current.st_nlink == 1, "Artifact changed while reading")
    return digest.hexdigest(), size, b"".join(data), _identity(before)


def _conclusion(value, private_root):
    require(mem.text(value) and len(value.encode("utf-8")) <= CONCLUSION_BYTES, "Invalid or oversized conclusion")
    text = re.sub(r"(?i)\b(?:basic|bearer)\s+\S+", "[REDACTED]", value)
    text = re.sub(r"-----BEGIN [A-Z ]*PRIVATE KEY-----[\s\S]*", "[REDACTED]", text)
    text = re.sub(r"\b(?:AKIA|ASIA)[A-Z0-9]{16}\b", "[REDACTED]", text)
    for key, secret in os.environ.items():
        if re.search(r"ACCESS_KEY|PRIVATE_KEY|CONNECTION_STRING", key, re.I) and len(secret) >= 4:
            text = text.replace(secret, "[REDACTED]")
    text = c.sanitize(text)
    for form in (str(private_root), Path(private_root).as_posix()):
        text = re.sub(re.escape(form), "[REDACTED]", text, flags=re.I)
    text = re.sub(r"(?i)\b(?:environment|environ|env|inputs?|arguments?|command|PATH|HOME|USERPROFILE)"
                  r"\s*[=:][^\r\n]*", "[REDACTED]", text)
    text = re.sub(r"(?i)(?:[a-z]:[\\/]|(?<![\w:/])/(?:home|Users|private|tmp|var)/)[^\s\"']+",
                  "[REDACTED]", text)
    require(len(text.encode("utf-8")) <= CONCLUSION_BYTES, "Redacted conclusion exceeds budget")
    return text


def _time(now):
    now = mem.timestamp(now) if isinstance(now, str) else now
    require(isinstance(now, datetime) and now.tzinfo is not None, "A timezone-aware staging time is required")
    return now.astimezone(timezone.utc)


def _record(kind, sha256, size, conclusion, now, *, source_sha256=None):
    created = _time(now)
    record = {
        "sha256": sha256, "size_bytes": size, "type": kind, "kind": kind,
        "artifact_id": f"{kind}-{sha256}", "location": f"unavailable:{kind}-{sha256}",
        "access": "private", "availability": "unavailable", "role": "diagnostic",
        "collected_at": created.isoformat(), "expires_at": (created + timedelta(days=RETENTION_DAYS)).isoformat(),
        "capture_sha256": c.m.file_record(c.__file__)["sha256"],
        "reducer_sha256": c.m.file_record(__file__)["sha256"], "reanalyzes": None, "metrics": [],
        "source_sha256": source_sha256 or sha256, "conclusion": conclusion, "reason": "private_binary",
    }
    mem.validate_artifact(record)
    return record


def stage_artifacts(private_root, public_root, entries, now):
    """Stage explicit entries, never a directory tree.

    Entries have exactly file (measurement file record, optional size_bytes),
    kind (summary or profile-trace), and conclusion. Summaries require
    schema_version=1, kind=workflow-summary, status and conclusion; the optional
    scalar fields are listed in SUMMARY_FIELDS. No dispatch policy knobs exist.
    Returned artifact records can be stored directly in memory's evidence list.
    """
    private, public = _path(private_root), _path(public_root, exists=False)
    require(private.is_dir() and not public.is_relative_to(private) and not private.is_relative_to(public),
            "Private and public artifact roots must be separate")
    require(not public.exists() or public.is_dir() and not any(public.iterdir()),
            "Public staging requires a new or empty directory")
    require(type(entries) is list and len(entries) <= MAX_ENTRIES, "Artifact entry inventory exceeded")
    records, files, identities = [], {}, set()
    source_total = 0
    text_limit = min(COMPACT_BYTES, c.LIMITS["json_bytes"], mem.LIMITS["file_bytes"])
    for entry in entries:
        require(type(entry) is dict and set(entry) == {"file", "kind", "conclusion"}
                and entry["kind"] in ("summary", "profile-trace"), "Unsupported artifact entry or kind")
        source, kind = entry["file"], entry["kind"]
        c.m.validate_file_record(source)
        require(set(source) <= {"path", "sha256", "size_bytes"}, "Unsupported file identity fields")
        path = _path(source["path"])
        require(path.is_relative_to(private) and path != private, "Artifact source is outside private root")
        relative = path.relative_to(private)
        require(not any(re.search(r"(?i)(?:^|[_.-])(?:private|inputs?|env|environment|credentials?|secrets?|dump)"
                                  r"(?:$|[_.-])", p) for p in relative.parts),
                "Private inputs, environments and dumps cannot be staged")
        require(path.suffix.lower() == (".json" if kind == "summary" else ".nettrace"),
                "Artifact extension does not match allowlisted kind")
        source_total += path.stat().st_size
        require(source_total <= c.LIMITS["total_bytes"], "Source inventory exceeds collector budget")
        conclusion = _conclusion(entry["conclusion"], private)
        sha256, size, data, _ = _read_file(path, limit=text_limit if kind == "summary" else None)
        require(sha256 == source["sha256"], "Source artifact hash differs")
        require("size_bytes" not in source or type(source["size_bytes"]) is int and source["size_bytes"] == size,
                "Source artifact size differs")
        require((kind, sha256) not in identities, "Duplicate artifact identity")
        identities.add((kind, sha256))
        if kind == "summary":
            summary = mem.decoded(data.decode("utf-8"))
            require(type(summary) is dict and set(summary) <= SUMMARY_FIELDS
                    and {"schema_version", "kind", "status", "conclusion"} <= set(summary)
                    and type(summary["schema_version"]) is int and summary["schema_version"] == 1
                    and summary["kind"] == "workflow-summary", "Unsupported public summary schema")
            for key in summary.keys() - {"schema_version", "kind"}:
                if key in ("conclusion", "reason"):
                    summary[key] = _conclusion(summary[key], private)
                else:
                    require(mem.identifier(summary[key]) and _conclusion(summary[key], private) == summary[key],
                            "Invalid public summary label")
            data = mem.encoded(summary)
            require(len(data) <= text_limit, "Sanitized summary exceeds size budget")
            record = _record(kind, hashlib.sha256(data).hexdigest(), len(data), conclusion, now,
                             source_sha256=sha256)
            record.update(access="public", availability="available", reason="sanitized_summary",
                          location=record["artifact_id"] + ".json")
            require(record["location"] not in files, "Duplicate sanitized artifact")
            files[record["location"]] = data
        else:
            record = _record(kind, sha256, size, conclusion, now)
            if size > min(PUBLIC_BYTES, c.LIMITS["trace_bytes"]):
                record["reason"] = "essential_binary_exceeds_budget"
        require(len(mem.encoded(record)) <= mem.LIMITS["file_bytes"], "Oversized artifact record")
        records.append(record)
    created = _time(now)
    manifest = {"schema_version": 1, "kind": "artifact-manifest", "created_at": created.isoformat(),
                "expires_at": (created + timedelta(days=RETENTION_DAYS)).isoformat(),
                "retention_days": RETENTION_DAYS, "artifacts": sorted(records, key=lambda r: r["artifact_id"]),
                "total_bytes": 0}
    content_size = sum(map(len, files.values()))
    while True:
        total = content_size + len(mem.encoded(manifest))
        if total == manifest["total_bytes"]:
            break
        manifest["total_bytes"] = total
    require(total <= min(PUBLIC_BYTES, COMPACT_BYTES, c.LIMITS["total_bytes"])
            and len(mem.encoded(manifest)) <= min(c.LIMITS["json_bytes"], mem.LIMITS["file_bytes"]),
            "Public/compact artifact aggregate budget exceeded")
    # Validate everything before creating output; the manifest is the last file.
    if not public.exists():
        public.mkdir(mode=0o700)
    for name, data in [*sorted(files.items()), ("manifest.json", mem.encoded(manifest))]:
        _path(public)
        with (public / name).open("xb") as stream:
            stream.write(data)
    return manifest


def verify_artifacts(public_root, manifest, now):
    """Offline restart checks: expiry, regular-file identity, actual size and SHA256."""
    now = _time(now)
    require(type(manifest) is dict and manifest.get("schema_version") == 1
            and manifest.get("kind") == "artifact-manifest"
            and type(manifest.get("artifacts")) is list and len(manifest["artifacts"]) <= MAX_ENTRIES,
            "Invalid artifact manifest")
    require(manifest.get("retention_days") == RETENTION_DAYS
            and mem.timestamp(manifest["expires_at"]) - mem.timestamp(manifest["created_at"])
            == timedelta(days=RETENTION_DAYS), "Artifact retention differs from fixed policy")
    for record in manifest["artifacts"]:
        require(record["collected_at"] == manifest["created_at"] and record["expires_at"] == manifest["expires_at"],
                "Artifact retention differs from its manifest")
    try:
        root = _path(public_root)
        _, _, persisted, _ = _read_file(root / "manifest.json", limit=mem.LIMITS["file_bytes"])
        require(persisted == mem.encoded(manifest), "Persisted artifact manifest differs")
        expected = {"manifest.json"} | {r["location"] for r in manifest["artifacts"]
                                       if r["availability"] == "available"}
        require({p.name for p in root.iterdir()} == expected, "Unlisted files in public staging")
        inventory_ok = True
    except (ValueError, OSError):
        inventory_ok = False
    result = []
    for record in manifest["artifacts"]:
        status = mem.artifact_availability(record, now)
        if status["available"]:
            try:
                require(inventory_ok, "Staged manifest is missing or changed")
                require(record["location"] == record["artifact_id"] + ".json"
                        and re.fullmatch(r"summary-[0-9a-f]{64}\.json", record["location"])
                        and record["type"] == "summary" and record["access"] == "public",
                        "Untrusted artifact location")
                sha256, size, _, _ = _read_file(root / record["location"],
                                               limit=min(COMPACT_BYTES, c.LIMITS["json_bytes"],
                                                         mem.LIMITS["file_bytes"]))
                require(sha256 == record["sha256"] and size == record["size_bytes"],
                        "Staged artifact hash or size differs")
                status = {"available": True, "reason": "verified_local"}
            except (ValueError, OSError):
                status = {"available": False, "reason": "missing_or_changed"}
        result.append(dict(status, artifact_id=record["artifact_id"], sha256=record["sha256"]))
    return result


def dump_retention_policy():
    return {"default": "ephemeral", "status": "unsupported", "persistent": False,
            "plaintext_fallback": False, "reason": "No approved encryption or restricted storage is configured"}


def _outside_git(path):
    require(not any((p / ".git").exists() or (p / "HEAD").is_file() and (p / "objects").is_dir()
                    for p in (path, *path.parents)), "Private dumps must be outside Git worktrees and stores")


@dataclass(frozen=True, eq=False)
class DumpWorkspace:
    path: Path
    identity: tuple


_DUMP_WORKSPACES = weakref.WeakSet()


def create_dump_workspace(path, upload_roots=()):
    """Create owner-only ephemeral storage; never reuse or adopt an existing directory."""
    path = _path(path, exists=False)
    _outside_git(path)
    for root in upload_roots:
        root = _path(root, exists=False)
        require(not path.is_relative_to(root) and not root.is_relative_to(path),
                "Dump storage must be outside upload roots")
    require(not path.exists(), "Dump workspace must be newly owned")
    c.private_directory(path)
    owned = DumpWorkspace(path, _identity(_path(path).stat()))
    _DUMP_WORKSPACES.add(owned)
    return owned


def claim_dump(owned, record):
    """Claim an explicit freshly collected dump before analysis; receipt stays in memory."""
    require(isinstance(owned, DumpWorkspace) and owned in _DUMP_WORKSPACES
            and _identity(_path(owned.path).stat()) == owned.identity, "Dump workspace identity changed")
    c.m.validate_file_record(record)
    path = _path(record["path"])
    require(path.is_relative_to(owned.path) and path.suffix.lower() == ".dmp", "Dump is not explicitly owned")
    sha256, size, _, identity = _read_file(path)
    require(sha256 == record["sha256"], "Dump hash differs")
    return {"path": path, "sha256": sha256, "size_bytes": size, "identity": identity}


def remove_owned_dump(owned, receipt, conclusion, now):
    """After analysis, remove only the claimed resolved object, retaining its safe conclusion/hash."""
    actual = claim_dump(owned, {"path": str(receipt["path"]), "sha256": receipt["sha256"]})
    require(actual == receipt, "Owned dump was replaced")
    try:
        conclusion = _conclusion(conclusion, owned.path)
    except ValueError:
        conclusion = "Dump analysis conclusion unavailable: invalid or oversized text"
    record = _record("dump", actual["sha256"], actual["size_bytes"], conclusion, now)
    record["reason"] = "ephemeral_dump_removed"
    _path(actual["path"]).unlink()
    return record


def analyze_owned_dump(owned, record, analyze, now):
    """Analyze ephemeral bytes; failures/cancellation retain evidence, never a plaintext fallback."""
    receipt = claim_dump(owned, record)
    try:
        conclusion = analyze(receipt["path"])
    except BaseException as error:
        error.artifact = remove_owned_dump(owned, receipt, "Dump analysis failed or was cancelled", now)
        raise
    return remove_owned_dump(owned, receipt, conclusion, now)
