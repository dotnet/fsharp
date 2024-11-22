ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Anon 06.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false,
                 Some
                   (DotGet
                      (App
                         (Atomic, false, Ident f, Const (Unit, (3,4--3,6)),
                          (3,3--3,6)), (3,6--3,7),
                       SynLongIdent ([F], [], [None]), (3,3--3,8)),
                    ((3,8--3,8), None)), [], (3,0--3,11),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(3,3)-(3,8) parse error Field bindings must have the form 'id = expr;'
