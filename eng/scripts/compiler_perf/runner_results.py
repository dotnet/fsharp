"""Strict pinned-SDK runner-reported results, not proof of test-body execution."""

from fnmatch import fnmatchcase
from collections import deque
from pathlib import PurePath
import re

import memory as mem

SDK = "11.0.100-rc.1.26420.103"
FRAMEWORK = "xUnit.net v3 3.2.2+728c1dce01"
DURATION = r"\d+(?:\.\d+)?(?:ms|s|m|h)(?: \d+(?:\.\d+)?(?:ms|s|m|h))*"
require = mem.require


def discovery_argv(command):
    separator = command.index("--")
    selectors = command[separator + 1:]
    return [*command[:command.index("--results-directory")], "--list-tests", "json", "--",
            *[arg for index, arg in enumerate(selectors)
              if arg not in ("--minimum-expected-tests", "--report-xunit", "--report-xunit-filename") and
              (index == 0 or selectors[index - 1] not in ("--minimum-expected-tests", "--report-xunit-filename"))]]


def reconcile(discovery, output, root, command, cwd):
    inventory = mem.decoded(discovery)
    require(isinstance(inventory, dict) and inventory.get("version") == "1.0" and
            isinstance(inventory.get("testContainers"), list) and len(inventory["testContainers"]) == 1,
            "Missing or ambiguous runner discovery module")
    module = inventory["testContainers"][0]
    require(isinstance(module, dict) and {"assemblyPath", "targetFramework", "architecture", "tests"} <= module.keys(),
            "Incomplete runner discovery module")
    tree = mem.m.absolute_path(cwd)
    assembly = mem.m.absolute_path(module["assemblyPath"])
    project = PurePath(command[command.index("--project") + 1])
    tfm = command[command.index("-f") + 1]
    require(assembly.is_relative_to(tree) and assembly.name == project.stem + ".dll" and
            module["targetFramework"] == tfm and module["architecture"] in ("x64", "x86", "arm64"),
            "Runner discovery project/module differs from selection")
    tests = module["tests"]
    require(isinstance(tests, list) and tests and
            all(isinstance(t, dict) and mem.text(t.get("displayName")) and mem.text(t.get("uid")) for t in tests) and
            len({t["uid"] for t in tests}) == len(tests) == len({t["displayName"] for t in tests}),
            "Missing or ambiguous discovered test identities")
    assemblies = root.findall("assembly")
    require(len(assemblies) == 1 and assemblies[0].get("name") == str(assembly) and
            assemblies[0].get("test-framework") == FRAMEWORK and
            assemblies[0].get("target-framework") == ".NETCoreApp,Version=v" + tfm[3:] and
            assemblies[0].get("not-run", "0") == "0", "Incompatible runner XML module/framework")
    leaves = assemblies[0].findall("./collection/test")
    names = {t["displayName"]: t["uid"] for t in tests}
    require(len(leaves) == len(names) and {t.get("name") for t in leaves} == set(names),
            "Runner discovery and XML identities differ")
    expected = {}
    for test in leaves:
        name, outcome = test.get("name"), test.get("result")
        require(mem.text(test.get("type")) and mem.text(test.get("method")) and
                outcome in ("Pass", "Skip") and name not in expected, "Ambiguous runner XML identity/outcome")
        for flag, identity in (("--filter-method", test.get("type") + "." + test.get("method")),
                               ("--filter-class", test.get("type"))):
            require(flag not in command or fnmatchcase(identity, command[command.index(flag) + 1]),
                    "Runner result does not match requested selection")
        expected[name] = {"uid": names[name], "name": name, "outcome": outcome,
                          "type": test.get("type"), "method": test.get("method"),
                          "reason": test.findtext("reason/message") or test.findtext("reason") or ""}
    label = f"{assembly} ({tfm}|{module['architecture']})"
    require(output.endswith("\n") and "\x1b" not in output and "\ufffd" not in output,
            "Truncated or incompatible runner execution capture")
    lines = deque(output.splitlines())
    require(lines and lines.popleft() == "Running tests from " + label, "Missing runner module start")
    observed = set()
    while lines and lines[0].startswith(("passed ", "skipped ")):
        line = lines.popleft()
        match = re.fullmatch(r"passed (.+) \(" + DURATION + r"\)", line)
        name = match[1] if match else line.removeprefix("skipped ")
        require(name in expected and name not in observed, "Duplicate or unexpected runner completion")
        record = expected[name]
        require(record["outcome"] == ("Pass" if match else "Skip"), "Conflicting runner completion")
        if not match:
            require(record["reason"] and all(lines and lines.popleft() == "  " + part
                    for part in record["reason"].splitlines()), "Runner skip reason differs from XML")
        require(lines and lines.popleft() == "  from " + label, "Missing or mismatched completion module")
        observed.add(name)
    passed = sum(r["outcome"] == "Pass" for r in expected.values())
    skipped = len(expected) - passed
    require(observed == set(expected) and passed > 0, "Missing runner per-test completion")
    tail = "\n".join(lines) + "\n"
    report = str(mem.m.absolute_path(command[command.index("--results-directory") + 1]) /
                 command[command.index("--report-xunit-filename") + 1])
    artifacts = r"(?:  In process file artifacts produced:\n    - " + re.escape(report) + r"\n\n)?"
    pattern = (re.escape(label) + rf" passed \[\+{passed}/x0/\?{skipped}\] \({DURATION}\)\n\n" +
               artifacts + rf"Test run summary: Passed!\n  total: {len(expected)}\n  failed: 0\n"
               rf"  succeeded: {passed}\n  skipped: {skipped}\n  duration: {DURATION}\n")
    require(re.fullmatch(pattern, tail), "Missing, ambiguous or inconsistent runner completion summary")
    return {"module": str(assembly), "target_framework": tfm, "architecture": module["architecture"],
            "tests": sorted(expected.values(), key=lambda r: r["name"])}
