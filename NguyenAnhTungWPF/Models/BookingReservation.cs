using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenAnhTungWPF.Models;
[Table("BookingReservation")]
public partial class BookingReservation
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookingReservationId { get; set; }

    public DateTime? BookingDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public int? CustomerId { get; set; }

    public string? BookingStatus { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Customer? Customer { get; set; }
}
