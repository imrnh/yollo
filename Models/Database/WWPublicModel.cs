
//WWPublic table holds info about wether the watch_list and watch_history of a user is visible to frinend or not
public class WWPublicModel{
    public int user_id {get; set;}
    public bool wh_public {get; set;} //indicates watch_history is public or not
    public bool wl_public {get; set;} //indicated wish list public or not


    public WWPublicModel(int user_id, bool wh, bool wl){
        this.user_id = user_id;
        this.wh_public = wh;
        this.wl_public = wl;
    }
}