namespace PokemonReviewApp.Models
{
    public class PokemonCategory
    {
        public int pokemonId { get; set; }
        public int CategoryId { get; set; }
        public Pokemon Pokemon { get; set; }
        public Category Category { get; set; }

    }
}
