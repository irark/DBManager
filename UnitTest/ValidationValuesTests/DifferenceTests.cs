using DBManager.Entities.Models;
using DBManager.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ValidationValuesTests
{
    public class DifferenceTests
    {
        private DBManager.DBManager dBManager = DBManager.DBManager.Instance;
        private Table _firstTable => dBManager.Database?.Tables[0];
        private Table _secondTable => dBManager.Database?.Tables[1];
        private void Setup()
        {
            dBManager.CreateDatabase("Test", "C:/Users/User/Documents/database.txt");
            dBManager.CreateTable("FirstTable");
            
            dBManager.CreateColumn(_firstTable, "Name", ColumnType.String, new List<string>());
            dBManager.CreateColumn(_firstTable, "Email", ColumnType.Email, new List<string>());
            dBManager.CreateColumn(_firstTable, "Age", ColumnType.Integer, new List<string>());

            dBManager.CreateRow(_firstTable);
            dBManager.CreateRow(_firstTable);
            dBManager.EditCell(_firstTable, 0, 0, "Anna"); 
            dBManager.EditCell(_firstTable, 1, 0, "anna@test.com"); 
            dBManager.EditCell(_firstTable, 2, 0, "12");
            dBManager.EditCell(_firstTable, 0, 1, "Devid");
            dBManager.EditCell(_firstTable, 1, 1, "devid@test.com");
            dBManager.EditCell(_firstTable, 2, 1, "22");
            dBManager.CreateTable("SecondTable");
            dBManager.CreateColumn(_secondTable, "Name", ColumnType.String, new List<string>());
            dBManager.CreateColumn(_secondTable, "Email", ColumnType.Email, new List<string>());
            dBManager.CreateColumn(_secondTable, "Phone", ColumnType.String, new List<string>());
            dBManager.CreateRow(_secondTable);
            dBManager.CreateRow(_secondTable);
            dBManager.EditCell(_secondTable, 0, 0, "Anna");
            dBManager.EditCell(_secondTable, 1, 0, "anna@test.com");
            dBManager.EditCell(_secondTable, 2, 0, "+38000000000");
            dBManager.EditCell(_secondTable, 0, 1, "Devid");
            dBManager.EditCell(_secondTable, 1, 1, "devid+1@test.com");
            dBManager.EditCell(_secondTable, 2, 1, "+38000000001");
        }
        [Fact]
        public void DifferenceSuccess()
        {
            //Arrange
            Setup();
     
            //Act
            var res = dBManager.Difference(_firstTable, _secondTable);

            //Assert
            Assert.True(res.Rows.Count == 1 && res.Columns.Count == 2 && res.Rows[0].Values.SequenceEqual(new List<string>() { "Devid", "devid@test.com" }));
        }
    }
}
