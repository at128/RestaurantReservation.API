namespace RestaurantReservation.API.Authorization;

public static class Permission
{
    public static class Reservations
    {
        public const string Read = "Reservations.Read";
        public const string Create = "Reservations.Create";
        public const string Update = "Reservations.Update";
        public const string Delete = "Reservations.Delete";

        public const string ViewOrders = "Reservations.Orders.Read";
        public const string ViewMenuItems = "Reservations.MenuItems.Read";
    }

    public static class Employees
    {
        public const string ViewManagers = "Employees.Managers.Read";
        public const string ViewAverageOrderAmount = "Employees.AverageOrderAmount.Read";
    }
}
