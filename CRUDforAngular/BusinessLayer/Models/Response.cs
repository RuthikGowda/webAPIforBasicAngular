﻿namespace CRUDforAngular.BusinessLayer.Models
{
    public class Response<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
    }

} 
 
