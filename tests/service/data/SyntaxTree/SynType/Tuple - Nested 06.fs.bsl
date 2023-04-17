ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple - Nested 06.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([a], [], [None])));
                        Star (3,7--3,8);
                        Type
                          (Paren
                             (Tuple
                                (false,
                                 [Type
                                    (LongIdent (SynLongIdent ([b1], [], [None])));
                                  Star (3,13--3,14);
                                  Type
                                    (LongIdent (SynLongIdent ([b2], [], [None])));
                                  Star (3,18--3,19);
                                  Type (FromParseError (3,19--3,19))],
                                 (3,10--3,19)), (3,9--3,21)))], (3,5--3,21)),
                    (3,1--3,21)), (3,0--3,1), Some (3,21--3,22), (3,0--3,22)),
              (3,0--3,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,20)-(3,21) parse error Unexpected symbol ')' in type
