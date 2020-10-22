namespace IDowiComponentNamespace
{
    /// <summary>
    /// Interfejst customowego komponentu
    /// </summary>
    public interface IDowiComponent
    {
        /// <summary>
        /// Właściwość pozwalająca określić czy komponent jest wypełniony
        /// </summary>
        bool HasValue { get; set; }
    }
}
