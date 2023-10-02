ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 04.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false,
                 Some
                   (App
                      (Atomic, false, Ident f, Const (Unit, (3,4--3,6)),
                       (3,3--3,6)), ((3,6--3,6), None)), [], (3,0--3,9),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,6) parse error Field bindings must have the form 'id = expr;'
