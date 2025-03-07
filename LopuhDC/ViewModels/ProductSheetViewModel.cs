using LopuhDC.Data.Remote.Models;
using LopuhDC.Domain.Commands;
using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Services;
using LopuhDC.Domain.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LopuhDC.ViewModels
{
    public class ProductSheetViewModel : ViewModel
    {
        private readonly ProductContext _context;
        private readonly LopuhDbEntities _db;
        private readonly List<Material> _originalMaterials = new List<Material>();
        private readonly HashSet<int> _removedOriginalMaterialIds = new HashSet<int>();

        public Product Product { get; } = new Product();
        public ProductType SelectedType { get; set; }
        public List<ProductType> ProductTypes { get; }
        public Material SelectedMaterial { get; set; }
        public ObservableCollection<Material> AvailableMaterials { get; }
        public ObservableCollection<Material> AddedMaterials { get; } = new ObservableCollection<Material>();

        public ICommand AddMaterialCommand { get; }
        public ICommand RemoveMaterialCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand OpenFileDialogCommand { get; }
        public ICommand SaveChangesCommand { get; }

        public ProductSheetViewModel(INavService navService, ProductContext context, LopuhDbEntities db)
        {
            _context = context;
            _db = db;

            ProductTypes = _db.ProductTypes.ToList();
            AvailableMaterials = new ObservableCollection<Material>(_db.Materials.ToList());

            GoBackCommand = new PopCommand(navService);
            AddMaterialCommand = new RelayCommand(AddMaterial);
            RemoveMaterialCommand = new RelayCommand(RemoveMaterial);
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
            SaveChangesCommand = new RelayCommand(SaveChanges);

            if (context.Product != null)
            {
                var p = context.Product;

                Product = new Product
                {
                    Id = p.Id,
                    Article = p.Article,
                    Title = p.Title,
                    Subtitle = p.Subtitle,
                    BinImage = p.BinImage,
                    ImagePath = p.ImagePath,
                    Brutto = p.Brutto,
                    Cost = p.Cost,
                    FactoryId = p.FactoryId,
                    Height = p.Height,
                    Length = p.Length,
                    Netto = p.Netto,
                    PeopleCount = p.PeopleCount,
                    ProductMaterials = p.ProductMaterials,
                    ProductType = p.ProductType,
                    TypeId = p.TypeId,
                    Width = p.Width,
                };

                _originalMaterials = Product.ProductMaterials.Select(it => it.Material).ToList();
                AddedMaterials = new ObservableCollection<Material>(_originalMaterials);
                SelectedType = Product.ProductType;
            }
        }

        private void AddMaterial()
        {
            if (SelectedMaterial != null)
            {
                AddedMaterials.Add(SelectedMaterial);
                AvailableMaterials.Remove(SelectedMaterial);
                SelectedMaterial = null;
            }
        }

        private void RemoveMaterial(object param)
        {
            if (param is Material material)
            {
                AvailableMaterials.Add(material);
                AddedMaterials.Remove(material);

                if (Product.ProductMaterials.Select(it => it.MaterialId).Contains(material.Id))
                {
                    _removedOriginalMaterialIds.Add(material.Id);
                }
            }
        }

        private void OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Медиа|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                Product.BinImage = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        private void SaveChanges()
        {
            try
            {
                if (Product.Id == 0)
                {
                    AddNewProduct();
                }
                else
                {
                    SaveProductUpdates();
                }

                GoBackCommand.Execute(null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void AddNewProduct()
        {
            throw new NotImplementedException();
        }

        private void SaveProductUpdates()
        {
            var p = _context.Product;

            p.Article = Product.Article;
            p.Title = Product.Title;
            p.Subtitle = Product.Subtitle;
            p.Cost = Product.Cost;
            p.TypeId = SelectedType.Id;
            p.ProductType = SelectedType;

            var materialIdsToAdd = AddedMaterials.Except(_originalMaterials).Select(it => it.Id);
            var materialIdsToRemove = _removedOriginalMaterialIds;

            if (materialIdsToRemove.Any())
            {
                var removingProductMaterials = _db.ProductMaterials
                        .Where(it => it.ProductId == Product.Id && materialIdsToRemove.Contains(it.MaterialId))
                        .ToList();
                _db.ProductMaterials.RemoveRange(removingProductMaterials);

                foreach (var item in removingProductMaterials)
                {
                    p.ProductMaterials.Remove(item);
                }
            }

            foreach (var item in materialIdsToAdd)
            {
                var addingProductMaterial = new ProductMaterial
                {
                    MaterialId = item,
                    ProductId = Product.Id,
                    MaterialCount = 1,
                };

                _db.ProductMaterials.Add(addingProductMaterial);
                p.ProductMaterials.Add(addingProductMaterial);
            }

            _db.SaveChanges();
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
