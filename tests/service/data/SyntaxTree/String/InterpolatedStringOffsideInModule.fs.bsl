ImplFile
  (ParsedImplFileInput
     ("/root/String/InterpolatedStringOffsideInModule.fs", false,
      QualifiedNameOfFile InterpolatedStringOffsideInModule, [],
      [SynModuleOrNamespace
         ([InterpolatedStringOffsideInModule], false, AnonModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [A],
                 PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (1,0--1,8)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (b, None), false, None, (2,8--2,9)), None,
                      InterpolatedString
                        ([String ("
", (3,8--4,1));
                          FillExpr (Const (Int32 0, (4,1--4,2)), None);
                          String ("", (4,2--4,4))], Regular, (3,8--4,4)),
                      (2,8--2,9), Yes (2,4--4,4),
                      { LeadingKeyword = Let (2,4--2,7)
                        InlineKeyword = None
                        EqualsRange = Some (2,10--2,11) })], (2,4--4,4))], false,
              (1,0--4,4), { ModuleKeyword = Some (1,0--1,6)
                            EqualsRange = Some (1,9--1,10) })], PreXmlDocEmpty,
          [], None, (1,0--4,4), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
