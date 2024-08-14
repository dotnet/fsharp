ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Let 03.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Wild (3,4--3,5), None,
                  ArbitraryAfterError ("localBinding2", (3,5--3,5)), (3,4--3,5),
                  Yes (3,0--3,5), { LeadingKeyword = Let (3,0--3,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (3,0--3,5));
           Expr (Const (Int32 1, (5,0--5,1)), (5,0--5,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,1) parse error Incomplete structured construct at or before this point in binding. Expected '=' or other token.
