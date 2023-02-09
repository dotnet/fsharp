SigFile
  (ParsedSigFileInput
     ("/root/ModuleRangeShouldStartAtFirstAttribute.fsi",
      QualifiedNameOfFile Bar, [], [],
      [SynModuleOrNamespaceSig
         ([Bar], false, NamedModule,
          [Val
             (SynValSig
                ([], SynIdent (s, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([string], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None,
                 /root/ModuleRangeShouldStartAtFirstAttribute.fsi (4,0--4,14),
                 { LeadingKeyword =
                    Val
                      /root/ModuleRangeShouldStartAtFirstAttribute.fsi (4,0--4,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/ModuleRangeShouldStartAtFirstAttribute.fsi (4,0--4,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes =
              [{ TypeName = SynLongIdent ([Foo], [], [None])
                 ArgExpr =
                  Const
                    (Unit,
                     /root/ModuleRangeShouldStartAtFirstAttribute.fsi (1,4--1,7))
                 Target = None
                 AppliesToGetterAndSetter = false
                 Range =
                  /root/ModuleRangeShouldStartAtFirstAttribute.fsi (1,4--1,7) }]
             Range =
              /root/ModuleRangeShouldStartAtFirstAttribute.fsi (1,0--1,11) }],
          None, /root/ModuleRangeShouldStartAtFirstAttribute.fsi (1,0--4,14),
          { LeadingKeyword =
             Module /root/ModuleRangeShouldStartAtFirstAttribute.fsi (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))