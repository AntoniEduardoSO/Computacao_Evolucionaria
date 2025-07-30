using src.Models;

namespace src.Scripts;

public class Selecao
{
    private static Random rnd = new();

    // <summary>
    /// Seleciona um indivíduo da população usando o método da Roleta.
    /// Indivíduos com maior fitness têm maior probabilidade de serem selecionados.
    /// </summary>
    /// <param name="populacao">A lista de indivíduos da geração atual.</param>
    /// <returns>O indivíduo selecionado para ser pai.</returns>
    /// 
    public static IndividuoInt SelecaoPorRoleta(IndividuoInt[] pop)
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

    public static IndividuoInt SelecaoPorTorneio(IndividuoInt[] pop)
    {
        int index1 = rnd.Next(0, pop.Length);
        int index2 = rnd.Next(0, pop.Length);

        return (pop[index1].Fitness > pop[index2].Fitness)
            ? pop[index1] 
            : pop[index2];
    }
    
}
