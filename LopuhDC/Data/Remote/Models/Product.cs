//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LopuhDC.Data.Remote.Models
{
    using LopuhDC.Domain.Utilities;
    using System;
    using System.Collections.Generic;

    public partial class Product : ObservableObject
    {
        private byte[] binImage;
        private ICollection<ProductMaterial> productMaterials;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProductMaterials = new HashSet<ProductMaterial>();
        }

        public int Id { get; set; }
        public int Article { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int TypeId { get; set; }
        public string ImagePath { get; set; }
        public byte[] BinImage
        {
            get => binImage; set
            {
                binImage = value; OnPropertyChanged();
            }
        }
        public Nullable<int> Cost { get; set; }
        public Nullable<int> PeopleCount { get; set; }
        public Nullable<int> FactoryId { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> Netto { get; set; }
        public Nullable<int> Brutto { get; set; }

        public virtual ProductType ProductType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMaterial> ProductMaterials
        {
            get => productMaterials; set
            {
                productMaterials = value; OnPropertyChanged();
            }
        }
    }
}
