"""A toy toolchain for real sandbox/controller tests; NOT an F# compiler."""

from pathlib import Path
import shutil
import sys

root = Path.cwd()
args = sys.argv[1:]
artifacts = root / "artifacts"
installation = artifacts / "bin" / "fsc" / "Release" / "net11.0"
failure = (artifacts / "fail").read_text() if (artifacts / "fail").exists() else ""
category = args[0]
if category == "--version":
    print("11.0.100-synthetic")
elif category == "--list-runtimes":
    print("synthetic runtime")
elif category == "--list-sdks":
    print("9.0.300 [synthetic]\n11.0.100-synthetic [synthetic]")
elif category == "pack":
    installation.mkdir(parents=True)
    for name, content in (("fsc.dll", "synthetic compiler"), ("FSharp.Compiler.Service.dll", "candidate"),
                          ("fsc.runtimeconfig.json", "runtime"), ("fsc.deps.json", "{}"), ("FSharp.Core.dll", "core")):
        (installation / name).write_text(content)
elif category == "build":
    target = artifacts / "bin" / Path(args[1]).stem / "Release" / "net11.0"
    target.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(installation / "FSharp.Compiler.Service.dll", target / "FSharp.Compiler.Service.dll")
elif "--help" in args:
    print("--filter-method --filter-class --report-xunit --report-xunit-filename")
else:
    source = root / "src" / "Compiler" / "Checking" / "Test.fs"
    assert "let answer = 42" in source.read_text(), "Toy algorithmic RED guard"
    assert failure != category, "Deliberate required-check failure"
    if category == "test":
        path = Path(args[args.index("--results-directory") + 1]) / args[args.index("--report-xunit-filename") + 1]
        source = root / "tests" / "FSharp.Compiler.ComponentTests" / "Conformance" / "Regression.fs"
        path.write_text('<assemblies><assembly total="1" passed="1" failed="0" skipped="0" errors="0">'
                        f'<collection><test name="regression" method="regression" source-file="{source}" result="Pass" />'
                        '</collection></assembly></assemblies>')
    elif category == "fsi":
        print("Results: 6/6 tests passed\nAll tests PASSED")
    else:
        raise ValueError("Unsupported fixture command")
