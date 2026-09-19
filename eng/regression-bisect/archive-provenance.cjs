const path = require("node:path");
const { requireValue, publicUrl, validateHttpResponse } = require("./toolchain-io.cjs");

function validateArchiveRequest({ policy, kind, sourceSha, product, version, platform, archive }) {
    const { validatePolicy, fingerprintInput } = require("./model.cjs");
    validatePolicy(policy);
    publicUrl(archive.url, policy);
    requireValue(archive.rid === platform.rid && archive.architecture === platform.architecture,
        "archive RID/architecture does not match request");
    const provenance = archive.provenance;
    if (provenance?.kind === "publisher-checksum") {
        requireValue(Object.keys(provenance).join() === "kind" && /^[a-f0-9]{128}$/.test(archive.sha512),
            "invalid publisher archive checksum provenance");
        return null;
    }
    requireValue(provenance?.kind === "approved-official-https-measured" &&
        Object.keys(provenance).sort().join() === "approvalDigest,approvalRevision,kind" &&
        !Object.hasOwn(archive, "sha512"), "invalid measured archive provenance");
    const approval = policy.toolchains.measuredArchives.find(a => a.url === archive.url &&
        a.product === product && a.version === version && a.rid === platform.rid && a.sourceSha === sourceSha);
    requireValue(kind === "source" && approval && version.includes("-") &&
        new URL(archive.url).origin === approval.origin && new URL(archive.url).href === archive.url &&
        provenance.approvalRevision === approval.approvalRevision &&
        provenance.approvalDigest === fingerprintInput(approval), "historical archive lacks exact policy approval");
    return approval;
}

function validateArchiveReceipt(options, receipt, filePath = path) {
    const { fingerprintInput } = require("./model.cjs");
    const approval = validateArchiveRequest(options);
    const { archive, policy } = options;
    requireValue(receipt.url === archive.url && receipt.name === archive.name &&
        receipt.rid === archive.rid && receipt.architecture === archive.architecture &&
        fingerprintInput(receipt.provenance) === fingerprintInput(archive.provenance) &&
        receipt.sha512 === archive.sha512 && /^[a-f0-9]{128}$/.test(receipt.actualSha512) &&
        /^[a-f0-9]{64}$/.test(receipt.sha256) &&
        (approval || receipt.actualSha512 === archive.sha512), "installer archive checksum or provenance mismatch");
    validateHttpResponse(archive.url, { url: receipt.redirectUrl, redirects: receipt.redirects }, policy,
        approval ? { allowedUrls: [approval.url, ...approval.redirectUrls], maxBytes: approval.maxBytes, timeoutMs: approval.timeoutMs } : undefined);
    requireValue(receipt.finalOrigin === new URL(receipt.redirectUrl).origin, "archive final origin mismatch");
    requireValue(Number.isSafeInteger(receipt.bytesReceived) && receipt.bytesReceived > 0 &&
        receipt.bytesReceived <= (approval?.maxBytes ?? 1073741824) &&
        typeof receipt.receivedAt === "string" && Number.isFinite(Date.parse(receipt.receivedAt)) &&
        new Date(receipt.receivedAt).toISOString() === receipt.receivedAt, "invalid archive receipt bounds or time");
    const retained = receipt.retained;
    requireValue(retained && typeof retained.path === "string" && !/[\r\n\0]/.test(retained.path) &&
        filePath.isAbsolute(retained.path) && (filePath !== path.win32 || filePath.parse(retained.path).root.length > 1) &&
        retained.size === receipt.bytesReceived &&
        retained.sha256 === receipt.sha256 && retained.sha512 === receipt.actualSha512,
    "retained archive identity mismatch");
    return true;
}

module.exports = { validateArchiveRequest, validateArchiveReceipt };
