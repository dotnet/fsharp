ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Let 05.fs", false, QualifiedNameOfFile Let 05, [], [],
      [SynModuleOrNamespace
         ([Let 05], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent
                       (|AP|, Some (HasParenthesis ((1,4--1,5), (1,9--1,10)))),
                     false, None, (1,4--1,10)), None,
                  ArbitraryAfterError ("localBinding2", (1,10--1,10)),
                  (1,4--1,10), Yes (1,0--1,10),
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = None
                    EqualsRange = None })], (1,0--1,10));
           Expr (Const (Unit, (3,0--3,2)), (3,0--3,2))], PreXmlDocEmpty, [],
          None, (1,0--3,2), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,0)-(3,1) parse error Incomplete structured construct at or before this point in binding. Expected '=' or other token.
(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'Let 05' based on the file name 'Let 05.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
