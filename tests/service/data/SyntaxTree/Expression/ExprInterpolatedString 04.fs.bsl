ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ExprInterpolatedString 04.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_EqualsDollar], [], [Some (OriginalNotation "=$")]),
                       None, (3,6--3,8)),
                    InterpolatedString
                      ([String ("foo", (3,0--3,6))], Regular, (3,0--3,6)),
                    (3,0--3,8)),
                 Const (String ("bar", Regular, (3,8--3,13)), (3,8--3,13)),
                 (3,0--3,13)), (3,0--3,13));
           Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Equality], [], [Some (OriginalNotation "=")]),
                       None, (4,7--4,8)),
                    InterpolatedString
                      ([String ("foo", (4,0--4,6))], Regular, (4,0--4,6)),
                    (4,0--4,8)),
                 InterpolatedString
                   ([String ("bar", (4,9--4,15))], Regular, (4,9--4,15)),
                 (4,0--4,15)), (4,0--4,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,15), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
