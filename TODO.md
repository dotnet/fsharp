# TODO

- [x] Review prior tailcall/pinned-local changes from PR 18893 and existing TailCalls tests.
- [x] Add regression coverage for pinning a local byref and the `&&` operator to ensure correct runtime output.
- [x] Update IlxGen tailcall suppression to account for locals whose address is taken.
- [ ] Evaluate if additional tailcall scenarios need coverage beyond the new regressions.
- [ ] Run any broader validation beyond the targeted component tests if time allows.
