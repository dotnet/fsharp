// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if UNUSED
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    /// <summary>
    /// Replacement type
    /// </summary>
    public enum TokenReplaceType
    {
        ReplaceString,
        ReplaceNumber,
        ReplaceCode
    }

    /// <summary>
    /// Contain a number of functions that handle token replacement
    /// </summary>
    [CLSCompliant(false)]
    public class TokenProcessor
    {
        #region fields
        // Internal fields
        private ArrayList tokenlist;


        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public TokenProcessor()
        {
            tokenlist = new ArrayList();
        }

        /// <summary>
        /// Reset list of TokenReplacer entries
        /// </summary>
        public virtual void Reset()
        {
            tokenlist.Clear();
        }


        /// <summary>
        /// Add a replacement type entry
        /// </summary>
        /// <param name="token">token to replace</param>
        /// <param name="replacement">replacement string</param>
        public virtual void AddReplace(string token, string replacement)
        {
            tokenlist.Add(new ReplacePairToken(token, replacement));
        }

        /// <summary>
        /// Add replace between entry
        /// </summary>
        /// <param name="tokenStart">Start token</param>
        /// <param name="tokenEnd">End token</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "tokenid")]
        public virtual void AddReplaceBetween(string tokenid, string tokenStart, string tokenEnd, string replacement)
        {
            tokenlist.Add(new ReplaceBetweenPairToken(tokenid, tokenStart, tokenEnd, replacement));
        }

        /// <summary>
        /// Add a deletion entry
        /// </summary>
        /// <param name="tokenToDelete">Token to delete</param>
        public virtual void AddDelete(string tokenToDelete)
        {
            tokenlist.Add(new DeleteToken(tokenToDelete));
        }
        #endregion

        #region TokenProcessing
        /// <summary>
        /// For all known token, replace token with correct value
        /// </summary>
        /// <param name="source">File of the source file</param>
        /// <param name="destination">File of the destination file</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Untoken")]
        public virtual void UntokenFile(string source, string destination)
        {
            if (source == null || destination == null || source.Length == 0 || destination.Length == 0)
                throw new ArgumentNullException("Token replacement target file is not valid.");

            // Make sure that the destination folder exists.
            string destinationFolder = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            //Open the file. Check to see if the File is binary or text.
            // NOTE: This is not correct because GetBinaryType will return true
            // only if the file is executable, not if it is a dll, a library or
            // any other type of binary file.

            uint binaryType;
            if (!NativeMethods.GetBinaryType(source, out binaryType))
            {
                string buffer = File.ReadAllText(source);
                foreach (object pair in tokenlist)
                {
                    if (pair is DeleteToken)
                        DeleteTokens(ref buffer, (DeleteToken)pair);
                    if (pair is ReplaceBetweenPairToken)
                        ReplaceBetweenTokens(ref buffer, (ReplaceBetweenPairToken)pair);
                    if (pair is ReplacePairToken)
                        ReplaceTokens(ref buffer, (ReplacePairToken)pair);
                }
                File.WriteAllText(destination, buffer);
            }
            else
                File.Copy(source, destination);

        }

        /// <summary>
        /// Replaces the tokens in a buffer with the replacement string
        /// </summary>
        /// <param name="buffer">Buffer to update</param>
        /// <param name="tokenToReplace">replacement data</param>
        public virtual void ReplaceTokens(ref string buffer, ReplacePairToken tokenToReplace)
        {
            buffer = buffer.Replace(tokenToReplace.Token, tokenToReplace.Replacement);
        }

        /// <summary>
        /// Deletes the token from the buffer
        /// </summary>
        /// <param name="buffer">Buffer to update</param>
        /// <param name="tokenToDelete">token to delete</param>
        public virtual void DeleteTokens(ref string buffer, DeleteToken tokenToDelete)
        {
            buffer = buffer.Replace(tokenToDelete.StringToDelete, string.Empty);
        }

        /// <summary>
        /// Replaces the token from the buffer between the provided tokens
        /// </summary>
        /// <param name="buffer">Buffer to update</param>
        /// <param name="rpBetweenToken">replacement token</param>
        public virtual void ReplaceBetweenTokens(ref string buffer, ReplaceBetweenPairToken rpBetweenToken)
        {
            string regularExp = rpBetweenToken.TokenStart + "[^" + rpBetweenToken.TokenIdentifier + "]*" + rpBetweenToken.TokenEnd;
            buffer = System.Text.RegularExpressions.Regex.Replace(buffer, regularExp, rpBetweenToken.TokenReplacement);
        }

        #endregion

        #region Guid generators
        /// <summary>
        /// Generates a string representation of a guid with the following format:
        /// 0x01020304, 0x0506, 0x0708, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10
        /// </summary>
        /// <param name="value">Guid to be generated</param>
        /// <returns>The guid as string</returns>
        public string GuidToForm1(Guid value)
        {
            byte[] GuidBytes = value.ToByteArray();
            StringBuilder ResultingStr = new StringBuilder(80);

            // First 4 bytes
            int i = 0;
            int Number = 0;
            for (i = 0; i < 4; ++i)
            {
                int CurrentByte = GuidBytes[i];
                Number += CurrentByte << (8 * i);
            }
            UInt32 FourBytes = (UInt32)Number;
            ResultingStr.AppendFormat(CultureInfo.InvariantCulture, "0x{0}", FourBytes.ToString("X", CultureInfo.InvariantCulture));

            // 2 chunks of 2 bytes
            for (int j = 0; j < 2; ++j)
            {
                Number = 0;
                for (int k = 0; k < 2; ++k)
                {
                    int CurrentByte = GuidBytes[i++];
                    Number += CurrentByte << (8 * k);
                }
                UInt16 TwoBytes = (UInt16)Number;
                ResultingStr.AppendFormat(CultureInfo.InvariantCulture, ", 0x{0}", TwoBytes.ToString("X", CultureInfo.InvariantCulture));
            }

            // 8 chunks of 1 bytes
            for (int j = 0; j < 8; ++j)
            {
                ResultingStr.AppendFormat(CultureInfo.InvariantCulture, ", 0x{0}", GuidBytes[i++].ToString("X", CultureInfo.InvariantCulture));
            }

            return ResultingStr.ToString();
        }

        /// <summary>
        /// Generates a string representation of a guid with the following format:
        /// 0x01020304, 0x0506, 0x0708, { 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10 }
        /// </summary>
        /// <param name="value">Guid to be generated.</param>
        /// <returns>The guid as string.</returns>
        private string GuidToForm2(Guid value)
        {
            byte[] GuidBytes = value.ToByteArray();
            StringBuilder ResultingStr = new StringBuilder(80);

            // First 4 bytes
            int i = 0;
            int Number = 0;
            for (i = 0; i < 4; ++i)
            {
                int CurrentByte = GuidBytes[i];
                Number += CurrentByte << (8 * i);
            }
            UInt32 FourBytes = (UInt32)Number;
            ResultingStr.AppendFormat(CultureInfo.InvariantCulture, "0x{0}", FourBytes.ToString("X", CultureInfo.InvariantCulture));

            // 2 chunks of 2 bytes
            for (int j = 0; j < 2; ++j)
            {
                Number = 0;
                for (int k = 0; k < 2; ++k)
                {
                    int CurrentByte = GuidBytes[i++];
                    Number += CurrentByte << (8 * k);
                }
                UInt16 TwoBytes = (UInt16)Number;
                ResultingStr.AppendFormat(CultureInfo.InvariantCulture, ", 0x{0}", TwoBytes.ToString("X", CultureInfo.InvariantCulture));
            }

            // 8 chunks of 1 bytes
            ResultingStr.AppendFormat(CultureInfo.InvariantCulture, ", {{ 0x{0}", GuidBytes[i++].ToString("X", CultureInfo.InvariantCulture));
            for (int j = 1; j < 8; ++j)
            {
                ResultingStr.AppendFormat(CultureInfo.InvariantCulture, ", 0x{0}", GuidBytes[i++].ToString("X", CultureInfo.InvariantCulture));
            }
            ResultingStr.Append(" }");

            return ResultingStr.ToString();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// This function will accept a subset of the characters that can create an
        /// identifier name: there are other unicode char that can be inside the name, but
        /// this function will not allow. By now it can work this way, but when and if the
        /// VSIP package will handle also languages different from english, this function
        /// must be changed.
        /// </summary>
        /// <param name="c">Character to validate</param>
        /// <returns>true if successful false otherwise</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        public /*protected, but public for FSharp.Project.dll*/ bool IsValidIdentifierChar(char c)
        {
            if ((c >= 'a') && (c <= 'z'))
            {
                return true;
            }
            if ((c >= 'A') && (c <= 'Z'))
            {
                return true;
            }
            if (c == '_')
            {
                return true;
            }
            if ((c >= '0') && (c <= '9'))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if the start character is valid
        /// </summary>
        /// <param name="c">Start character</param>
        /// <returns>true if successful false otherwise</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
        public /*protected, but public for FSharp.Project.dll*/ bool IsValidIdentifierStartChar(char c)
        {
            if (!IsValidIdentifierChar(c))
            {
                return false;
            }
            if ((c >= '0') && (c <= '9'))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The goal here is to reduce the risk of name conflict between 2 classes
        /// added in different directories. This code does NOT garanty uniqueness.
        /// To garanty uniqueness, you should change this function to work with
        /// the language service to verify that the namespace+class generated does
        /// not conflict.
        /// </summary>
        /// <param name="fileFullPath">Full path to the new file</param>
        /// <returns>Namespace to use for the new file</returns>
        public string GetFileNamespace(string fileFullPath, ProjectNode node)
        {
            // Get base namespace from the project
            string namespce = node.GetProjectProperty("RootNamespace");
            if (String.IsNullOrEmpty(namespce))
                namespce = Path.GetFileNameWithoutExtension(fileFullPath); ;

            // If the item is added to a subfolder, the name space should reflect this.
            // This is done so that class names from 2 files with the same name but different
            // directories don't conflict.
            string relativePath = Path.GetDirectoryName(fileFullPath);
            string projectPath = Path.GetDirectoryName(node.GetMkDocument());
            // Our project system only support adding files that are sibling of the project file or that are in subdirectories.
            if (String.Compare(projectPath, 0, relativePath, 0, projectPath.Length, true, CultureInfo.CurrentCulture) == 0)
            {
                relativePath = relativePath.Substring(projectPath.Length);
            }
            else
            {
                Debug.Fail("Adding an item to the project that is NOT under the project folder.");
                // We are going to use the full file path for generating the namespace
            }

            // Get the list of parts
            int index = 0;
            string[] pathParts;
            pathParts = relativePath.Split(Path.DirectorySeparatorChar);

            // Use a string builder with default size being the expected size
            StringBuilder result = new StringBuilder(namespce, namespce.Length + relativePath.Length + 1);
            // For each path part
            while (index < pathParts.Length)
            {
                string part = pathParts[index];
                ++index;

                // This could happen if the path had leading/trailing slash, we want to ignore empty pieces
                if (String.IsNullOrEmpty(part))
                    continue;

                // If we reach here, we will be adding something, so add a namespace separator '.'
                result.Append('.');

                // Make sure it starts with a letter
                if (!char.IsLetter(part, 0))
                    result.Append('N');

                // Filter invalid namespace characters
                foreach (char c in part)
                {
                    if (char.IsLetterOrDigit(c))
                        result.Append(c);
                }
            }
            return result.ToString();
        }
        #endregion

    }

    /// <summary>
    ///  Storage classes for replacement tokens
    /// </summary>
    public class ReplacePairToken
    {
        /// <summary>
        /// token string
        /// </summary>
        private string token;

        /// <summary>
        /// Replacement string
        /// </summary>
        private string replacement;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">replaceable token</param>
        /// <param name="replacement">replacement string</param>
        public ReplacePairToken(string token, string replacement)
        {
            this.token = token;
            this.replacement = replacement;
        }

        /// <summary>
        /// Token that needs to be replaced
        /// </summary>
        public string Token
        {
            get { return token; }
        }
        /// <summary>
        /// String to replace the token with
        /// </summary>
        public string Replacement
        {
            get { return replacement; }
        }
    }

    /// <summary>
    /// Storage classes for token to be deleted
    /// </summary>
    public class DeleteToken
    {
        /// <summary>
        /// String to delete
        /// </summary>
        private string token;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">Deletable token.</param>
        public DeleteToken(string token)
        {
            this.token = token;
        }

        /// <summary>
        /// Token marking the end of the block to delete
        /// </summary>
        public string StringToDelete
        {
            get { return token; }
        }
    }

    /// <summary>
    /// Storage classes for string to be deleted between tokens to be deleted 
    /// </summary>
    public class ReplaceBetweenPairToken
    {
        /// <summary>
        /// Token start
        /// </summary>
        private string tokenStart;

        /// <summary>
        /// End token
        /// </summary>
        private string tokenEnd;

        /// <summary>
        /// Replacement string
        /// </summary>
        private string replacement;

        /// <summary>
        /// Token identifier string
        /// </summary>
        private string tokenidentifier;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockStart">Start token</param>
        /// <param name="blockEnd">End Token</param>
        /// <param name="replacement">Replacement string.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "tokenid")]
        public ReplaceBetweenPairToken(string tokenid, string blockStart, string blockEnd, string replacement)
        {
            tokenStart = blockStart;
            tokenEnd = blockEnd;
            this.replacement = replacement;
            tokenidentifier = tokenid;
        }

        /// <summary>
        /// Token marking the begining of the block to delete
        /// </summary>
        public string TokenStart
        {
            get { return tokenStart; }
        }

        /// <summary>
        /// Token marking the end of the block to delete
        /// </summary>
        public string TokenEnd
        {
            get { return tokenEnd; }
        }

        /// <summary>
        /// Token marking the end of the block to delete
        /// </summary>
        public string TokenReplacement
        {
            get { return replacement; }
        }

        /// <summary>
        /// Token Identifier
        /// </summary>
        public string TokenIdentifier
        {
            get { return tokenidentifier; }
        }
    }
}
#endif
