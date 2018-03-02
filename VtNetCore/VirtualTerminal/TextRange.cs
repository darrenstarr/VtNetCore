namespace VtNetCore.VirtualTerminal
{
    using System;

    public class TextRange : IEquatable<TextRange>
    {
        public TextPosition Start { get; set; } = new TextPosition();
        public TextPosition End { get; set; } = new TextPosition();

        public bool Within(TextPosition position)
        {
            return position.Within(Start, End);
        }

        public bool Within(int column, int row)
        {
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
                left.End == right.End;
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
    }
}
