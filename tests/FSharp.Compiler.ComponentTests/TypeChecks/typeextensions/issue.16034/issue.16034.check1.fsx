#load "issue.16034.fsx"
open Issue.``16034``
open Issue.``16034``.Extensions
t.indexed1(aa1="nok") <- 1 // error FS0073: internal error: The input must be non-negative. (Parameter 'n') (ArgumentException)