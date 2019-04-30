
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{
    public static class HtmlHelpersExtensions
    {

        // [#]([a-z_!]*)  #tag
        public static FormattedString ExtractUrlsFromString(this string text, ICommand urlCommand,ICommand commonCommand,object commonCommandParameter)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            FormattedString fs = new FormattedString();
            if (!text.Contains("http"))
            {
                Span normalSpan = new Span
                {
                    Text = text
                };
                AddCommand(normalSpan, commonCommand, commonCommandParameter);
                fs.Spans.Add(normalSpan);
                return fs;
            }

            var matchCollection = Regex.Matches(text, @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?");
            int lastPosition = 0;
            for (int i = 0; i < matchCollection.Count; i++)
            {
                Match match = matchCollection[i];

                // normal span
                if ((match.Index - 1) - lastPosition > 0)
                {
                    Span normalSpan = new Span
                    {
                        Text = text.Substring(lastPosition, match.Index - 1 - lastPosition),
                    };
                    AddCommand(normalSpan, commonCommand, commonCommandParameter);
                    fs.Spans.Add(normalSpan);
                }

                // url span
                Span urlSpan = new Span
                {
                    Text = text.Substring(match.Index, match.Length),
                    TextColor = Color.FromHex("#0645AD"),
                    TextDecorations = TextDecorations.Underline
                };
                AddCommand(urlSpan, urlCommand, match.Value);
                fs.Spans.Add(urlSpan);

                lastPosition = match.Index + match.Length;
            }

            if (lastPosition < text.Length)
            {
                Span normalSpan = new Span
                {
                    Text = text.Substring(lastPosition)
                };
                AddCommand(normalSpan, commonCommand, commonCommandParameter);
                fs.Spans.Add(normalSpan);
            }

            void AddCommand(Span span, ICommand command, object commandParam)
            {
                if (command != null)
                {
                    TapGestureRecognizer tgr = new TapGestureRecognizer
                    {
                        Command = command,
                        CommandParameter = commandParam
                    };
                    span.GestureRecognizers.Add(tgr);
                }
            }

            return fs;
    }

        private static readonly Regex UrlRegex = new Regex(@"((mailto\:|(news|(ht|f)tp(s?))\:\/\/)(?<URL>\S+))",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private static readonly Regex YouTubeUrlRegex = new Regex(@"https://(youtu.be/|www.youtube.com/watch?v=)(?<Id>\S+)");

        public static string ToHttp(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            if (!source.StartsWith("http"))
            {
                return "http://" + source;
            }

            return source;
        }

        public static string ClearFromHTMLsymbols(this string source)
        {
            var str = source.Replace("&quot;","\"");
            str = str.Replace("&nbsp;", " ");
            str = str.Replace("&laquo;", "\"");
            str = str.Replace("&raquo;", "\"");
            str = str.Replace("&ndash;", "-");
            str = str.Replace("<p>", "");
            str = str.Replace("</p>", Environment.NewLine);
            str = str.Replace("<br />", Environment.NewLine);
            return str;
        }

        /// <summary>
        /// Находит в строке урлы и обрамляет их html-тегами a
        /// </summary>
        /// <param name="str">Сырая строка с урлами</param>
        /// <returns>Строка с тегами</returns>
        public static string ParseLinks(this string str)
        {
            return UrlRegex.Replace(str, m => string.Format("<a href='{0}'>{0}</a>", m.Value));
        }

        public static string ParseYouTubeLinks(this string str)
        {
            return YouTubeUrlRegex.Replace(str, m =>
                $"<iframe width='853' height='480' src='https://www.youtube.com/embed/{m.Value}' frameborder='0' allowfullscreen></iframe>");
        }

        /// <summary>
        /// Находит в строке переводы строки и заменяет их на html-теги p
        /// </summary>
        /// <param name="str">Сырая строка с переводами строки</param>
        /// <returns>Строка с тегами</returns>
        public static string ParseParagraphs(this string str)
        {
            var builder = new StringBuilder();
            str
                .Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .ForEach(s => builder
                    .Append("<p>")
                    .Append(s)
                    .Append("</p>"));
            return builder.ToString();
        }

        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return !string.IsNullOrEmpty(lines)
                ? lines.Replace("\r\n", replacement)
                    .Replace("\n\r", replacement)
                    .Replace("\r", replacement)
                    .Replace("\n", replacement)
                : null;
        }

        /// <summary>
        /// Форматирует строку, включая парсинг ссылок, парсинг абзацев, защиту от XSS
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="content">Строка, которую нужно форматировать</param>
        /// <returns>Html-строка, отформатированная, безопасная</returns>
        /// <seealso cref="ParseLinks"/>
        /// <seealso cref="ParseParagraphs"/>
        /// <seealso cref="SafeHtml"/>
        public static string FormatAsHTML(this string html)
        {
            return html.ParseLinks().ParseParagraphs();
        }

        /// <summary>
        /// Заменяет пробелы на символ неразрывного пробела &nbsp;, чтобы строка всегда писалась в одну строку.
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="str">Строка с пробелами</param>
        /// <returns>Html-строка с неразрывными пробелами</returns>
        public static string ToMonolithString(this string html)
        {
            return html.Replace(" ", "&nbsp;");
        }

    }
}
