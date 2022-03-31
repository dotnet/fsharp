neg_invalid_constructor.fs (3,29)-(3,56) typecheck error A unique overload for method 'ImmutableStack`1' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a list

Candidates:
 - new: col: 'b -> ImmutableStack<'a>
 - private new: items: 'a list -> ImmutableStack<'a>
neg_invalid_constructor.fs (4,93)-(4,111) typecheck error A unique overload for method 'ImmutableStack`1' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a list

Candidates:
 - new: col: 'b -> ImmutableStack<'a>
 - private new: items: 'a list -> ImmutableStack<'a>
neg_invalid_constructor.fs (7,30)-(7,60) typecheck error A unique overload for method 'ImmutableStack`1' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'a list

Candidates:
 - new: col: 'b -> ImmutableStack<'a> when 'b :> seq<'c>
 - private new: items: 'a list -> ImmutableStack<'a>
neg_invalid_constructor.fs (7,30)-(7,60) typecheck error This is not a valid object construction expression. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor.