# SmartEF
Simple library with EF helpers  

```
Install-Package SmartEF
```  

## Usage

Bellow a sample  

```csharp
    class Program
    {
        static void Main(string[] args)
        {
            string dBTableName = Helper.GetDBTableName<MyTableEntity>();
            string dBColumnName = Helper.GetDBColumnName<MyTableEntity>(nameof(MyTableEntity.Id));

            string cmd1 = Helper.DeleteSQLCommand(new List<int> { 1, 2, 3 }, "dbo", dBTableName, dBColumnName);
            string cmd2 = Helper.DeleteSQLCommand<MyTableEntity>(new List<int> { 1, 2, 3, 4 }, "dbo", a => a.Id);
            string cmd3 = Helper.DeleteSQLCommand<MyTableEntity,int>(new List<int> { 1, 2, 3, 5 }, "dbo", a => a.Id);
            string cmd4 = Helper.DeleteSQLCommand<MyTableEntity>(new List<int> { 1, 2, 3, 4, 5, 6 }, "dbo", nameof(MyTableEntity.Id));
            string cmd5 = Helper.DeleteSQLCommand(new List<int> { 1, 2, 3, 4, 5, 6, 7 }, "dbo", (MyTableEntity a) => a.Id);

            Console.WriteLine(cmd1);
            Console.WriteLine(cmd2);
            Console.WriteLine(cmd3);
            Console.WriteLine(cmd4);
            Console.WriteLine(cmd5);
            
            Console.ReadKey();
        }
    }

    [Table("MyTable")]
    public class MyTableEntity
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }
    }
```
  
Danke   
