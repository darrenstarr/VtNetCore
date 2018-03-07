namespace VtNetCore.XTermParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class XTermInputBuffer
    {
        public byte [] Buffer { get; private set; }

        private class StreamState
        {
            public int Position { get; set; }
            public EMode Mode { get; set; }
        }

        private List<StreamState> StateStack { get; set; } = new List<StreamState>();

        public int Position { get; set; } = 0;

        public enum EMode
        {
            Raw,
            UTF8,
            USASCII,
            C0,
            UK
        };

        public EMode Mode { get; set; } = EMode.UTF8;

        public byte [] Stacked
        {
            get
            {
                var first = StateStack.First();
                return Buffer.Take(Position).Skip(first.Position).ToArray();
            }
        }

        public void Add(byte [] data)
        {
            if (Buffer == null)
                Buffer = data;
            else
                Buffer = Buffer.Concat(data).ToArray();
        }

        public void PushState()
        {
            StateStack.Add(
                new StreamState
                {
                    Position = Position,
                    Mode = Mode
                }
            );
        }

        public void PopState()
        {
            if (StateStack.Count < 1)
                throw new Exception("There are no more states to pop");

            var last = StateStack.Last();
            StateStack.RemoveAt(StateStack.Count - 1);

            Position = last.Position;
            Mode = last.Mode;
        }

        public void PopAllStates()
        {
            if (StateStack.Count < 1)
                return;

            var first = StateStack.First();
            StateStack.Clear();

            Position = first.Position;
            Mode = first.Mode;
        }

        public void RemoveTailState()
        {
            if (StateStack.Count < 1)
                throw new Exception("There are no more states to remove");

            StateStack.RemoveAt(StateStack.Count - 1);
        }

        public void RemoveHeadState()
        {
            if (StateStack.Count < 1)
                throw new Exception("There are no more states to remove");

            StateStack.RemoveAt(0);
        }

        public void Commit()
        {
            StateStack.Clear();
        }

        public void Flush()
        {
            if (StateStack.Count > 0)
                throw new Exception("The buffer should not be flushed when it is holding a state");

            Buffer = Buffer.Skip(Position).ToArray();
            Position = 0;
        }

        public bool AtEnd
        {
            get
            {
                return Buffer == null ? true : (Position >= Buffer.Length);
            }
        }

        public int Remaining
        {
            get
            {
                return Buffer == null ? 0 : (Buffer.Length - Position);
            }
        }

        public byte PeekAhead(int skip)
        {
            if (skip >= Remaining)
                throw new IndexOutOfRangeException("Buffer does not contain enough data to process request");

            return Buffer[Position + skip];
        }

        public char Read(bool utf8=false)
        {
            if(utf8)
                return ReadUtf8();
            return ReadRaw();
        }

        public char ReadRaw()
        {
            var result = (char)PeekAhead(0);
            Position++;
            return result;
        }

        public char ReadUtf8()
        {
            int first = PeekAhead(0);
            if ((first & 0x80) == 0x00)
            {
                Position++;
                return (char)first;
            }

            if ((first & 0xE0) == 0xC0)
            {
                int second = PeekAhead(1);
                if ((second & 0xC0) == 0x80)
                {
                    Position += 2;
                    return (char)(((first & 0x1F) << 6) | (second & 0x3F));
                }
            }

            if ((first & 0xF0) == 0xE0)
            {
                int second = PeekAhead(1);
                int third = PeekAhead(2);

                if (
                    ((second & 0xC0) == 0x80) &&
                    ((third & 0xC0) == 0x80)
                )
                {
                    Position += 3;
                    return (char)(((first & 0x1F) << 12) | ((second & 0x3F) << 6) | (third & 0x3F));
                }
            }

            if ((first & 0xF8) == 0xF0)
            {
                int second = PeekAhead(1);
                int third = PeekAhead(2);
                int fourth = PeekAhead(3);

                if (
                    ((second & 0xC0) == 0x80) &&
                    ((third & 0xC0) == 0x80) &&
                    ((fourth & 0xC0) == 0x80)
                )
                {
                    Position += 4;
                    return (char)(((first & 0x1F) << 18) | ((second & 0x3F) << 12) | ((third & 0x3F) << 6) | (fourth & 0x3F));
                }
            }

            throw new ArgumentException("The incoming data is not a proper UTF-8 character");
        }
    }
}
