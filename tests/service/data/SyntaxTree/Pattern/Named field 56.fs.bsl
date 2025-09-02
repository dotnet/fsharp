ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 56.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        Pats
                          [FromParseError
                             (Paren
                                (FromParseError (Wild (4,4--4,4), (4,4--4,4)),
                                 (4,3--4,4)), (4,3--4,4))], None, (4,2--4,4)),
                     None, Const (Unit, (4,66--4,68)), (4,2--4,68), Yes,
                     { ArrowRange = Some (4,63--4,65)
                       BarRange = Some (4,0--4,1) })], (3,0--4,68),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,68))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,68), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,44)-(4,45) parse error Unexpected symbol '=' in pattern. Expected ')' or other token.
(4,3)-(4,4) parse error Unmatched '('
