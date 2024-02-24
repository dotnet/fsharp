ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (FromParseError (3,6--3,6)); Star (3,5--3,6);
                        Type (LongIdent (SynLongIdent ([b], [], [None])))],
                       (3,5--3,8)), (3,1--3,8)), (3,0--3,1), Some (3,8--3,9),
                 (3,0--3,9)), (3,0--3,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(3,6) parse error Expecting type
