using System.Data.Entity.ModelConfiguration;
using CamprayPortal.Core.Domain.News;

namespace CamprayPortal.Data.Mapping.News
{
    public partial class NewsItemMap : EntityTypeConfiguration<NewsItem>
    {
        public NewsItemMap()
        {
            this.ToTable("News");
            this.HasKey(bp => bp.Id);
            this.Property(bp => bp.Title).IsRequired();
            this.Property(bp => bp.Short).IsRequired();
            this.Property(bp => bp.Full).IsRequired();
            this.Property(bp => bp.MetaKeywords).HasMaxLength(400);
            this.Property(bp => bp.MetaTitle).HasMaxLength(400);
            
            this.HasRequired(bp => bp.Language)
                .WithMany()
                .HasForeignKey(bp => bp.LanguageId).WillCascadeOnDelete(true);

            this.Ignore(u => u.NewsType);
        }
    }
}