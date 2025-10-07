using Microsoft.EntityFrameworkCore; 
using System;
using TOB.Identity.Infrastructure.Data.Entities;

namespace TOB.Identity.Infrastructure.Data;

public partial class IdentityDBContext : DbContext
{
    public IdentityDBContext()
    {
    }

    public IdentityDBContext(DbContextOptions<IdentityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Tenant> Tenants { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<RolePermissionMapping> RolePermissionMappings { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRoleMapping> UserRoleMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(e => e.TenantId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.PermissionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<RolePermissionMapping>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.RolePermissionMappings)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissionMappings_Right");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RolePermissionMappings)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissionMappings_Role");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).ValueGeneratedNever();
        });

        modelBuilder.Entity<UserRoleMapping>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Role)
                .WithMany(p => p.UserRoleMappings)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoleMappings_Roles");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserRoleMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoleMappings_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Tenant)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Tenant");

            entity.HasOne(d => d.Manager)
                .WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Users_Manager");
        });


        AddSeedData(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    private void AddSeedData(ModelBuilder modelBuilder)
    {
        // Modules
        var permissionId1 = new Guid("08e19d7e-1c60-44ee-a86e-47e11c9c10cb");
        var permissionId2 = new Guid("28e2f626-d9a8-4ee0-8fcb-a5dec5f56d8d");
        var permissionId3 = new Guid("5E87911C-7F0D-4CC1-B543-C839A690AE40");
        var permissionId4 = new Guid("f286d516-c072-4b52-870a-442611ec1348");
        var permissionId5 = new Guid("67dd9410-e052-48a9-bb8f-bc30754c3a1d");
        var permissionId6 = new Guid("c5a6ab49-b4f4-4892-bb3b-e8b51993d003");
        var permissionId7 = new Guid("c5f4ccc3-92ac-40da-ae0a-a346a6438195");
        var permissionId8 = new Guid("C7CBDB48-FFE8-489C-B0C0-96075ED54530");

        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId1, PermissionName = "Register Tenant", IsActive = true, SortOrder = 1 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId2, PermissionName = "Edit Tenant", IsActive = true, SortOrder = 2 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId3, PermissionName = "Deactivate Tenant", IsActive = true, SortOrder = 3 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId4, PermissionName = "Add User", IsActive = true, SortOrder = 4 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId5, PermissionName = "Edit User", IsActive = true, SortOrder = 5 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId6, PermissionName = "Deactivate User", IsActive = true, SortOrder = 6 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId7, PermissionName = "Multi Tenant Access", IsActive = true, SortOrder = 7 });
        modelBuilder.Entity<Permission>().HasData(new Permission { PermissionId = permissionId8, PermissionName = "Tenant Admin", IsActive = true, SortOrder = 8 });

        // Roles
        var roleId1 = new Guid("4338af59-af06-45f3-b666-c133d6bb7d6a");
        var roleId2 = new Guid("B2312EB3-3B9C-459A-94B8-6A7A61D028A9");
        var roleId3 = new Guid("643b7b98-e603-47f5-bdc0-1fefa10041b9");
        var roleId4 = new Guid("CFEF957C-D3B8-4CEC-847E-FF5BC10993F3");

        modelBuilder.Entity<Role>().HasData(new Role { RoleId = roleId1, RoleName = "Super Admin", SortOrder = 1 });
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = roleId2, RoleName = "Admin", SortOrder = 2 });
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = roleId3, RoleName = "Power User", SortOrder = 3 });
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = roleId4, RoleName = "User", SortOrder = 4 }); 
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}