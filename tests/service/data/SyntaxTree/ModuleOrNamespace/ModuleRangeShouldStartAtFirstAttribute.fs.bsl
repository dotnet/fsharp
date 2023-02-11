ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs", false,
      QualifiedNameOfFile Bar, [], [],
      [SynModuleOrNamespace
         ([Bar], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (s, None), false, None,
                     /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,4--5,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([string], [], [None])),
                        /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,8--5,14),
                        [],
                        { ColonRange =
                           Some
                             /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,6--5,7) })),
                  Typed
                    (Const
                       (String
                          ("s", Regular,
                           /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,17--5,20)),
                        /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,17--5,20)),
                     LongIdent (SynLongIdent ([string], [], [None])),
                     /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,17--5,20)),
                  /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,4--5,5),
                  Yes
                    /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,0--5,20),
                  { LeadingKeyword =
                     Let
                       /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,15--5,16) })],
              /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (5,0--5,20))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes =
              [{ TypeName = SynLongIdent ([Foo], [], [None])
                 ArgExpr =
                  Const
                    (Unit,
                     /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (2,4--2,7))
                 Target = None
                 AppliesToGetterAndSetter = false
                 Range =
                  /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (2,4--2,7) }]
             Range =
              /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (2,0--2,11) }],
          None,
          /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (2,0--5,20),
          { LeadingKeyword =
             Module
               /root/ModuleOrNamespace/ModuleRangeShouldStartAtFirstAttribute.fs (3,0--3,6) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
