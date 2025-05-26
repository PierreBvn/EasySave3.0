using EasySave_2._0.Models;

public interface IBackupType
{
    void Transfer(int id, string name, string source, string target);
}
