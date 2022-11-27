using DBManager.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest.ValidationValuesTests
{
    public class ValidationTests
    {
        private DBManager.DBManager dBManager = DBManager.DBManager.Instance;
        private void Setup()
        {
            dBManager.CreateDatabase("Test", "C:/Users/User/Documents/database.txt");
            dBManager.CreateTable("TestTable");
        }
        [Fact]
        public void CreateIntegerColumnSuccess()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "IntegerColumn", ColumnType.Integer, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "1");

            //Assert
            Assert.True(res == true);
        }

        [Fact]
        public void CreateIntegerColumnError()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "IntegerColumn", ColumnType.Integer, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "1.1");

            //Assert
            Assert.True(res == false);
        }
        [Fact]
        public void CreateRealColumnSuccess()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "RealColumn", ColumnType.Real, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "1,1");

            //Assert
            Assert.True(res == true);
        }
        [Fact]
        public void CreateRealColumnError()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "RealColumn", ColumnType.Real, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "1.1w");

            //Assert
            Assert.True(res == false);
        }
        [Fact]
        public void CreateCharColumnSuccess()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "CharColumn", ColumnType.Char, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "c");

            //Assert
            Assert.True(res == true);
        }
        [Fact]
        public void CreateCharColumnError()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "CharColumn", ColumnType.Char, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "ct");

            //Assert
            Assert.True(res == false);
        }
        [Fact]
        public void CreateEmailColumnSuccess()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "EmailColumn", ColumnType.Email, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "test@test.test");

            //Assert
            Assert.True(res == true);
        }
        [Fact]
        public void CreateEmailColumnError()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "EmailColumn", ColumnType.Email, new List<string>());
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "test");

            //Assert
            Assert.True(res == false);
        }
        [Fact]
        public void CreateEnumColumnSuccess()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "EnumColumn", ColumnType.Enum, new List<string>() { "dog", "cat"});
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "dog");

            //Assert
            Assert.True(res == true);
        }
        [Fact]
        public void CreateEnumColumnError()
        {
            //Arrange
            Setup();
            var table = dBManager.Database.Tables.FirstOrDefault();
            var createColRes = dBManager.CreateColumn(table, "EnumColumn", ColumnType.Enum, new List<string>() { "dog", "cat" });
            dBManager.CreateRow(table);
            //Act
            var res = dBManager.EditCell(table, 0, 0, "bird");

            //Assert
            Assert.True(res == false);
        }

    }
}
