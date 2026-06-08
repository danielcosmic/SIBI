using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public class SibiDbContext : DbContext
{
    public SibiDbContext(DbContextOptions<SibiDbContext> options) : base(options) { }

    public DbSet<Placa> Placas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Encargado> Encargados { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Ubicacion> Ubicaciones { get; set; }
    public DbSet<Activo> Activos { get; set; }
    public DbSet<Historial> Historial { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Placa>(e =>
        {
            e.ToTable("Placa");
            e.HasKey(p => p.Numero);
        });

        mb.Entity<Usuario>(e =>
        {
            e.ToTable("Usuario");
            e.HasKey(u => u.Correo);
        });

        mb.Entity<Encargado>().ToTable("Encargado");

        mb.Entity<Categoria>().ToTable("Categoria");

        mb.Entity<Ubicacion>(e =>
        {
            e.ToTable("Ubicacion");
            e.Property(u => u.EncargadoActualId).HasColumnName("EncargadoActual");
            e.Property(u => u.EncargadoAnteriorId).HasColumnName("EncargadoAnterior");
            e.HasOne(u => u.EncargadoActual)
             .WithMany()
             .HasForeignKey(u => u.EncargadoActualId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(u => u.EncargadoAnterior)
             .WithMany()
             .HasForeignKey(u => u.EncargadoAnteriorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Activo>(e =>
        {
            e.ToTable("Activo");
            e.HasKey(a => a.Placa);
            // Placa es simultáneamente PK y FK a la tabla Placa
            e.HasOne(a => a.PlacaNavigation)
             .WithMany()
             .HasForeignKey(a => a.Placa)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Categoria)
             .WithMany()
             .HasForeignKey(a => a.CategoriaId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.UbicacionNavigation)
             .WithMany()
             .HasForeignKey(a => a.UbicacionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Historial>(e =>
        {
            e.ToTable("Historial");
            e.HasOne(h => h.Activo)
             .WithMany()
             .HasForeignKey(h => h.ActivoPlaca)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(h => h.Usuario)
             .WithMany()
             .HasForeignKey(h => h.UsuarioCorreo)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
