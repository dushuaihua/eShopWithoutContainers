namespace eShopWithoutContainers.Services.Ordering.Infrastructure.EntityConfigurations;
class PaymentMethodEntityTypeConfiguration
    : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> paymentMethodConfiguration)
    {
        paymentMethodConfiguration.ToTable("paymentMethods", OrderingContext.DEFAULT_SCHEMA);

        paymentMethodConfiguration.HasKey(pm => pm.Id);

        paymentMethodConfiguration.Ignore(pm => pm.DomainEvents);

        paymentMethodConfiguration.Property(pm => pm.Id)
            .UseHiLo("paymentseq", OrderingContext.DEFAULT_SCHEMA);

        paymentMethodConfiguration.Property<int>("BuyerId")
            .IsRequired();

        paymentMethodConfiguration
            .Property<string>("_cardHolderName")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardHolderName")
            .HasMaxLength(200)
            .IsRequired();

        paymentMethodConfiguration
            .Property<string>("_alias")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Alias")
            .HasMaxLength(200)
            .IsRequired();

        paymentMethodConfiguration
            .Property<string>("_cardNumber")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardNumber")
            .HasMaxLength(25)
            .IsRequired();

        paymentMethodConfiguration
            .Property<DateTime>("_expiration")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Expiration")
            .HasMaxLength(25)
            .IsRequired();

        paymentMethodConfiguration
            .Property<int>("_cardTypeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardTypeId")
            .IsRequired();

        paymentMethodConfiguration.HasOne(pm => pm.CardType)
            .WithMany()
            .HasForeignKey("_cardTypeId");
    }
}
