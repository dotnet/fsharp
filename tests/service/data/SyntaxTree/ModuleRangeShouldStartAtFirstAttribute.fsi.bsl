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
                 PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None,
                 /root/ModuleRangeShouldStartAtFirstAttribute.fsi (5,0--5,14),
                 { LeadingKeyword =
                    Val
                      /root/ModuleRangeShouldStartAtFirstAttribute.fsi (5,0--5,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/ModuleRangeShouldStartAtFirstAttribute.fsi (5,0--5,14))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes =
              [{ TypeName = SynLongIdent ([Foo], [], [None])
                 ArgExpr =
                  Const
                    (Unit,
                     /root/ModuleRangeShouldStartAtFirstAttribute.fsi (2,4--2,7))
                 Target = None
                 AppliesToGetterAndSetter = false
                 Range =
                  /root/ModuleRangeShouldStartAtFirstAttribute.fsi (2,4--2,7) }]
             Range =
              /root/ModuleRangeShouldStartAtFirstAttribute.fsi (2,0--2,11) }],
          None, /root/ModuleRangeShouldStartAtFirstAttribute.fsi (2,0--5,14),
          { LeadingKeyword =
             Module /root/ModuleRangeShouldStartAtFirstAttribute.fsi (3,0--3,6) })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))