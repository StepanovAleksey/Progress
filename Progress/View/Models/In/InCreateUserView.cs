using System.ComponentModel;
using Entity.Models;

namespace View.Models.In;

public class InCreateUserView
{
    public string? Name { get; set; } 
    public string Email { get; set; }
    public string Password { get; set; }
    
    public string? IpAdress { get; set; }
}