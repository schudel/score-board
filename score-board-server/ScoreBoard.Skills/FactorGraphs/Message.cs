using System;

namespace ScoreBoard.Skills.FactorGraphs
{
    public class Message<T>
    {
        private readonly string nameFormat;
        private readonly object[] nameFormatArgs;

        public Message()
            : this(default, null, null)
        {
        }

        public Message(T value, string nameFormat, params object[] args)

        {
            this.nameFormat = nameFormat;
            nameFormatArgs = args;
            Value = value;
        }

        public T Value { get; set; }

        public override string ToString()
        {
            return nameFormat == null ? base.ToString() : String.Format(nameFormat, nameFormatArgs);
        }
    }
}