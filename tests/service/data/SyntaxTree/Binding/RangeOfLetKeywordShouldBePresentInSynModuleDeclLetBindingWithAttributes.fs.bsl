ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfLetKeywordShouldBePresentInSynModuleDeclLetBindingWithAttributes.fs",
      false,
      QualifiedNameOfFile
        RangeOfLetKeywordShouldBePresentInSynModuleDeclLetBindingWithAttributes,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfLetKeywordShouldBePresentInSynModuleDeclLetBindingWithAttributes],
          false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([SomeAttribute], [], [None])
                         ArgExpr = Const (Unit, (3,2--3,15))
                         Target = None
                         AppliesToGetterAndSetter = false
                         Range = (3,2--3,15) }]
                     Range = (3,0--3,17) }],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (5,4--5,5)), None,
                  Const (Int32 12, (5,8--5,10)), (2,0--5,5), Yes (3,0--5,10),
                  { LeadingKeyword = Let (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange = Some (5,6--5,7) })], (2,0--5,10))],
          PreXmlDocEmpty, [], None, (3,0--6,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [LineComment (4,0--4,15)] }, set []))
