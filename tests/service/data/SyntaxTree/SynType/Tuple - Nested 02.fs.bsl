ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple - Nested 02.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type
                          (Paren
                             (Tuple
                                (false,
                                 [Type
                                    (LongIdent (SynLongIdent ([a1], [], [None])));
                                  Star (3,9--3,10);
                                  Type
                                    (LongIdent (SynLongIdent ([a2], [], [None])))],
                                 (3,6--3,13)), (3,5--3,14))); Star (3,15--3,16);
                        Type (LongIdent (SynLongIdent ([b], [], [None])))],
                       (3,5--3,18)), (3,1--3,18)), (3,0--3,1), Some (3,18--3,19),
                 (3,0--3,19)), (3,0--3,19))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,19), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
