using System.Collections.Generic;
using System.IO;

namespace ChipherCrackingGA.cipher
{
    /// <summary>
    /// A singleton class which loads and provides a english word dicionary. 
    /// </summary>
    public class DictionaryStatisticsHelper
    {
        //singleton
        private DictionaryStatisticsHelper()
        {
            Dictionary = LoadDictionary();
        }
        private static readonly DictionaryStatisticsHelper _dictionaryStatisticsHelper = new DictionaryStatisticsHelper();

        public static DictionaryStatisticsHelper GetDictionaryStatistics()
        {
            return _dictionaryStatisticsHelper;
        }

        /// <summary>
        /// Store dictionary as a HashSet to ensure fast search.
        /// </summary>
        public HashSet<string> Dictionary { get; set; }

        /// <summary>
        /// Load english dictionary of file into HashSet.
        /// </summary>
        /// <returns>returns <see cref="HashSet{string}"/></returns>
        private HashSet<string> LoadDictionary()
        {
            string[] lines = File.ReadAllLines("../../../statistics/dictionary/words_alpha.txt");
            return new HashSet<string>(lines);
        }
    }
}
