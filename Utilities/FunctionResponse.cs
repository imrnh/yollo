public class FunctionResponse{
    public bool status{get; set;}
    public dynamic value {get; set;}

    public FunctionResponse(){

    }
    public FunctionResponse(bool status, dynamic value){
        this.value = value;
        this.status = status;
    }
}