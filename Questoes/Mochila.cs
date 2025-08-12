using System.Runtime.CompilerServices;
using Computacao_Evolucionaria.Models;
using Computacao_Evolucionaria.Scripts;

namespace Computacao_Evolucionaria.Questoes;

public class Mochila
{
    private static Random random = new();
    private static readonly int NumeroDeCaixas = 40;
    private static readonly int TamanhoDaPopulacao = 10;
    private static readonly int NumeroDeGeracoes = 100;
    private static readonly int TamanhoDoTorneio = 5;
    private static readonly double TaxaDeMutacao = 0.05f;

    public class Container(double comprimento, double largura, double altura)
    {
        public double Comprimento { get; } = comprimento;
        public double Largura { get; } = largura;
        public double Altura { get; } = altura;

        public double Volume { get; } = comprimento * largura * altura;
    }

    public static List<IndividuoCaixa.Individuo> CriarPopulacaoInicial(int tamanhoDaPopulacao, List<IndividuoCaixa.Caixa> listaDeCaixaDisponivel)
    {
        List<IndividuoCaixa.Individuo> pop = [];

        for (int i = 0; i < tamanhoDaPopulacao; i++)
        {
            pop.Add(CriarIndividuoAleatorio(listaDeCaixaDisponivel));
        }

        return pop;
    }
    public static IndividuoCaixa.Individuo CriarIndividuoAleatorio(List<IndividuoCaixa.Caixa> listaDeCaixaDisponivel)
    {
        // Passo 1: criar uma cópia embaralhada de lista de caixas.
        // OrderBy(c => random.next()) sintaticamente estamos apenas embaralhando a caixas.
        var caixasEmbaralhadas = listaDeCaixaDisponivel.OrderBy(c => random.Next()).ToList();

        List<IndividuoCaixa.Gene> cromossomo = [];

        foreach (var caixa in caixasEmbaralhadas)
        {
            int rotacaoAleatoria = random.Next(1, 7); // Random dos 1 até 6 rotações disponiveis.

            cromossomo.Add(new IndividuoCaixa.Gene(caixa, rotacaoAleatoria)); // primary construtor.
        }

        return new IndividuoCaixa.Individuo(cromossomo);
    }

    public static List<IndividuoCaixa.Caixa> GerarCaixasAleatorias(int quantidade)
    {
        List<IndividuoCaixa.Caixa> caixas = [];

        for (int i = 0; i < quantidade; i++)
        {
            string id = $"Caixa-{i + 1}";

            double c = random.Next(10, 51); // gerar comprimento entre 10 a 50
            double l = random.Next(10, 51); // gerar largura entre 10 a 50
            double a = random.Next(10, 51); // gerar altura entre 10 a 50

            caixas.Add(new IndividuoCaixa.Caixa(id, c, l, a)); // primary construtor.
        }

        return caixas;
    }

    public static void PrintarPop(List<IndividuoCaixa.Individuo> pop)
    {
        int contadorIndividuo = 1;
        foreach (var individuo in pop)
        {
            Console.WriteLine($"\n--- Indivíduo {contadorIndividuo++} | Fitness Inicial: {individuo.Fitness} ---");
            Console.WriteLine("Cromossomo (Ordem de empacotamento):");

            foreach (var gene in individuo.Cromossomo)
            {
                // Acessando as propriedades e imprimindo
                var caixa = gene.CaixaBase;
                Console.WriteLine(
                    $"  -> Gene: [Caixa: {caixa.Id} | " +
                    $"Dimensões: {caixa.Comprimento}x{caixa.Largura}x{caixa.Altura} | " +
                    $"Rotação a ser aplicada: {gene.TipoDeRotacao}]"
                );
            }
        }
    }

    public static IndividuoCaixa.Individuo SelecaoPorTorneio(List<IndividuoCaixa.Individuo> pop)
    {
        List<IndividuoCaixa.Individuo> participantes = [];

        for (int i = 0; i < TamanhoDoTorneio; i++)
        {
            int indiceAleatorio = random.Next(pop.Count);
            participantes.Add(pop[indiceAleatorio]);
        }

        return participantes.OrderByDescending(p => p.Fitness).First();
    }

    public static (IndividuoCaixa.Individuo, IndividuoCaixa.Individuo) Crossover(IndividuoCaixa.Individuo pai1, IndividuoCaixa.Individuo pai2)
    {
        int tamanho = pai1.Cromossomo.Count;
        var cromossomoFilho1 = new IndividuoCaixa.Gene[tamanho];
        var cromossomoFilho2 = new IndividuoCaixa.Gene[tamanho];

        int ponto1 = random.Next(tamanho);
        int ponto2 = random.Next(tamanho);
        if (ponto1 > ponto2) (ponto1, ponto2) = (ponto2, ponto1);

        // Processamento para o Filho 1
        var fatiaPai1 = new HashSet<string>();
        for (int i = ponto1; i <= ponto2; i++)
        {
            cromossomoFilho1[i] = pai1.Cromossomo[i];
            fatiaPai1.Add(pai1.Cromossomo[i].CaixaBase.Id);
        }
        var genesParaPreencher1 = pai2.Cromossomo.Where(g => !fatiaPai1.Contains(g.CaixaBase.Id)).ToList();
        int indiceGene1 = 0;
        for (int i = 0; i < tamanho; i++)
        {
            if (cromossomoFilho1[i] == null)
            {
                cromossomoFilho1[i] = genesParaPreencher1[indiceGene1++];
            }
        }

        // Processamento para o Filho 2
        var fatiaPai2 = new HashSet<string>();
        for (int i = ponto1; i <= ponto2; i++)
        {
            cromossomoFilho2[i] = pai2.Cromossomo[i];
            fatiaPai2.Add(pai2.Cromossomo[i].CaixaBase.Id);
        }
        var genesParaPreencher2 = pai1.Cromossomo.Where(g => !fatiaPai2.Contains(g.CaixaBase.Id)).ToList();
        int indiceGene2 = 0;
        for (int i = 0; i < tamanho; i++)
        {
            if (cromossomoFilho2[i] == null)
            {
                cromossomoFilho2[i] = genesParaPreencher2[indiceGene2++];
            }
        }

        return (new IndividuoCaixa.Individuo(cromossomoFilho1.ToList()), new IndividuoCaixa.Individuo(cromossomoFilho2.ToList()));
    }

    public static void Mutacao(IndividuoCaixa.Individuo individuo)
    {
        // Mutação por troca.
        if (random.NextDouble() < TaxaDeMutacao)
        {
            int ponto1 = random.Next(individuo.Cromossomo.Count);
            int ponto2 = random.Next(individuo.Cromossomo.Count);

            (individuo.Cromossomo[ponto1], individuo.Cromossomo[ponto2]) = (individuo.Cromossomo[ponto2], individuo.Cromossomo[ponto1]);
        }

        // Mutação de Rotação.
        if (random.NextDouble() < TaxaDeMutacao)
        {
            int ponto = random.Next(individuo.Cromossomo.Count);
            individuo.Cromossomo[ponto].TipoDeRotacao = random.Next(1, 7);
        }
    }

    public static void AlgoritmoGenetico(Container container, List<IndividuoCaixa.Individuo> pop)
    {
        Avaliador avaliador = new(container);
        IndividuoCaixa.Individuo? melhorIndividuoGlobal = null;

        Console.WriteLine("\n--- INICIANDO PROCESSO EVOLUTIVO ---");
        Console.WriteLine($"Container: {container.Comprimento}x{container.Largura}x{container.Altura} | População: {TamanhoDaPopulacao} | Gerações: {NumeroDeGeracoes}");

        for (int i = 0; i < NumeroDeGeracoes; i++)
        {
            foreach (var individuo in pop)
            {
                avaliador.AvaliarIndividuo(individuo);
            }

            // PrintarPop(pop);

            var melhorDaGeracao = pop.OrderByDescending(p => p.Fitness).First();

            if (melhorIndividuoGlobal == null || melhorDaGeracao.Fitness > melhorIndividuoGlobal.Fitness)
            {
                var cloneDoMelhor = new IndividuoCaixa.Individuo(melhorDaGeracao.Cromossomo.ToList())
                {
                    Fitness = melhorDaGeracao.Fitness
                };
                melhorIndividuoGlobal = cloneDoMelhor;
            }

            // if (i % 10 == 0 || i == NumeroDeGeracoes - 1) // Imprime a cada 10 gerações
            // {
            //     Console.WriteLine($"Geração {i.ToString().PadLeft(3)} | Melhor Fitness: {melhorDaGeracao.Fitness:P4} | Melhor Global: {melhorIndividuoGlobal.Fitness:P4}");
            // }

            // Em Mochila.AlgoritmoGenetico, a linha de impressão do progresso
            Console.WriteLine($"Geração {i.ToString().PadLeft(3)} | Melhor Fitness: {melhorIndividuoGlobal.Fitness:F4} | Caixas: {Math.Floor(melhorIndividuoGlobal.Fitness)}");


            List<IndividuoCaixa.Individuo> novaPopulacao = [];
            // Adiciona o melhor indivíduo da geração passada diretamente na nova (Elitismo)
            // Isso garante que não perderemos a melhor solução encontrada.
            novaPopulacao.Add(melhorIndividuoGlobal);

            while (novaPopulacao.Count < TamanhoDaPopulacao)
            {
                // a. Seleciona dois pais
                var pai1 = SelecaoPorTorneio(pop);
                var pai2 = SelecaoPorTorneio(pop);

                // b. Gera dois filhos através do crossover
                var (filho1, filho2) = Crossover(pai1, pai2);

                // c. Aplica mutação nos filhos
                Mutacao(filho1);
                Mutacao(filho2);

                // d. Adiciona os novos filhos à nova população
                novaPopulacao.Add(filho1);
                if (novaPopulacao.Count < TamanhoDaPopulacao)
                {
                    novaPopulacao.Add(filho2);
                }
            }

            // 3. ATUALIZAÇÃO: A nova população substitui a antiga
            pop = novaPopulacao;
        }

        Console.WriteLine("\n--- PROCESSO EVOLUTIVO CONCLUÍDO ---");
        Console.WriteLine($"Melhor Fitness encontrado globalmente: {melhorIndividuoGlobal!.Fitness:P4}");
        Console.WriteLine("Cromossomo da Melhor Solução:");
        foreach (var gene in melhorIndividuoGlobal.Cromossomo)
        {
            var caixa = gene.CaixaBase;
            Console.WriteLine($"  -> Caixa: {caixa.Id,-8} | Rotação: {gene.TipoDeRotacao}");
        }

    }
    public static void Executar()
    {
        AlgoritmoGenetico(new Container(comprimento: 100, largura: 100, altura: 100), CriarPopulacaoInicial(TamanhoDaPopulacao, GerarCaixasAleatorias(NumeroDeCaixas)));
    }
}