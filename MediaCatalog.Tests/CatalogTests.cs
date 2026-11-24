using Xunit;
using MediaCatalog.Core.Models;
using MediaCatalog.Core.Services;

namespace MediaCatalog.Tests
{
    public class CatalogTests
    {
        [Fact]
        public void Add_ValidMovie_SavesSuccessfully()
        {
            // Arrange
            string testFile = "test_catalog.json";
            if (File.Exists(testFile)) File.Delete(testFile);
            var repo = new CatalogRepository(testFile);
            var movie = new Movie("Матрица", 1999, "Фантастика", "Вачовски");

            // Act
            repo.Add(movie);

            // Assert
            Assert.Single(repo.Items);
            Assert.True(File.Exists(testFile));
            Assert.Equal("Матрица", repo.Items[0].Title);

            // Cleanup
            File.Delete(testFile);
        }

        [Fact]
        public void Add_InvalidYear_ThrowsArgumentException()
        {
            // Arrange
            var repo = new CatalogRepository("test2.json");
            var badBook = new Book("Книга", 1800, "Жанр", "Автор");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repo.Add(badBook));
        }

        [Fact]
        public void Add_EmptyTitle_ThrowsArgumentException()
        {
            var repo = new CatalogRepository("test3.json");
            var badMovie = new Movie("", 2020, "Боевик", "Режиссер");

            Assert.Throws<ArgumentException>(() => repo.Add(badMovie));
        }

        [Fact]
        public void Search_ByTitle_ReturnsResults()
        {
            // Arrange
            var repo = new CatalogRepository("test4.json");
            repo.Add(new Book("C# in Depth", 2019, "Программирование", "Jon Skeet"));
            repo.Add(new Game("Minecraft", 2011, "Песочница", "PC"));

            // Act
            var results = repo.Search("C#");

            // Assert
            Assert.Single(results);
            Assert.Equal("C# in Depth", results.First().Title);

            File.Delete("test4.json");
        }
    }
}