namespace Computacao_Evolucionaria.Models;

public class IndividuoCaixa
{
    public class Caixa(string id, double comprimento, double largura, double altura)
    {
        public string Id { get; private set; } = id;

        public double Comprimento { get; private set; } = comprimento;
        public double Largura { get; private set; } = largura;
        public double Altura { get; private set; } = altura;

        public double Volume => Comprimento * Largura * Altura;

        public (double dX, double dY, double dZ) ObterDimensoesComRotacao(int tipoDeRotacao)
        {
            switch (tipoDeRotacao)
            {
                case 1: return (Comprimento, Largura, Altura);
                case 2: return (Comprimento, Altura, Largura);
                case 3: return (Largura, Comprimento, Altura);
                case 4: return (Largura, Altura, Comprimento);
                case 5: return (Altura, Comprimento, Largura);
                case 6: return (Altura, Largura, Comprimento);
                default: throw new ArgumentException("Tipo de rotação inválido. Deve ser de 1 a 6.");
            }
        }
    }
    public class Gene(Caixa caixaBase, int tipoDeRotacao)
    {
        public Caixa CaixaBase { get; private set; } = caixaBase;
        public int TipoDeRotacao { get; set; } = tipoDeRotacao;
    }
    public class Individuo(List<Gene> cromossomo)
    {
        public List<Gene> Cromossomo { get; private set; } = cromossomo;
        public double Fitness { get; set; } = 0.0;
    }

}
