---
applyTo: 'src/Compiler/Utilities/**'
---

Low-level shared algorithms and collections used across the compiler: LruCache, EditDistance, DependencyGraph.

- Preserve `LruCache` `requiredToKeep` exemption when demoting strong entries to weak ones.
- When promoting a weak hit in `TryGet`, reinsert the node at the strong-list head before returning so recency and retention stay consistent.
- Keep `CalculateEditDistance` using the restricted Damerau-Levenshtein path and the same `JaroWinklerDistance` fallback for suggestions.
- Keep weak-list trimming ordered from oldest to newest so collected entries are purged before eviction can drop a still-live node.
