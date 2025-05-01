ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 05.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false,
                 Some
                   (DiscardAfterMissingQualificationAfterDot
                      (App
                         (Atomic, false, Ident f, Const (Unit, (3,4--3,6)),
                          (3,3--3,6)), (3,6--3,7), (3,3--3,7)),
                    ((3,10--3,10), None)), [], (3,0--3,10),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,6)-(3,7) parse error Missing qualification after '.'
(3,3)-(3,10) parse error Field bindings must have the form 'id = expr;'
