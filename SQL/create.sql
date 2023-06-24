create table users(
	id SERIAL,
	email VARCHAR(50),
	password TEXT,
	full_name VARCHAR(50),
	dob DATE,
    isAdmin bool default false,
	parental_control_active BOOL DEFAULT false,
	created_at DATE default current_timestamp,
	primary key(id)
);

create table genre(
	id SERIAL,
	name VARCHAR(50) UNIQUE,
	primary key(id)
);

create table publisher(
	id SERIAL,
	name VARCHAR(50) UNIQUE,
	primary key(id)
);

CREATE TABLE movie (
    id SERIAL PRIMARY KEY,
    title VARCHAR(50) UNIQUE,
    description TEXT,
    published_at DATE,
    age_limit INTEGER, -- 14 means user must be atleast 14 yrs old to watch the movie
    banner_url TEXT,
    movie_files TEXT[],
    no_of_episodes INTEGER,
	isSeries BOOL DEFAULT false
);

CREATE TABLE movie_genres (
	id SERIAL PRIMARY KEY,
    movie_id INTEGER REFERENCES movie(id),
    genre_id INTEGER REFERENCES genre(id)
);


CREATE TABLE movie_publishers (
	id SERIAL PRIMARY KEY,
    movie_id INTEGER REFERENCES movie(id),
    publisher_id INTEGER REFERENCES publisher(id)
);



create table friend(
	id SERIAL,
	user1 INTEGER REFERENCES users(id),
	user2 INTEGER REFERENCES users(id),
	PRIMARY KEY(id)
);


create table whpublic(
	user_id INTEGER,
	wh_public BOOL DEFAULT false,
	wl_public BOOL DEFAULT false,
	primary key(user_id),
	foreign key(user_id) REFERENCES users(id)
);

create table payment(
	id serial PRIMARY KEY,
	user_id INTEGER REFERENCES users(id),
	amount INTEGER,
	bill_cycle INTEGER, -- for how many month the subscription is bought.
	date_paid DATE default current_timestamp
);


create table parental_control(
	user_id integer REFERENCES users(id),
	control_password varchar(250),
	age_limit integer,
	PRIMARY KEY(user_id)
);

create table prntlctrl_allowed_genres(
	user_id INTEGER REFERENCES users(id),
	genre_id INTEGER REFERENCES genre(id),
	PRIMARY KEY(user_id)
);

create table watch_history(
	id serial,
	user_id integer,
	movie_id integer,
	primary key(id),
	foreign key(user_id) references users(id),
	foreign key(movie_id) references movie(id)
);

create table watch_later(
	id serial,
	user_id integer,
	movie_id integer,
	created_at date default current_timestamp, -- ei onusra order kore data return kroabo query diyei. Model e creted_at ar lagbena
	primary key(id),
	foreign key(user_id) references users(id),
	foreign key(movie_id) references movie(id)
);


create table review(
	id serial PRIMARY KEY,
	user_id integer,
	movie_id integer,
	review text,
	rating integer,
	created_at date default current_timestamp,
	foreign key(user_id) references users(id),
	foreign key(movie_id) references movie(id)
);

create table ratings_count(
	movie_id integer,
	avg_ratigns decimal,
	primary key(movie_id),
	foreign key(movie_id) references movie(id)
);