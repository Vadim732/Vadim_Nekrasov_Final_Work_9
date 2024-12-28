﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicWallet.Models;

public class WalletContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    public WalletContext(DbContextOptions<WalletContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Wallets)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId);

        modelBuilder.Entity<Wallet>()
            .HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId);

        modelBuilder.Entity<ServiceProvider>()
            .HasMany(sp => sp.Payments)
            .WithOne(p => p.ServiceProvider)
            .HasForeignKey(p => p.ServiceProviderId);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Counterparty)
            .WithMany()
            .HasForeignKey(t => t.CounterpartyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}