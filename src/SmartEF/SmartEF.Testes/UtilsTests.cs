using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEF.Testes
{
    [TestClass]
    public sealed class UtilsTests
    {
        [TestMethod]
        public void Dado_Tabela_Quando_GetDBTableName_Devo_ObterNomeCorreto()
        {
            string dBTableName = Utils.GetDBTableName<MyTableEntity>();

            dBTableName.ShouldBe("MyTable");
        }

        [TestMethod]
        public void Dado_Coluna_Quando_GetDBColumnName_Devo_ObterNomeCorreto()
        {
            string dBColumnName = Utils.GetDBColumnName<MyTableEntity>(nameof(MyTableEntity.Id));

            dBColumnName.ShouldBe("ID");
        }

        [TestMethod]
        public void Dado_TabelaComIdsParaExcluir_QuandoGerarComando_Devo_GerarCorretamente()
        {
            string dBTableName = Utils.GetDBTableName<MyTableEntity>();
            string dBColumnName = Utils.GetDBColumnName<MyTableEntity>(nameof(MyTableEntity.Id));

            string cmd1 = Utils.DeleteSQLCommand(new List<int> { 1, 2, 3 }, "dbo", dBTableName, dBColumnName);
            string cmd2 = Utils.DeleteSQLCommand<MyTableEntity>(new List<int> { 1, 2, 3, 4 }, "dbo", a => a.Id);
            string cmd3 = Utils.DeleteSQLCommand<MyTableEntity, int>(new List<int> { 1, 2, 3, 5 }, "dbo", a => a.Id);
            string cmd4 = Utils.DeleteSQLCommand<MyTableEntity>(new List<int> { 1, 2, 3, 4, 5, 6 }, "dbo", nameof(MyTableEntity.Id));
            string cmd5 = Utils.DeleteSQLCommand(new List<int> { 1, 2, 3, 4, 5, 6, 7 }, "dbo", (MyTableEntity a) => a.Id);

            Console.WriteLine(cmd1);
            Console.WriteLine(cmd2);
            Console.WriteLine(cmd3);
            Console.WriteLine(cmd4);
            Console.WriteLine(cmd5);

            cmd1.ShouldBe("DELETE FROM [dbo].[MyTable] WHERE [ID] IN (1,2,3);");
            cmd2.ShouldBe("DELETE FROM [dbo].[MyTable] WHERE [ID] IN (1,2,3,4);");
            cmd3.ShouldBe("DELETE FROM [dbo].[MyTable] WHERE [ID] IN (1,2,3,5);");
            cmd4.ShouldBe("DELETE FROM [dbo].[MyTable] WHERE [ID] IN (1,2,3,4,5,6);");
            cmd5.ShouldBe("DELETE FROM [dbo].[MyTable] WHERE [ID] IN (1,2,3,4,5,6,7);");

            var columnsName = Utils.GetDBColumnsName<MyTableEntity>();

            columnsName.Count.ShouldBe(2);
        }
    }

    [Table("MyTable")]
    public sealed class MyTableEntity
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }
    }
}
