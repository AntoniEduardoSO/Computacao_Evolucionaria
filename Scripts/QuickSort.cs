using src.Models;
using static src.Questoes.Questao2;

namespace src.Scripts;

public class QuickSort
{
    public static IndividuoInt[] ExecutarInt(IndividuoInt[] arr, int inivet, int fimVet)
    {
        int i, j, pivo;

        i = inivet;
        j = fimVet;
        pivo = arr[(inivet + fimVet) / 2].Fitness;

        while (i <= j)
        {
            while (arr[i].Fitness < pivo)
                i++;

            while (arr[j].Fitness > pivo)
                j--;

            if (i <= j)
            {
                IndividuoInt aux = arr[i];
                arr[i] = arr[j];
                arr[j] = aux;

                i++;
                j--;
            }
        }
        if (inivet < j)
            ExecutarInt(arr, inivet, j);
        if (i < fimVet)
            ExecutarInt(arr, i, fimVet);


        return arr;
    }
    
    public static Individuo[] ExecutarDouble(Individuo[] arr, int inivet, int fimVet)
    {
        int i, j;
        double pivo;

        i = inivet;
        j = fimVet;
        pivo = arr[(inivet + fimVet) / 2].Fitness;

        while (i <= j)
        {
            while (arr[i].Fitness < pivo)
                i++;

            while (arr[j].Fitness > pivo)
                j--;

            if (i <= j)
            {
                Individuo aux = arr[i];
                arr[i] = arr[j];
                arr[j] = aux;

                i++;
                j--;
            }
        }
        if (inivet < j)
            ExecutarDouble(arr, inivet, j);
        if (i < fimVet)
            ExecutarDouble(arr, i, fimVet);
        

        return arr;
    }
}