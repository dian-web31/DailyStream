using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DailyStream.Models;

public partial class DailystreamContext : DbContext
{
    public DailystreamContext()
    {
    }

    public DailystreamContext(DbContextOptions<DailystreamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Dislike> Dislikes { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionDailyStream");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_comentario");

            entity.ToTable("comentario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contenido)
                .HasColumnType("text")
                .HasColumnName("contenido");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Idvideo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idvideo");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("fk_comentario_usuario");
        });

        modelBuilder.Entity<Dislike>(entity =>
        {
            entity.HasKey(e => new { e.Idusuario, e.Idvideo }).HasName("pk_dislike");

            entity.ToTable("dislike");

            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Idvideo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idvideo");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Dislikes)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("fk_dislike_usuario");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => new { e.Idusuario, e.Idvideo }).HasName("pk_like");

            entity.ToTable("like");

            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Idvideo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idvideo");

            entity.HasOne(d => d.IdusuarioNavigation).WithMany(p => p.Likes)
                .HasForeignKey(d => d.Idusuario)
                .HasConstraintName("fk_like_usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("pk_usuario");

            entity.ToTable("usuario");

            entity.HasIndex(e => e.Username, "uq_usuario_username").IsUnique();

            entity.Property(e => e.Idusuario).HasColumnName("idusuario");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
