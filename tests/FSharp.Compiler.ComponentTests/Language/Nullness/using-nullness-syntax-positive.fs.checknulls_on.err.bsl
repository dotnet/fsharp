using-nullness-syntax-positive.fs (11,18)-(11,22) typecheck error Nullness warning: The type 'obj' does not support 'null'.
using-nullness-syntax-positive.fs (12,18)-(12,37) typecheck error Nullness warning: The type 'String | null' supports 'null' but a non-null type is expected.
using-nullness-syntax-positive.fs (13,18)-(13,24) typecheck error Nullness warning: The type ''a option' uses 'null' as a representation value but a non-null type is expected.
using-nullness-syntax-positive.fs (17,15)-(17,19) typecheck error Nullness warning: The type 'obj' does not support 'null'.
using-nullness-syntax-positive.fs (18,39)-(18,41) typecheck error Nullness warning: The type 'String | null' supports 'null' but a non-null type is expected.
using-nullness-syntax-positive.fs (19,26)-(19,28) typecheck error Nullness warning: The type 'int option' uses 'null' as a representation value but a non-null type is expected.
using-nullness-syntax-positive.fs (27,14)-(27,17) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (27,14)-(27,17) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (28,14)-(28,19) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (28,14)-(28,19) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (29,14)-(29,17) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (29,14)-(29,17) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (43,26)-(43,30) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (83,72)-(83,100) typecheck error Nullness warning: The types 'string' and 'String | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (84,95)-(84,123) typecheck error Nullness warning: The types 'string' and 'String | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (85,63)-(85,70) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (85,63)-(85,70) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (86,81)-(86,90) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (86,81)-(86,90) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (86,92)-(86,102) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (86,92)-(86,102) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (90,63)-(90,91) typecheck error Nullness warning: The types 'string' and 'String | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (91,53)-(91,60) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (91,53)-(91,60) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (92,72)-(92,81) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (92,72)-(92,81) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (92,83)-(92,93) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (92,83)-(92,93) typecheck error Nullness warning: The types 'C' and 'C | null' do not have compatible nullability.
using-nullness-syntax-positive.fs (120,32)-(120,36) typecheck error Nullness warning: The type 'obj array' does not support 'null'.
using-nullness-syntax-positive.fs (129,4)-(129,34) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (134,5)-(134,44) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (159,36)-(159,40) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (162,41)-(162,45) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (164,37)-(164,41) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (183,14)-(183,16) typecheck error Nullness warning: The type 'string' does not support 'null'.
using-nullness-syntax-positive.fs (189,17)-(189,26) typecheck error Nullness warning: The type 'String' does not support 'null'.
using-nullness-syntax-positive.fs (217,14)-(217,16) typecheck error The struct, record or union type 'C7' is not structurally comparable because the type '(int -> int) | null' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'C7' to clarify that the type is not comparable
using-nullness-syntax-positive.fs (217,14)-(217,16) typecheck error The struct, record or union type 'C7' does not support structural equality because the type '(int -> int) | null' does not satisfy the 'equality' constraint. Consider adding the 'NoEquality' attribute to the type 'C7' to clarify that the type does not support structural equality