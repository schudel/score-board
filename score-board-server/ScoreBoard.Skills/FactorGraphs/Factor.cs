using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScoreBoard.Skills.FactorGraphs
{    
    public abstract class Factor<TValue>        
    {
        private readonly List<Message<TValue>> messages = new List<Message<TValue>>();

        private readonly Dictionary<Message<TValue>, Variable<TValue>> messageToVariableBinding =
            new Dictionary<Message<TValue>, Variable<TValue>>();

        private readonly string name;
        private readonly List<Variable<TValue>> variables = new List<Variable<TValue>>();

        protected Factor(string name)
        {
            this.name = "Factor[" + name + "]";
        }

        /// Returns the log-normalization constant of that factor
        public virtual double LogNormalization => 0;

        /// Returns the number of messages that the factor has
        public int NumberOfMessages => messages.Count;

        protected ReadOnlyCollection<Variable<TValue>> Variables => variables.AsReadOnly();

        protected ReadOnlyCollection<Message<TValue>> Messages => messages.AsReadOnly();

        /// Update the message and marginal of the i-th variable that the factor is connected to
        public virtual double UpdateMessage(int messageIndex)
        {
            Guard.ArgumentIsValidIndex(messageIndex, messages.Count, "messageIndex");
            return UpdateMessage(messages[messageIndex], messageToVariableBinding[messages[messageIndex]]);
        }

        protected virtual double UpdateMessage(Message<TValue> message, Variable<TValue> variable)
        {
            throw new NotImplementedException();
        }

        /// Resets the marginal of the variables a factor is connected to
        public virtual void ResetMarginals()
        {
            foreach (var currentVariable in messageToVariableBinding.Values)
            {
                currentVariable.ResetToPrior();
            }
        }

        /// Sends the ith message to the marginal and returns the log-normalization constant
        public virtual double SendMessage(int messageIndex)
        {
            Guard.ArgumentIsValidIndex(messageIndex, messages.Count, "messageIndex");

            Message<TValue> message = messages[messageIndex];
            Variable<TValue> variable = messageToVariableBinding[message];
            return SendMessage(message, variable);
        }

        protected abstract double SendMessage(Message<TValue> message, Variable<TValue> variable);

        //public abstract Message<TValue> CreateVariableToMessageBinding(Variable<TValue> variable);

        protected Message<TValue> CreateVariableToMessageBinding(Variable<TValue> variable, Message<TValue> message)
        {
            //int index = messages.Count;
            messages.Add(message);
            messageToVariableBinding[message] = variable;
            variables.Add(variable);

            return message;
        }

        public override string ToString()
        {
            return name ?? base.ToString();
        }
    }
}