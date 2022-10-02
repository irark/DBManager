﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager.Models
{
    public class Database
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();
        public Database (string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
