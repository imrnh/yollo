public class FriendModel{

    public int id {get; set;}
    public int user_id_1  {get; set;}
    public int user_id_2 {get; set;}

    public FriendModel(int id, int u1, int u2){
        this.id = id;
        this.user_id_1 = u1;
        this.user_id_2 = u2;
    }
}