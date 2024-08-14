ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 03.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Fun
                      (LongIdent (SynLongIdent ([a], [], [None])),
                       FromParseError (3,9--3,9), (3,5--3,9),
                       { ArrowRange = (3,7--3,9) }), (3,1--3,9)), (3,0--3,1),
                 Some (3,10--3,11), (3,0--3,11)), (3,0--3,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,10)-(3,11) parse error Unexpected symbol ')' in type
