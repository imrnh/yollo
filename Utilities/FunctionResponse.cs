public class FunctionResponse{
    public bool status{get; set;}
    public dynamic value {get; set;}
    
    public bool IsSuccess(){return this.status;}

    public FunctionResponse(){

    }
    public FunctionResponse(bool status, dynamic value){
        this.value = value;
        this.status = status;
    }
}