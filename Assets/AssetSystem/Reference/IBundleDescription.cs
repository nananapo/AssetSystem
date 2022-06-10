namespace AssetSystem.Reference
{
    public interface IBundleDescription
    {
        public string Name { get; }
        
        public string BundleId { get; }
        
        public string AuthorId { get; }

        public long CreatedTime { get; }
        
        public long UpdatedTime { get; }
    }
}