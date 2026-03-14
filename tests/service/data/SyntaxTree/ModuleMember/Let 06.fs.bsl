ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Let 06.fs", false, QualifiedNameOfFile Let 06, [],
      [SynModuleOrNamespace
         ([Let 06], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (1,4--1,5)), None,
                  Const (Int32 0, (1,8--1,9)), (1,4--1,5), Yes (1,0--1,9),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = Some (1,6--1,7) })], (1,0--1,12),
              { InKeyword = Some (1,10--1,12) });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (2,4--2,5)), None,
                  Const (Int32 1, (2,8--2,9)), (2,4--2,5), Yes (2,0--2,9),
                  { LeadingKeyword = Let (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange = Some (2,6--2,7) })], (2,0--2,12),
              { InKeyword = Some (2,10--2,12) });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  Const (Int32 0, (3,8--3,9)), (3,4--3,5), Yes (3,0--3,9),
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,6--3,7) })], (3,0--3,12),
              { InKeyword = Some (3,10--3,12) });
           Expr (Const (Unit, (4,0--4,2)), (4,0--4,2))], PreXmlDocEmpty, [],
          None, (1,0--4,2), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Let 06' based on the file name 'Let 06.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
