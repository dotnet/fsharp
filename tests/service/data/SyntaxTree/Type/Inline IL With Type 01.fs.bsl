ImplFile
  (ParsedImplFileInput
     ("/root/Type/Inline IL With Type 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([unbox], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (x, None), false, None, (3,18--3,19)),
                              LongIdent (SynLongIdent ([obj], [], [None])),
                              (3,18--3,24)), (3,17--3,25))], None, (3,11--3,25)),
                  Some
                    (SynBindingReturnInfo
                       (Var (SynTypar (T, None, false), (3,28--3,30)),
                        (3,28--3,30), [], { ColonRange = Some (3,26--3,27) })),
                  Typed
                    (ArbitraryAfterError ("parenExpr2", (4,4--4,26)),
                     Var (SynTypar (T, None, false), (3,28--3,30)), (4,4--4,26)),
                  (3,11--3,25), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                             InlineKeyword = Some (3,4--3,10)
                                             EqualsRange = Some (3,31--3,32) })],
              (3,0--4,26), { InKeyword = None });
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PrefixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })], (4,27--4,31))), [],
                     [x],
                     PreXmlDoc ((4,22), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,32--4,33)),
                  Simple (None (4,32--4,33), (4,32--4,33)), [], None,
                  (4,32--4,33), { LeadingKeyword = Type (4,22--4,26)
                                  EqualsRange = None
                                  WithKeyword = None })], (4,22--4,33))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--4,33), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,62)] }, set []))

(4,22)-(4,26) parse error Incomplete structured construct at or before this point in binding. Expected # or other token.
(4,4)-(4,5) parse error Unmatched '('
(4,22)-(4,26) parse error Unexpected keyword 'type' in binding. Expected incomplete structured construct at or before this point or other token.
(3,0)-(3,3) parse error Incomplete value or function definition. If this is in an expression, the body of the expression must be indented to the same column as the 'let' keyword.
(4,34)-(4,35) parse error Unexpected symbol ':' in type definition. Expected '=' or other token.
