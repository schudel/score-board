using System;

namespace ScoreBoard.Skills.FactorGraphs
{
    public class VariableFactory<TValue>
    {
        // using a Func<TValue> to encourage fresh copies in case it's overwritten
        private readonly Func<TValue> variablePriorInitializer;

        public VariableFactory(Func<TValue> variablePriorInitializer)
        {
            this.variablePriorInitializer = variablePriorInitializer;
        }

        public Variable<TValue> CreateBasicVariable(string nameFormat, params object[] args)
        {
            var newVar = new Variable<TValue>(
                String.Format(nameFormat, args),                                
                variablePriorInitializer());

            return newVar;
        }

        public KeyedVariable<TKey, TValue> CreateKeyedVariable<TKey>(TKey key, string nameFormat, params object[] args)
        {
            var newVar = new KeyedVariable<TKey, TValue>(
                key,
                String.Format(nameFormat, args),                                
                variablePriorInitializer());
            
            return newVar;
        }
    }
}