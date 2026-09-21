const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { mock } = require("node:test");

// A synthetic Actions host, separate from the production offline-injection path.
async function main() {
    assert.equal(process.env.GITHUB_ACTIONS, "");
    assert.equal(process.env.RB_LOCAL_TEST, "explicit-offline-fixture");
    const file = process.env.RB_LOCAL_TEST_FILE;
    const db = JSON.parse(await fs.readFile(file, "utf8"));
    const service = await require("./controller-service.cjs").connect(file);
    const root = path.resolve(__dirname, "..", "..", "..");
    const controller = require("../controller-cli.cjs"), bytes = controller.bytes;
    controller.bytes = async (file, limit) => {
        const relative = path.relative(root, file).split(path.sep).join("/");
        if (relative === ".github/regression-bisect/deployment.json" ||
            relative === ".github/workflows/regression-bisect-discovery.yml")
            return Buffer.from(db.files[relative], "base64");
        return bytes(file, limit);
    };
    require("node:child_process").execFileSync = (_, args) => {
        if (args.join(" ") === "rev-parse HEAD") return db.revision;
        assert.deepEqual(args, ["status", "--porcelain", "--untracked-files=all"]);
        return "";
    };
    global.fetch = service.send;
    mock.timers.enable({ apis: ["Date"], now: Date.parse(db.now) });
    return require("../routing-cli.cjs").main([], { ...process.env, GITHUB_ACTIONS: "true",
        RB_LOCAL_TEST: "", RB_LOCAL_TEST_FILE: "" });
}

main().then(result => console.log(JSON.stringify(result))).catch(error => {
    console.log(JSON.stringify({ status: "blocked", reason: error.message }));
    process.exitCode = 1;
});
