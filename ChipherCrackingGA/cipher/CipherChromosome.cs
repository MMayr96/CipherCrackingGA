using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;

namespace ChipherCrackingGA.Cipher
{
    /// <summary>
    /// A solution candidate used in the genetic algorithm. 
    /// Base class is the abstract class <see cref="ChromosomeBase"/>.
    /// The solution candidates main purpose is to store and manipulate a key used to decipher the ciphertext. 
    /// A key is represented as an array of <see cref="Gene"/> where each Gene stores an integer value corresponding to the character position of the alphabeth.
    /// <example>Key: "abc" -> genes[0]=0; genes[1]=1; genes[2]=2;</example>
    /// </summary>
    class CipherChromosome : ChromosomeBase
    {
        private int _keyLength;
        /// <summary>
        /// Initialize a random solution candidate.
        /// </summary>
        /// <param name="keyLength">The length of the solution candidate.</param>
        public CipherChromosome(int keyLength) : base(keyLength)
        {
            _keyLength = keyLength;

            for(int i = 0; i < keyLength; i++)
            {
                GenerateGene(i);
            }
        }

        /// <summary>
        /// Overrides the base implementation of the creation of a new solution candidate.
        /// </summary>
        /// <returns>A new <see cref="CipherChromosome"/></returns>
        public override IChromosome CreateNew()
        {
            return new CipherChromosome(_keyLength);
        }

        /// <summary>
        /// Generates a new random integer <see cref="Gene"/>.
        /// We can generate a Gene at a specific location
        /// </summary>
        /// <param name="geneIndex"></param>
        /// <returns>A new <see cref="Gene"/></returns>
        public override Gene GenerateGene(int geneIndex)
        {
            var gene = new Gene(RandomizationProvider.Current.GetInt(0, 25));
            ReplaceGene(geneIndex, gene);
            return gene;   
        }
        
        /// <summary>
        /// Converts the integer representation of the key to a string representation. 
        /// <example>[0,1,2]="abc"</example>
        /// </summary>
        /// <returns>The key as a string.</returns>
        public override string ToString()
        {
            char[] keys = new char[_keyLength];
            int i = 0;
            foreach (Gene g in GetGenes())
            {
                keys[i] = Convert.ToChar(((int)g.Value) + 97);
                i++;
            }
            return new string(keys);
        }  
    }
}
