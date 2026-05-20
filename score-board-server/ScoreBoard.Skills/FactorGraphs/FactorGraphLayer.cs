using System;
using System.Collections.Generic;

namespace ScoreBoard.Skills.FactorGraphs
{
    public abstract class FactorGraphLayerBase<TValue>
    {
        public abstract IEnumerable<Factor<TValue>> UntypedFactors { get; }
        public abstract void BuildLayer();

        public virtual Schedule CreatePriorSchedule()
        {
            return null;
        }

        public virtual Schedule CreatePosteriorSchedule()
        {
            return null;
        }

        // HACK

        public abstract void SetRawInputVariablesGroups(object value);
        public abstract object GetRawOutputVariablesGroups();
    }

    public abstract class FactorGraphLayer<TParentGraph, TValue, TBaseVariable, TInputVariable, TFactor, TOutputVariable>
        : FactorGraphLayerBase<TValue>
        where TParentGraph : FactorGraph<TParentGraph, TValue, TBaseVariable>
        where TBaseVariable : Variable<TValue>
        where TInputVariable : TBaseVariable
        where TFactor : Factor<TValue>
        where TOutputVariable : TBaseVariable
    {
        private readonly List<TFactor> localFactors = new List<TFactor>();
        private readonly List<IList<TOutputVariable>> outputVariablesGroups = new List<IList<TOutputVariable>>();

        protected FactorGraphLayer(TParentGraph parentGraph)
        {
            ParentFactorGraph = parentGraph;
        }

        protected IList<IList<TInputVariable>> InputVariablesGroups { get; private set; } = new List<IList<TInputVariable>>();

        // HACK

        public TParentGraph ParentFactorGraph { get; }

        public IList<IList<TOutputVariable>> OutputVariablesGroups => outputVariablesGroups;

        public IList<TFactor> LocalFactors => localFactors;

        public override IEnumerable<Factor<TValue>> UntypedFactors => localFactors;

        public override void SetRawInputVariablesGroups(object value)
        {
            if (!(value is IList<IList<TInputVariable>> newList))
            {
                // TODO: message
                throw new ArgumentException();
            }

            InputVariablesGroups = newList;
        }

        public override object GetRawOutputVariablesGroups()
        {
            return outputVariablesGroups;
        }

        protected Schedule ScheduleSequence<TSchedule>(
            IEnumerable<TSchedule> itemsToSequence,
            string nameFormat,
            params object[] args)
            where TSchedule : Schedule

        {
            string formattedName = String.Format(nameFormat, args);
            return new ScheduleSequence<TValue, TSchedule>(formattedName, itemsToSequence);
        }

        protected void AddLayerFactor(TFactor factor)
        {
            localFactors.Add(factor);
        }

        // Helper utility
        protected double Square(double x)
        {
            return x*x;
        }
    }
}