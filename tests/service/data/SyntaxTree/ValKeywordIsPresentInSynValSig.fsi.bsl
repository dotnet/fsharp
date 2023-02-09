SigFile
  (ParsedSigFileInput
     ("/root/ValKeywordIsPresentInSynValSig.fsi", QualifiedNameOfFile Meh, [],
      [],
      [SynModuleOrNamespaceSig
         ([Meh], false, NamedModule,
          [Val
             (SynValSig
                ([{ Attributes =
                     [{ TypeName = SynLongIdent ([Foo], [], [None])
                        ArgExpr =
                         Const
                           (Unit,
                            /root/ValKeywordIsPresentInSynValSig.fsi (3,2--3,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/ValKeywordIsPresentInSynValSig.fsi (3,2--3,5) }]
                    Range = /root/ValKeywordIsPresentInSynValSig.fsi (3,0--3,7) }],
                 SynIdent (a, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, /root/ValKeywordIsPresentInSynValSig.fsi (3,0--5,11),
                 { LeadingKeyword =
                    Val /root/ValKeywordIsPresentInSynValSig.fsi (5,0--5,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/ValKeywordIsPresentInSynValSig.fsi (3,0--5,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/ValKeywordIsPresentInSynValSig.fsi (1,0--5,11),
          { LeadingKeyword =
             Module /root/ValKeywordIsPresentInSynValSig.fsi (1,0--1,6) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment /root/ValKeywordIsPresentInSynValSig.fsi (4,0--4,6)] },
      set []))