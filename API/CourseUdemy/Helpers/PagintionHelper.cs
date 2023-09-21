namespace CourseUdemy.Helpers
{
    public class PagintionHelper
    {
        public PagintionHelper ( int currentPage, int itemsPerPage, int totalItems, int totalPage )
        {
            this.currentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPage = totalPage;
        }

        public int currentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPage { get; set; }
    }
}
