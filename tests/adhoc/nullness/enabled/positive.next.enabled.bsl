
tests\adhoc\nullness\enabled\positive.fs(13,18): warning FS3261: Nullness warning: The type ''a option' uses 'null' as a representation value but a non-null type is expected.

tests\adhoc\nullness\enabled\positive.fs(19,26): warning FS3261: Nullness warning: The type 'int option' uses 'null' as a representation value but a non-null type is expected.

tests\adhoc\nullness\enabled\positive.fs(51,15): error FS0043: A type parameter is missing a constraint 'when 'T: __notnull'

tests\adhoc\nullness\enabled\positive.fs(51,33): error FS0043: A type parameter is missing a constraint 'when 'T: __notnull'

tests\adhoc\nullness\enabled\positive.fs(208,17): error FS0001: The type '(int * int)' does not have 'null' as a proper value

tests\adhoc\nullness\enabled\positive.fs(214,17): error FS0001: The type 'int list' does not have 'null' as a proper value

tests\adhoc\nullness\enabled\positive.fs(220,17): error FS0001: The type 'int list __withnull' does not have 'null' as a proper value

tests\adhoc\nullness\enabled\positive.fs(222,17): error FS0001: The type 'String list __withnull' does not have 'null' as a proper value

tests\adhoc\nullness\enabled\positive.fs(224,17): error FS0001: The type 'String list' does not have 'null' as a proper value
