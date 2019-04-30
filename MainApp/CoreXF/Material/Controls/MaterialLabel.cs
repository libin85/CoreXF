using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace CoreXF
{

    public interface IMaterialLabel
    {
        string Text { get; set; }
        string FontFamily { get; set; }
        double FontSize { get; set; }
        double LineHeight { get; set; }
        MaterialTextBlock.TextContainer WordsContainer { get; set; }
        int MaxLines { get; set; }
        Color TextColor { get; set; }
        bool EnableTouchEvents { get; set; }
        MaterialGestures Gesture { get; set; }
        ICommand Command { get; set; }
        object CommandParameter { get; set; }
    }

    public class TouchAnalizer
    {
        SKTouchAction? _lastAction;
        SKPoint? _startLocation;

        public bool OnTouch(SKTouchEventArgs e, MaterialGestures gesture)
        {
            bool result = false;

            float deltaX = Math.Abs((_startLocation?.X ?? 0) - e.Location.X);
            float deltaY = Math.Abs((_startLocation?.Y ?? 0) - e.Location.Y);
            float maxDelta = Math.Max(deltaX, deltaY) * (float)Device.Info.ScalingFactor;
            //Debug.WriteLine($" OnTouch>> {e.ActionType} ");

            switch (gesture)
            {
                case MaterialGestures.PressedAndReleased:

                    if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
                    {
                        e.Handled = true;
                        //Debug.WriteLine($" OnTouch handled ({e.ActionType})");
                        if (e.ActionType == SKTouchAction.Pressed)
                        {
                            _startLocation = e.Location;
                        }
                    }
                    else if (
                       e.ActionType == SKTouchAction.Released && (
                               _lastAction == SKTouchAction.Pressed
                               ||
                               _lastAction == SKTouchAction.Moved && maxDelta < 20 * Device.Info.ScalingFactor
                            )
                        )
                    {
                        e.Handled = true;
                        result = true;
                        //Debug.WriteLine($" OnTouch PressedAndReleased ExecuteCommand move delta {maxDelta}");
                    }
                    /*
                    else
                    {
                        Debug.WriteLine(" OnTouch handled ()");
                    }
                    */
                    break;

                case MaterialGestures.Pressed:
                    if (e.ActionType == SKTouchAction.Pressed)
                    {
                        e.Handled = true;
                        //Debug.WriteLine(" OnTouch Pressed ExecuteCommand");
                        result = true;
                    }
                    break;

            }
            _lastAction = e.ActionType;

            return result;
        }
    }


    public class MaterialTextBlock : SKCanvasView, IMaterialLabel
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MaterialTextBlock), default(string));
        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MaterialTextBlock), default(string));
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MaterialTextBlock), 14d);
        public static readonly BindableProperty LineHeightProperty = BindableProperty.Create(nameof(LineHeight), typeof(double), typeof(MaterialTextBlock), default(double));
        public static readonly BindableProperty MaxLinesProperty = BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(MaterialTextBlock), default(int));
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MaterialTextBlock), Color.Black);
        //public static readonly BindableProperty GestureProperty = BindableProperty.Create(nameof(Gesture), typeof(MaterialGestures), typeof(MaterialTextBlock), MaterialGestures.Pressed);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MaterialTextBlock), default(ICommand));
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(IMaterialLabel), default(object));

        public MaterialGestures Gesture { get; set; } = MaterialGestures.PressedAndReleased;

        public static readonly BindableProperty WordsContainerProperty = BindableProperty.Create(nameof(WordsContainer), typeof(TextContainer), typeof(MaterialTextBlock), default(TextContainer));


        TouchAnalizer _touchAnalizer = new TouchAnalizer();

        public MaterialTextBlock()
        {
            BackgroundColor = Color.White;
        }


        public class CommandRegion
        {
            public SKRect rect;
            public ICommand command;
            public string text;
        }
        List<CommandRegion> _listOfCommandRegion = new List<CommandRegion>();

        protected override void OnTouch(SKTouchEventArgs e)
        {
            if (_touchAnalizer.OnTouch(e, Gesture))
            {
                CommandRegion region = _listOfCommandRegion.FirstOrDefault(x => x.rect.Contains(e.Location));
                if (region?.command != null)
                {
                    e.Handled = true;
                    if (region.command.CanExecute(region.text))
                    {
                        region.command.Execute(region.text);
                    }
                }
                else
                {
                    if (Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                    }
                }

            }
            e.Handled = true;
        }

        #region Properties

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        /*
        public MaterialGestures Gesture
        {
            get { return (MaterialGestures)GetValue(GestureProperty); }
            set { SetValue(GestureProperty, value); }
        }
        */
        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public TextContainer WordsContainer
        {
            get { return (TextContainer)GetValue(WordsContainerProperty); }
            set { SetValue(WordsContainerProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        public class TextContainer 
        {
            public string Text;
            public ProcessSettings Settings { get; set; }
            public List<LineUnit> Lines { get; set; }
        }

        public class LineUnit
        {
            public string Text { get; set; }
            public List<TextUnit> Words { get; set; } = new List<TextUnit>();
        }
       
        public class TextUnit 
        {
            public int SpacesPrefixCount;
            public int SpacesPostfixCount;
            public string Word;
            public float Width;
            public SettingsItem SettingsItem;

            public override string ToString()
            {
                return Word;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(MaxLines):
                case nameof(WordsContainer):
                case nameof(Text):
                    if (!DisableLayout)
                    {
                        InvalidateMeasure();
                        InvalidateSurface();
                    }
                    break;

                case nameof(Command):
                    if(Command != null)
                    {
                        EnableTouchEvents = true;
                    }
                    break;
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(BackgroundColor.ToSKColor());
            var area = SKRect.Create(0, 0, e.Info.Width, e.Info.Height);

            if (string.IsNullOrEmpty(Text) && WordsContainer == null)
                return;

            PrepareTextContainer();

            DrawTextBlock(this, area, WordsContainer,_listOfCommandRegion, e.Surface.Canvas);
        }

        void PrepareTextContainer()
        {
            if (WordsContainer == null)
            {
                WordsContainer = PrepareText(Text);
            }
            if(WordsContainer.Settings.DefaultPaint == null)
                WordsContainer.Settings.DefaultPaint = GetTextPaint(this);

            foreach(var elm in WordsContainer.Settings.SettingsList)
            {
                if(elm.Paint == null)
                {
                    elm.Paint = GetTextPaint(this);
                    if(elm.TextColor != Material.NoColor)
                    {
                        elm.Paint.Color = elm.TextColor.ToSKColor();
                    }
                }
            }

        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (string.IsNullOrEmpty(Text) && WordsContainer == null)
                return new SizeRequest();

            PrepareTextContainer();

            return MeasureTextBlock(this, widthConstraint, heightConstraint,WordsContainer);
        }

        public static SizeRequest MeasureTextBlock(IMaterialLabel materialLabel,double widthConstraint, double heightConstraint,TextContainer container)
        {

            float scaleFactor = (float)Device.Info.ScalingFactor;

            

            Size size = DrawTextBlock(materialLabel, new SKRect(0, 0, (float)widthConstraint * scaleFactor, (float)heightConstraint * scaleFactor), container);

            return new SizeRequest(size, size);

        }

        static bool IsEndSybmol(char c) => Char.IsSeparator(c) || Char.IsPunctuation(c);

        public class SettingsItem : IDisposable
        {
            public string RegexString;
            public ICommand Command;
            public Color TextColor = Material.NoColor;
            public TextDecorations TextDecoration;

            public SKPaint Paint;

            public void Dispose()
            {
                Paint?.Dispose();
            }

        }

        public class ProcessSettings : IDisposable
        {
            public List<SettingsItem> SettingsList = new List<SettingsItem>();
            public SKPaint DefaultPaint;

            public SettingsItem UrlParser { get; set; }
            public ProcessSettings EnableUrlParsing (ICommand command)
            {
                if (UrlParser != null)
                    return this;

                UrlParser = new SettingsItem
                {
                    RegexString = @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?",
                    TextColor = Color.FromHex("#0000EE"),
                    TextDecoration = TextDecorations.Underline,
                    Command = command
                };
                SettingsList.Add(UrlParser);
                return this;
            }

            public SettingsItem HashtagParser { get; set; }
            public ProcessSettings EnableHashtagParser(ICommand command)
            {
                if (HashtagParser != null)
                    return this;

                HashtagParser = new SettingsItem
                {
                    RegexString = @"#(\w+)",
                    TextColor = Color.FromHex("#0000EE"),
                    Command = command
                };
                SettingsList.Add(HashtagParser);
                return this;
            }

            public void Dispose()
            {
                DefaultPaint?.Dispose();

                foreach (var elm in SettingsList)
                    elm?.Dispose();
            }
        }

        class SettingMatch
        {
            public int StartIndex;
            public int EndIndex;
            public SettingsItem SettingsItem;
            public string String;
        }

        static void ProcessLine(LineUnit currentLine, ProcessSettings settings )
        {
            // Find regexes
            List<SettingMatch> matches = null;
            if (settings != null)
            {
                foreach (SettingsItem elm in settings.SettingsList)
                {
                    MatchCollection matchCollection = Regex.Matches(currentLine.Text, elm.RegexString);
                    foreach(Match match in matchCollection)
                    {
                        if(matches == null)
                             matches = new List<SettingMatch>();

                        if (string.IsNullOrEmpty(match.Value))
                            continue;

                        matches.Add(new SettingMatch
                        {
                            StartIndex = match.Index,
                            EndIndex = match.Index + match.Length,
                            String = match.Value,
                            SettingsItem = elm
                        });
                    }
                }
            }

            //var dd = text.AsSpan().Slice(2, 6);
            //var dds = new string(dd);
            // todo сделать присоединение начальных пробелов к слову если слово находится в начале физической строки
            char currentChar;
            bool isWordStarting = false;  
            int spacePrefixCount = 0;
            int spacePostfixCount = 0;
            int startWordPoz = 0;
            int lineLenght = currentLine.Text.Length;
            SettingMatch settingMatch = null;
            char[] lineSeparators = { };
            for (int i = 0; i < lineLenght; i++)
            {
                // parse matches
                if(matches != null)
                {
                    settingMatch = matches.FirstOrDefault(x => x.StartIndex == i);
                    if(settingMatch != null)
                    {
                        var wordsArr = settingMatch.String.Split(' ');
                        for(int y = 0; y < wordsArr.Length; y++)
                        {
                            AddWord(wordsArr[y], spacePrefixCount,settingMatch.SettingsItem);
                            spacePrefixCount = 0;
                            isWordStarting = false;
                        }

                        // find postfix spaces
                        spacePostfixCount = 0;
                        for(int z = settingMatch.EndIndex; z < lineLenght; z++)
                        {
                            if (!char.IsWhiteSpace(currentLine.Text[z]))
                                break;
                            spacePostfixCount++;
                        }
                        if(spacePostfixCount > 0)
                        {
                            currentLine.Words[currentLine.Words.Count - 1].SpacesPostfixCount = spacePostfixCount;
                        }

                        i = settingMatch.EndIndex - 1 + spacePostfixCount;

                        // remove processed line
                        matches.Remove(settingMatch);
                        if(matches.Count == 0)
                        {
                            matches = null;
                        }

                        continue;
                    }
                }

                currentChar = currentLine.Text[i];

                // spaces before a word
                if (currentChar == ' ' && !isWordStarting)
                {
                    spacePrefixCount++;
                    continue;
                }
                // word starts
                else if (currentChar != ' ' && !isWordStarting)
                {
                    startWordPoz = i;
                    isWordStarting = true;
                    continue;
                }
                // end of a word
                else if ((Char.IsSeparator(currentChar) || Char.IsPunctuation(currentChar) || i == (lineLenght - 1)) && isWordStarting)
                {
                    AddWord(currentLine.Text.Substring(startWordPoz, i - startWordPoz + 1), spacePrefixCount,null);

                    isWordStarting = false;
                    spacePrefixCount = 0;

                }
            }

            void AddWord(string word,int spacesPrefixCount_,SettingsItem settingsItem)
            {
                var wrd = new TextUnit
                {
                    SpacesPrefixCount = spacesPrefixCount_,
                    Word = word,
                    SettingsItem = settingsItem
                };
                currentLine.Words.Add(wrd);
            }

        }

        static public TextContainer PrepareText(string text, ProcessSettings settings = null)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            TextContainer container = new TextContainer
            {
                Text = text,
                Settings = settings
            };

            var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            container.Lines = new List<LineUnit>(lines.Length);
            LineUnit currentLine;
            for(int linecounter = 0; linecounter < lines.Length; linecounter++)
            {
                currentLine = new LineUnit
                {
                    Text = lines[linecounter]
                };
                container.Lines.Add(currentLine);

                // words
                ProcessLine(currentLine, settings);
            }

            return container;
        }

        public static SKPaint GetTextPaint(IMaterialLabel materialLabel)
        {
            float scaleFactor = (float)Device.Info.ScalingFactor;

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true,
                Color = materialLabel.TextColor.ToSKColor(),
                TextSize = (float)materialLabel.FontSize * scaleFactor
            };
            if (!string.IsNullOrEmpty(materialLabel.FontFamily))
            {
                var fnt = SKFontCache.Instance.GetOrCreateValue(materialLabel.FontFamily);
                paint.Typeface = fnt.Typeface;
            }

            // Font
            if (!string.IsNullOrEmpty(materialLabel.FontFamily))
            {
                paint.Typeface = SKFontCache.Instance.GetOrCreateValue(materialLabel.FontFamily)?.Typeface;
            }
            else
            {
                // Hack from Andrei
                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    paint.Typeface = SKFontCache.Instance.GetOrCreateValue("Default")?.Typeface;
                //}
            }
            
            return paint;
        }

        static public Size DrawTextBlock(IMaterialLabel materialLabel, SKRect area, TextContainer textContainer, List<CommandRegion> listOfCommandRegion = null, SKCanvas canvas = null)
        {
            listOfCommandRegion?.Clear();

            /*
            var container = materialLabel.WordsContainer;

            if (string.IsNullOrEmpty(materialLabel.Text) && container == null)
                return new Size();

            if(container == null)
            {
                container = PrepareText(materialLabel.Text);
                materialLabel.WordsContainer = container;
            }
             */
            
            SKPaint defaultPaint = textContainer.Settings.DefaultPaint;
            SKPaint paint = defaultPaint;

            float scaleFactor = (float)Device.Info.ScalingFactor;
            float lineHeight = materialLabel.LineHeight == 0 ? (float)materialLabel.FontSize * scaleFactor * 1.4f : (float)materialLabel.LineHeight * scaleFactor;
            var spaceWidth = paint.MeasureText(" ");

            paint.GetFontMetrics(out SKFontMetrics metrics);
            float correction = metrics.XHeight / 2 + metrics.Descent / 2;

            float ypoz = area.Top;
            float xpoz = area.Left;
            float wordsize = 0;
            float width = area.Right - area.Left;
            int lineCounter = 0;
            int maxLines = materialLabel.MaxLines;
            SettingsItem settingItem = null;
            foreach (var line in textContainer.Lines)
            {
                // new line 
                if (!AddNewLine())
                    goto Exit;

                // add leading spaces 
                if (line.Words.Count > 0)
                {
                    xpoz += line.Words[0].SpacesPrefixCount * spaceWidth;
                }

                foreach (var word in line.Words)
                {
                    paint = word.SettingsItem?.Paint ?? defaultPaint;

                    if (word.Width > 0)
                    {
                        wordsize = word.Width;
                    }
                    else
                    {
                        word.Width = wordsize = paint.MeasureText(word.Word);
                    }

                    if (wordsize > width - xpoz // here is enough space for this word
                        && !(xpoz == 0 && wordsize > width) // and word isn't longer then width control
                        ) 
                    {
                        // new line 
                        if (!AddNewLine())
                            goto Exit;
                    }

                    if(canvas != null)
                    {
                        canvas.DrawText(word.Word, xpoz, ypoz - correction, paint);

                        settingItem = word.SettingsItem;
                        if (settingItem != null)
                        {
                            // underline
                            if (settingItem.TextDecoration == TextDecorations.Underline)
                            {
                                canvas.DrawLine(xpoz, ypoz - correction / 2, xpoz + wordsize, ypoz - correction / 2, paint);
                            }

                            // commands
                            if(settingItem.Command != null)
                            {
                                listOfCommandRegion.Add(new CommandRegion
                                {
                                    text = word.Word,
                                    command = settingItem.Command,
                                    rect = new SKRect(xpoz, ypoz - lineHeight, xpoz + wordsize, ypoz)
                                });
                            }
                            
                        }
                    }


                    xpoz = xpoz + wordsize + word.SpacesPostfixCount * spaceWidth;
                }
            }

            Exit:

            if (!materialLabel.EnableTouchEvents && (listOfCommandRegion?.Count ?? 0) > 0)
            {
                materialLabel.EnableTouchEvents = true;
            }

            return new Size(width / scaleFactor, (ypoz - area.Top) / scaleFactor);

            bool AddNewLine()
            {
                if(maxLines > 0 &&  ++lineCounter > maxLines)
                {
                    return false;
                }
                ypoz += lineHeight;
                xpoz = 0;
                return true;
            }
        }


    }

}
