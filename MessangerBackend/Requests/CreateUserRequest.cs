using System.ComponentModel.DataAnnotations;

namespace MessangerBackend.Requests;

public class CreateUserRequest
{
    [MaxLength(20)]
    public string Nickname { get; set; }
    
    //[RegularExpression("^.*(?=.{8,})(?=.*[a-zA-Z])(?=.*\\d)(?=.*[!#$%&? \"]).*$")]
    public string Password { get; set; }

    public string role { get; set; }
}