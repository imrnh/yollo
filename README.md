A ott platform API with ASP



# Routes

## Admin
  **1. Create an user**
  - <i>path:</i> /admin/addmovieorseries <br>
  - <i>request_type:</i> POST <br>
  - <i>require_auth:</i> Bearer <br>
  - <i>req_body:</i>

  ```
      {
        "title": "WALL-E",
        "description": "A machine responsible for cleaning a waste-covered Earth meets another robot and falls in love with her. Together, they set out on a journey that will alter the fate of mankind.",
        "genres": [1, 78],
        "publishers": [14],
        "publishedat": "2008-06-27",
        "agelimit": 5,
        "bannerurl": "https://images.pexels.com/photos/2103864/pexels-photo-2103864.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
        "moviefiles": ["https://youtu.be/CZ1CATNbXg0"],
        "noofepisodes": 1,
        "isseries": false
    }
```

**2. Create a new Genre**
  - <i>path:</i> /admin/addgenre <br>
  - <i>request_type:</i> POST <br>
  - <i>require_auth:</i> Bearer <br>
  - <i>req_body:</i>
 ```
  {
      "name": "Romance"
  }
 ```


 **3. Create a new Publisher**
  - <i>path:</i>/admin/addpublisher <br>
  - <i>request_type:</i> POST <br>
  - <i>require_auth:</i> Bearer <br>
  - <i>req_body:</i>
 ```
  {
      "name": "HBO"
  }
 ```

**4. View all gernes**
  - <i>path:</i> /admin/allgenres <br>
  - <i>request_type:</i> GET <br>
  - <i>require_auth:</i> Bearer <br>

**5. View all gernes**
  - <i>path:</b> /admin/allpublishers <br>
  - <i>request_type:</i>GET <br>
  - <i>require_auth:</i> Bearer <br>



## Authentication - both admin and user level.
  **1. Create an account**
  - <i>path:</i> /auth/signup <br>
  - <i>request_type:</i> POST <br>
  - <i>require_auth:</i> Bearer <br>
  - <i>req_body:</i>

  ```
     {
        "email": "user2@gmail.com",
        "password": "password123",
        "fullName": "User Full name",
        "dob": "2002-04-17"
     }
```


  **1. Signin**
  - <i>path:</i> /auth/signin <br>
  - <i>request_type:</i> POST <br>
  - <i>require_auth:</i> Bearer <br>
  - <i>req_body:</i>

  ```
    {
        "email": "admin@yollo.com",
        "password": "admin_password"
    }
```
  

## Users.
