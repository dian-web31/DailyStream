using System;
using System.Collections.Generic;

namespace DailyStream.Models;

public partial class Dislike
{
    public int Idusuario { get; set; }

    public string Idvideo { get; set; } = null!;

    public virtual Usuario IdusuarioNavigation { get; set; } = null!;
}
