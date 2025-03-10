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
    using System;
    using System.Collections.Generic;
    
    public partial class Material
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Material()
        {
            this.ProductMaterials = new HashSet<ProductMaterial>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public int TypeId { get; set; }
        public int Count { get; set; }
        public int UnitId { get; set; }
        public int WarehouseCount { get; set; }
        public int MinCount { get; set; }
        public int Cost { get; set; }
    
        public virtual MaterialType MaterialType { get; set; }
        public virtual MaterialUnit MaterialUnit { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
    }
}
