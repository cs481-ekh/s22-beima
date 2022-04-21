namespace BEIMA.Backend.StorageService
{
    /// <summary>
    /// Helps to store the instance of the storage provider being used.
    /// </summary>
    /// <remarks>
    /// The purpose of this class is to allow us to switch out the default
    /// storage provider with a mock storage provider for testing.
    /// </remarks>
    public static class StorageDefinition
    {
        /// <summary>
        /// Gets and sets the currently defined instance of the storage provider
        /// </summary>
        public static IStorageProvider StorageInstance { get; set; } = StorageProvider.Instance;
    }
}
