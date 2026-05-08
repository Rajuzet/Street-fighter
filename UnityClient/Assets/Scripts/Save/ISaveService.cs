namespace StreetFighter.Save
{
    /// <summary>
    /// Contract for save and load services used by runtime systems.
    /// </summary>
    public interface ISaveService
    {
        void Initialize(SaveSettings settings);
        bool Save<T>(string key, T record) where T : class;
        T Load<T>(string key) where T : class, new();
    }
}
