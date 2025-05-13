namespace Kol1_APBD.Services;
using Microsoft.Data.SqlClient;
using Kol1_APBD.Models;
using System.Data.Common;
public class DBservice : IDBservice
{
    private readonly IConfiguration _configuration;
    public DBservice(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private readonly string _connectionString = "Data Source=localhost, 1433; User=SA; Password=yourStrong(!)Password; Initial Catalog=kolokwium; Integrated Security=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False\n";
    // await transaction.CommitAsync(); -do commitowania zmian do bazy
    // execute scalar - zwraca liczbę, stałą (object)
    // execute reader - do selecta
    // execute nonquery - do insert update delete
    
    
    
    
    
}