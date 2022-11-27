using System;
using System.Collections.Generic;
using DBManager.GrpcService;
using System.Text.RegularExpressions;

namespace DBManager.GrpcService
{
    public partial class Column
    {
        public Column(string name, ColumnType type, List<string> availableValues = null)
        {
            Name = name;
            Type = type;
            AvailableValues.Clear();
            AvailableValues.AddRange(availableValues);
        }
        private const string EmailPattern = @"^[\d\w\._\-\+]+@([\d\w\._\-]+\.)+[\w]+$";

        public bool IsValid(string value)
        {
            return Type switch
            {
                ColumnType.Integer => int.TryParse(value, out _),
                ColumnType.Real => double.TryParse(value, out _),
                ColumnType.Char => char.TryParse(value, out _),
                ColumnType.String => true,
                ColumnType.Enum => AvailableValues?.Contains(value) ?? false,
                ColumnType.Email => !string.IsNullOrEmpty(value) &&
                                    Regex.IsMatch(value, EmailPattern, RegexOptions.IgnoreCase),
                _ => false
            };
        }
    }
}
