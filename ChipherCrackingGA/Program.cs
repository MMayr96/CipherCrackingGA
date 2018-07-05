using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ChipherCrackingGA.Cipher;
using ChipherCrackingGA.Cipher.Types;
using ChipherCrackingGA.operators;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace FunctionOptimizationWithGeneticSharp
{
    /// <summary>
    /// The main class of the genetic cipher cracking problem.
    /// The genetic algorithm library used is <see cref="https://github.com/giacomelli/GeneticSharp"/>.
    /// </summary>
    class ChipherCrackingGA
    {
        public static void Main(string[] args)
        {
            string plaintextFilename = "text_sample.txt";
            string logFilename = "log.txt";
            string key = "cipherkey";

            int populationSize = 100;
            int offspring = 10;
            float mutationProbability = 0.25f;
            float crossoverProbability = 0.85f;
            int maxGenerations = 500;

            using (StreamWriter w = File.AppendText($"../../../logs/{logFilename}"))
            {
                //read in plaintext
                string plaintext = File.ReadAllText($"../../../statistics/text/{plaintextFilename}").ToLower();
                
                //init cipher and encipher plaintext
                ICipher cipher = new VigenereCipher(plaintext);
                cipher.Encipher(key);
                Console.WriteLine(cipher.CipherText);

                //init genetic alorithm operators
                //use tournament selection to minimize the likelyhood of staying in a lokal optima
                var selection = new TournamentSelection(5);
                //crossover the solution candidates by choosing a interval of the key and swap the interval
                var crossover = new TwoPointCrossover();
                //mutate the solution candidates with the own implemented mutation operator
                var mutation = new CipherMutation();
                //evaluate the fitness of the solution candidates with the own implemented fitness function
                var fitness = new CipherFitness(cipher);
                //initalize the solution candidates with the own implemented solution candidate implementation
                var chromosome = new CipherChromosome(key.Length);
                //initialize a population
                var population = new Population(populationSize, populationSize + offspring, chromosome);

                //initialize the genetic algorithm with the above initialized operators
                var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);

                //set mutation and crossover probabilities
                ga.MutationProbability = mutationProbability;
                ga.CrossoverProbability = crossoverProbability;

                //specify the algorithm termination with max generations or a fitness threshold
                ga.Termination = new OrTermination(new GenerationNumberTermination(maxGenerations), new FitnessThresholdTermination(0.99));

                var latestFitness = 0.0;

                //hang into the GenerationRan event which gets called by the library after each generation ran.
                ga.GenerationRan += (sender, e) =>
                {
                    //select the best solution candidate in the current generation
                    var bestChromosome = ga.BestChromosome as CipherChromosome;
                    //if fitness in current generation is better than fitness in previous generations 
                    if (bestChromosome.Fitness != latestFitness)
                    {
                        latestFitness = bestChromosome.Fitness.Value;
                        string genText = String.Format("Generation {0}: Best solution found is Key:{1} with {2} fitness.", ga.GenerationsNumber, bestChromosome.ToString(), bestChromosome.Fitness);
                        Console.WriteLine(genText);
                        w.WriteLine(genText);
                    }
                };
                string gaText = $"------Plaintextfile:{plaintextFilename}, Key:{key}------\n------Genetic Algorithm Settings: Populationsize:{populationSize}, Mutation:{mutationProbability}, Crossover:{crossoverProbability}------";
                Console.WriteLine(gaText);
                w.WriteLine(gaText);

                //start ga
                ga.Start();

                var finalSolution = ga.BestChromosome as CipherChromosome;
                string doneText = $"GA done in {ga.GenerationsNumber} generations.\n Best solution found is Key:{finalSolution.ToString()} (L={finalSolution.ToString().Length}) with {finalSolution.Fitness} fitness. ";

                string decipheredText = cipher.Decipher(finalSolution.ToString());
                Console.WriteLine(doneText+"\n"+decipheredText);
                w.WriteLine(doneText+"\n"+decipheredText);
            }
        }
    }
}

