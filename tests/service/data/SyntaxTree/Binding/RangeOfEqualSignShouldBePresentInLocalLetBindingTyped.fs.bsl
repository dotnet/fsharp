ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfEqualSignShouldBePresentInLocalLetBindingTyped.fs",
      false,
      QualifiedNameOfFile RangeOfEqualSignShouldBePresentInLocalLetBindingTyped,
      [], [],
      [SynModuleOrNamespace
         ([RangeOfEqualSignShouldBePresentInLocalLetBindingTyped], false,
          AnonModule,
          [Expr
             (Do
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None, SynValInfo ([], SynArgInfo ([], false, None)),
                           None),
                        Named (SynIdent (z, None), false, None, (3,8--3,9)),
                        Some
                          (SynBindingReturnInfo
                             (LongIdent (SynLongIdent ([int], [], [None])),
                              (3,11--3,14), [],
                              { ColonRange = Some (3,9--3,10) })),
                        Typed
                          (Const (Int32 2, (3,17--3,18)),
                           LongIdent (SynLongIdent ([int], [], [None])),
                           (3,17--3,18)), (3,8--3,9), Yes (3,4--3,18),
                        { LeadingKeyword = Let (3,4--3,7)
                          InlineKeyword = None
                          EqualsRange = Some (3,15--3,16) })],
                    Const (Unit, (4,4--4,6)), (3,4--4,6), { InKeyword = None }),
                 (2,0--4,6)), (2,0--4,6))], PreXmlDocEmpty, [], None, (2,0--5,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
