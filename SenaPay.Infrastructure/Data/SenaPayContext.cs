using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;

namespace SenaPay.Infrastructure.Data;

public partial class SenaPayContext : DbContext
{
    public SenaPayContext()
    {
    }

    public SenaPayContext(DbContextOptions<SenaPayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminCafeterium> AdminCafeteria { get; set; }

    public virtual DbSet<Aprendix> Aprendices { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<DetalleTransaccion> DetalleTransaccions { get; set; }

    public virtual DbSet<Funcionario> Funcionarios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<RecuperacionPassword> RecuperacionPasswords { get; set; }

    public virtual DbSet<ResultadoCompra> ResultadoCompras { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tiendum> Tienda { get; set; }

    public virtual DbSet<Transaccione> Transacciones { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Sede> Sedes { get; set; }
    public virtual DbSet<Reporte> Reportes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=PRUEBAPROYECTO;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminCafeterium>(entity =>
        {
            entity.HasKey(e => e.IdAdminCafeteria).HasName("PK__Admin_Ca__DF3DD0C93F1A5BE0");

            entity.ToTable("Admin_Cafeteria");

            entity.Property(e => e.IdAdminCafeteria).HasColumnName("Id_Admin_Cafeteria");
            entity.Property(e => e.Correo).IsUnicode(false);
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Nombre).IsUnicode(false);
            entity.Property(e => e.Telefono).IsUnicode(false);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.AdminCafeteria)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdminCa_Usuario");
        });

        modelBuilder.Entity<Aprendix>(entity =>
        {
            entity.HasKey(e => e.IdAprendiz).HasName("PK__Aprendic__B3B981A28882B0A6");

            entity.Property(e => e.IdAprendiz).HasColumnName("Id_Aprendiz");
            entity.Property(e => e.Correo).IsUnicode(false);
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(90)
                .IsUnicode(false);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Aprendices)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Aprendices_Usuarios");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id_Categoria).HasName("PK__Categori__CB903349C2827147");

            entity.Property(e => e.Id_Categoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.Nombre_Categoria)
                .IsUnicode(false)
                .HasColumnName("Nombre_Categoria");
        });

        modelBuilder.Entity<DetalleTransaccion>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Detalle___9274780BF4D67244");

            entity.ToTable("Detalle_Transaccion");

            entity.Property(e => e.IdDetalle).HasColumnName("Id_Detalle");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.IdTransaccion).HasColumnName("Id_Transaccion");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Precio_Unitario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleTransaccions)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleTransaccion_Productos");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.DetalleTransaccions)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleTransaccion_Transacciones");
        });

        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.IdFuncionario).HasName("PK__Funciona__8888ED0316F767CD");

            entity.Property(e => e.IdFuncionario).HasColumnName("Id_Funcionario");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Funcionarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Funcionarios_Usuarios");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__2085A9CF161870AC");

            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.IdCategoria).HasColumnName("Id_Categoria");
            entity.Property(e => e.IdTienda).HasColumnName("Id_Tienda");
            entity.Property(e => e.Imagen)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NombreProducto)
                .IsUnicode(false)
                .HasColumnName("Nombre_Producto");
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Productos_Categoria");

            entity.HasOne(d => d.IdTiendaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTienda)
                .HasConstraintName("FK_Productos_Tiendas");
        });

        modelBuilder.Entity<RecuperacionPassword>(entity =>
        {
            entity.HasKey(e => e.IdRecuperacion).HasName("PK__Recupera__53946CED08B3815F");

            entity.ToTable("RecuperacionPassword");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Token).IsUnicode(false);
            entity.Property(e => e.Usado).HasDefaultValue(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.RecuperacionPasswords)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recuperac__Id_Us__03F0984C");
        });

        modelBuilder.Entity<ResultadoCompra>(entity =>
        {
            entity.HasKey(e => e.IdResultadoCompra).HasName("PK__Resultad__F8D1B0F498A3A03B");

            entity.ToTable("ResultadoCompra");

            entity.Property(e => e.IdResultadoCompra).HasColumnName("Id_Resultado_Compra");
            entity.Property(e => e.IdTransaccion).HasColumnName("Id_Transaccion");
            entity.Property(e => e.Mensaje)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SaldoRestante).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdTransaccionNavigation).WithMany(p => p.ResultadoCompras)
                .HasForeignKey(d => d.IdTransaccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResultadoCompra_Transacciones");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__55932E86F5ED9BCC");

            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(90)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tiendum>(entity =>
        {
            entity.HasKey(e => e.IdTienda).HasName("PK__Tienda__C76BF1710F3C424A");

            entity.Property(e => e.IdTienda).HasColumnName("Id_Tienda");
            entity.Property(e => e.IdAdminCafeteria).HasColumnName("Id_Admin_Cafeteria");
            entity.Property(e => e.IdSede).HasColumnName("Id_Sede");
            entity.Property(e => e.Nombre).IsUnicode(false);
            entity.Property(e => e.Ubicacion).IsUnicode(false);

            entity.HasOne(d => d.IdAdminCafeteriaNavigation).WithMany(p => p.Tienda)
                .HasForeignKey(d => d.IdAdminCafeteria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AdminCa_Tienda");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Tienda)
               .HasForeignKey(d => d.IdSede)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_Tienda_Sedes");
        });

        modelBuilder.Entity<Transaccione>(entity =>
        {
            entity.HasKey(e => e.IdTransaccion).HasName("PK__Transacc__BEDEB8A52DE9FD87");

            entity.Property(e => e.IdTransaccion).HasColumnName("Id_Transaccion");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdAprendiz).HasColumnName("Id_Aprendiz");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdAprendizNavigation).WithMany(p => p.Transacciones)
                .HasForeignKey(d => d.IdAprendiz)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transacciones_Aprendices");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__63C76BE2EEB8D0F7");

            entity.Property(e => e.IdUsuario).HasColumnName("Id_Usuario");
            entity.Property(e => e.Clave).IsUnicode(false);
            entity.Property(e => e.IdRol).HasColumnName("Id_Rol");
            entity.Property(e => e.IdSede).HasColumnName("Id_Sede");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Roles");

            entity.HasOne(d => d.IdSedeNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdSede)
                .HasConstraintName("FK_Usuarios_Sedes");
        });

        modelBuilder.Entity<Sede>(entity =>
        {
            entity.HasKey(e => e.IdSede).HasName("PK__Sedes__A3F9F16A29A8BA10");

            entity.Property(e => e.IdSede).HasColumnName("Id_Sede");
            entity.Property(e => e.Ciudad).IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre).IsUnicode(false);
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.HasKey(e => e.Id_Reporte)
                  .HasName("PK__Reportes__IdReporte");

            entity.ToTable("Reportes");

            entity.HasIndex(e => e.Radicado)
                  .IsUnique()
                  .HasDatabaseName("UQ_Radicado");

            entity.HasIndex(e => e.Id_Usuario)
                  .HasDatabaseName("IX_Reportes_Usuario");

            entity.HasIndex(e => e.Estado)
                  .HasDatabaseName("IX_Reportes_Estado");

            entity.Property(e => e.Id_Reporte)
                  .HasColumnName("Id_Reporte");

            entity.Property(e => e.Radicado)
                  .HasMaxLength(20)
                  .IsUnicode(false);

            entity.Property(e => e.Tipo_Reporte)
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("Tipo_Reporte");

            entity.Property(e => e.Descripcion)
                  .HasMaxLength(1000)
                  .IsUnicode(false);

            entity.Property(e => e.Evidencia_Path)
                  .HasMaxLength(500)
                  .IsUnicode(false)
                  .HasColumnName("Evidencia_Path");

            entity.Property(e => e.Estado)
                  .HasMaxLength(20)
                  .IsUnicode(false)
                  .HasDefaultValue("Pendiente");

            entity.Property(e => e.Fecha_Creacion)
                  .HasColumnType("datetime2(0)")
                  .HasColumnName("Fecha_Creacion")
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.Fecha_Resolucion)
                  .HasColumnType("datetime2(0)")
                  .HasColumnName("Fecha_Resolucion");

            entity.Property(e => e.Id_Usuario)
                  .HasColumnName("Id_Usuario");

            // FK hacia Usuarios — mismo patrón que el resto del contexto
            entity.HasOne(d => d.Usuario)
                  .WithMany()                         // Usuarios no necesita colección inversa aún
                  .HasForeignKey(d => d.Id_Usuario)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Reportes_Usuarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
