public class FunctionResponse{
    public bool status{get; set;}
    public dynamic message {get; set;}

    public FunctionResponse(){

    }
    public FunctionResponse(bool status, dynamic message){
        this.message = message;
        this.status = status;
    }

    public Object toJson(){
        return new {status_code=this.status, message = this.message};
    }
}