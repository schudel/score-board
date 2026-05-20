using System;
using System.Collections.Generic;

namespace ScoreBoard.Skills.FactorGraphs
{
    public abstract class Schedule
    {
        private readonly string name;

        protected Schedule(string name)
        {
            this.name = name;
        }

        public abstract double Visit(int depth, int maxDepth);

        public double Visit()
        {
            return Visit(-1, 0);
        }
                
        public override string ToString()
        {
            return name;
        }
    }

    public class ScheduleStep<T> : Schedule
    {
        private readonly Factor<T> factor;
        private readonly int index;

        public ScheduleStep(string name, Factor<T> factor, int index)
            : base(name)
        {
            this.factor = factor;
            this.index = index;
        }

        public override double Visit(int depth, int maxDepth)
        {
            double delta = factor.UpdateMessage(index);
            return delta;
        }
    }
        
    public class ScheduleSequence<TValue> : ScheduleSequence<TValue, Schedule>
    {
        public ScheduleSequence(string name, IEnumerable<Schedule> schedules)
            : base(name, schedules)
        {
        }
    }

    public class ScheduleSequence<TValue, TSchedule> : Schedule
        where TSchedule : Schedule
    {
        private readonly IEnumerable<TSchedule> schedules;

        public ScheduleSequence(string name, IEnumerable<TSchedule> schedules)
            : base(name)
        {
            this.schedules = schedules;
        }

        public override double Visit(int depth, int maxDepth)
        {
            double maxDelta = 0;

            foreach (TSchedule currentSchedule in schedules)
            {
                maxDelta = Math.Max(currentSchedule.Visit(depth + 1, maxDepth), maxDelta);
            }
            
            return maxDelta;
        }
    }

    public class ScheduleLoop : Schedule
    {
        private readonly double maxDelta;
        private readonly Schedule scheduleToLoop;

        public ScheduleLoop(string name, Schedule scheduleToLoop, double maxDelta)
            : base(name)
        {
            this.scheduleToLoop = scheduleToLoop;
            this.maxDelta = maxDelta;
        }

        public override double Visit(int depth, int maxDepth)
        {
            double delta = scheduleToLoop.Visit(depth + 1, maxDepth);
            while (delta > maxDelta)
            {
                delta = scheduleToLoop.Visit(depth + 1, maxDepth);
            }

            return delta;
        }
    }
}