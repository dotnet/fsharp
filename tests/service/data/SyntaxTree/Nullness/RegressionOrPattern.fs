match exn with
| InternalError (s, _)
| Failure s as exn -> ()