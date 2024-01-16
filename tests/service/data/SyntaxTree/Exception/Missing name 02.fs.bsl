ImplFile
  (ParsedImplFileInput
     ("/root/Exception/Missing name 02.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([{ Attributes = [{ TypeName = SynLongIdent ([A], [], [None])
                                       ArgExpr = Const (Unit, (3,12--3,13))
                                       Target = None
                                       AppliesToGetterAndSetter = false
                                       Range = (3,12--3,13) }]
                       Range = (3,10--3,15) }],
                    SynUnionCase
                      ([], SynIdent (, None), Fields [], PreXmlDocEmpty, None,
                       (3,0--3,15), { BarRange = None }), None,
                    PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                    (3,0--3,15)), None, [], (3,0--3,15)), (3,0--3,15));
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

(5,0)-(5,9) parse error Unexpected keyword 'exception' in exception definition. Expected identifier or other token.
