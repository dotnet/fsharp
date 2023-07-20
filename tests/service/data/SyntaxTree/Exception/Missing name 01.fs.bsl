ImplFile
  (ParsedImplFileInput
     ("/root/Exception/Missing name 01.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (, None), Fields [], PreXmlDocEmpty, None,
                       (3,0--3,9), { BarRange = None }), None,
                    PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (3,0--3,9)), None, [], (3,0--3,9)), (3,0--3,9));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (B, None), Fields [], PreXmlDocEmpty, None,
                       (5,10--5,11), { BarRange = None }), None,
                    PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (5,0--5,11)), None, [], (5,0--5,11)), (5,0--5,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,10)-(5,0) parse error Incomplete structured construct at or before this point in exception definition. Expected identifier or other token.
