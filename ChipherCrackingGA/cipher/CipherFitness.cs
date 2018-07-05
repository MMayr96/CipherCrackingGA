using ChipherCrackingGA.cipher;
using ChipherCrackingGA.Cipher.Types;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;

namespace ChipherCrackingGA.Cipher
{
    /// <summary>
    /// Evaluates a solution candidate.
    /// </summary>
    class CipherFitness : IFitness
    {
        public ICipher Cipher { get; set; }

        /// <summary>
        /// Constructor injection of the used cipher <see cref="ICipher"/>
        /// </summary>
        /// <param name="cipher">Used cipher</param>
        public CipherFitness(ICipher cipher)
        {
            this.Cipher = cipher;
        }

        /// <summary>
        /// The fitness function of the genetic algorithm.
        /// Gets called by the <see cref="GeneticSharp"/> library to evaluate a solution candidate.
        /// Consists of two independent fitness functions <see cref="languageStatisticFitness(string)"/> & <see cref="dictionaryStatisticFitness(string)"/>.
        /// The above mentioned functions try to rate the quality of the attempted decipher of the ciphertext with the key of the given chromosome.
        /// </summary>
        /// <param name="chromosome">The solution candidate to be evaluated.</param>
        /// <returns>A double fitness value between 0(bad) and 1(perfect).</returns>
        public double Evaluate(IChromosome chromosome)
        {
            string decryptedText = Cipher.Decipher((chromosome as CipherChromosome).ToString()).ToLower();

            double langStat = languageStatisticFitness(decryptedText);
            double dictStat = dictionaryStatisticFitness(decryptedText);

            return (langStat+dictStat)/12.521;
        }

        /// <summary>
        /// Uses n-grams (uni,bi,tri) to calculate a language frequency similarity
        /// of the attempted decipher and the common english language <see cref="LanguageStatisticsHelper"/>.
        /// </summary>
        /// <param name="decryptedText">Attempted deciphered text.</param>
        /// <returns>A double fitness value between 0 and 1.</returns>
        private double languageStatisticFitness(string decryptedText)
        {            
            var statisticHelper = LanguageStatisticsHelper.GetLanguageStatistics();
            var cipherUD = statisticHelper.CreateUniGramStatistic(decryptedText);
            var cipherBD = statisticHelper.CreateBiGramStatistic(decryptedText);
            var cipherTD = statisticHelper.CreateTriGramStatistic(decryptedText);
            var nativUD = statisticHelper.UniGramDict;
            var nativBD = statisticHelper.BiGramDict;
            var nativTD = statisticHelper.TriGramDict;

            double alpha = 0.3;
            double beta = 0.3;
            double gamma = 0.3;

            double uniGramProb = 0.0;
            double biGramProb = 0.0;
            double triGramProb = 0.0;

            for (char c = 'a'; c <= 'z'; c++)
            {
                double cNativ;
                double cCipher;
                nativUD.TryGetValue(c, out cNativ);
                cipherUD.TryGetValue(c, out cCipher);
                //add unigram 
                uniGramProb += Math.Abs(cNativ - cCipher);
                
                //bigram 
                for (char c1 = 'a'; c1 <= 'z'; c1++)
                {
                    double cNativA;
                    double cCipherA;
                    char[] blockC = {c,c1};
                    string blockS = new string(blockC);
                    nativBD.TryGetValue(blockS, out cNativA);
                    cipherBD.TryGetValue(blockS, out cCipherA);
                    //add bigram prob
                    biGramProb += Math.Abs(cNativA - cCipherA);

                    //trigram
                    for(char c2 = 'a'; c2 <= 'z'; c2++)
                    {
                        double cNativB;
                        double cCipherB;
                        char[] blockD = { c, c1, c2 };
                        string blockF = new string(blockD);
                        nativTD.TryGetValue(blockF, out cNativB);
                        cipherTD.TryGetValue(blockF, out cCipherB);
                        //add trigram prob
                        triGramProb += Math.Abs(cNativB - cCipherB);
                    }            
                }
            }
            return alpha * uniGramProb + beta * biGramProb + gamma * triGramProb;
        }

        /// <summary>
        /// Uses a dictionary to recognize known english words in the attempted deciphered text <see cref="DictionaryStatisticsHelper"/>.
        /// </summary>
        /// <param name="decryptedText">Attempted deciphered text.</param>
        /// <returns>A double fitness value between 0 and 12 (weigh the recognition of words more).</returns>
        private double dictionaryStatisticFitness(string decryptedText)
        {
            var helper = DictionaryStatisticsHelper.GetDictionaryStatistics();
            string[] words = decryptedText.Split();
            int wCount = words.Length;
            int kCount = 0;
            foreach(string word in words)
            {
                if (helper.Dictionary.Contains(word))
                {
                    kCount++;
                }
            }
            double prob = (double)kCount / wCount;
            return prob*12;
        }
    }
}
