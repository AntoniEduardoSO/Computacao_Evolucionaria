using Computacao_Evolucionaria.Models;
using Computacao_Evolucionaria.Questoes;

namespace Computacao_Evolucionaria.Scripts
{
    public class Avaliador(Mochila.Container container)
    {
        private readonly Mochila.Container _container = container;
        private static Random _random = new Random();
        private int[,,] _espacoOcupado = new int[0, 0, 0];

        public void AvaliarIndividuo(IndividuoCaixa.Individuo individuo)
        {
            _espacoOcupado = new int[
                (int)Math.Ceiling(_container.Comprimento),
                (int)Math.Ceiling(_container.Largura),
                (int)Math.Ceiling(_container.Altura)
            ];

            int numeroDeCaixasEmbaladas = 0;
            double somaDasCoordenadas = 0; // Para nosso critério de desempate

            foreach (var gene in individuo.Cromossomo)
            {
                var dimensoesCaixa = gene.CaixaBase.ObterDimensoesComRotacao(gene.TipoDeRotacao);

                // Passamos uma variável de saída 'pos' para obter a posição da caixa
                bool foiEmbalado = TentarEmbalarCaixa(gene, dimensoesCaixa, out (int x, int y, int z) pos);
                if (foiEmbalado)
                {
                    numeroDeCaixasEmbaladas++;
                    // Adicionamos a soma das coordenadas da posição da caixa
                    somaDasCoordenadas += pos.x + pos.y + pos.z;
                }
            }

            double fitnessPrincipal = numeroDeCaixasEmbaladas;

            // Critério 2: Compacidade (o desempate, valor entre 0 e 1)
            // Usamos 1.0 para evitar divisão por zero se nenhuma caixa for embalada.
            double fatorCompacidade = 1.0 / (1.0 + somaDasCoordenadas);

            individuo.Fitness = fitnessPrincipal + fatorCompacidade;
        }

        private bool TentarEmbalarCaixa(IndividuoCaixa.Gene gene, (double dX, double dY, double dZ) dimensoes, out (int x, int y, int z) posicao)
        {
            posicao = (-1, -1, -1); // Valor padrão se não for embalado

            List<int> ordemDosEixos = [0, 1, 2];
            ordemDosEixos = ordemDosEixos.OrderBy(e => _random.Next()).ToList();

            Func<int, int> getLength = i => _espacoOcupado.GetLength(i);

            for (int i = 0; i < getLength(ordemDosEixos[0]); i++)
            {
                for (int j = 0; j < getLength(ordemDosEixos[1]); j++)
                {
                    for (int k = 0; k < getLength(ordemDosEixos[2]); k++)
                    {
                        int[] coords = new int[3];
                        coords[ordemDosEixos[0]] = i;
                        coords[ordemDosEixos[1]] = j;
                        coords[ordemDosEixos[2]] = k;

                        int x = coords[0];
                        int y = coords[1];
                        int z = coords[2];

                        if (PosicaoEValida(x, y, z, dimensoes))
                        {
                            OcuparEspaco(x, y, z, dimensoes, int.Parse(gene.CaixaBase.Id.Split('-')[1]));
                            posicao = (x, y, z); // Guarda a posição de sucesso
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool PosicaoEValida(int posX, int posY, int posZ, (double dX, double dY, double dZ) dim)
        {
            if (posX + dim.dX > _espacoOcupado.GetLength(0) ||
                posY + dim.dY > _espacoOcupado.GetLength(1) ||
                posZ + dim.dZ > _espacoOcupado.GetLength(2))
            {
                return false;
            }

            for (int x = posX; x < posX + dim.dX; x++)
            {
                for (int y = posY; y < posY + dim.dY; y++)
                {
                    for (int z = posZ; z < posZ + dim.dZ; z++)
                    {
                        if (_espacoOcupado[x, y, z] != 0)
                        {
                            return false; // Conflito de espaço
                        }
                    }
                }
            }

            if (posZ > 0)
            {
                bool temSuporte = false;
                for (int x = posX; x < posX + dim.dX; x++)
                {
                    for (int y = posY; y < posY + dim.dY; y++)
                    {
                        if (_espacoOcupado[x, y, posZ - 1] != 0)
                        {
                            temSuporte = true;
                            break;
                        }
                    }
                    if (temSuporte) break;
                }

                if (!temSuporte)
                {
                    return false;
                }
            }
            return true;
        }

        private void OcuparEspaco(int posX, int posY, int posZ, (double dX, double dY, double dZ) dim, int id)
        {
            for (int x = posX; x < posX + dim.dX; x++)
            {
                for (int y = posY; y < posY + dim.dY; y++)
                {
                    for (int z = posZ; z < posZ + dim.dZ; z++)
                    {
                        _espacoOcupado[x, y, z] = id;
                    }
                }
            }
        }
    }
}