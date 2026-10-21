// Host-only: the runtime token is never passed to the optimizer or its mounts.
const fs = require("node:fs");
const path = require("node:path");
const receipt = process.stdout.write.bind(process.stdout);
process.stdout.write = process.stderr.write.bind(process.stderr);
const { DefaultArtifactClient } = require(process.env.COMPILER_PERF_ARTIFACT_MODULE);

async function main() {
  const input = JSON.parse(fs.readFileSync(0, "utf8"));
  const client = new DefaultArtifactClient();
  const listed = await client.listArtifacts();
  if (!Array.isArray(listed.artifacts) || listed.artifacts.length > 256) {
    throw new Error("Artifact listing is malformed or exceeds bounds");
  }
  const matching = listed.artifacts.filter(a => a.name === input.name);
  if (matching.length > 1) throw new Error("Artifact identity is ambiguous");
  let id = matching[0]?.id;
  if (!id) {
    const roots = new Set(input.files.map(f => path.dirname(f.path)));
    if (roots.size !== 1) throw new Error("Artifact inventory must have one checked root");
    const result = await client.uploadArtifact(input.name, input.files.map(f => f.path),
      [...roots][0], { retentionDays: 14, compressionLevel: 0 });
    id = result.id;
  }
  if (!Number.isSafeInteger(id) || id <= 0) throw new Error("Artifact ID unavailable");
  await client.downloadArtifact(id, { path: input.destination });
  receipt(JSON.stringify({ id }));
}
main().catch(() => { console.error("Actions artifact transport failed"); process.exitCode = 2; });
