namespace MessangerBackend.Core.Models;

/*
 *
 * [Name] varchar(20) not null
 */

public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenOnline { get; set; }
    public virtual ICollection<Chat> Chats { get; set; }
}

//[NotMapped] - не зберігає в таблицю
//[Key] - primary key
//[Column] - тип колонки
//[MaxLength] - валідація довжини