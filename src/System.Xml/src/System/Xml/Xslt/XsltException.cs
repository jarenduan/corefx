// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
    using Res = System.Xml.Utils.XmlUtilsRes;

#if SERIALIZABLE_DEFINED
    [Serializable]
#endif
    public class XsltException : System.Exception
    {
        private string _res;
        private string _sourceUri;
        private int _lineNumber;
        private int _linePosition;

        // message != null for V1 & V2 exceptions deserialized in Whidbey
        // message == null for created V2 exceptions; the exception message is stored in Exception._message
        private string _message;

        public XsltException() : this(string.Empty, (Exception)null) { }

        public XsltException(String message) : this(message, (Exception)null) { }

        public XsltException(String message, Exception innerException) :
            this(Res.Xml_UserException, new string[] { message }, null, 0, 0, innerException)
        {
        }

        internal static XsltException Create(string res, params string[] args)
        {
            return new XsltException(res, args, null, 0, 0, null);
        }

        internal static XsltException Create(string res, string[] args, Exception inner)
        {
            return new XsltException(res, args, null, 0, 0, inner);
        }

        internal XsltException(string res, string[] args, string sourceUri, int lineNumber, int linePosition, Exception inner)
            : base(CreateMessage(res, args, sourceUri, lineNumber, linePosition), inner)
        {
            HResult = HResults.XmlXslt;
            _res = res;
            _sourceUri = sourceUri;
            _lineNumber = lineNumber;
            _linePosition = linePosition;
        }

        public virtual string SourceUri
        {
            get { return _sourceUri; }
        }

        public virtual int LineNumber
        {
            get { return _lineNumber; }
        }

        public virtual int LinePosition
        {
            get { return _linePosition; }
        }

        public override string Message
        {
            get
            {
                return (_message == null) ? base.Message : _message;
            }
        }

        private static string CreateMessage(string res, string[] args, string sourceUri, int lineNumber, int linePosition)
        {
            try
            {
                string message = FormatMessage(res, args);
                if (res != Res.Xslt_CompileError && lineNumber != 0)
                {
                    message += " " + FormatMessage(Res.Xml_ErrorFilePosition, sourceUri, lineNumber.ToString(CultureInfo.InvariantCulture), linePosition.ToString(CultureInfo.InvariantCulture));
                }
                return message;
            }
            catch (MissingManifestResourceException)
            {
                return "UNKNOWN(" + res + ")";
            }
        }

        private static string FormatMessage(string key, params string[] args)
        {
            string message = key;
            if (message != null && args != null)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, args);
            }
            return message;
        }
    }

#if SERIALIZABLE_DEFINED
    [Serializable]
#endif
    public class XsltCompileException : XsltException
    {
        public XsltCompileException() : base() { }

        public XsltCompileException(String message) : base(message) { }

        public XsltCompileException(String message, Exception innerException) : base(message, innerException) { }

        public XsltCompileException(Exception inner, string sourceUri, int lineNumber, int linePosition) :
            base(
                lineNumber != 0 ? Res.Xslt_CompileError : Res.Xslt_CompileError2,
                new string[] { sourceUri, lineNumber.ToString(CultureInfo.InvariantCulture), linePosition.ToString(CultureInfo.InvariantCulture) },
                sourceUri, lineNumber, linePosition, inner
            )
        { }
    }
}