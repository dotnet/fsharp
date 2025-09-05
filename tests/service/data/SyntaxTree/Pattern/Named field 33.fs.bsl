ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 33.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (Const (Unit, (4,2--4,4)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        Pats
                          [FromParseError
                             (Paren
                                (Tuple
                                   (false,
                                    [Named
                                       (SynIdent (a, None), false, None,
                                        (6,4--6,5));
                                     Named
                                       (SynIdent (b, None), false, None,
                                        (6,7--6,8))], [(6,5--6,6)], (6,4--6,8)),
                                 (6,3--6,8)), (6,3--6,8))], None, (6,2--6,8)),
                     None, Ident a, (6,2--6,18), Yes,
                     { ArrowRange = Some (6,14--6,16)
                       BarRange = Some (6,0--6,1) })], (3,0--6,18),
                 Yes (3,0--3,3), Yes (5,0--5,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--5,4)
                   WithKeyword = (5,0--5,4)
                   WithToEndRange = (5,0--6,18) }), (3,0--6,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,9)-(6,10) parse error Unexpected symbol '=' in pattern. Expected ')' or other token.
(6,3)-(6,4) parse error Unmatched '('
