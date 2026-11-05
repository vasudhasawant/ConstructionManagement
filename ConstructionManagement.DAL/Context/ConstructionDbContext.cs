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

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<SiteUserAccess> SiteUserAccesses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkApproval> WorkApprovals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConstructionSite>(entity =>
        {
            entity.HasKey(e => e.SiteId).HasName("PK__Construc__B9DCB9630FEF9507");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.SiteName).HasMaxLength(150);

            entity.HasOne(d => d.Builder).WithMany(p => p.ConstructionSites)
                .HasForeignKey(d => d.BuilderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Construct__Build__3F466844");
        });

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.HasKey(e => e.ContractorId).HasName("PK__Contract__E964EB7DE2EBAA30");

            entity.HasIndex(e => e.UserId, "UQ__Contract__1788CC4D002BEFEA").IsUnique();

            entity.Property(e => e.ContractorCompany).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.Contractor)
                .HasForeignKey<Contractor>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracto__UserI__49C3F6B7");
        });

        modelBuilder.Entity<DailyWorkSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__DailyWor__449EE1251280B823");

            entity.HasIndex(e => new { e.SiteId, e.WorkDate }, "IX_DailyWork_SiteDate");

            entity.HasIndex(e => new { e.SiteId, e.ContractorId, e.WorkDate }, "UQ__DailyWor__E36E24C11AA03148").IsUnique();

            entity.Property(e => e.AttendanceSelfiePath).HasMaxLength(500);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Status)
                .HasMaxLength(40)
                .HasDefaultValue("PendingSupervisor");
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.WorkDescription).HasMaxLength(1000);
            entity.Property(e => e.WorkPhotoPath).HasMaxLength(500);

            entity.HasOne(d => d.Contractor).WithMany(p => p.DailyWorkSubmissions)
                .HasForeignKey(d => d.ContractorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyWork__Contr__534D60F1");

            entity.HasOne(d => d.Site).WithMany(p => p.DailyWorkSubmissions)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyWork__SiteI__52593CB8");
        });

        modelBuilder.Entity<MaterialDelivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Material__626D8FCEC03EF101");

            entity.Property(e => e.ChallanNumber).HasMaxLength(100);
            entity.Property(e => e.ChallanPhotoPath).HasMaxLength(500);
            entity.Property(e => e.DeliveryDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.InvoiceNumber).HasMaxLength(100);
            entity.Property(e => e.InvoicePhotoPath).HasMaxLength(500);
            entity.Property(e => e.ReceivedQuantity).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Remarks).HasMaxLength(1000);
            entity.Property(e => e.TotalAmount)
                .HasComputedColumnSql("(case when [UnitRate] IS NOT NULL then [ReceivedQuantity]*[UnitRate]  end)", true)
                .HasColumnType("decimal(25, 4)");
            entity.Property(e => e.UnitRate).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.DemandItem).WithMany(p => p.MaterialDeliveries)
                .HasForeignKey(d => d.DemandItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialD__Deman__6FE99F9F");

            entity.HasOne(d => d.ReceivedByNavigation).WithMany(p => p.MaterialDeliveries)
                .HasForeignKey(d => d.ReceivedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialD__Recei__70DDC3D8");
        });

        modelBuilder.Entity<MaterialDemand>(entity =>
        {
            entity.HasKey(e => e.DemandId).HasName("PK__Material__36081845C4CD5395");

            entity.HasIndex(e => new { e.SiteId, e.Status }, "IX_Demands_SiteStatus");

            entity.Property(e => e.DemandDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Remarks).HasMaxLength(1000);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.MaterialDemands)
                .HasForeignKey(d => d.RequestedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialD__Reque__6754599E");

            entity.HasOne(d => d.Site).WithMany(p => p.MaterialDemands)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialD__SiteI__66603565");
        });

        modelBuilder.Entity<MaterialDemandItem>(entity =>
        {
            entity.HasKey(e => e.DemandItemId).HasName("PK__Material__C20F40616D83FFB0");

            entity.Property(e => e.ApprovedQuantity).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.MaterialName).HasMaxLength(150);
            entity.Property(e => e.RequestedQuantity).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Unit).HasMaxLength(30);
            entity.Property(e => e.UnitRate).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Demand).WithMany(p => p.MaterialDemandItems)
                .HasForeignKey(d => d.DemandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialD__Deman__6B24EA82");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1230137186");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "IX_Notifications_UserId_IsRead");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Message).HasMaxLength(1000);
            entity.Property(e => e.NotificationType).HasMaxLength(50);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<SiteUserAccess>(entity =>
        {
            entity.HasKey(e => new { e.SiteId, e.UserId }).HasName("PK__SiteUser__68A435A7FD8298A1");

            entity.ToTable("SiteUserAccess");

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.SiteUserAccessAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SiteUserA__Assig__44FF419A");

            entity.HasOne(d => d.Site).WithMany(p => p.SiteUserAccesses)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SiteUserA__SiteI__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.SiteUserAccessUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SiteUserA__UserI__440B1D61");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C38EB6C41");

            entity.HasIndex(e => e.Phone, "UQ__Users__5C7E359E6B6322BB").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(30);
        });

        modelBuilder.Entity<WorkApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__WorkAppr__328477F48D060EFB");

            entity.HasIndex(e => new { e.SubmissionId, e.ApprovalLevel }, "IX_Approvals_Submission");

            entity.HasIndex(e => new { e.SubmissionId, e.ApprovalLevel }, "UQ__WorkAppr__C3D28A908940DE85").IsUnique();

            entity.Property(e => e.Decision).HasMaxLength(20);
            entity.Property(e => e.DecisionAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Remarks).HasMaxLength(1000);

            entity.HasOne(d => d.Approver).WithMany(p => p.WorkApprovals)
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkAppro__Appro__5AEE82B9");

            entity.HasOne(d => d.Submission).WithMany(p => p.WorkApprovals)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WorkAppro__Submi__59FA5E80");
        });
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuId);

            entity.Property(e => e.MenuName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Icon)
                .HasMaxLength(100);

            entity.Property(e => e.Route)
                .HasMaxLength(250);

            entity.HasOne(e => e.ParentMenu)
                .WithMany(e => e.ChildMenus)
                .HasForeignKey(e => e.ParentMenuId)
                .OnDelete(DeleteBehavior.Restrict);
        });

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
