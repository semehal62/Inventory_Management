namespace Inventory_management_System.Models
{
    public class Sale
    {
        public int Id {  get; set; }
        public BaseUser ? BaseUser { get; set; }
        public int BaseUserId { get; set; }
        public DateTime Sold_date { get; set; }
        public Item ? Items { get; set; }
        public int ItemsId { get; set; }
        public required int Quantity_Sold { get; set; }
        public float Total_prices { get; set; }
        public TrackStatus TrackStatus { get; set; } = TrackStatus.Pending;


    }
}
