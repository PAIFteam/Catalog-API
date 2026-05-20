namespace Catalog.Core.Domain.Entities
{
    /// <summary>
    /// Entidade que representa as informações completas de um jogo com sua empresa e tipo de jogo
    /// Utilizada para carga e consulta em Elasticsearch
    /// </summary>
    public class GameInfo
    {
        public GameInfo(int idGame, string name, decimal price, int idCompany, string nameCompany, 
                        int idTypeGame, string nameTypeGame)
        {
            IdGame = idGame;
            Name = name;
            Price = price;
            IdCompany = idCompany;
            NameCompany = nameCompany;
            IdTypeGame = idTypeGame;
            NameTypeGame = nameTypeGame;
        }

        /// <summary>
        /// Identificador único do jogo
        /// </summary>
        public int IdGame { get; set; }

        /// <summary>
        /// Nome do jogo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Preço do jogo
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Identificador da empresa/publisher
        /// </summary>
        public int IdCompany { get; set; }

        /// <summary>
        /// Nome da empresa/publisher
        /// </summary>
        public string NameCompany { get; set; }

        /// <summary>
        /// Identificador do tipo de jogo
        /// </summary>
        public int IdTypeGame { get; set; }

        /// <summary>
        /// Nome do tipo de jogo (gênero)
        /// </summary>
        public string NameTypeGame { get; set; }
    }
}
