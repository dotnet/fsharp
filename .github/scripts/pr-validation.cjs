const repository = { owner: 'dotnet', repo: 'fsharp' };

async function prepare({ github, context }) {
    const { comment, issue } = context.payload;
    if (context.repo.owner !== repository.owner || context.repo.repo !== repository.repo ||
        !issue.pull_request || comment.user.type !== 'User') {
        throw new Error('Only user comments on dotnet/fsharp PRs can request validation.');
    }
    if (!/^\/(?:dart|pr-val)$/.test(comment.body.trim())) {
        throw new Error('Use a standalone /dart or /pr-val, without a SHA argument.');
    }

    // Authorize the original commenter, not the actor who might rerun the workflow.
    const { data: permission } = await github.rest.repos.getCollaboratorPermissionLevel({
        ...repository, username: comment.user.login
    });
    if (!['write', 'maintain', 'admin'].includes(permission.permission)) {
        throw new Error('The requester needs repository write access.');
    }
    const prNumber = issue.number;
    if (!Number.isSafeInteger(prNumber) || prNumber <= 0) {
        throw new Error('Invalid PR number.');
    }
    const { data: pr } = await github.rest.pulls.get({ ...repository, pull_number: prNumber });
    if (pr.state !== 'open' || pr.base.repo.full_name !== 'dotnet/fsharp' || pr.base.ref !== 'main') {
        throw new Error('Only open PRs targeting dotnet/fsharp main are supported.');
    }
    const headSha = pr.head.sha;
    const baseSha = pr.base.sha;
    if (![headSha, baseSha].every(sha => typeof sha === 'string' && /^[0-9a-f]{40}$/.test(sha))) {
        throw new Error('GitHub did not return full head/base commit SHAs.');
    }
    return {
        resources: { repositories: { self: { refName: 'refs/heads/main', version: baseSha } } },
        templateParameters: { prNumber: String(prNumber), headSha, baseSha }
    };
}

async function verifyMembership({ github, context }) {
    // This client uses a Microsoft-org App token, not dotnet/fsharp's GITHUB_TOKEN.
    try {
        const response = await github.rest.orgs.checkMembershipForUser({
            org: 'microsoft', username: context.payload.comment.user.login
        });
        if (response.status !== 204) {
            throw new Error(`Unexpected Microsoft membership response: ${response.status}`);
        }
    } catch (error) {
        if (error.status === 404) {
            throw new Error('Microsoft-org membership could not be confirmed for the requester.');
        }
        throw error;
    }
}

async function queue({ pipelineId, token, request, send = fetch }) {
    if (!/^[1-9][0-9]*$/.test(pipelineId || '')) {
        throw new Error('Configure FSHARP_APEX_PIPELINE_ID in the fsharp_pr_validation environment.');
    }
    if (!token || !token.trim()) {
        throw new Error('Azure did not return an access token.');
    }
    const response = await send(
        `https://dev.azure.com/devdiv/DevDiv/_apis/pipelines/${pipelineId}/runs?api-version=7.1`,
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
            body: JSON.stringify(request),
            redirect: 'error'
        }
    );
    if (!response.ok) {
        throw new Error(`Azure DevOps rejected the queue request (HTTP ${response.status}).`);
    }
    const run = await response.json();
    if (!Number.isSafeInteger(run?.id) || run.id <= 0) {
        throw new Error('Azure DevOps did not return a valid run ID.');
    }
    return `https://dev.azure.com/devdiv/DevDiv/_build/results?buildId=${run.id}`;
}

module.exports = { prepare, verifyMembership, queue };
