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
                            /root/ValKeywordIsPresentInSynValSig.fsi (4,2--4,5))
                        Target = None
                        AppliesToGetterAndSetter = false
                        Range =
                         /root/ValKeywordIsPresentInSynValSig.fsi (4,2--4,5) }]
                    Range = /root/ValKeywordIsPresentInSynValSig.fsi (4,0--4,7) }],
                 SynIdent (a, None), SynValTyparDecls (None, true),
                 LongIdent (SynLongIdent ([int], [], [None])),
                 SynValInfo ([], SynArgInfo ([], false, None)), false, false,
                 PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector), None,
                 None, /root/ValKeywordIsPresentInSynValSig.fsi (4,0--6,11),
                 { LeadingKeyword =
                    Val /root/ValKeywordIsPresentInSynValSig.fsi (6,0--6,3)
                   InlineKeyword = None
                   WithKeyword = None
                   EqualsRange = None }),
              /root/ValKeywordIsPresentInSynValSig.fsi (4,0--6,11))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/ValKeywordIsPresentInSynValSig.fsi (2,0--6,11),
          { LeadingKeyword =
             Module /root/ValKeywordIsPresentInSynValSig.fsi (2,0--2,6) })],
      { ConditionalDirectives = []
        CodeComments =
         [LineComment /root/ValKeywordIsPresentInSynValSig.fsi (5,0--5,6)] },
      set []))