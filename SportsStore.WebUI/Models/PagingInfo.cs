using System;

namespace SportsStore.WebUI.Models
{
    public class PagingInfo
    {
        // Количество товаров
        public int TotalProducts { get; set; }

        // Количество товаров на одной странице
        public int ProductsPerPage { get; set; }

        // Номер текущей страницы
        public int CurrentPage { get; set; }

        // Общее количество страниц
        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalProducts / ProductsPerPage); }
        }
    }
}
