using System;
using System.Collections.Generic;

namespace DailyStream.Models;

public partial class Usuario
{
    public int Idusuario { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public virtual ICollection<Dislike> Dislikes { get; set; } = new List<Dislike>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
}
