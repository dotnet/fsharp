# Frustration-Driven Instructions

---

## 2026-06-11

NEVER claim a test failure is 'pre-existing', 'flaky', or an 'infra issue' without proof. Default assumption: MY BRANCH CAUSED IT. Required proof before dismissing a failure: (1) the SAME test name failing on at least 2 recent successful or other-PR builds based on the same main commit, AND (2) the same failure mode/diff. Without both pieces of evidence, treat the failure as your own regression and investigate the root cause.

---
