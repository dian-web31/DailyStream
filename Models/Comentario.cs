using System;
using System.Collections.Generic;

namespace DailyStream.Models;

public partial class Comentario
{
    public int Id { get; set; }

    public int Idusuario { get; set; }

    public string Idvideo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public virtual Usuario IdusuarioNavigation { get; set; } = null!;
}
