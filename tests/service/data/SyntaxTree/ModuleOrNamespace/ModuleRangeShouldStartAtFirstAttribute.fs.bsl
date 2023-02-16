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
                  Named (SynIdent (s, None), false, None, (5,4--5,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([string], [], [None])),
                        (5,8--5,14), [], { ColonRange = Some (5,6--5,7) })),
                  Typed
                    (Const (String ("s", Regular, (5,17--5,20)), (5,17--5,20)),
                     LongIdent (SynLongIdent ([string], [], [None])),
                     (5,17--5,20)), (5,4--5,5), Yes (5,0--5,20),
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,15--5,16) })], (5,0--5,20))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
          [{ Attributes = [{ TypeName = SynLongIdent ([Foo], [], [None])
                             ArgExpr = Const (Unit, (2,4--2,7))
                             Target = None
                             AppliesToGetterAndSetter = false
                             Range = (2,4--2,7) }]
             Range = (2,0--2,11) }], None, (2,0--5,20),
          { LeadingKeyword = Module (3,0--3,6) })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
