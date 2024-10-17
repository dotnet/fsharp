ImplFile
  (ParsedImplFileInput
     ("/root/Binding/RangeOfEqualSignShouldBePresentInSynModuleDeclLetBindingTyped.fs",
      false,
      QualifiedNameOfFile
        RangeOfEqualSignShouldBePresentInSynModuleDeclLetBindingTyped, [], [],
      [SynModuleOrNamespace
         ([RangeOfEqualSignShouldBePresentInSynModuleDeclLetBindingTyped], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (v, None), false, None, (2,4--2,5)),
                  Some
                    (SynBindingReturnInfo
                       (LongIdent (SynLongIdent ([int], [], [None])),
                        (2,8--2,11), [], { ColonRange = Some (2,6--2,7) })),
                  Typed
                    (Const (Int32 12, (2,14--2,16)),
                     LongIdent (SynLongIdent ([int], [], [None])), (2,14--2,16)),
                  (2,4--2,5), Yes (2,0--2,16),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,12--2,13) })], (2,0--2,16))],
          PreXmlDocEmpty, [], None, (2,0--3,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
