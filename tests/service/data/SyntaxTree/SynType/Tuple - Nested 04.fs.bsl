ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple - Nested 04.fs", false, QualifiedNameOfFile Module,
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
                                 [Type (FromParseError (3,12--3,12));
                                  Star (3,11--3,12);
                                  Type
                                    (LongIdent (SynLongIdent ([b2], [], [None])));
                                  Star (3,16--3,17);
                                  Type
                                    (LongIdent (SynLongIdent ([b3], [], [None])))],
                                 (3,11--3,20)), (3,9--3,21)))], (3,5--3,21)),
                    (3,1--3,21)), (3,0--3,1), Some (3,21--3,22), (3,0--3,22)),
              (3,0--3,22))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,22), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,11)-(3,12) parse error Expecting type
