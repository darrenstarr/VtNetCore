namespace VtNetCore.VirtualTerminal
{
    using System;

    public class TextPosition : IEquatable<TextPosition>
    {
        public int Column { get; set; }

        public int Row { get; set; }

        public TextPosition()
        {
        }

        public TextPosition(int column, int row)
        {
            Set(column, row);
        }

        public TextPosition(TextPosition other)
        {
            Set(other);
        }

        public void Set(TextPosition other)
        {
            Column = other.Column;
            Row = other.Row;
        }

        public void Set(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public TextPosition OffsetBy(int columns, int rows)
        {
            return new TextPosition
            {
                Column = Column + columns,
                Row = Row + rows
            };
        }

        public bool IsValid { get { return Row == -1; } }

        public static bool operator > (TextPosition left, TextPosition right)
        {
            return left.Row > right.Row || (left.Row == right.Row && left.Column > right.Column);
        }

        public static bool operator >= (TextPosition left, TextPosition right)
        {
            return left.Row > right.Row || (left.Row == right.Row && left.Column >= right.Column);
        }

        public static bool operator < (TextPosition left, TextPosition right)
        {
            return right.Row > left.Row || (right.Row == left.Row && right.Column > left.Column);
        }

        public static bool operator <= (TextPosition left, TextPosition right)
        {
            return right.Row > left.Row || (right.Row == left.Row && right.Column >= left.Column);
        }

        public bool Within(TextPosition start, TextPosition end)
        {
            if (start > end)
                return this >= end && this <= start;

            return this >= start && this <= end;
        }

        public bool Within(TextRange range)
        {
            if (range.RectangleMode)
                return WithinRect(range.TopLeft, range.BottomRight);

            return Within(range.Start, range.End);
        }

        public TextPosition Clone()
        {
            return new TextPosition
            {
                Column = Column,
                Row = Row
            };
        }

        public bool WithinRect(TextPosition topLeft, TextPosition bottomRight)
        {
            return
                Column >= Math.Min(topLeft.Column, bottomRight.Column) &&
                Column <= Math.Max(topLeft.Column, bottomRight.Column) &&
                Row >= Math.Min(topLeft.Row, bottomRight.Row) &&
                Row <= Math.Max(topLeft.Row, bottomRight.Row);
        }

        public bool Equals(int column, int row)
        {
            return Column == column && Row == row;
        }

        public bool Equals(TextPosition other)
        {
            if (other != null)
                return this == other;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is TextPosition)
                return this == (obj as TextPosition);

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (TextPosition left, TextPosition right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null)
                return false;

            if (right is null)
                return false;

            return
                left.Column == right.Column &&
                left.Row == right.Row;
        }

        public static bool operator !=(TextPosition left, TextPosition right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return "(c:" + Column.ToString() + ",r:" + Row.ToString() + ")";
        }
    }
}
