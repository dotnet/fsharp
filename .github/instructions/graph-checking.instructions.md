---
applyTo: 'src/Compiler/Driver/GraphChecking/**'
---

Parallel project dependency analysis and scheduling: Graph, DependencyResolution, TrieMapping, GraphProcessing.

- `processGraph` and `processGraphAsync` must only queue a node after all `ProcessedDepsCount` dependencies are complete.
- `mkGraph` must exclude implementation files that have backing signatures from the trie, because `TrieMapping.mkTrie` should only expose signature-backed symbols once.
- `collectGhostDependencies` should add at most one earlier file for an unused namespace open; keep the ghost edge minimal and before the current file index.
- Keep `Graph.transitive` and `Graph.reverse` consistent with the original DAG; `processGraph` assumes every node and dependency is present.
