ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/GenericTypeNotStructAndOtherConstraint-I_am_Still_Sane.fs",
      false,
      QualifiedNameOfFile GenericTypeNotStructAndOtherConstraint-I_am_Still_Sane,
      [], [],
      [SynModuleOrNamespace
         ([GenericTypeNotStructAndOtherConstraint-I_am_Still_Sane], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })],
                           [WhereTyparIsReferenceType
                              (SynTypar (T, None, false), (1,15--1,29));
                            WhereTyparIsEquatable
                              (SynTypar (T, None, false), (1,34--1,45))],
                           (1,6--1,46))), [], [C],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,6)),
                  ObjectModel (Class, [], (1,49--1,58)), [], None, (1,5--1,58),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,47--1,48)
                    WithKeyword = None })], (1,0--1,58))], PreXmlDocEmpty, [],
          None, (1,0--2,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(1,0)-(2,0) parse warning The declarations in this file will be placed in an implicit module 'GenericTypeNotStructAndOtherConstraint-I_am_Still_Sane' based on the file name 'GenericTypeNotStructAndOtherConstraint-I_am_Still_Sane.fs'. However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.
