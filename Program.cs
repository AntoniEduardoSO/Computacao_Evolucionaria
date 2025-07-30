using src.Questoes;

namespace src;

public class Program
{
    public static void Main()
    {

        while (true)
        {
            Console.WriteLine("Banco de questoes de Algoritmo genetico.");
            Console.WriteLine("Digite 0 para parar o programa.");
            Console.WriteLine("Digite 1 => Questao 1 (Problema da maior ocorrência da substring \"01\").");
            Console.WriteLine("Digite 2 => Questao 2 (Problema das raizes de um polinômino de 2 grau).");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {

                case 0:
                    return;

                case 1:
                    Questao1.Executar();
                    break;

                case 2:
                    Questao2.Executar();
                    break;

                default:
                    break;
            }
        }
    }
}