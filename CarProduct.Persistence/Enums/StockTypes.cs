using System.ComponentModel;

namespace CarProduct.Persistence.Enums
{
    public enum StockTypes
    {
        [Description("New & used cars")] All = 1,
        [Description("New & certified cars")] New_Cpo = 2,
        [Description("New cars")] New = 3,
        [Description("Certified cars")] Cpo = 4,
    }
}
