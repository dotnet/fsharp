"""Allowlisted summaries and bounded, recipient-encrypted diagnostic transport."""

from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
import hashlib
import os
from pathlib import Path
import re
import stat
import subprocess
import tempfile
import weakref

import memory as mem

c = mem.c
require = mem.require
PUBLIC_BYTES = 512 * 1024**2
COMPACT_BYTES = 20 * 1024**2
RETENTION_DAYS = 14
CONCLUSION_BYTES = 2048
MAX_ENTRIES = 64
AGE_VERSION = "v1.2.1"
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
    return path.resolve(strict=exists)


def _identity(info):
    return info.st_dev, info.st_ino


def _open_file(path, *, delete=False):
    if os.name != "nt":
        return os.open(path, os.O_RDONLY | getattr(os, "O_NOFOLLOW", 0))
    import ctypes
    from ctypes import wintypes
    import msvcrt

    kernel = ctypes.WinDLL("kernel32", use_last_error=True)
    kernel.CreateFileW.restype = wintypes.HANDLE
    kernel.CreateFileW.argtypes = [wintypes.LPCWSTR, wintypes.DWORD, wintypes.DWORD,
                                  ctypes.c_void_p, wintypes.DWORD, wintypes.DWORD, wintypes.HANDLE]
    kernel.CloseHandle.argtypes = [wintypes.HANDLE]
    # Deny writes/deletes while hashing or passing this same handle to the encryptor.
    handle = kernel.CreateFileW(str(path), 0x80000000 | (0x10000 if delete else 0),
                                0x1, None, 3, 0x200000, None)
    if handle == ctypes.c_void_p(-1).value:
        raise ctypes.WinError(ctypes.get_last_error())
    descriptor = None
    try:
        descriptor = msvcrt.open_osfhandle(handle, os.O_RDONLY | os.O_BINARY)
        os.set_inheritable(descriptor, False)
        return descriptor
    except BaseException:
        if descriptor is None:
            kernel.CloseHandle(handle)
        else:
            os.close(descriptor)
        raise


def _unlink_open_file(path, stream):
    if os.name == "nt":
        import ctypes
        from ctypes import wintypes
        import msvcrt

        kernel = ctypes.WinDLL("kernel32", use_last_error=True)
        kernel.SetFileInformationByHandle.argtypes = [wintypes.HANDLE, ctypes.c_int,
                                                      ctypes.c_void_p, wintypes.DWORD]
        kernel.SetFileInformationByHandle.restype = wintypes.BOOL
        disposition = wintypes.BOOLEAN(True)
        if not kernel.SetFileInformationByHandle(msvcrt.get_osfhandle(stream.fileno()), 4,
                                                 ctypes.byref(disposition), ctypes.sizeof(disposition)):
            raise ctypes.WinError(ctypes.get_last_error())
    else:
        # POSIX cannot unlink by handle. Isolate the name before checking which object moved.
        directory = Path(tempfile.mkdtemp(prefix=".dump-cleanup-", dir=path.parent))
        isolated = directory / "dump.dmp"
        try:
            path.rename(isolated)
            if _identity(isolated.lstat()) != _identity(os.fstat(stream.fileno())):
                os.link(isolated, path, follow_symlinks=False)  # Never overwrite a newly installed path.
                isolated.unlink()
                raise ValueError("Owned dump was replaced")
            isolated.unlink()
        finally:
            if not isolated.exists():
                directory.rmdir()


def _read_file(path, *, limit=None, use=None, remove=None):
    """Check the open object as well as every ancestor; never hash a recorded path alone."""
    path = _path(path)
    before = path.stat()
    require(stat.S_ISREG(before.st_mode) and before.st_nlink == 1,
            "Artifacts must be regular files without hardlinks")
    require(before.st_size <= (c.LIMITS["artifact_bytes"] if limit is None else limit),
            "Artifact size budget exceeded")
    digest, data = hashlib.sha256(), []
    descriptor = _open_file(path) if remove is None else _open_file(path, delete=True)
    with os.fdopen(descriptor, "rb") as stream:
        opened = os.fstat(stream.fileno())
        require(_identity(opened) == _identity(before), "Artifact identity changed")
        if use is not None:
            use(stream)
            stream.seek(0)
        size = 0
        while block := stream.read(1024 * 1024):
            size += len(block)
            require(size <= before.st_size, "Artifact grew while reading")
            digest.update(block)
            if limit is not None:
                data.append(block)
        after = os.fstat(stream.fileno())
        current = _path(path).stat()
        # Windows handle ChangeTime can advance on reads; the sharing lock prevents writes.
        timestamp = "st_birthtime_ns" if os.name == "nt" else "st_ctime_ns"
        require(size == before.st_size and _identity(before) == _identity(after) == _identity(current)
                and before.st_mtime_ns == opened.st_mtime_ns == after.st_mtime_ns == current.st_mtime_ns
                and before.st_ctime_ns == current.st_ctime_ns
                and (os.name == "nt" or opened.st_ctime_ns == after.st_ctime_ns)
                and all(getattr(info, timestamp, info.st_ctime_ns) == getattr(before, timestamp, before.st_ctime_ns)
                        for info in (opened, after, current))
                and current.st_size == size and current.st_nlink == 1, "Artifact changed while reading")
        if remove is not None:
            require({"path": path, "sha256": digest.hexdigest(), "size_bytes": size,
                     "identity": _identity(before)} == remove, "Owned dump was replaced")
            _unlink_open_file(path, stream)
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


def _record(kind, sha256, size, conclusion, now, *, source_sha256=None, provenance=None):
    created = _time(now)
    record = {
        "sha256": sha256, "size_bytes": size, "type": kind, "kind": kind,
        "artifact_id": f"{kind}-{sha256}", "location": f"unavailable:{kind}-{sha256}",
        "access": "private", "availability": "unavailable", "role": "diagnostic",
        "collected_at": created.isoformat(), "expires_at": (created + timedelta(days=RETENTION_DAYS)).isoformat(),
        "capture_sha256": c.m.file_record(c.__file__)["sha256"],
        "reducer_sha256": c.m.file_record(__file__)["sha256"], "reanalyzes": None, "metrics": [],
        "source_sha256": source_sha256 or sha256, "conclusion": conclusion, "reason": "private_binary",
        "schema_version": 2, "transport_sha256": None, "transport_size_bytes": 0,
        "encryption": None, "acknowledgment": None,
        "provenance": provenance or {"run_id": "offline", "fingerprint": source_sha256 or sha256},
    }
    mem.validate_artifact(record)
    return record


def _encrypt(path, record, config, root, budget):
    require(type(config) is dict and set(config) == {"recipient", "tool"},
            "Missing owner encryption configuration")
    recipient = config["recipient"]
    require(isinstance(recipient, str) and re.fullmatch(r"age1[023456789acdefghjklmnpqrstuvwxyz]{58}", recipient),
            "Only one native age X25519 recipient is supported")
    c.m.validate_file_record(config["tool"])
    tool = _path(config["tool"]["path"])
    require(_read_file(tool)[0] == config["tool"]["sha256"], "Encryption executable identity differs")
    # Single native recipient: 168-byte header, 16-byte nonce, 16-byte tag per 64 KiB chunk.
    expected = record["size_bytes"] + 184 + 16 * max(1, (record["size_bytes"] + 65535) // 65536)
    require(expected <= budget, "Ciphertext exceeds artifact budget")
    environment = c.tool_environment()
    probe = c.run_tool([str(tool), "--version"], root, environment, root, "age-version", timeout=30)
    require(probe["status"] == "collected" and probe["stdout"].strip() == AGE_VERSION,
            "Pinned age executable unavailable or incompatible")
    output = root / "encrypted.age"
    require(not output.exists() and path != output, "Encryption input/output collision")

    def encrypt(stream):
        def monitor():
            if output.exists():
                require(_path(output).stat().st_size <= expected, "Ciphertext size overflow")
        result = c.run_tool([str(tool), "--encrypt", "--recipient", recipient, "--output", str(output)],
                            root, environment, root, "age-encrypt", input_file=stream, monitor=monitor)
        require(result["status"] == "collected" and result["exit_status"] == 0, "Encryption tool failed")

    sha256, size, _, _ = _read_file(path, use=encrypt)
    require((sha256, size) == (record["sha256"], record["size_bytes"]), "Encryption input changed")
    transport_sha256, transport_size, _, _ = _read_file(output)
    with output.open("rb") as stream:
        header = stream.read(168)
    require(transport_size == expected and re.fullmatch(
        rb"age-encryption.org/v1\n-> X25519 [A-Za-z0-9+/]{43}\n[A-Za-z0-9+/]{43}\n--- [A-Za-z0-9+/]{43}\n",
        header), "Truncated or unsupported encrypted output")
    require(_read_file(tool)[0] == config["tool"]["sha256"], "Encryption executable changed")
    record["artifact_id"] = f"{record['type']}-{transport_sha256}"
    record.update(transport_sha256=transport_sha256, transport_size_bytes=transport_size,
                  encryption={"method": "age-X25519", "version": AGE_VERSION,
                              "recipient_sha256": hashlib.sha256(recipient.encode("ascii")).hexdigest(),
                              "tool_sha256": config["tool"]["sha256"]},
                  access="restricted", availability="pending", reason="encrypted_pending",
                  location=record["artifact_id"] + ".age")
    return output


def stage_artifacts(private_root, public_root, entries, now, *, encryption=None, provenance=None,
                    dump_workspace=None, used_bytes=0, used_compact_bytes=0, seal_manifest=None):
    """Stage explicit entries, never a directory tree.

    Only the protected controller supplies entries, recipient and provenance.
    Publication is atomic; a manifest is a pending transport, never a sink receipt.
    """
    private, public = _path(private_root), _path(public_root, exists=False)
    require(private.is_dir() and not public.is_relative_to(private) and not private.is_relative_to(public),
            "Private and public artifact roots must be separate")
    require(not public.exists() or public.is_dir() and not any(public.iterdir()),
            "Public staging requires a new or empty directory")
    require(type(entries) is list and len(entries) <= MAX_ENTRIES, "Artifact entry inventory exceeded")
    require(type(used_bytes) is int and 0 <= used_bytes <= PUBLIC_BYTES
            and type(used_compact_bytes) is int and 0 <= used_compact_bytes <= COMPACT_BYTES,
            "Invalid previously staged byte counts")
    records, files, binaries, identities = [], {}, [], set()
    source_total = 0
    text_limit = min(COMPACT_BYTES, c.LIMITS["json_bytes"], mem.LIMITS["file_bytes"])
    for entry in entries:
        require(type(entry) is dict and set(entry) == {"file", "kind", "conclusion"}
                and entry["kind"] in ("summary", "profile-trace", "dump"), "Unsupported artifact entry or kind")
        source, kind = entry["file"], entry["kind"]
        c.m.validate_file_record(source)
        require(set(source) <= {"path", "sha256", "size_bytes"}, "Unsupported file identity fields")
        path = _path(source["path"])
        require(path.is_relative_to(private) and path != private, "Artifact source is outside private root")
        relative = path.relative_to(private)
        require(kind != "summary" or not any(re.search(r"(?i)(?:^|[_.-])(?:private|dump)(?:$|[_.-])", p)
                                             for p in relative.parts), "Private summary path is forbidden")
        require(kind == "dump" or not any(re.search(r"(?i)(?:^|[_.-])(?:inputs?|env|environment|credentials?|secrets?)"
                                  r"(?:$|[_.-])", p) for p in relative.parts),
                "Private inputs, environments and dumps cannot be staged")
        require(path.suffix.lower() == {"summary": ".json", "profile-trace": ".nettrace", "dump": ".dmp"}[kind],
                "Artifact extension does not match allowlisted kind")
        if kind == "dump":
            claim_dump(dump_workspace, source)
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
                             source_sha256=sha256, provenance=provenance)
            record.update(access="public", availability="pending", reason="sanitized_summary",
                          location=record["artifact_id"] + ".json", transport_sha256=record["sha256"],
                          transport_size_bytes=len(data))
            require(record["location"] not in files, "Duplicate sanitized artifact")
            files[record["location"]] = data
        else:
            record = _record(kind, sha256, size, conclusion, now, provenance=provenance)
            binaries.append((path, record))
        require(len(mem.encoded(record)) <= mem.LIMITS["file_bytes"], "Oversized artifact record")
        records.append(record)
    created = _time(now)
    manifest = {"schema_version": 2, "kind": "artifact-manifest", "created_at": created.isoformat(),
                "expires_at": (created + timedelta(days=RETENTION_DAYS)).isoformat(),
                "retention_days": RETENTION_DAYS, "artifacts": sorted(records, key=lambda r: r["artifact_id"]),
                "total_bytes": 0, "compact_bytes": 0, "status": "pending",
                "limits": {"total_bytes": PUBLIC_BYTES, "compact_bytes": COMPACT_BYTES}}

    def totals():
        while True:
            compact = sum(map(len, files.values())) + len(mem.encoded(manifest))
            total = compact + sum(r["transport_size_bytes"] for _, r in binaries)
            if (total, compact) == (manifest["total_bytes"], manifest["compact_bytes"]):
                return total, compact
            manifest.update(total_bytes=total, compact_bytes=compact)

    with tempfile.TemporaryDirectory(prefix="retention-", dir=private) as temporary:
        scratch = Path(temporary)
        payload = scratch / "payload"
        payload.mkdir(mode=0o700)
        for path, record in binaries:
            try:
                require(record["size_bytes"] <= min(c.LIMITS["trace_bytes"], PUBLIC_BYTES - used_bytes),
                        "Essential binary exceeds budget")
                encrypted = _encrypt(path, record, encryption, scratch, PUBLIC_BYTES - used_bytes - totals()[0])
                require(totals()[0] + used_bytes <= PUBLIC_BYTES, "Ciphertext aggregate budget exceeded")
                encrypted.rename(payload / record["location"])
            except (ValueError, OSError, subprocess.SubprocessError) as error:
                record["artifact_id"] = f"{record['type']}-{record['sha256']}"
                record.update(availability="unavailable", access="private", encryption=None,
                              transport_sha256=None, transport_size_bytes=0,
                              location="unavailable:" + record["artifact_id"],
                              reason="blocked_retention: " + _conclusion(str(error), private)[:512])
                manifest["status"] = "blocked_retention"
            finally:
                if (scratch / "encrypted.age").exists():
                    _path(scratch / "encrypted.age").unlink()
        manifest["artifacts"].sort(key=lambda r: r["artifact_id"])
        total, compact = totals()
        require(total + used_bytes <= min(PUBLIC_BYTES, c.LIMITS["total_bytes"])
                and compact + used_compact_bytes <= COMPACT_BYTES
                and len(mem.encoded(manifest)) <= min(c.LIMITS["json_bytes"], mem.LIMITS["file_bytes"]),
                "Public/compact artifact aggregate budget exceeded")
        for name, data in [*sorted(files.items()), ("manifest.json", mem.encoded(manifest))]:
            with (payload / name).open("xb") as stream:
                stream.write(data)
                stream.flush()
                os.fsync(stream.fileno())
        if seal_manifest is not None:
            seal_manifest(manifest)
        _path(public, exists=False)
        if public.exists():
            public.rmdir()
        payload.rename(public)
    return manifest


def verify_artifacts(public_root, manifest, now):
    """Offline restart checks: expiry, regular-file identity, actual size and SHA256."""
    now = _time(now)
    require(type(manifest) is dict and manifest.get("schema_version") in (1, 2)
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
                                       if r["availability"] in ("pending", "available")}
        require({p.name for p in root.iterdir()} == expected, "Unlisted files in public staging")
        total = sum(_path(p).stat().st_size for p in root.iterdir())
        compact = sum(_path(p).stat().st_size for p in root.iterdir() if p.suffix == ".json")
        require(total == manifest["total_bytes"] and total <= PUBLIC_BYTES and compact <= COMPACT_BYTES,
                "Staged aggregate size differs or exceeds budget")
        if manifest["schema_version"] == 2:
            require(compact == manifest["compact_bytes"]
                    and manifest["limits"] == {"total_bytes": PUBLIC_BYTES, "compact_bytes": COMPACT_BYTES},
                    "Staged limits differ")
        inventory_ok = True
    except (ValueError, OSError):
        inventory_ok = False
    result = []
    for record in manifest["artifacts"]:
        status = mem.artifact_availability(record, now)
        uploadable = False
        if status["reason"] != "expired" and record["availability"] in ("pending", "available"):
            try:
                require(inventory_ok, "Staged manifest is missing or changed")
                require(record.get("schema_version", 1) == manifest["schema_version"]
                        and (manifest["schema_version"] == 2 or
                             record["type"] == "summary" and record["access"] == "public"),
                        "Legacy binary transport is forbidden")
                suffix = ".json" if record["type"] == "summary" else ".age"
                require(record["location"] == record["artifact_id"] + suffix
                        and re.fullmatch(r"(?:summary|profile-trace|dump)-[0-9a-f]{64}" + re.escape(suffix),
                                         record["location"]),
                        "Untrusted artifact location")
                sha256, size, _, _ = _read_file(root / record["location"],
                                               limit=mem.LIMITS["file_bytes"] if suffix == ".json" else None)
                require(sha256 == record.get("transport_sha256", record["sha256"])
                        and size == record.get("transport_size_bytes", record["size_bytes"]),
                        "Staged artifact hash or size differs")
                uploadable = True
                if manifest["schema_version"] == 1:
                    status = {"available": False, "reason": "legacy_local_only"}
            except (ValueError, OSError):
                status = {"available": False, "reason": "missing_or_changed"}
        result.append(dict(status, artifact_id=record["artifact_id"], sha256=record["sha256"],
                           uploadable=uploadable, inventory_verified=inventory_ok))
    return result


def retention_result(public_root, manifest, now):
    """Exact upload inventory for a trusted sink; local existence is not remote durability."""
    checked = verify_artifacts(public_root, manifest, now)
    files = [{"path": str(Path(public_root) / record["location"]), "kind": record["type"],
              "sha256": record.get("transport_sha256", record["sha256"]),
              "size_bytes": record.get("transport_size_bytes", record["size_bytes"])}
             for record, status in zip(manifest["artifacts"], checked) if status["uploadable"]]
    healthy = all(s["inventory_verified"] for s in checked) and all(
                  s["uploadable"] for r, s in zip(manifest["artifacts"], checked)
                  if r["availability"] in ("pending", "available"))
    if healthy and any(s["reason"] != "expired" for s in checked):
        sha256, size, _, _ = _read_file(Path(public_root) / "manifest.json", limit=mem.LIMITS["file_bytes"])
        files.append({"path": str(Path(public_root) / "manifest.json"), "kind": "manifest",
                      "sha256": sha256, "size_bytes": size})
    expired = mem.timestamp(manifest["expires_at"]) <= _time(now)
    return {"status": "expired" if expired else manifest.get("status", "pending") if healthy else "blocked_retention",
            "files": files if healthy else [], "artifacts": checked,
            "manifest_sha256": hashlib.sha256(mem.encoded(manifest)).hexdigest(),
            "retention_days": RETENTION_DAYS, "created_at": manifest["created_at"], "expires_at": manifest["expires_at"],
            "limits": {"total_bytes": PUBLIC_BYTES, "compact_bytes": COMPACT_BYTES},
            "total_bytes": manifest["total_bytes"], "compact_bytes": manifest.get("compact_bytes", manifest["total_bytes"])}


def acknowledge_artifacts(public_root, manifest, now, sink):
    """Call only a protected uploader, never an agent-supplied callback or receipt."""
    plan = retention_result(public_root, manifest, now() if callable(now) else now)
    require(plan["files"] and sum(f["size_bytes"] for f in plan["files"]) == plan["total_bytes"],
            "No complete verified upload inventory")
    receipt = sink(plan)
    completed = _time(now() if callable(now) else now)
    require(type(receipt) is dict and set(receipt) == {
        "scope", "acknowledged", "manifest_sha256", "artifact_id", "location", "retention_days", "created_at", "expires_at"}
        and receipt["scope"] in ("actions", "local_fixture") and receipt["acknowledged"] is True
        and receipt["manifest_sha256"] == plan["manifest_sha256"]
        and receipt["retention_days"] == RETENTION_DAYS
        and mem.timestamp(plan["created_at"]) <= mem.timestamp(receipt["created_at"]) <= completed
        and mem.timestamp(receipt["expires_at"]) - mem.timestamp(receipt["created_at"]) == timedelta(days=RETENTION_DAYS)
        and mem.identifier(receipt["artifact_id"]) and mem.text(receipt["location"]), "Invalid sink acknowledgment")
    if receipt["scope"] == "actions":
        require(re.fullmatch(r"https://github\.com/dotnet/fsharp/actions/runs/[0-9]+/artifacts/[0-9]+",
                             receipt["location"]) and all(receipt["location"] ==
                    f"https://github.com/dotnet/fsharp/actions/runs/{r['provenance']['run_id']}/artifacts/{receipt['artifact_id']}"
                    for r in manifest["artifacts"]), "Invalid Actions artifact owner/location")
    require(retention_result(public_root, manifest, completed) == plan,
            "Staging changed or expired during upload")
    records = []
    for record in manifest["artifacts"]:
        updated = dict(record)
        if receipt["scope"] == "actions" and record["availability"] == "pending":
            updated.update(availability="available", acknowledgment=dict(receipt))
            updated["acknowledgment"]["validation"] = mem.artifact_validation(updated)
        records.append(updated)
    return records


def dump_retention_policy():
    return {"default": "encrypted", "status": "owner_configuration_required", "persistent": False,
            "plaintext_fallback": False, "reason": "Encrypted pending transport requires a trusted sink acknowledgment"}


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
    _read_file(actual["path"], remove=receipt)
    return record


def analyze_owned_dump(owned, record, analyze, now, retain=None):
    """Analyze before optional encryption, then remove only the same owned plaintext object."""
    receipt = claim_dump(owned, record)
    failure = None
    try:
        conclusion = analyze(receipt["path"])
    except BaseException as error:
        conclusion = "Dump analysis failed or was cancelled"
        failure = error
    try:
        retained = retain(receipt, conclusion) if retain is not None else None
    except BaseException as error:
        error.artifact = remove_owned_dump(owned, receipt, conclusion, now)
        error.artifact["reason"] = "retention_failed_plaintext_removed"
        raise
    removed = remove_owned_dump(owned, receipt, conclusion, now)
    result = retained if retained is not None else removed
    if failure is not None:
        failure.artifact = result
        raise failure
    return result
