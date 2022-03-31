namespace eShopWithoutContainers.Services.Ordering.Infrastructure.EntityConfigurations;
class CardTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypeConfiguration)
    {
        cardTypeConfiguration.ToTable("cardtypes", OrderingContext.DEFAULT_SCHEMA);

        cardTypeConfiguration.HasKey(ct => ct.Id);

        cardTypeConfiguration.Property(ct => ct.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        cardTypeConfiguration.Property(ct => ct.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
