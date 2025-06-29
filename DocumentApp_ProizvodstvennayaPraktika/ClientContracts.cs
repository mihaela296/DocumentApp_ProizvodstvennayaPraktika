
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DocumentApp_ProizvodstvennayaPraktika
{

using System;
    using System.Collections.Generic;
    
public partial class ClientContracts
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public ClientContracts()
    {

        this.ContractHistory = new HashSet<ContractHistory>();

    }


    public int ContractId { get; set; }

    public int TemplateId { get; set; }

    public int ClientId { get; set; }

    public Nullable<int> LawyerId { get; set; }

    public string ContractData { get; set; }

    public string ContractNumber { get; set; }

    public Nullable<System.DateTime> ContractDate { get; set; }

    public Nullable<System.DateTime> CreatedAt { get; set; }

    public Nullable<System.DateTime> UpdatedAt { get; set; }

    public string Status { get; set; }

    public string Notes { get; set; }



    public virtual Users Users { get; set; }

    public virtual ContractTemplates ContractTemplates { get; set; }

    public virtual Users Users1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ContractHistory> ContractHistory { get; set; }

}

}
