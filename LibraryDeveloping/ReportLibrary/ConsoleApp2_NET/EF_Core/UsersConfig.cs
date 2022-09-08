using System;
using System.Collections.Generic;

namespace ConsoleApp2_NET
{
    public partial class UsersConfig
    {
        public int Id { get; set; }
        public string UserLogin { get; set; } = null!;
        public string ConfigPropertyName { get; set; } = null!;
        public string? ConfigPropertyValue { get; set; }
    }
}
