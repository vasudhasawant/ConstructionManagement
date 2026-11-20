
using System;
using System.Collections.Generic;
using ConstructionManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.DAL.Context;

public partial class ConstructionDbContext : DbContext
{
    public ConstructionDbContext(DbContextOptions<ConstructionDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConstructionSite> ConstructionSites { get; set; }

    public virtual DbSet<Contractor> Contractors { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuRolePermission> MenuRolePermissions { get; set; }

    public virtual DbSet<DailyWorkSubmission> DailyWorkSubmissions { get; set; }

    public virtual DbSet<MaterialDelivery> MaterialDeliveries { get; set; }

    public virtual DbSet<MaterialDemand> MaterialDemands { get; set; }

    public virtual DbSet<MaterialDemandItem> MaterialDemandItems { get; set; }

    public virtual DbSet<MaterialDemandStatusMaster> MaterialDemandStatusMasters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkApproval> WorkApprovals { get; set; }

    public virtual DbSet<WorkSubmissionStatusMaster> WorkSubmissionStatusMasters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Material Demand Status Master
        modelBuilder.Entity<MaterialDemandStatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("MaterialDemandStatusMaster");

            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        // Work Submission Status Master
        modelBuilder.Entity<WorkSubmissionStatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("WorkSubmissionStatusMaster");

            entity.Property(e => e.StatusName)
                .HasMaxLength(50);
        });

        // Construction Site
        modelBuilder.Entity<ConstructionSite>(entity =>
        {
            entity.HasKey(e => e.SiteId);

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.SiteName).HasMaxLength(150);

            entity.HasOne(d => d.Builder)
                .WithMany(p => p.ConstructionSites)
                .HasForeignKey(d => d.BuilderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Contractor
        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.HasKey(e => e.ContractorId);

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.ContractorCompany).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User)
                .WithOne(p => p.Contractor)
                .HasForeignKey<Contractor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Daily Work Submission
        modelBuilder.Entity<DailyWorkSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);

            entity.HasIndex(e => new { e.SiteId, e.WorkDate },
                "IX_DailyWork_SiteDate");

            entity.HasIndex(e => new
            {
                e.SiteId,
                e.ContractorId,
                e.WorkDate
            }).IsUnique();

            entity.Property(e => e.AttendanceSelfiePath).HasMaxLength(500);

            entity.Property(e => e.Latitude).HasMaxLength(50);

            entity.Property(e => e.Longitude).HasMaxLength(50);

            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.Property(e => e.WorkDescription).HasMaxLength(1000);

            entity.Property(e => e.WorkPhotoPath).HasMaxLength(500);

            entity.HasOne(d => d.Contractor)
                .WithMany(p => p.DailyWorkSubmissions)
                .HasForeignKey(d => d.ContractorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Site)
                .WithMany(p => p.DailyWorkSubmissions)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatusNavigation)
                .WithMany(p => p.DailyWorkSubmissions)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Material Delivery
        modelBuilder.Entity<MaterialDelivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId);

            entity.Property(e => e.ChallanNumber).HasMaxLength(100);
            entity.Property(e => e.ChallanPhotoPath).HasMaxLength(500);
            entity.Property(e => e.DeliveryDate)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.InvoicePhotoPath).HasMaxLength(500);
            entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Remarks).HasMaxLength(1000);

            entity.Property(e => e.TotalAmount)
                .HasComputedColumnSql(
                    "(case when [UnitRate] IS NOT NULL then [ReceivedQuantity]*[UnitRate] end)",
                    true)
                .HasColumnType("decimal(25, 4)");

            entity.Property(e => e.UnitRate).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.DemandItem)
                .WithMany(p => p.MaterialDeliveries)
                .HasForeignKey(d => d.DemandItemId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReceivedByNavigation)
                .WithMany(p => p.MaterialDeliveries)
                .HasForeignKey(d => d.ReceivedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Material Demand
        modelBuilder.Entity<MaterialDemand>(entity =>
        {
            entity.HasKey(e => e.DemandId);

            entity.ToTable("MaterialDemands");

            entity.HasIndex(e => new { e.SiteId, e.StatusId },
                "IX_Demands_SiteStatus");

            entity.Property(e => e.DemandDate)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.Property(e => e.Remarks).HasMaxLength(1000);

            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.RequestedByNavigation)
                .WithMany(p => p.MaterialDemands)
                .HasForeignKey(d => d.RequestedBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Site)
                .WithMany(p => p.MaterialDemands)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatusNavigation)
                .WithMany(p => p.MaterialDemands)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Material Demand Items
        modelBuilder.Entity<MaterialDemandItem>(entity =>
        {
            entity.HasKey(e => e.DemandItemId);

            entity.Property(e => e.ApprovedQuantity)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.MaterialName).HasMaxLength(150);

            entity.Property(e => e.RequestedQuantity)
                .HasColumnType("decimal(12, 2)");

            entity.Property(e => e.Unit).HasMaxLength(30);

            entity.Property(e => e.UnitRate)
                .HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Demand)
                .WithMany(p => p.MaterialDemandItems)
                .HasForeignKey(d => d.DemandId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Notifications
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);

            entity.HasIndex(e => new { e.UserId, e.IsRead },
                "IX_Notifications_UserId_IsRead");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Phone).IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(30);
        });

        // Work Approval
        modelBuilder.Entity<WorkApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId);

            entity.HasIndex(e => new { e.SubmissionId, e.ApprovalLevel },
                "IX_Approvals_Submission");

            entity.HasIndex(e => new { e.SubmissionId, e.ApprovalLevel })
                .IsUnique();

            entity.Property(e => e.Decision).HasMaxLength(20);

            entity.Property(e => e.DecisionAt)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.Property(e => e.Remarks).HasMaxLength(1000);

            entity.HasOne(d => d.ApprovalByNavigation)
                .WithMany(p => p.WorkApprovals)
                .HasForeignKey(d => d.ApprovalBy)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Submission)
                .WithMany(p => p.WorkApprovals)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Menu Item
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuId);

            entity.Property(e => e.MenuName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Icon).HasMaxLength(100);

            entity.Property(e => e.Route).HasMaxLength(250);

            entity.HasOne(e => e.ParentMenu)
                .WithMany(e => e.ChildMenus)
                .HasForeignKey(e => e.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Menu Role Permission
        modelBuilder.Entity<MenuRolePermission>(entity =>
        {
            entity.HasKey(e => new { e.MenuId, e.RoleName });

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsRequired();

            entity.HasOne(e => e.Menu)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
