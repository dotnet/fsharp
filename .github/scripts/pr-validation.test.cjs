const assert = require('node:assert/strict');
const { test } = require('node:test');
const { prepare, verifyMembership, queue } = require('./pr-validation.cjs');

function fixture() {
    const calls = [];
    const context = {
        repo: { owner: 'dotnet', repo: 'fsharp' },
        actor: 'rerunning-admin',
        payload: {
            comment: { body: '/dart', user: { login: 'maintainer', type: 'User' } },
            issue: { number: 42, pull_request: {} }
        }
    };
    const pr = {
        state: 'open',
        head: { sha: 'a'.repeat(40), repo: { full_name: 'contributor/fsharp' } },
        base: { sha: 'b'.repeat(40), ref: 'main', repo: { full_name: 'dotnet/fsharp' } }
    };
    const permission = { permission: 'write' };
    const github = { rest: {
        repos: { getCollaboratorPermissionLevel: async args => {
            calls.push(args);
            return { data: permission };
        } },
        pulls: { get: async args => {
            calls.push(args);
            return { data: pr };
        } },
        orgs: { checkMembershipForUser: async args => { calls.push(args); return { status: 204 }; } }
    } };
    return { github, context, pr, permission, calls };
}

for (const command of ['/dart', '/pr-val', ' \n/pr-val \n']) {
    for (const authorRepo of ['dotnet/fsharp', 'contributor/fsharp']) {
        test(`${command.trim()} captures ${authorRepo} snapshot and pins trusted YAML`, async () => {
            const f = fixture();
            f.context.payload.comment.body = command;
            f.pr.head.repo.full_name = authorRepo;
            const result = await prepare(f);
            assert.deepEqual(result, {
                resources: { repositories: { self: {
                    refName: 'refs/heads/main', version: f.pr.base.sha
                } } },
                templateParameters: { prNumber: '42', headSha: f.pr.head.sha, baseSha: f.pr.base.sha }
            });
            assert.equal(f.calls[0].username, 'maintainer');
            assert.equal(f.calls[1].pull_number, 42);
        });
    }
}

for (const command of ['/dart abc1234', 'please /dart', '`/dart`', '/dart\n/pr-val', '/dart-extra']) {
    test(`rejects command ${JSON.stringify(command)} before API access`, async () => {
        const f = fixture();
        f.context.payload.comment.body = command;
        await assert.rejects(prepare(f), /standalone/);
        assert.equal(f.calls.length, 0);
    });
}

for (const [name, mutate, error] of [
    ['read-only requester', f => { f.permission.permission = 'read'; }, /write access/],
    ['bot', f => { f.context.payload.comment.user.type = 'Bot'; }, /user comments/],
    ['issue', f => { delete f.context.payload.issue.pull_request; }, /user comments/],
    ['wrong repository', f => { f.context.repo.owner = 'other'; }, /dotnet\/fsharp/],
    ['closed PR', f => { f.pr.state = 'closed'; }, /open PRs/],
    ['non-main PR', f => { f.pr.base.ref = 'release/test'; }, /main/],
    ['foreign target', f => { f.pr.base.repo.full_name = 'other/fsharp'; }, /main/],
    ['short SHA', f => { f.pr.head.sha = 'abcdef0'; }, /full head\/base/],
    ['missing SHA', f => { delete f.pr.base.sha; }, /full head\/base/],
    ['injected PR number', f => { f.context.payload.issue.number = '1; command'; }, /PR number/]
]) {
    test(`rejects ${name}`, async () => {
        const f = fixture();
        mutate(f);
        await assert.rejects(prepare(f), error);
    });
}

for (const permission of ['write', 'maintain', 'admin']) {
    test(`accepts ${permission} permission`, async () => {
        const f = fixture();
        f.permission.permission = permission;
        await prepare(f);
    });
}

test('membership check uses original commenter, not rerun actor', async () => {
    const f = fixture();
    await verifyMembership(f);
    assert.deepEqual(f.calls, [{ org: 'microsoft', username: 'maintainer' }]);
});

for (const status of [401, 403, 404, 429, 500]) {
    test(`membership status ${status} fails closed`, async () => {
        const f = fixture();
        const failure = Object.assign(new Error('lookup failed'), { status });
        f.github.rest.orgs.checkMembershipForUser = async () => { throw failure; };
        await assert.rejects(verifyMembership(f), status === 404 ? /could not be confirmed/ : failure);
    });
}

for (const endpoint of ['permission', 'pull']) {
    test(`${endpoint} API failure does not produce a queue request`, async () => {
        const f = fixture();
        const fail = async () => { throw new Error('API unavailable'); };
        if (endpoint === 'permission') f.github.rest.repos.getCollaboratorPermissionLevel = fail;
        else f.github.rest.pulls.get = fail;
        await assert.rejects(prepare(f), /API unavailable/);
    });
}

test('unexpected membership response fails closed', async () => {
    const f = fixture();
    f.github.rest.orgs.checkMembershipForUser = async () => ({ status: 200 });
    await assert.rejects(verifyMembership(f), /Unexpected Microsoft membership response/);
});

test('queues the captured snapshot using the Azure bearer token', async () => {
    const request = await prepare(fixture());
    const url = await queue({
        pipelineId: '123', token: 'test-token', request,
        send: async (url, options) => {
            assert.equal(url, 'https://dev.azure.com/devdiv/DevDiv/_apis/pipelines/123/runs?api-version=7.1');
            assert.equal(options.method, 'POST');
            assert.equal(options.headers.Authorization, 'Bearer test-token');
            assert.equal(options.headers['Content-Type'], 'application/json');
            assert.deepEqual(JSON.parse(options.body), request);
            assert.equal(options.redirect, 'error');
            return { ok: true, json: async () => ({ id: 456 }) };
        }
    });
    assert.equal(url, 'https://dev.azure.com/devdiv/DevDiv/_build/results?buildId=456');
});

for (const [pipelineId, token] of [['', 'token'], ['0', 'token'], ['123/path', 'token'], ['123', '']]) {
    test(`invalid queue configuration ${pipelineId}/${token} makes no request`, async () => {
        let called = false;
        await assert.rejects(queue({
            pipelineId, token, request: {},
            send: async () => { called = true; }
        }), /Configure FSHARP_APEX_PIPELINE_ID|access token/);
        assert.equal(called, false);
    });
}

for (const status of [401, 403, 429, 500]) {
    test(`queue HTTP ${status} is not reported as success`, async () => {
        await assert.rejects(queue({
            pipelineId: '123', token: 'token', request: {},
            send: async () => ({ ok: false, status })
        }), new RegExp(`HTTP ${status}`));
    });
}

for (const id of [undefined, null, 0, -1, 1.5, '456']) {
    test(`rejects invalid queue response ID ${id}`, async () => {
        await assert.rejects(queue({
            pipelineId: '123', token: 'token', request: {},
            send: async () => ({ ok: true, json: async () => ({ id }) })
        }), /valid run ID/);
    });
}

for (const failure of ['network', 'invalid JSON']) {
    test(`queue ${failure} error is propagated`, async () => {
        const fail = async () => { throw new Error(failure); };
        await assert.rejects(queue({
            pipelineId: '123', token: 'token', request: {},
            send: failure === 'network' ? fail : async () => ({ ok: true, json: fail })
        }), new RegExp(failure));
    });
}
