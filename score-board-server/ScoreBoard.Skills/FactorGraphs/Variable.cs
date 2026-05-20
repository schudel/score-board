using System;

namespace ScoreBoard.Skills.FactorGraphs
{
    public class Variable<TValue>
    {
        private readonly string name;
        private readonly TValue prior;

        public Variable(string name, TValue prior)
        {
            this.name = "Variable[" + name + "]";
            this.prior = prior;
            ResetToPrior();
        }

        public virtual TValue Value { get; set; }

        public void ResetToPrior()
        {
            Value = prior;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class DefaultVariable<TValue> : Variable<TValue>
    {
        public DefaultVariable()
            : base("Default", default)
        {
        }

        public override TValue Value
        {
            get => default;
            set => throw new NotSupportedException();
        }
    }

    public class KeyedVariable<TKey, TValue> : Variable<TValue>
    {
        public KeyedVariable(TKey key, string name, TValue prior)
            : base(name, prior)
        {
            Key = key;
        }

        public TKey Key { get; }
    }
}