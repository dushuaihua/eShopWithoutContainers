namespace eShopWithoutContainers.Services.Ordering.Infrastructure.EntityConfigurations;
class BuyerEntityTypeConfiguration
    : IEntityTypeConfiguration<Buyer>
{
    public void Configure(EntityTypeBuilder<Buyer> buyerConfiguration)
    {
        buyerConfiguration.ToTable("buys", OrderingContext.DEFAULT_SCHEMA);

        buyerConfiguration.HasKey(buyer => buyer.Id);

        buyerConfiguration.Ignore(buyer => buyer.DomainEvents);

        buyerConfiguration.Property(buyer => buyer.Id)
            .UseHiLo("buyerseq", OrderingContext.DEFAULT_SCHEMA);

        buyerConfiguration.Property(buyer => buyer.IdentityGuid)
            .HasMaxLength(200)
            .IsRequired();

        buyerConfiguration.HasIndex(buyer => buyer.IdentityGuid)
            .IsUnique(true);

        buyerConfiguration.Property(buyer => buyer.Name);

        buyerConfiguration.HasMany(buyer => buyer.PaymentMethods)
            .WithOne()
            .HasForeignKey(buyer => buyer.Id)
            .OnDelete(DeleteBehavior.Cascade);

        var navigation = buyerConfiguration.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
