namespace hrms_api.Model;

public class RefreshToken
{
    public int Id {get;set;}
    
    public string refreshToken {get;set;}
    
    public DateTime expires {get;set;}
    
    public int systemuserId {get;set;}
    
    public SystemUser systemUser {get;set;}
}