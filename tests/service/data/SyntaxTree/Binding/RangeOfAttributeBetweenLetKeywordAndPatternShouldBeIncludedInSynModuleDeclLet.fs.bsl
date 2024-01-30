ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfAttributeBetweenLetKeywordAndPatternShouldBeIncludedInSynModuleDeclLet.fs",
      false,
      QualifiedNameOfFile
        RangeOfAttributeBetweenLetKeywordAndPatternShouldBeIncludedInSynModuleDeclLet,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfAttributeBetweenLetKeywordAndPatternShouldBeIncludedInSynModuleDeclLet],
          false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([Literal], [], [None])
                         ArgExpr = Const (Unit, (2,6--2,13))
                         Target = None
                         AppliesToGetterAndSetter = false
                         Range = (2,6--2,13) }]
                     Range = (2,4--2,15) }],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Paren
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        Pats
                          [Named (SynIdent (x, None), false, None, (2,19--2,20))],
                        None, (2,17--2,20)), (2,16--2,21)), None,
                  Const (Int32 1, (2,24--2,25)), (2,4--2,21), Yes (2,0--2,25),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,22--2,23) })], (2,0--2,25))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
