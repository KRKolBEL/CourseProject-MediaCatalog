using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MediaCatalog.Core.Models;
using MediaCatalog.Core.Services;

namespace MediaCatalog.WPF
{
    public partial class MainWindow : Window
    {
        private CatalogRepository _repo;

        public MainWindow()
        {
            InitializeComponent();
            _repo = new CatalogRepository("catalog.json");
            UpdateTable();
        }

        private void UpdateTable()
        {
            var items = _repo.Search(TxtSearch.Text);

            DgItems.ItemsSource = items.Select(x => new {
                x.Id,
                x.MediaType,
                x.Title,
                x.Year,
                x.Genre,
                ExtraInfo = x is Movie m ? m.Director :
                            x is Book b ? b.Author :
                            x is Game g ? g.Platform : ""
            }).ToList();
        }

        private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtExtra == null || LblExtra == null) return;
            var type = (CmbType.SelectedItem as ComboBoxItem)?.Content.ToString();
            LblExtra.Text = type switch
            {
                "Фильм" => "Режиссер:",
                "Книга" => "Автор:",
                "Игра" => "Платформа:",
                _ => "Доп. инфо:"
            };
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string type = (CmbType.SelectedItem as ComboBoxItem)?.Content.ToString();
                string title = TxtTitle.Text;
                string genre = TxtGenre.Text;
                string extra = TxtExtra.Text;

                if (!int.TryParse(TxtYear.Text, out int year))
                    throw new Exception("Год должен быть числом.");

                MediaItem item = type switch
                {
                    "Фильм" => new Movie(title, year, genre, extra),
                    "Книга" => new Book(title, year, genre, extra),
                    "Игра" => new Game(title, year, genre, extra),
                    _ => throw new Exception("Выберите тип")
                };

                _repo.Add(item);
                UpdateTable();

                TxtTitle.Clear(); TxtYear.Clear(); TxtGenre.Clear(); TxtExtra.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgItems.SelectedItem == null)
            {
                MessageBox.Show("Выберите элемент для удаления.");
                return;
            }

            // Получаем ID из анонимного типа через рефлексию
            var selected = DgItems.SelectedItem;
            var idProperty = selected.GetType().GetProperty("Id");
            if (idProperty == null) return;

            Guid id = (Guid)idProperty.GetValue(selected)!;

            if (MessageBox.Show("Удалить выбранный элемент?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _repo.Delete(id);
                UpdateTable();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Все изменения автоматически сохраняются в catalog.json при добавлении и удалении.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateTable();
        }
    }
}