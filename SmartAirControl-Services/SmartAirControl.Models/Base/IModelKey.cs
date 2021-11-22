namespace SmartAirControl.Models.Base
{
    /// <summary>
    /// Identifies a class as a key of a type T.
    /// </summary>
    /// <typeparam name="T">Class that is marked as key.</typeparam>
    public interface IModelKey<T> where T : new() { }
}
