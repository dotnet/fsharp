ImplFile
  (ParsedImplFileInput
     ("/root/Type/TupleTypeExtensionRecovery 01.fs", false,
      QualifiedNameOfFile TupleTypeExtensionRecovery 01, [],
      [SynModuleOrNamespace
         ([TupleTypeExtensionRecovery 01], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [],
                     Some
                       (Paren
                          (Var (SynTypar (T1, None, false), (1,6--1,9)),
                           (1,5--1,10))),
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,10)),
                  Simple (None (1,5--1,10), (1,5--1,10)), [], None, (1,5--1,10),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = None
                    WithKeyword = None })], (1,0--1,10))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'TupleTypeExtensionRecovery 01' based on the file name 'TupleTypeExtensionRecovery 01.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
