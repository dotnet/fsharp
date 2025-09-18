ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 09.fs", false, QualifiedNameOfFile Module, [],
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
                                (Named
                                   (SynIdent (a, None), false, None, (4,4--4,5)),
                                 (4,3--4,5)), (4,3--4,5))], None, (4,2--4,5)),
                     None, Ident a, (4,2--4,18), Yes,
                     { ArrowRange = Some (4,14--4,16)
                       BarRange = Some (4,0--4,1) })], (3,0--4,18),
                 { MatchKeyword = (3,0--3,5)
                   WithKeyword = (3,8--3,12) }), (3,0--4,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,5)-(4,6) parse error Unexpected symbol ';' in pattern. Expected ')' or other token.
(4,3)-(4,4) parse error Unmatched '('
