using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using src.Models;
using src.Scripts;

namespace src.Questoes;

public class Questao2
{
    public const int TAMANHO_POPULACAO = 10;
    public const int NIVEL_DA_RAIZ = 2;
    public const int TAMANHO_DA_PALAVRA = 5;
    const double TOLERANCIA_FITNESS = 1e-9;
    public static readonly Random rnd = new();
    public const string equacao = "x^2 - 3x + 2 = 0";
    public const double TAXA_DE_MUTACAO = 0.05f;

    public struct Individuo
    {
        public string Binario { get; set; }
        public double Fitness { get; set; }
        public double Value { get; set; }
    }
    public struct Coeficientes
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
    }

    public static Coeficientes ExtrairCoeficientes(string equacaoStr)
    {
        equacaoStr = equacaoStr.Replace(" ", "").Split("=")[0];

        double a = 0, b = 0, c = 0;

        var matchA = Regex.Match(equacaoStr, @"([+-]?\d*\.?\d*)x\^2");
        if (matchA.Success)
        {
            string coeffStr = matchA.Groups[1].Value;
            if (coeffStr == "" || coeffStr == "+") a = 1;
            else if (coeffStr == "-") a = -1;
            else double.TryParse(coeffStr, out a);

            equacaoStr = equacaoStr.Replace(matchA.Value, "");
        }

        var matchB = Regex.Match(equacaoStr, @"([+-]?\d*\.?\d*)x(?![\^])");
        if (matchB.Success)
        {
            string coeffStr = matchB.Groups[1].Value;
            if (coeffStr == "" || coeffStr == "+") b = 1;
            else if (coeffStr == "-") b = -1;
            else double.TryParse(coeffStr, out b);

            equacaoStr = equacaoStr.Replace(matchB.Value, "");
        }

        if (!string.IsNullOrEmpty(equacaoStr))
        {
            double.TryParse(equacaoStr, out c);
        }

        Console.WriteLine($"Coeficientes extraídos: a = {a}, b = {b}, c ={c}");
        return new Coeficientes { A = a, B = b, C = c };
    }

    public static void PrintarPopulacao(Individuo[] pop, int geracao)
    {
        Console.WriteLine($"Geracao {geracao}, com a media de fitness da populacao: " + pop.Average(pop => pop.Fitness));
        Console.WriteLine("--------------");
        for (int i = TAMANHO_POPULACAO - 1; i >= 0; i--)
        {
            Console.WriteLine($"Individuo {pop[i].Binario}, ({pop[i].Value}) fitness = {pop[i].Fitness} ");
        }

        Console.WriteLine("--------------");
    }

    public static Individuo[] Popular()
    {
        Individuo[] pop = new Individuo[TAMANHO_POPULACAO];
        string palavra;

        for (int i = 0; i < TAMANHO_POPULACAO; i++)
        {
            palavra = "";
            for (int j = 0; j < TAMANHO_DA_PALAVRA; j++)
            {
                int corte = rnd.Next(2);
                palavra += corte > 0 ? '1' : '0';
            }
            pop[i].Binario = palavra;
        }

        return pop;
    }
    public static void Decodificar(Individuo[] pop)
    {

        for (int i = 0; i < TAMANHO_POPULACAO; i++)
        {
            double score = 0;
            for (int j = 0; j < TAMANHO_DA_PALAVRA; j++)
            {
                if (pop[i].Binario[j] == '1')
                    score += Math.Pow(2, j);
            }

            if (pop[i].Binario[TAMANHO_DA_PALAVRA - 1] == '1' && score != 0)
                score *= -1;

            pop[i].Value = score;
        }
    }

    public static void AvaliarFitness(Individuo[] pop, Coeficientes coeficientes, List<double> raizes)
    {
        const double raioDeRepulsao = 0.5;

        for (int i = 0; i < TAMANHO_POPULACAO; i++)
        {
            // --- Passo 1: Calcular o "Fitness Base" ---
            double resultadoEquacao = (Math.Pow(pop[i].Value, 2) * coeficientes.A) + (pop[i].Value * coeficientes.B) + coeficientes.C;
            double fitnessAbsoluto = Math.Abs(resultadoEquacao);

            // Adiciona à lista de raízes se for uma solução nova e válida
            if (fitnessAbsoluto < TOLERANCIA_FITNESS && !raizes.Any(r => Math.Abs(r - pop[i].Value) < TOLERANCIA_FITNESS))
            {
                raizes.Add(pop[i].Value);
            }

            double baseFitness = 1.0 / (fitnessAbsoluto + TOLERANCIA_FITNESS);

            // --- Passo 2: Aplicar a Heurística de Penalização ---

            // Se já encontramos pelo menos uma raiz, começamos a penalizar.
            if (raizes.Count > 0)
            {
                // Encontra a distância do indivíduo atual para a raiz encontrada mais próxima.
                double distanciaMinimaDaRaiz = raizes.Select(r => Math.Abs(pop[i].Value - r)).Min();

                // Se o indivíduo está dentro da zona de repulsão...
                if (distanciaMinimaDaRaiz < raioDeRepulsao)
                {
                    // Calcula um fator de penalidade.
                    // Será 0 se estiver exatamente na raiz, e 1 se estiver no limite do raio.
                    // Isso cria uma "rampa" de penalidade.
                    double fatorDePenalidade = distanciaMinimaDaRaiz / raioDeRepulsao;

                    // Aplica a penalidade ao fitness base.
                    // O fitness será reduzido drasticamente perto da raiz.
                    pop[i].Fitness = baseFitness * fatorDePenalidade;
                }
                else
                {
                    // Se está fora da zona de repulsão, o fitness não é penalizado.
                    pop[i].Fitness = baseFitness;
                }
            }
            else
            {
                // Se nenhuma raiz foi encontrada ainda, não há o que penalizar.
                pop[i].Fitness = baseFitness;
            }
        }
    }

    public static Individuo SelecaoPorRoleta(Individuo[] pop)
    {
        double somaFitness = pop.Sum(individuo => individuo.Fitness);

        if (somaFitness == 0)
            return pop[rnd.Next(pop.Length)];

        double pontoSorteado = rnd.NextDouble() * somaFitness;

        double fitnessAcumulado = 0;

        foreach (var individuo in pop)
        {
            fitnessAcumulado += individuo.Fitness;
            if (fitnessAcumulado >= pontoSorteado)
            {
                return individuo;
            }
        }


        return pop.Last();
    }

    public static Individuo Crossover(Individuo pai1, Individuo pai2)
    {
        int pontoDeCorte = rnd.Next(0, TAMANHO_DA_PALAVRA);

        string primeiraParte = pai1.Binario[..pontoDeCorte];
        string segundaParte = pai2.Binario[pontoDeCorte..];

        StringBuilder filhoString = new(primeiraParte + segundaParte);

        for (int i = 0; i < TAMANHO_DA_PALAVRA; i++)
        {
            if (rnd.NextDouble() < TAXA_DE_MUTACAO)
                filhoString[i] = (filhoString[i] == '0') ? '1' : '0';
        }

        Individuo filho = new()
        {
            Binario = filhoString.ToString()
        };

        return filho;
    }
    public static List<double> AlgoritmoGenetico(Individuo[] pop)
    {
        int geracao = 0;
        List<double> raizes = [];
        Coeficientes coeficientes = ExtrairCoeficientes(equacao);

        Decodificar(pop);
        AvaliarFitness(pop, coeficientes, raizes);
        QuickSort.ExecutarDouble(pop, 0, TAMANHO_POPULACAO - 1);

        while ((raizes.Count < 2) && geracao < 100000)
        {
            Individuo[] novaPop = new Individuo[TAMANHO_POPULACAO];
            geracao++;

            Decodificar(pop);


            for (int i = 0; i < TAMANHO_POPULACAO; i++)
            {
                Individuo pai1 = SelecaoPorRoleta(pop);
                Individuo pai2 = SelecaoPorRoleta(pop);

                while (pai1.Binario == pai2.Binario)
                    pai2 = SelecaoPorRoleta(pop);

                novaPop[i] = Crossover(pai1, pai2);
            }

            pop = novaPop;
            Decodificar(pop);
            AvaliarFitness(pop, coeficientes, raizes);
            QuickSort.ExecutarDouble(pop, 0, TAMANHO_POPULACAO - 1);
            PrintarPopulacao(pop, geracao);
        }

        return raizes;
    }
    public static void Executar()
    {
        List<double> raizes = AlgoritmoGenetico(Popular());

        Console.WriteLine("Raize(s) encontradas.");
        for (int i = 0; i < raizes.Count; i++)
            Console.WriteLine($"X{i + 1}: {raizes[i]}");
    }
}