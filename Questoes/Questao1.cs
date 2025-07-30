using System.Text;
using src.Models;
using src.Scripts;

namespace src.Questoes;

public class Questao1
{
    private static readonly Random rnd = new();
    private static readonly int TAMANHO_DA_POPULACAO = 10;
    private static readonly int TAMANHO_DO_INDIVIDUO = alvo.Length;
    private static readonly double TAXA_DE_MUTACAO = 0.5f;
    private const string alvo = "01010101";

    public static void PrintarPopulacao(IndividuoInt[] pop, int geracao)
    {
        Console.WriteLine($"Geracao {geracao}, com a media de fitness da populacao: " + pop.Average(pop => pop.Fitness));
        Console.WriteLine("--------------");
        for (int i = TAMANHO_DA_POPULACAO - 1; i >= 0; i--)
        {
            Console.WriteLine($"Individuo {pop[i].Binario}, fitness = {pop[i].Fitness} ");
        }

        Console.WriteLine("--------------");
    }
    public static IndividuoInt[] Popular()
    {
        IndividuoInt[] pop = new IndividuoInt[TAMANHO_DA_POPULACAO];

        for (int i = 0; i < pop.Length; i++)
        {
            StringBuilder sb = new(TAMANHO_DO_INDIVIDUO);

            for (int j = 0; j < TAMANHO_DO_INDIVIDUO; j++)
            {
                int corte = rnd.Next(2);
                sb.Append((corte > 0) ? '1' : '0');
            }
            pop[i].Binario = sb.ToString();
        }

        return pop;
    }

    public static void AvaliarFitness(IndividuoInt[] pop)
    {
        for (int i = 0; i < TAMANHO_DA_POPULACAO; i++)
        {
            int score = 0;

            for (int j = 0; j < TAMANHO_DO_INDIVIDUO; j++)
            {
                if (pop[i].Binario[j] == alvo[j])
                    score++;
            }

            pop[i].Fitness = score;   
        }
    }

    public static IndividuoInt Crossover(IndividuoInt pai1, IndividuoInt pai2)
    {
        int pontoDeCorte = rnd.Next(0, TAMANHO_DO_INDIVIDUO);
        
        string primeiraParte = pai1.Binario[..pontoDeCorte];
        string segundaParte = pai2.Binario[pontoDeCorte..];

        StringBuilder filhoString = new(primeiraParte + segundaParte);

        for (int i = 0; i < TAMANHO_DO_INDIVIDUO; i++)
        {
            if (rnd.NextDouble() < TAXA_DE_MUTACAO)
                filhoString[i] = (filhoString[i] == '0') ? '1' : '0';
        }

        IndividuoInt filho = new()
        {
            Binario = filhoString.ToString()
        };

        return filho;
    }

    public static void AlgoritmoGenetico(IndividuoInt[] pop)
    {
        int geracao = 0;
        AvaliarFitness(pop);
        QuickSort.ExecutarInt(pop, 0, TAMANHO_DA_POPULACAO - 1);

        while (!pop.Any(individuo => individuo.Binario == alvo)) // Enquanto o individuo nao for o alvo, continue o while.
        {
            geracao++;
            IndividuoInt[] novaPopulacao = new IndividuoInt[TAMANHO_DA_POPULACAO];

            novaPopulacao[0] = pop.Last(); // Salvando o melhor individuo, com  o melhor fitness

            for (int i = 1; i < TAMANHO_DA_POPULACAO; i++)
            {

                IndividuoInt pai1 = Selecao.SelecaoPorRoleta(pop);
                IndividuoInt pai2 = Selecao.SelecaoPorRoleta(pop);

                while (pai1.Binario == pai2.Binario)
                    pai2 = Selecao.SelecaoPorRoleta(pop);

                novaPopulacao[i] = Crossover(pai1, pai2);
            }

            pop = novaPopulacao;

            AvaliarFitness(pop);
            QuickSort.ExecutarInt(pop, 0, TAMANHO_DA_POPULACAO - 1);

            PrintarPopulacao(pop, geracao);
        }
        Console.WriteLine();
        Console.WriteLine("x-----x-----x");
        Console.WriteLine();
        Console.WriteLine("Algoritmo Finalizado!, encontrado o individuo perfeito!");

        AvaliarFitness(pop);
        QuickSort.ExecutarInt(pop, 0, TAMANHO_DA_POPULACAO - 1);
        PrintarPopulacao(pop, geracao);
    }
    public static void Executar()
    {
        AlgoritmoGenetico(Popular());
    }

}