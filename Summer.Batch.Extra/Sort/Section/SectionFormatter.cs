using Summer.Batch.Extra.Sort.Legacy.Accessor;
using Summer.Batch.Extra.Sort.Legacy.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Summer.Batch.Extra.Sort.Section
{
    class SectionFormatter<T> : AbstractParser
    {

        private static readonly string skip = "SKIP=";
        private static readonly string header3 = "HEADER3=";
        private static readonly string trailer3 = "TRAILER3=";
        
        private static readonly Regex DefaultEmptyRegex = new Regex("");
         /// <summary>
        /// The encoding of the records. Default is <see cref="System.Text.Encoding.Default"/>.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SectionFormatter()
        {
            Encoding = Encoding.Default;
        }
        public ISection<string> ParseSection(string configuration, Encoding encoding)
        {
            ISection<string> section = null;
            
            var lexer = new Lexer(configuration);
            
            if (lexer.MoveNext())
            {
                var parentheses = lexer.Current == OpeningPar;
                if (parentheses)
                {
                    lexer.MoveNext();
                }
                section = ParseSection(lexer, encoding);
                if (parentheses)
                {
                    lexer.Parse(ClosingPar);
                }
            }
            return section;
        }

        private ISection<string> ParseSection(Lexer lexer, Encoding encoding)
        {
            ISection<string> section = new ISection<string>();
            int start = lexer.ParseInt();
            int length = lexer.ParseInt();
            section.accessor = (IAccessor<string>)GetAccessor(start, length, "CH", encoding);
            string nextElement = lexer.Parse();
            if (null != nextElement && nextElement.IndexOf(skip, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                int lines = Convert.ToInt16(nextElement.ToUpper().Replace(skip, "").Replace("L", ""));
                section.skipLines = lines;
                nextElement = lexer.Parse();
            }
            if (null != nextElement && nextElement.IndexOf(header3, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                String info = lexer.Current.Replace(header3, "");
                String headerContent = ExtractElement(lexer, info);
                section.header3 = headerContent;
                nextElement = lexer.Parse();
            }
            if (null != nextElement && nextElement.IndexOf(trailer3, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                String info = lexer.Current.Replace(trailer3, "");
                String trailerContent = ExtractElement(lexer, info);
                section.trailer3 = trailerContent;
            }
            return section;
        }

        public string ParseElement(string configuration, Encoding encoding)
        {
            string elementContent = "";
            var lexer = new Lexer(configuration);
            if (lexer.MoveNext())
            {
                var parentheses = lexer.Current == OpeningPar;
                if (parentheses)
                {
                    lexer.MoveNext();
                }
                elementContent = ExtractElement(lexer, "");
                if (parentheses)
                {
                    lexer.Parse(ClosingPar);
                }
            }
            return elementContent;
        }

    }
}
