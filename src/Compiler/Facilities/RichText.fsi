// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Text

open FSharp.Compiler.DiagnosticMessage

/// Represents text made of tagged parts, e.g. a diagnostic message in which types, identifiers and
/// punctuation are classified, so that tooling is able to render them with colors.
///
/// Text that carries no classification is represented as a single part tagged TextTag.Text, so that
/// a plain string is always representable and Text is always equal to the original string.
///
/// Two rich texts are equal when they read the same. Classification does not take part in equality,
/// since the places that compare texts - such as deciding whether two types can be told apart in a
/// message - are asking about what reaches the reader.
[<Sealed>]
type public RichText =

    /// Gets the tagged parts of the text
    member Parts: TaggedText[]

    /// Gets the text of all parts concatenated
    member Text: string

    /// Gets whether the text has no parts
    member IsEmpty: bool

module internal RichText =

    /// Text with no parts
    val empty: RichText

    /// Creates text from already tagged parts
    val ofParts: parts: TaggedText[] -> RichText

    /// Creates text from a single tagged part
    val ofTaggedText: part: TaggedText -> RichText

    /// Creates text from a single part with the given classification. Text that is empty has no parts,
    /// so that where a part boundary falls is never visible in the result.
    val ofTag: tag: TextTag -> text: string -> RichText

    /// Creates text from a single part with the classification the name says, for the classifications
    /// a diagnostic message uses. mkText is unclassified text, i.e. text with nothing in it to classify.
    /// Prefer computing the classification from what is being named, as richTextOfEntityRefName and
    /// richTextOfValName do, over choosing one of these by hand.
    val mkText: text: string -> RichText
    val mkActivePatternCase: text: string -> RichText
    val mkActivePatternResult: text: string -> RichText
    val mkAlias: text: string -> RichText
    val mkClass: text: string -> RichText
    val mkDelegate: text: string -> RichText
    val mkEnum: text: string -> RichText
    val mkEvent: text: string -> RichText
    val mkField: text: string -> RichText
    val mkFunction: text: string -> RichText
    val mkInterface: text: string -> RichText
    val mkKeyword: text: string -> RichText
    val mkLineBreak: text: string -> RichText
    val mkLocal: text: string -> RichText
    val mkMember: text: string -> RichText
    val mkMethod: text: string -> RichText
    val mkModule: text: string -> RichText
    val mkModuleBinding: text: string -> RichText
    val mkNamespace: text: string -> RichText
    val mkNumericLiteral: text: string -> RichText
    val mkOperator: text: string -> RichText
    val mkParameter: text: string -> RichText
    val mkProperty: text: string -> RichText
    val mkPunctuation: text: string -> RichText
    val mkRecord: text: string -> RichText
    val mkRecordField: text: string -> RichText
    val mkSpace: text: string -> RichText
    val mkStringLiteral: text: string -> RichText
    val mkStruct: text: string -> RichText
    val mkTypeParameter: text: string -> RichText
    val mkUnion: text: string -> RichText
    val mkUnionCase: text: string -> RichText
    val mkUnknownEntity: text: string -> RichText
    val mkUnknownType: text: string -> RichText
    val mkUnresolvedName: text: string -> RichText

    /// Concatenates two texts
    val append: left: RichText -> right: RichText -> RichText

    /// Concatenates any number of texts
    val concat: texts: RichText seq -> RichText

    /// Concatenates any number of texts, inserting a separator between them
    val concatWith: separator: RichText -> texts: RichText seq -> RichText

    /// Replaces every part with zero or more parts, e.g. to split parts containing line breaks
    val collectParts: mapping: (TaggedText -> TaggedText[]) -> text: RichText -> RichText

    /// A dotted name, classifying the namespace and the dots, and the name itself with the given
    /// constructor. For names that arrive from metadata, reflection or a type provider as one string;
    /// not for an assembly-qualified name, since an assembly version has dots in it too.
    val ofQualifiedName: leafOfName: (string -> RichText) -> name: string -> RichText

    /// A dotted type name whose kind is not known, e.g. because the type could not be dereferenced
    val ofQualifiedTypeName: name: string -> RichText

/// Splices classified arguments into the holes of a message that comes from a resource file.
///
/// A resource accessor returns a message that is already formatted, so the holes can no longer be told
/// apart afterwards. Instead the message is formatted with a sentinel in place of each classified
/// argument, and the sentinels are then replaced with the parts they stand for. This way the resource
/// key stays a compile-checked member reference, and translations are free to reorder, repeat or drop
/// holes.
///
/// This is what the generated FSComp accessors taking classified arguments are built on. Call those
/// directly where they exist; these take a function instead, for the messages that have no such
/// overload - the ones from FSStrings:
///
///     RichMessage.text (fun rich -> RecursionE().Format name (rich ty1) (rich ty2) (rich tpcs))
module internal RichMessage =

    /// Formats a message with no diagnostic number
    val text: format: ((RichText -> string) -> string) -> RichText

    /// Formats a message with a diagnostic number. The formatted message it is given is the
    /// unclassified text the numbered accessors return, i.e. one part, which the parts standing in for
    /// the classified arguments are spliced back into.
    val numbered: format: ((RichText -> string) -> int * RichText) -> int * RichText

/// Accumulates rich text. Adjacent parts with the same classification are merged, so that where one
/// append ended is not visible in the result.
///
/// AppendString has the same name and signature as the StringBuilder extension in lib.fs, so that
/// message formatting code can be moved over to rich text without being rewritten, and can then be
/// converted to emit classified parts one message at a time.
[<Sealed>]
type internal RichTextBuilder =

    new: unit -> RichTextBuilder

    /// Appends unclassified text, tagged TextTag.Text
    member Append: value: string -> unit

    /// Appends a single tagged part
    member Append: value: TaggedText -> unit

    /// Appends the parts of another rich text
    member Append: value: RichText -> unit

    /// Appends a message from FSStrings, classifying each of its arguments. The FSComp accessors are
    /// generated with overloads taking classified arguments, so those are called directly and their
    /// result appended; the FSStrings ones are declared by hand and have no such overload.
    member Append: message: ResourceString<string -> string> * a0: RichText -> unit

    /// Appends a message from a resource file, classifying each of its arguments
    member Append: message: ResourceString<string -> string -> string> * a0: RichText * a1: RichText -> unit

    /// Appends a message from a resource file, classifying each of its arguments
    member Append:
        message: ResourceString<string -> string -> string -> string> * a0: RichText * a1: RichText * a2: RichText ->
            unit

    /// Appends a message from a resource file, classifying each of its arguments
    member Append:
        message: ResourceString<string -> string -> string -> string -> string> *
        a0: RichText *
        a1: RichText *
        a2: RichText *
        a3: RichText ->
            unit

    /// Appends a message whose arguments are spliced in by the given function, for messages that mix
    /// classified and plain arguments. See RichMessage.
    member Append: format: ((RichText -> string) -> string) -> unit

    /// Gets whether nothing has been appended
    member IsEmpty: bool

    /// Gets the accumulated text
    member ToRichText: unit -> RichText
