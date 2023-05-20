---
title: IncrementalBuilder caches
category: Language Service Internals
categoryindex: 300
index: 1300
---
# IncrementalBuilder SyntaxTree cache

Incremental builder keeps in a cache at most one `ParsedInput` for each file it parses.
This behavior can be toggled with `useSyntaxTreeCache` parameter.

Memory impact of this feature can be in range of tens of MB for larger solutions. This can be inspected in memory profilng tools by searching for `ParsedInput` instances.
When partial checking is enabled, implementation files backed by signature will not be parsed or cached, as expected.
