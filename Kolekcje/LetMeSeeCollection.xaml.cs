using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Kolekcje
{
    public partial class LetMeSeeCollection : ContentPage
    {
        public ObservableCollection<CollectionElement> Elements { get; set; } = new ObservableCollection<CollectionElement>();

        public LetMeSeeCollection(string collectionName)
		{
			InitializeComponent();
			CollectionNameLabel.Text = $"Kolekcja: {collectionName}";
			elementsView.ItemsSource = Elements;
			LoadElements(collectionName);
		}

        private async void LoadElements(string collectionName)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length >= 4 && parts[0] == collectionName)
                    {
                        decimal price;
                        if (!decimal.TryParse(parts[3], out price))
                        {
                            price = 0;
                        }
                        Elements.Add(new CollectionElement { Name = parts[1], State = parts[2], Price = price });
                    }
                }
            }
        }

        private void EditName_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var elementToEdit = button?.CommandParameter as CollectionElement;
            if (elementToEdit != null)
            {
                PromptEdit("Edytuj nazwę", elementToEdit);
            }
        }

        private void EditState_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var elementToEdit = button?.CommandParameter as CollectionElement;
            if (elementToEdit != null)
            {
                PromptEdit("Edytuj stan", elementToEdit);
            }
        }

        private void EditPrice_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var elementToEdit = button?.CommandParameter as CollectionElement;
            if (elementToEdit != null)
            {
                PromptEdit("Edytuj cenę", elementToEdit);
            }
        }

        private async void PromptEdit(string title, CollectionElement element)
        {
            var promptMessage = title == "Edytuj cenę" ? "Wprowadź nową wartość: (liczbę)" : "Wprowadź nową wartość:";
            var result = await DisplayPromptAsync(title, promptMessage);
            if (result != null)
            {
                switch (title)
                {
                    case "Edytuj nazwę":
                        element.Name = result;
                        break;
                    case "Edytuj stan":
                        element.State = result;
                        break;
                    case "Edytuj cenę":
                        if (decimal.TryParse(result, out decimal newPrice))
                        {
                            element.Price = newPrice;
                        }
                        break;
                }
                UpdateElement(element);
                elementsView.ItemsSource = null;
                elementsView.ItemsSource = Elements;
            }
        }

        private async void UpdateElement(CollectionElement element)
        {
            string collectionName = CollectionNameLabel.Text.Replace("Kolekcja: ", "");
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var newLines = lines.Select(line =>
                {
                    var parts = line.Split(';');
                    if (parts.Length >= 4 && parts[0] == collectionName && parts[1] == element.Name)
                    {
                        return $"{collectionName};{element.Name};{element.State};{element.Price}";
                    }
                    return line;
                }).ToArray();
                await File.WriteAllLinesAsync(filePath, newLines);
            }
        }

        private async void DeleteElement_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var elementToDelete = button?.CommandParameter as CollectionElement;
            if (elementToDelete != null)
            {
                string collectionName = CollectionNameLabel.Text.Replace("Kolekcja: ", "");
                string elementLine = $"{collectionName};{elementToDelete.Name};{elementToDelete.State};{elementToDelete.Price};";
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");

                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);
                    var newLines = lines.Where(line => line != elementLine).ToArray();
                    await File.WriteAllLinesAsync(filePath, newLines);
                }

                Elements.Remove(elementToDelete);
                elementsView.ItemsSource = null;
                elementsView.ItemsSource = Elements;
            }
        }

        private async void AddElement_Clicked(object sender, EventArgs e)
        {
            if(newElementName.Text == string.Empty) return;
            string collectionName = CollectionNameLabel.Text.Replace("Kolekcja: ", "");
            string elementName = newElementName.Text;
            newElementName.Text = string.Empty;
            string elementState = "Nowy";
            decimal elementPrice = 0;

            var newElement = new CollectionElement { Name = elementName, State = elementState, Price = elementPrice };
            Elements.Add(newElement);

            string newElementLine = $"{collectionName};{elementName};{elementState};{elementPrice};";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");

            await File.AppendAllTextAsync(filePath, newElementLine + Environment.NewLine);
            elementsView.ItemsSource = null;
            elementsView.ItemsSource = Elements;
        }
    }
}