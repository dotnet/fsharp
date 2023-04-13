ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Div 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([a], [], [None])));
                        Slash (3,7--3,8);
                        Type (LongIdent (SynLongIdent ([b], [], [None])));
                        Slash (3,11--3,12); Type (FromParseError (3,12--3,12))],
                       (3,5--3,12)), (3,1--3,12)), (3,0--3,1), Some (3,13--3,14),
                 (3,0--3,14)), (3,0--3,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,13)-(3,14) parse error Unexpected symbol ')' in type
