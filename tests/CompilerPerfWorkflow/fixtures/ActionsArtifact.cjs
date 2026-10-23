const fs = require("node:fs");
const path = require("node:path");
const receipt = process.env.COMPILER_PERF_FIXTURE_RECEIPT || path.join(__dirname, "effects.json");
let state = fs.existsSync(receipt) ? JSON.parse(fs.readFileSync(receipt)) : { uploads: 0 };

class DefaultArtifactClient {
  async listArtifacts() {
    process.stdout.write("Toolkit progress bypasses console.log\n");
    return { artifacts: state.uploads ? [{ id: 42, name: state.name }] : [] };
  }
  async uploadArtifact(name, files, root, options) {
    if (options.retentionDays !== 14 || options.compressionLevel !== 0 ||
        !files.every(f => path.dirname(f) === root)) throw new Error("Invalid upload contract");
    state = { name, files, uploads: state.uploads + 1 };
    fs.writeFileSync(receipt, JSON.stringify(state));
    return { id: 42 };
  }
  async downloadArtifact(id, { path: destination }) {
    if (id !== 42) throw new Error("Wrong artifact");
    if (fs.existsSync(receipt + ".lose-ack")) {
      fs.unlinkSync(receipt + ".lose-ack");
      throw new Error("Upload succeeded, acknowledgment lost");
    }
    for (const file of state.files) fs.copyFileSync(file, path.join(destination, path.basename(file)));
  }
}
module.exports = { DefaultArtifactClient };
