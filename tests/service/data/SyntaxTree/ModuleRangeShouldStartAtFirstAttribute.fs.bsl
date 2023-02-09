ImplFile
  (ParsedImplFileInput
     ("/root/ModuleRangeShouldStartAtFirstAttribute.fs", false,
      QualifiedNameOfFile Bar, [], [],
      [SynModuleOrNamespace
         ([Bar], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (s, None), false, None,
                     /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,4--4,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([string], [], [None])),
                        /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,8--4,14),
                        [],
                        { ColonRange =
                           Some
                             /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,6--4,7) })),
                  Typed
                    (Const
                       (String
                          ("s", Regular,
                           /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,17--4,20)),
                        /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,17--4,20)),
                     LongIdent (SynLongIdent ([string], [], [None])),
                     /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,17--4,20)),
                  /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,4--4,5),
                  Yes
                    /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,0--4,20),
                  { LeadingKeyword =
                     Let
                       /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,15--4,16) })],
              /root/ModuleRangeShouldStartAtFirstAttribute.fs (4,0--4,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes =
              [{ TypeName = SynLongIdent ([Foo], [], [None])
                 ArgExpr =
                  Const
                    (Unit,
                     /root/ModuleRangeShouldStartAtFirstAttribute.fs (1,4--1,7))
                 Target = None
                 AppliesToGetterAndSetter = false
                 Range =
                  /root/ModuleRangeShouldStartAtFirstAttribute.fs (1,4--1,7) }]
             Range = /root/ModuleRangeShouldStartAtFirstAttribute.fs (1,0--1,11) }],
          None, /root/ModuleRangeShouldStartAtFirstAttribute.fs (1,0--4,20),
          { LeadingKeyword =
             Module /root/ModuleRangeShouldStartAtFirstAttribute.fs (2,0--2,6) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))