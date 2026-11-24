using System.Text.Json.Serialization;

namespace MediaCatalog.Core.Models
{
    // Полиморфная сериализация (для правильной загрузки из JSON)
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Movie), "movie")]
    [JsonDerivedType(typeof(Book), "book")]
    [JsonDerivedType(typeof(Game), "game")]
    public abstract class MediaItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;

        [JsonIgnore]
        public abstract string MediaType { get; }

        protected MediaItem() { }

        protected MediaItem(string title, int year, string genre)
        {
            Title = title;
            Year = year;
            Genre = genre;
        }

        public override string ToString() => $"[{MediaType}] {Title} ({Year}) - {Genre}";
    }

    public class Movie : MediaItem
    {
        public string Director { get; set; } = string.Empty;
        public override string MediaType => "Фильм";

        public Movie(string title, int year, string genre, string director) : base(title, year, genre)
            => Director = director;
        public Movie() { }
    }

    public class Book : MediaItem
    {
        public string Author { get; set; } = string.Empty;
        public override string MediaType => "Книга";

        public Book(string title, int year, string genre, string author) : base(title, year, genre)
            => Author = author;
        public Book() { }
    }

    public class Game : MediaItem
    {
        public string Platform { get; set; } = string.Empty;
        public override string MediaType => "Игра";

        public Game(string title, int year, string genre, string platform) : base(title, year, genre)
            => Platform = platform;
        public Game() { }
    }
}