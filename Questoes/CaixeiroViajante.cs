using System.Security.Cryptography;

namespace Computacao_Evolucionaria.Questoes;

public class CaixeiroViajante
{
    public const string filePath = "Perguntas/CaixeiroViajante.md";
    private static Random random = new();

    public struct Individuo(int numeroDeVertices)
    {
        public List<int> Caminho { get; set; } = new List<int>(numeroDeVertices);
        public int Custo { get; set; } 
        public int Score { get; set; } = 0;
    }
    public static int[][] CriaMatrizDeDistancias()
    {
        try
        {
            string markDowncontent = File.ReadAllText(filePath);

            string[] lines = markDowncontent.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            int numeroDeVertices = int.Parse(lines[0]);

            int[][] matrizDeDistancias = new int[numeroDeVertices][];

            for (int i = 0; i < numeroDeVertices; i++)
            {
                string linhaDaMatriz = lines[i + 1];

                string[] valoresNaLinha = linhaDaMatriz.Split(',');

                matrizDeDistancias[i] = valoresNaLinha.Select(s => int.Parse(s.Trim())).ToArray();
            }

            return matrizDeDistancias;
        }
        catch
        {
            throw new Exception("ERRO NA TRADUCAO DO ARQUIVO .MD");
        }

    }

    public static Individuo[] Popular(int numeroDeVertices)
    {
        Individuo[] individuos = new Individuo[10];

        for (int i = 0; i < 10; i++)
        {
            List<int> novoCaminho = Enumerable.Range(0, numeroDeVertices).ToList();

            int n = novoCaminho.Count;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (novoCaminho[k], novoCaminho[n]) = (novoCaminho[n], novoCaminho[k]); // swap
            }

            individuos[i] = new Individuo(numeroDeVertices)
            {
                Caminho = novoCaminho
            };
        }

        return individuos;
    }

    public static void AlgoritmoGenetico(int[][] matrizDeDistancias)
    {
        Individuo[] individuos = Popular(matrizDeDistancias.Length);

        foreach (var individuo in individuos)
        {
            string caminhoFormatado = string.Join(" -> ", individuo.Caminho);
            Console.WriteLine($"Caminho: [ {caminhoFormatado} ], Score: {individuo.Score}");
        }
    }
    public static void Executar()
    {
        AlgoritmoGenetico(CriaMatrizDeDistancias());
    }
}


// for (int i = 0; i < matrizDeDistancias.Length; i++)
// {
//     // Pega a linha atual da matriz
//     int[] linha = matrizDeDistancias[i];
//     // Junta os números da linha com ", " para imprimir de forma legível
//     Console.WriteLine(string.Join(", ", linha));
// }
// Console.WriteLine("---------------------------------");