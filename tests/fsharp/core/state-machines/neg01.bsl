
neg01.fsx(88,24,89,34): typecheck error FS3501: Invalid resumable code. Recursive bindings are not allowed. Please lift to a top-level recursive function definition

neg01.fsx(96,37,96,41): typecheck error FS3501: Invalid resumable code. Resumable code parameter must have name beginning with '__expand'

neg01.fsx(102,14,102,26): typecheck error FS3402: The construct '__resumeAt' may only be used in valid resumable code. For example, 'if __useResumableCode then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(110,21,110,31): typecheck error FS3402: The construct '__resumeAt' may only be used in valid resumable code. For example, 'if __useResumableCode then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(115,9,115,10): typecheck error FS3501: Invalid resumable code. Any method of function accepting or returning resumable code must be marked 'inline'

neg01.fsx(121,23,121,41): typecheck error FS3402: The construct '__resumableEntry' may only be used in valid resumable code. For example, 'if __useResumableCode then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(132,35,132,49): typecheck error FS3516: All methods on a template struct type used with __structStateMachine must be marked 'inline'.

neg01.fsx(143,35,143,49): typecheck error FS3501: Invalid resumable code. A template struct type used with __structStateMachine must implement precisely one interface 'System.Runtime.CompilerServices.IAsyncStateMachine'. If necessary mark the struct type with [<NoEquality; NoComparison>].

neg01.fsx(160,35,160,51): typecheck error FS3402: The construct '__resumableEntry' may only be used in valid resumable code. For example, 'if __useResumableCode then ...' and the `[<ResumableCode>]' attribute must be used.

neg01.fsx(163,33,163,43): typecheck error FS3402: The construct '__resumeAt' may only be used in valid resumable code. For example, 'if __useResumableCode then ...' and the `[<ResumableCode>]' attribute must be used.
