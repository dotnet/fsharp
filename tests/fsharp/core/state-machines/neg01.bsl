
neg01.fsx(86,24,87,34): typecheck error FS3501: Invalid resumable code. Recursive bindings are not allowed. Please lift to a top-level recursive function definition

neg01.fsx(94,37,94,41): typecheck error FS3501: Invalid resumable code. Resumable code parameter must have name beginning with '__expand'

neg01.fsx(100,14,100,26): typecheck error FS3402: The construct '__resumeAt' may only be used in valid resumable code. For example, 'if __useResumableStateMachines then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(108,21,108,31): typecheck error FS3402: The construct '__resumeAt' may only be used in valid resumable code. For example, 'if __useResumableStateMachines then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(113,9,113,10): typecheck error FS3501: Invalid resumable code. Any method of function accepting or returning resumable code must be marked 'inline'

neg01.fsx(119,23,119,41): typecheck error FS3402: The construct '__resumableEntry' may only be used in valid resumable code. For example, 'if __useResumableStateMachines then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(130,35,130,49): typecheck error FS3516: All methods on a template struct type used with __structStateMachine must be marked 'inline'.

neg01.fsx(141,35,141,49): typecheck error FS3501: Invalid resumable code. A template struct type used with __structStateMachine must implement precisely one interface 'System.Runtime.CompilerServices.IAsyncStateMachine'. If necessary mark the struct type with [<NoEquality; NoComparison>].
