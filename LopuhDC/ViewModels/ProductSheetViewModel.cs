using LopuhDC.Data.Remote.Models;
using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Services;
using LopuhDC.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LopuhDC.ViewModels
{
    public class ProductSheetViewModel : ViewModel
    {
        private readonly LopuhDbEntities _db;

        public Product Product { get; } = new Product();
        public ProductType SelectedType { get; set; }
        public List<ProductType> ProductTypes { get; }
        public Material SelectedMaterial { get; set; }
        public ObservableCollection<Material> AvailableMaterials { get; }
        public ObservableCollection<Material> AddedMaterials { get; } = new ObservableCollection<Material>();

        public ProductSheetViewModel(INavService navService, ProductContext context, LopuhDbEntities db)
        {
            _db = db;

            ProductTypes = _db.ProductTypes.ToList();
            AvailableMaterials = new ObservableCollection<Material>(_db.Materials.ToList());

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

                SelectedType = Product.ProductType;
            }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
