#Genetic-Algorithm_Roulette_Wheel_Selection
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic_Algorithms
{
    public class Knapsack
    {
        string s;
        string[] st;
        private Int32 capacity = 0;
        private Int32 n = 0;
        private Int32 generations = 0;
        private Int32 population_size = 0;
        private Double crossover_rate = 0.0;
        private Double mutation_rate = 0.0;
        private Double total_fitness = 0.0;
        private Int32 generation_count = 1;
        private Boolean mutation = false;
        private int crossover_count = 0;

        private List<Double> weights = new List<Double>();
        private List<Double> values = new List<Double>();
        private List<String> population = new List<string>();
        private List<Double> fitness = new List<Double>();
        private List<String> bestSolutionOfGeneration = new List<String>();
        private List<Double> meanFitnessOfGenerations = new List<Double>();
        private List<String> new_population = new List<string>();
        private List<Double> best_fitness_of_generation = new List<Double>();

        static void Main(string[] args)
        {
            Knapsack ks = new Knapsack();

        }
        public Knapsack()
        {
            this.userInput();
            this.makeKnapsack();
            this.optimization();
        }

        private void optimization()
        {
            Console.WriteLine("\nList of items to include\n");
            double best_fitness = 0.0;
            int bestGen = 0;

            for (int gen = 0; gen < this.generations-1; gen++)
            {
                if (this.best_fitness_of_generation[gen] > best_fitness)
                {
                    best_fitness = this.best_fitness_of_generation[gen];
                    bestGen = gen;
                }
            }
            String best_list = this.bestSolutionOfGeneration[bestGen];
            Console.WriteLine("Optimal List................... \n");
            Console.WriteLine(best_list);
            Console.WriteLine();
        }
        public void makeKnapsack()
        {
            this.constructPopulation();     // population for 1st generation
            Console.WriteLine("\n1st Generation : ");
            Console.WriteLine("Population: \n");
            Console.WriteLine("---------------------");
            for (Int32 i = 0; i < population_size; i++)
            {
                Console.WriteLine(this.population[i]);
            }

            this.fitnessOfPopulation();     // calculation of fitness
            Console.WriteLine("\nFitness Values: \n");
            Console.WriteLine("---------------------");
            for (Int32 i = 0; i < population_size; i++)
            {
                Console.WriteLine(this.fitness[i]);
            }

            String k=this.population[this.getBestChromosomePosition()];
            this.bestSolutionOfGeneration.Add(k);
            Console.WriteLine("\nBest Candidate Solution with greatest fitness of Generation 1 : {0}\n", k);
            Console.WriteLine("\nFitness Score = {0}\n", fitnessOfChromosome(k));

            this.meanFitnessOfGenerations.Add(this.getMeanFitnessOfGeneration());

            this.best_fitness_of_generation.Add(this.fitnessOfChromosome(this.population[this.getBestChromosomePosition()]));
            Console.WriteLine("Fitness score of best solution of 1st generation: {0}\n",this.best_fitness_of_generation[0]);

            if (this.generations > 1)
                constructFurtherGenerations();
        }

        private void constructFurtherGenerations()
        {
            for (int i = 1; i<this.generations; i++)
            {
                this.crossover_count = 0;
                this.mutation = false;
                for (int j = 0; j < this.population_size / 2; j++)
                {
                    this.newPopulation();
                }
                this.fitness.Clear();
                this.fitnessOfNewPopulation();
                this.population.Clear();

                // Copy new_population to population
                for (int k = 0; k < this.population_size; k++)
                {
                    this.population.Add(this.new_population[k]);
                }

                Console.WriteLine("\nGeneration {0}: ",(i+1));
                Console.WriteLine("Population: \n");
                Console.WriteLine("---------------------");
                for (Int32 j = 0; j < population_size; j++)
                {
                    Console.WriteLine(this.population[j]);
                }

                Console.WriteLine("\nFitness Values: \n");
                Console.WriteLine("---------------------");
                for (Int32 j = 0; j < population_size; j++)
                {
                    Console.WriteLine(this.fitness[j]);
                }

                this.new_population.Clear();
                this.bestSolutionOfGeneration.Add(this.population[this.getBestChromosomePosition()]);

                Console.WriteLine("\nBest Candidate Solution with greatest fitness of Generation {0} : {1}\n",(i+1), bestSolutionOfGeneration[i]);
                Console.WriteLine("\nFitness Score = {0}\n", fitnessOfChromosome(bestSolutionOfGeneration[i]));

                this.meanFitnessOfGenerations.Add(this.getMeanFitnessOfGeneration());

                this.best_fitness_of_generation.Add(this.fitnessOfChromosome(this.population[this.getBestChromosomePosition()]));
                Console.WriteLine("Fitness score of best solution of generation {0} : {1}\n",(i+1),this.best_fitness_of_generation[i]);

                Console.WriteLine("Crossover occured {0}",crossover_count," times");

            }
        }

        private void constructPopulation()
        {
            Int32 i;
            population.Add(constructChromosome());
            for (i = 1; i < population_size; i++)
            {
                string s = constructChromosome();
                while (s.Equals(population[i - 1]))
                    s = constructChromosome();
                population.Add(s);
            }
        }

        private String constructChromosome()
        {
            Random rnd = new Random();
            StringBuilder chromosome = new StringBuilder();
            char gene;
            Int32 i;
            for (i = 0; i < n; i++)
            {
                gene = '0';
                double r = rnd.NextDouble();
                if (r > 0.5)
                    gene = '1';
                chromosome.Append(gene);
            }
            return chromosome.ToString();
        }

        private void newPopulation()
        {
            // select 2 chromosomes for breeding
            int c1=0, c2=0;
            generation_count += 1;

            if(population_size%2==1)         // cloning best soln of previous generation
                new_population.Add(bestSolutionOfGeneration[generation_count-1]);

            // Roulette wheel selection
            while (c1 == c2)
            {
                c1 = selectChromosome();
                c2 = selectChromosome();
            }

            // perform crossover
            crossover(c1, c2);
        }

        private int selectChromosome()
        {
            Random rnd = new Random();
            Double total_fitness = 0.0;
            for (int i = 0; i < population_size; i++)
            {
                total_fitness += fitness[i];
            }
            //Double r = rnd.Next(0, (int)Math.Round(total_fitness));
            Double r = rnd.NextDouble() * total_fitness;
            Double sum = 0.0;
            for (int i = 0; i < population_size; i++)
            {
                sum += fitness[i];
                if (sum >= r)
                    return i;
            }
            return 0;
        }

        private void crossover(int c1, int c2)
        {
            String new_c1, new_c2;
            Random rnd = new Random();
            crossover_count += 1;
            int crossover_point = rnd.Next(0, n) + 1;
            new_c1 = population[c1].Substring(0, crossover_point) + population[c2].Substring(crossover_point);
            new_c2 = population[c2].Substring(0, crossover_point) + population[c1].Substring(crossover_point);
            new_population.Add(new_c1);
            new_population.Add(new_c2);

            mutateGene();
        }

        private void mutateGene()
        {
            Random rnd=new Random();
            Double rand_mutate = rnd.NextDouble();
            if (rand_mutate <= mutation_rate)
            {
                mutation = true;
                String mut_gene;
                StringBuilder new_mut_gene;
                int mut_point = 0;
                int which_chromosome = rnd.Next(0, population_size);
                // gene to be mutated is selected randomly
                if (which_chromosome <= 10)
                {
                    mut_gene = new_population[0];
                    mut_point = rnd.Next(0, n-2);
                    if (mut_gene[mut_point].Equals("1"))
                    {
                        //new_mut_gene = mut_gene.Substring(0, mut_point) + "0" + mut_gene.Substring(mut_point);
                        new_mut_gene = new StringBuilder(mut_gene);
                        new_mut_gene.Replace('1', '0');
                        new_population[0] = new_mut_gene.ToString();
                    }
                    if (mut_gene[mut_point].Equals("0"))
                    {
                        //new_mut_gene = mut_gene.Substring(0, mut_point) + "1" + mut_gene.Substring(mut_point);
                        new_mut_gene = new StringBuilder(mut_gene);
                        new_mut_gene.Replace('0', '1');
                        new_population[0] = new_mut_gene.ToString();
                    }
                }
                else
                {
                    mut_gene = new_population[1];
                    mut_point = n / 2;
                    if (mut_gene[mut_point].Equals("1"))
                    {
                        //new_mut_gene = mut_gene.Substring(0, mut_point) + "0" + mut_gene.Substring(mut_point);
                        new_mut_gene = new StringBuilder(mut_gene);
                        new_mut_gene.Replace('1', '0');
                        new_population[1] = new_mut_gene.ToString();
                    }
                    if (mut_gene[mut_point].Equals("0"))
                    {
                        //new_mut_gene = mut_gene.Substring(0, mut_point) + "1" + mut_gene.Substring(mut_point);
                        new_mut_gene = new StringBuilder(mut_gene);
                        new_mut_gene.Replace('0', '1');
                        new_population[1] = new_mut_gene.ToString();
                    }
                }
            }
        }

        private double fitnessOfChromosome(String chromosome)
        {
            Double fitness = 0.0;
            Double tw = 0.0;
            Double tv = 0.0;
            Int32 i;
            char gene;
            char []s=chromosome.ToCharArray();
            for (i = 0; i < n; i++)
            {
                gene = s[i];
                if (gene == '1')
                {
                    tw += weights[i];
                    tv += values[i];
                }
            }
            if (capacity >= tw)
                fitness = tv;
            return fitness;
        }

        private void fitnessOfPopulation()
        {
            total_fitness = 0.0;
            Double tmp = 0.0;
            for (Int32 i = 0; i < population_size; i++)
            {
                tmp = fitnessOfChromosome(population[i]);
                fitness.Add(tmp);
                total_fitness += tmp;
            }
        }

        private int getBestChromosomePosition()
        {
            int bestPos = 0,i;
            Double current_fitness = 0.0;
            Double best_fitness = 0.0;
            for (i = 0; i < population_size; i++)
            {
                current_fitness = fitnessOfChromosome(population[i]);
                if (current_fitness > best_fitness)
                {
                    best_fitness = current_fitness;
                    bestPos = i;
                }
            }
            return bestPos;
        }

        private Double getMeanFitnessOfGeneration()
        {
            Double total = 0.0;
            for (int i = 0; i < population_size; i++)
            {
                total += fitness[i];
            }
            Double m = total / population_size;
            return m;
        }

        private void fitnessOfNewPopulation()
        {
            total_fitness = 0.0;
            for (int i = 0; i < population_size; i++)
            {
                Double tmp = fitnessOfChromosome(new_population[i]);
                fitness.Add(tmp);
                total_fitness += tmp;
            }
        }

        private void userInput()
        {
            Console.WriteLine("Enter the knapsack capacity.");
            capacity = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter number of items.");
            n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the weight of items.");
            for (Int32 i = 1; i <= n; i++)
            {
                Console.WriteLine("Enter weight of item {0}",i);
                weights.Add(Convert.ToDouble(Console.ReadLine()));
            }

            Console.WriteLine("Enter the value of items.");
            for (Int32 i = 1; i <= n; i++)
            {
                Console.WriteLine("Enter value of item {0}", i);
                values.Add(Convert.ToDouble(Console.ReadLine()));
            }

            Console.WriteLine("Enter number of generations to iterate.");
            generations = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter population size for the generations");
            population_size = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter crossover rate.");
            crossover_rate = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Enter mutation rate.");
            mutation_rate = Convert.ToDouble(Console.ReadLine());
        }
    }
}
