using System.Collections.Generic;
using System.Linq;

namespace ScoreBoard.Skills.FactorGraphs
{
    /// <summary>
    /// Helper class for computing the factor graph's normalization constant.
    /// </summary>    
    public class FactorList<TValue>
    {        
        private readonly List<Factor<TValue>> list = new List<Factor<TValue>>();

        public double LogNormalization
        {
            get
            {                
                list.ForEach(f => f.ResetMarginals());

                double sumLogZ = 0.0;
                                
                foreach (var f in list)
                {
                    for (int j = 0; j < f.NumberOfMessages; j++)
                    {
                        sumLogZ += f.SendMessage(j);
                    }
                }
                                
                double sumLogS = list.Aggregate(0.0, (acc, fac) => acc + fac.LogNormalization);

                return sumLogZ + sumLogS;
            }
        }

        public int Count => list.Count;

        public Factor<TValue> AddFactor(Factor<TValue> factor)
        {
            list.Add(factor);
            return factor;
        }
    }
}