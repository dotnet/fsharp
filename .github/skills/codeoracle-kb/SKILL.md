---
name: codeoracle-kb
description: >-
  Drill into the CodeOracle knowledge base behind this repo's generated `.instructions.md` rules
  — grounded findings, evidence, file history and graph neighbors. Use when a rule's rationale or
  evidence is unclear, when asked "why is this the case / where is this enforced", when you need a
  file or symbol's history, or when you want related code, concepts or findings for a path.
---

# codeoracle-kb

The generated `.instructions.md` files carry self-contained rules. This skill is the optional
drill-down layer: it lets you fetch the findings, evidence, history and related code behind those
rules from the knowledge graph. The instruction files themselves stay clean — no headings that
restate `applyTo`, no IDs, no evidence comments, no per-file footer — so all provenance is
retrieved on demand here.

## Drill-down protocol

Every `.instructions.md` file is a graph node identified by its `applyTo` glob (the value in its
frontmatter). To see what backs a rule, call `get_instruction_provenance` with
`instruction:<applyTo>`. Pass the optional 1-based `ordinal` to scope the answer to a single rule
(the Nth bullet in the file); omit it for the whole file. This mapping is uniform across every
instruction, which is why the files carry no footer of their own.

## Capabilities

- `get_instruction_provenance` — the findings, evidence and rationale behind a rule (by
  `instruction:<applyTo>`, optional `ordinal`).
- `search_knowledge_base` — find grounded findings by keyword.
- `get_history` — how a file or entity changed over time.
- `graph_neighbors` — related code, concepts and findings for a ref.
- `list_concepts` / `get_concept` — browse the concept map.

## Boundaries

- The knowledge base is **read-only**. This skill never writes, mutates or deletes anything.
- There is **no arbitrary SQL**: only the curated, safe query tools above are available. Raw
  database access is intentionally not exposed.
- Every answer is grounded in stored findings and their citations; do not invent evidence.

## Graceful degradation

If the CodeOracle backend is **not connected** or the knowledge base is unavailable, this skill
should **degrade** gracefully: say so plainly and fall back to the self-contained Tier-0 rules in
the `.instructions.md` files, which remain valid on their own.
