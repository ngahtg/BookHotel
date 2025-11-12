using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenAnhTungWPF.Models;
[Table("RoomType")]
public partial class RoomType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoomTypeId { get; set; }

    public string? RoomTypeName { get; set; }

    public string? TypeDescription { get; set; }

    public string? TypeNote { get; set; }

    public virtual ICollection<RoomInformation> RoomInformations { get; set; } = new List<RoomInformation>();
}
