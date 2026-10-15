using System;
using System.Collections.Generic;

namespace asp_net_3_2.Models;

public partial class Category : IEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
