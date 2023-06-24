public class HttpResponse{
    public int status{get; set;}
    public string message {get; set;}

    public HttpResponse(){

    }
    public HttpResponse(int status, string message){
        this.message = message;
        this.status = status;
    }

    public Object toJson(){
        return new {status_code=this.status, message = this.message};
    }
}