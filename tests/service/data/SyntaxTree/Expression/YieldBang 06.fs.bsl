ImplFile
  (ParsedImplFileInput
     ("/root/Expression/YieldBang 06.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturnFrom
                ((true, false),
                 Typed
                   (ArrayOrListComputed
                      (false,
                       IndexRange
                         (Some (Const (Int32 1, (3,9--3,10))), (3,11--3,13),
                          Some (Const (Int32 10, (3,14--3,16))), (3,9--3,10),
                          (3,14--3,16), (3,9--3,16)), (3,7--3,18)),
                    FromParseError (3,20--3,20), (3,7--3,20)), (3,0--3,18),
                 { YieldOrReturnFromKeyword = (3,0--3,6) }), (3,0--3,18))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,18), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Incomplete structured construct at or before this point in expression
