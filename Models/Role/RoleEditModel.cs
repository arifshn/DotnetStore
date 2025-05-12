using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class RoleEditModel
{

    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Role Adı")]
    public string RoleAdi { get; set; } = null!;


}


