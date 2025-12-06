using Microsoft.EntityFrameworkCore;

namespace Jewelry_Model.Entity;

public partial class JewelryAwsContext : DbContext
{
    public JewelryAwsContext()
    {
    }

    public JewelryAwsContext(DbContextOptions<JewelryAwsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSize> ProductSizes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");
            
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(50);
        });
        
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");
        });

        modelBuilder.Entity<ProductSize>(entity =>
        {
            entity.ToTable("ProductSize");
            entity.HasOne(d => d.Product).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSize_Product");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductSizes)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductSize_Size_1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");
            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Product_1");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("Size");
            entity.Property(e => e.Label).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
