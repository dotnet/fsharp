SigFile
  (ParsedSigFileInput
     ("/root/SignatureType/RangeOfMembersShouldBeIncludedInSynExceptionSigAndSynModuleSigDeclException.fsi",
      QualifiedNameOfFile FSharp.Compiler.ParseHelpers, [], [],
      [SynModuleOrNamespaceSig
         ([FSharp; Compiler; ParseHelpers], false, NamedModule,
          [Exception
             (SynExceptionSig
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (SyntaxError, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([obj], [], [None])), false,
                             PreXmlDoc ((4,25), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (4,25--4,28), { LeadingKeyword = None });
                          SynField
                            ([], false, Some range,
                             LongIdent (SynLongIdent ([range], [], [None])),
                             false,
                             PreXmlDoc ((4,31), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (4,31--4,43), { LeadingKeyword = None })],
                       PreXmlDocEmpty, None, (4,10--4,43), { BarRange = None }),
                    None, PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (4,0--4,43)), Some (4,44--4,48),
                 [Member
                    (SynValSig
                       ([], SynIdent (Meh, None), SynValTyparDecls (None, true),
                        Fun
                          (LongIdent (SynLongIdent ([string], [], [None])),
                           LongIdent (SynLongIdent ([int], [], [None])),
                           (5,17--5,30), { ArrowRange = (5,24--5,26) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, None)]],
                           SynArgInfo ([], false, None)), false, false,
                        PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                        None, None, (5,4--5,30),
                        { LeadingKeyword = Member (5,4--5,10)
                          InlineKeyword = None
                          WithKeyword = None
                          EqualsRange = None }),
                     { IsInstance = true
                       IsDispatchSlot = false
                       IsOverrideOrExplicitImpl = false
                       IsFinal = false
                       GetterOrSetterIsCompilerGenerated = false
                       MemberKind = Member }, (5,4--5,30),
                     { GetSetKeywords = None })], (4,0--5,30)), (4,0--5,30));
           Open
             (ModuleOrNamespace (SynLongIdent ([Foo], [], [None]), (7,5--7,8)),
              (7,0--7,8))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [],
          Some (Internal (2,7--2,15)), (2,0--7,8),
          { LeadingKeyword = Module (2,0--2,6) })], { ConditionalDirectives = []
                                                      CodeComments = [] },
      set []))
