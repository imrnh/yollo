A ott platform API with ASP



## Routes

### Admin
  **1. Create an user**
    > path: {{HOST}}/admin/addmovieorseries
    > request_type: POST
    > require_auth: Bearer
    > req_body: {
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

### Users.
