ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Unary - Reserved 02.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, false, Ident x,
                    ArbitraryAfterError ("unfinished identifier", (3,2--3,3)),
                    (3,0--3,3)), Ident y, (3,0--3,5)), (3,0--3,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,2)-(3,3) parse error This is not a valid identifier
