﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipherCrackingGA.Cipher
{
    /// <summary>
    /// A singletion class which provides n-gram helper functions.
    /// </summary>
    public class LanguageStatisticsHelper
    {
        /// <summary>
        /// Singleton which loads uni,bi & tri-gram statistics of the known english language.
        /// </summary>
        private LanguageStatisticsHelper()
        {
            BiGramDict = LoadBiGramStatistic();
            UniGramDict = LoadUniGramStatistic();
            TriGramDict = LoadTriGramStatistic();
        }
        private static readonly LanguageStatisticsHelper _languageStatisticsHelper = new LanguageStatisticsHelper();

        public static LanguageStatisticsHelper GetLanguageStatistics()
        {
            return _languageStatisticsHelper;
        }

        //Dictionary which stores the UniGram-Frequencies of the english language
        public IDictionary<char, double> UniGramDict { get; set; }

        //Dictionary which stores the BiGram-Frequencies of the english language
        public IDictionary<string,double> BiGramDict { get; set; }
        //Dictionary which stores the TriGram-Frequencies of the english language
        public IDictionary<string,double> TriGramDict { get; set; }

        /// <summary>
        /// Loads UniGram statistic of file and transforms the absolute values to frequencies.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{char, double}"/></returns>
        public IDictionary<char, double> LoadUniGramStatistic()
        {
            return File.ReadLines("../../../statistics/ngrams/unigram.csv").Select(line => line.Split(';')).ToDictionary(line => Convert.ToChar(line[0]), line => Convert.ToDouble(line[1]) / 100);
        }

        /// <summary>
        /// Loads BiGram statistic of file and transforms the absolute values to frequencies.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{string, double}"/></returns>
        public IDictionary<string,double> LoadBiGramStatistic()
        {
            return File.ReadLines("../../../statistics/ngrams/bigram.csv").Select(line => line.Split(',')).ToDictionary(line => line[0], line => Convert.ToDouble(line[1])/2800000000000);
        }

        /// <summary>
        /// Loads TriGram statistic of file and transforms the absolute values to frequencies.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{string, double}"/></returns>
        public IDictionary<string,double> LoadTriGramStatistic()
        {
            return File.ReadLines("../../../statistics/ngrams/trigram.csv").Select(line => line.Split(',')).ToDictionary(line => line[0], line => Convert.ToDouble(line[1]) / 4500000000);
        }

        /// <summary>
        /// Creates unigram statistic of given text.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{char, double}"/></returns>
        public IDictionary<char,double> CreateUniGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count());
            return text.GroupBy(c => c).Select(c => new { Char = c.Key, Count = c.Count() }).ToDictionary(x => x.Char, x => x.Count/_len);
        }

        /// <summary>
        /// Creates bigram statistic of given text.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{string, double}"/></returns>
        public IDictionary<string,double> CreateBiGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count())/2;

            ConcurrentDictionary<string, double> biGramDict = new ConcurrentDictionary<string, double>();

            for(int i = 1; i < text.Length; i++)
            {
                char prev = text[i - 1];
                char curr = text[i];
                char[] currBlock = { prev, curr };
                string key = new string(currBlock);
                biGramDict.AddOrUpdate(key, 1, (id, count) => count + 1);
            }
            return biGramDict.ToDictionary(x => x.Key, x => x.Value / _len);
        }

        /// <summary>
        /// Creates trigram statistic of given text.
        /// </summary>
        /// <returns>A dictionary <see cref="Dictionary{string, double}"/></returns>
        public IDictionary<string,double> CreateTriGramStatistic(string text)
        {
            double _len = Convert.ToDouble(text.Count())/3;

            ConcurrentDictionary<string, double> triGramDict = new ConcurrentDictionary<string, double>();

            for(int i = 2; i < text.Length; i++)
            {
                char prevprev = text[i - 2];
                char prev = text[i - 1];
                char curr = text[i];
                char[] currBlock = { prevprev, prev, curr };
                string key = new string(currBlock);
                triGramDict.AddOrUpdate(key, 1, (id, count) => count + 1);
            }
            return triGramDict.ToDictionary(x => x.Key, x => x.Value / _len);
        }
    }
}
