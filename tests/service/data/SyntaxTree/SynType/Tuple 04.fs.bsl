ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Tuple 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Tuple
                      (false,
                       [Type (LongIdent (SynLongIdent ([a], [], [None])));
                        Star (3,7--3,8); Type (FromParseError (3,8--3,8))],
                       (3,5--3,8)), (3,1--3,8)), (3,0--3,1), Some (3,9--3,10),
                 (3,0--3,10)), (3,0--3,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,9)-(3,10) parse error Unexpected symbol ')' in type
