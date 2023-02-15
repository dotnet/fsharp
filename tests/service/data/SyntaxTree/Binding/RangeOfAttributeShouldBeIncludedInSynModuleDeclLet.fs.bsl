ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs",
      false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynModuleDeclLet, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynModuleDeclLet], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes = [{ TypeName = SynLongIdent ([Foo], [], [None])
                                     ArgExpr = Const (Unit, (2,2--2,5))
                                     Target = None
                                     AppliesToGetterAndSetter = false
                                     Range = (2,2--2,5) }]
                     Range = (2,0--2,7) }],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (3,4--3,5)), None,
                  Const (Int32 0, (3,8--3,9)), (2,0--3,5), Yes (2,0--3,9),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (2,0--3,9))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
