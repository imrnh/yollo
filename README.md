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
       "title": "Passengers 1",
       "description": "During a voyage to a distant planet, Jim's hypersleep pod malfunctions which wakes him up. So, he and fellow passenger work together to prevent their spaceship from meeting with a disaster.",
       "genres": [4,5],
       "publishers": [7,12],
       "published_at": "04/22/2017",
       "agelimit": 14,
       "bannerurl": "https://images.pexels.com/photos/828658/pexels-photo-828658.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
       "moviefiles": ["https://images.pexels.com/photos/828658/pexels-photo-828658.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1"],
       "noofepisods": 1,
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

  

## Users.
