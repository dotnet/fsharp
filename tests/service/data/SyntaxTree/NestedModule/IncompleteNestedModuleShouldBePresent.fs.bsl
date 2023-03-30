ImplFile
  (ParsedImplFileInput
     ("/root/NestedModule/IncompleteNestedModuleShouldBePresent.fs", false,
      QualifiedNameOfFile A.B, [], [],
      [SynModuleOrNamespace
         ([A; B], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (3,0--3,8)), false, [], false, (3,0--3,8),
              { ModuleKeyword = Some (3,0--3,6)
                EqualsRange = None });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (5,4--5,5)), None,
                  Const (Unit, (5,8--5,10)), (5,4--5,5), Yes (5,0--5,10),
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,6--5,7) })], (5,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,3) parse error Unexpected keyword 'let' or 'use' in definition. Expected '=' or other token.
