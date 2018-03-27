namespace VtNetCore.VirtualTerminal
{
    using System;

    public class TextRange : IEquatable<TextRange>
    {
        public TextPosition Start { get; set; } = new TextPosition();
        public TextPosition End { get; set; } = new TextPosition();

        public bool RectangleMode { get; set; }

        public bool Contains(TextPosition position)
        {
            if (RectangleMode)
                position.WithinRect(TopLeft, BottomRight);

            return position.Within(Start, End);
        }

        public bool Contains(int column, int row)
        {
            if(RectangleMode)
                return (new TextPosition { Column = column, Row = row }).WithinRect(TopLeft, BottomRight);

            return (new TextPosition { Column = column, Row = row }).Within(Start, End);
        }

        public static bool operator ==(TextRange left, TextRange right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return
                left.Start == right.Start &&
                left.End == right.End &&
                left.RectangleMode == right.RectangleMode;
        }

        public static bool operator !=(TextRange left, TextRange right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is TextRange && obj != null)
                return this == (obj as TextRange);

            return false;
        }

        public bool Equals(TextRange other)
        {
            if (other != null)
                return this == other;

            return false;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() & End.GetHashCode();
        }

        public override string ToString()
        {
            return Start.ToString() + "-" + End.ToString();
        }

        public TextPosition TopLeft
        {
            get
            {
                if (RectangleMode)
                {
                    return new TextPosition
                    {
                        Column = Math.Min(Start.Column, End.Column),
                        Row = Math.Min(Start.Row, End.Row)
                    };
                }
                else
                {
                    if (Start <= End)
                        return Start;

                    return End;
                }
            }
        }

        public TextPosition BottomRight
        {
            get
            {
                if (RectangleMode)
                {
                    return new TextPosition
                    {
                        Column = Math.Max(Start.Column, End.Column),
                        Row = Math.Max(Start.Row, End.Row)
                    };
                }
                else
                {
                    if (Start >= End)
                        return Start;

                    return End;
                }
            }
        }
    }
}
