using CarProduct.Persistence.Enums;

namespace CarProduct.Application.Helpers
{
    public static class ProcessingStatusHelper
    {
        public static bool IsAvailable(this ProcessingStatus value)
        {
            return value != ProcessingStatus.NotProcessed
                   && value != ProcessingStatus.Succeeded
                   && value != ProcessingStatus.Failed;
        }
    }
}