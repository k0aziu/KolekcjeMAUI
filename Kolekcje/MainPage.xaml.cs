using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace Kolekcje
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> Collections;

        public MainPage()
        {
            InitializeComponent();
            Collections = new ObservableCollection<string>();
            LoadCollections();
        }

        private async void LoadCollections()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collectionNames.txt");
            Debug.WriteLine($"Ścieżka do pliku collectionNames.txt: {filePath}");

            if (File.Exists(filePath))
            {
                string[] collections = await File.ReadAllLinesAsync(filePath);
                foreach (var collection in collections)
                {
                    if (!Collections.Contains(collection))
                    {
                        Collections.Add(collection);
                    }
                }
            }
            collectionsView.ItemsSource = Collections;
        }

        private async void AddCollection_Clicked(object sender, EventArgs e)
        {
            string newCollection = newCollectionName.Text;
            if (!string.IsNullOrWhiteSpace(newCollection) && !Collections.Contains(newCollection))
            {
                Collections.Add(newCollection);
                newCollectionName.Text = string.Empty;

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collectionNames.txt");
                await File.WriteAllLinesAsync(filePath, Collections);

                Debug.WriteLine("Aktualne kolekcje:");
                foreach (var collection in Collections)
                {
                    Debug.WriteLine(collection);
                }
            }
        }

        private async void DeleteCollection_Clicked(object sender, EventArgs e)
        {
            string collectionToDelete = (sender as Button)?.BindingContext as string;
            if (collectionToDelete != null)
            {
                Collections.Remove(collectionToDelete);

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collectionNames.txt");
                await File.WriteAllLinesAsync(filePath, Collections);

                string collectionsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
                if (File.Exists(collectionsFilePath))
                {
                    var allLines = await File.ReadAllLinesAsync(collectionsFilePath);
                    var filteredLines = allLines.Where(line => !line.StartsWith(collectionToDelete + ";")).ToArray();
                    await File.WriteAllLinesAsync(collectionsFilePath, filteredLines);
                }

                Debug.WriteLine($"Usunięto kolekcję: {collectionToDelete}");
            }
        }

        private async void EditCollection_Clicked(object sender, EventArgs e)
        {
            string collectionName = (sender as Button)?.BindingContext as string;
            if (!string.IsNullOrEmpty(collectionName))
            {
                await Navigation.PushAsync(new LetMeSeeCollection(collectionName));
            }
        }
    }
}