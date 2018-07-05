using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using System;

namespace ChipherCrackingGA.operators
{
    /// <summary>
    /// The main mutation class of the genetic algorithm. Overrides some methods of the base mutation class <see cref="MutationBase"/>
    /// </summary>
    public class CipherMutation : MutationBase
    {
        //random helper variable
        private readonly IRandomization m_rnd;

        public CipherMutation()
        {
            m_rnd = RandomizationProvider.Current;
        }

        /// <summary>
        /// Gets called by the <see cref="GeneticSharp"/> library when a mutation of a solution candidate is performed.
        /// Consists of two mutation operators <see cref="MutateByOne(IChromosome)"/> & <see cref="MutateByCharacterFlip(IChromosome)"/>
        /// </summary>
        /// <param name="chromosome">The solution candidate to mutate.</param>
        /// <param name="probability">The probability of a mutation.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            var genesLength = chromosome.Length;

            if(m_rnd.GetDouble() <= probability)
            {
                MutateByOne(chromosome);          
            }
            if(m_rnd.GetDouble() <= probability)
            {
                MutateByCharacterFlip(chromosome);
            }
        }

        /// <summary>
        /// Selects a random gene in the solution candidate and shifts the value by -1 or +1.
        /// </summary>
        /// <param name="chromosome">The solution candidate.</param>
        private void MutateByOne(IChromosome chromosome)
        {
            var randIdx = m_rnd.GetInt(0, chromosome.Length);
            var gene = chromosome.GetGenes()[randIdx];
            var odd = m_rnd.GetInt(0, 2);

            if(odd == 0 && (Convert.ToInt32(gene.Value)+1 <= 25))
            {
                chromosome.ReplaceGene(randIdx, new Gene(Convert.ToInt32(gene.Value) + 1));
            }
            else
            {
                if(Convert.ToInt32(gene.Value)-1 >= 0)
                {
                    chromosome.ReplaceGene(randIdx, new Gene(Convert.ToInt32(gene.Value) - 1));
                }
                else
                {
                    chromosome.ReplaceGene(randIdx, new Gene(Convert.ToInt32(gene.Value) + 1));
                }            
            }
        }

        /// <summary>
        /// Selects a random gene in the solution candidate and replaces it with a random generated gene.
        /// </summary>
        /// <param name="chromosome">The solution candidate.</param>
        private void MutateByCharacterFlip(IChromosome chromosome)
        {
            int randIdx = RandomizationProvider.Current.GetInt(0, chromosome.Length);
            chromosome.GenerateGene(randIdx);
        }
    }
}
