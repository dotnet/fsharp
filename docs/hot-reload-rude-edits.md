# F# hot reload rude-edit diagnostics

F# hot reload reports an `FSHRDL` diagnostic when an edit cannot be applied safely to the running process. The application is rebuilt and restarted instead of applying a delta that could leave it in an invalid state.

The diagnostic message identifies the affected declaration and the reason for the restart. These codes are owned by the F# compiler and are separate from Roslyn's `ENC` diagnostic namespace.

| Code | Meaning | What to do |
| --- | --- | --- |
| `FSHRDL001` | A member signature changed. | Undo the signature change to apply in place, or allow the rebuild and restart. |
| `FSHRDL002` | An `inline` annotation changed. | Allow the rebuild and restart. |
| `FSHRDL003` | A type representation or layout changed. | Allow the rebuild and restart. |
| `FSHRDL004` | A declaration was added in a shape the runtime cannot add. | Allow the rebuild and restart. |
| `FSHRDL005` | A declaration was removed. | Allow the rebuild and restart. |
| `FSHRDL006` | A virtual, abstract, or override member was added. | Allow the rebuild and restart. |
| `FSHRDL007` | A constructor was added. | Allow the rebuild and restart. |
| `FSHRDL008` | A user-defined operator was added. | Allow the rebuild and restart. |
| `FSHRDL009` | An explicit interface implementation was added. | Allow the rebuild and restart. |
| `FSHRDL010` | A member was added to an interface. | Allow the rebuild and restart. |
| `FSHRDL011` | A field was added in a shape the runtime cannot add. | Allow the rebuild and restart. |
| `FSHRDL012` | A lambda's lowered shape changed incompatibly. | Allow the rebuild and restart. |
| `FSHRDL013` | A state machine's resumable or hoisted layout changed incompatibly. | Keep the existing resume-point and captured-value layout, or allow the rebuild and restart. |
| `FSHRDL014` | A query expression's lowered shape changed incompatibly. | Allow the rebuild and restart. |
| `FSHRDL015` | A synthesized compiler declaration changed incompatibly. | Allow the rebuild and restart. |
| `FSHRDL016` | The runtime did not advertise a capability required by the edit. | Update the runtime if a newer version supports the capability, or allow the rebuild and restart. |
| `FSHRDL099` | The edit is unsupported for another fail-closed reason. | Follow the detailed message and allow the rebuild and restart. |

These diagnostics are intentionally fail closed. If the compiler cannot prove that an edit is safe, it requests a restart and leaves the running application unchanged.
